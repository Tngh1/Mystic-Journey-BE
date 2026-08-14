using BLL.Mappings;
using BLL.Services;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mystic_Journey_API.Filters;
using System.IO;
using System.Text;

var workingDirectory = Directory.GetCurrentDirectory();
LoadEnvIfExists(
    Path.Combine(workingDirectory, ".env"),
    Path.GetFullPath(Path.Combine(workingDirectory, "..", ".env")));

static void LoadEnvIfExists(params string[] paths)
{
    foreach (var path in paths)
    {
        if (!File.Exists(path))
            continue;

        Env.Load(path);
        return;
    }
}
var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT signing key is missing. Configure Jwt__Key in the environment or .env file.");
if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("JWT signing key must be at least 32 bytes. Configure a stronger Jwt__Key value.");
if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new InvalidOperationException("JWT issuer is missing. Configure Jwt__Issuer in the environment or .env file.");
if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("JWT audience is missing. Configure Jwt__Audience in the environment or .env file.");

// Add services to the container.
builder.Services.AddDbContext<MysticJourneyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// OTP, cờ xác thực email và session đều nằm ở IDistributedCache (bên dưới), không phải
// IMemoryCache — nên ở đây không đăng ký MemoryCache nữa.
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (string.IsNullOrWhiteSpace(redisConnectionString))
{
    redisConnectionString = builder.Configuration["Redis:ConnectionString"];
}

if (!string.IsNullOrWhiteSpace(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "MysticJourney:";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddAutoMapper(mapconfig => mapconfig.AddProfile<AutoMapperProfile>());

// Transaction Manager
builder.Services.AddScoped<ITransactionManager, TransactionManager>();

// Auth Services
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Item Services
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();

// Monster Services
builder.Services.AddScoped<IMonsterRepository, MonsterRepository>();
builder.Services.AddScoped<IMonsterService, MonsterService>();

// Dungeon Services
builder.Services.AddScoped<IDungeonConfigRepository, DungeonConfigRepository>();
builder.Services.AddScoped<IChestRepository, ChestRepository>();
builder.Services.AddScoped<IDungeonConfigService, DungeonConfigService>();

// Dungeon Session Services
builder.Services.AddScoped<IDungeonSessionRepository, DungeonSessionRepository>();
builder.Services.AddScoped<IDungeonProgressRepository, DungeonProgressRepository>();
builder.Services.AddScoped<IDungeonSessionService, DungeonSessionService>();

// Shop Services
builder.Services.AddScoped<IShopItemRepository, ShopItemRepository>();
builder.Services.AddScoped<IShopItemService, ShopItemService>();

// Player Currency and Shop Services
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IPlayerShopRepository, PlayerShopRepository>();
builder.Services.AddScoped<IPlayerShopService, PlayerShopService>();

// Gacha Services
builder.Services.AddScoped<IGachaBannerRepository, GachaBannerRepository>();
builder.Services.AddScoped<IGachaBannerService, GachaBannerService>();

// Quest Services
builder.Services.AddScoped<IQuestRepository, QuestRepository>();
builder.Services.AddScoped<IQuestService, QuestService>();
builder.Services.AddScoped<IPlayerQuestRepository, PlayerQuestRepository>();
builder.Services.AddScoped<IPlayerQuestService, PlayerQuestService>();
builder.Services.AddScoped<IWorldRepository, WorldRepository>();
builder.Services.AddScoped<IWorldService, WorldService>();

// Achievement Services
builder.Services.AddScoped<IAchievementRepository, AchievementRepository>();
builder.Services.AddScoped<IAchievementService, AchievementService>();

// Content Services
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IContentService, ContentService>();

// Mailbox Services
builder.Services.AddScoped<IMailboxRepository, MailboxRepository>();
builder.Services.AddScoped<IMailboxService, MailboxService>();

// Inventory Services
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IRewardDeliveryService, RewardDeliveryService>();

// Skill Services
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<ISkillService, SkillService>();

// PlayerProfile Services
builder.Services.AddScoped<IPlayerProfileRepository, PlayerProfileRepository>();
builder.Services.AddScoped<IPlayerProfileService, PlayerProfileService>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IFriendService, FriendService>();

// PlayerHeartbeat Service
builder.Services.AddScoped<IPlayerHeartbeatService, PlayerHeartbeatService>();

// Chat Services
builder.Services.Configure<AzureContentSafetyOptions>(builder.Configuration.GetSection("AzureContentSafety"));
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IChatModerationRepository, ChatModerationRepository>();
builder.Services.AddScoped<IContentSafetyProvider, AzureContentSafetyProvider>();
builder.Services.AddScoped<IChatModerationService, ChatModerationService>();
builder.Services.AddScoped<IChatService, ChatService>();

// Character Services
builder.Services.AddScoped<IPlayerStatRepository, PlayerStatRepository>();
builder.Services.AddScoped<IClassConfigRepository, ClassConfigRepository>();
builder.Services.AddScoped<ICharacterService, CharacterService>();

// Account Admin Services
builder.Services.AddScoped<IAccountAdminService, AccountAdminService>();

// PlayerAchievement Services
builder.Services.AddScoped<IPlayerAchievementRepository, PlayerAchievementRepository>();

// Purchase History Services
builder.Services.AddScoped<IPurchaseHistoryRepository, PurchaseHistoryRepository>();
builder.Services.AddScoped<IPurchaseHistoryService, PurchaseHistoryService>();

// Sale Services
builder.Services.AddScoped<ISaleService, SaleService>();

// Dashboard Services
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Daily Login Reward Services
builder.Services.AddScoped<IDailyLoginRewardRepository, DailyLoginRewardRepository>();
builder.Services.AddScoped<IDailyLoginRewardService, DailyLoginRewardService>();

// Guild Services
builder.Services.AddScoped<IGuildRepository, GuildRepository>();
builder.Services.AddScoped<BLL.Services.Interfaces.IGuildService, BLL.Services.GuildService>();

// Wiki Services 
builder.Services.AddScoped<IWikiRepository, WikiRepository>();
builder.Services.AddScoped<IWikiService, WikiService>();

// Background Jobs
builder.Services.AddHostedService<Mystic_Journey_API.BackgroundJobs.GuildContributionResetJob>();

builder.Services.AddSignalR();
builder.Services.AddScoped<ISessionNotifier, Mystic_Journey_API.Hubs.SignalRSessionNotifier>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                else if (context.HttpContext.Request.Path.StartsWithSegments("/hubs/game"))
                {
                    var queryToken = context.Request.Query["access_token"].ToString();
                    if (!string.IsNullOrEmpty(queryToken))
                    {
                        context.Token = queryToken;
                    }
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = async context =>
            {
                var cache = context.HttpContext.RequestServices.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                if (cache != null)
                {
                    var userIdClaim = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var sidClaim = context.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sid)?.Value
                                ?? context.Principal?.FindFirst("sid")?.Value;
                    var clientTypeClaim = context.Principal?.FindFirst(BLL.Services.AuthService.ClientTypeClaim)?.Value;
                    if (!string.IsNullOrEmpty(userIdClaim) && !string.IsNullOrEmpty(sidClaim) && int.TryParse(userIdClaim, out int accountId))
                    {
                        var sessionKey = BLL.Services.AuthService.ActiveSessionKey(accountId, clientTypeClaim);
                        var activeSid = await cache.GetStringAsync(sessionKey);
                        if (activeSid != null && activeSid != sidClaim)
                        {
                            context.Fail("SESSION_OVERRIDDEN");
                        }
                    }
                }
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var isOverridden = context.AuthenticateFailure?.Message == "SESSION_OVERRIDDEN";
                var response = new BLL.DTOs.ApiResponse<object>
                {
                    Success = false,
                    Message = isOverridden 
                        ? "Your account has been logged in on another device." 
                        : "Unauthorized access. Please log in to continue.",
                    ErrorCode = isOverridden 
                        ? "SESSION_OVERRIDDEN" 
                        : Mystic_Journey_API.Extensions.ErrorCodes.Unauthorized
                };
                await context.Response.WriteAsJsonAsync(response);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var response = new BLL.DTOs.ApiResponse<object>
                {
                    Success = false,
                    Message = "Access denied. You do not have permission to access this resource.",
                    ErrorCode = Mystic_Journey_API.Extensions.ErrorCodes.Forbidden
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mystic Journey API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token. Example: eyJhbGciOi..."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:3001",
                "https://localhost:3000",
                "https://localhost:3001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next();

    if (!context.Response.HasStarted && context.Response.StatusCode >= 400 && context.Response.ContentType == null)
    {
        context.Response.ContentType = "application/json";
        var message = ApiExceptionFilter.GetDefaultStatusMessage(context.Response.StatusCode);
        var response = new BLL.DTOs.ApiResponse<object>
        {
            Success = false,
            Message = message,
            ErrorCode = $"HTTP_{context.Response.StatusCode}"
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<Mystic_Journey_API.Hubs.GameHub>("/hubs/game");

app.Run();
