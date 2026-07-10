using BLL.Mappings;
using BLL.Services;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mystic_Journey_API.Filters;
using System.IO;
using System.Text;

LoadEnvIfExists(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
LoadEnvIfExists(Path.Combine(Directory.GetCurrentDirectory(), "Mystic-Journey-API", ".env"));

static void LoadEnvIfExists(string path)
{
    if (File.Exists(path))
    {
        Env.Load(path);
    }
}
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MysticJourneyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MemoryCache for OTP/verification (AuthService)
builder.Services.AddMemoryCache();

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

// GameSetting Services
builder.Services.AddScoped<IGameSettingRepository, GameSettingRepository>();
builder.Services.AddScoped<IGameSettingService, GameSettingService>();

// Content Services
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IContentService, ContentService>();

// Mail Services
builder.Services.AddScoped<IMailRepository, MailRepository>();
builder.Services.AddScoped<IMailService, MailService>();

// Inventory Services
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

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
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
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
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
