# Mystic Journey — Backend API

RESTful API cho **Mystic Journey**, một MMORPG dark fantasy. Xây dựng bằng .NET 8,
Entity Framework Core 8 và PostgreSQL, theo kiến trúc 3 lớp (Clean Architecture).

---

## Tech Stack

| Hạng mục | Công nghệ |
|---|---|
| Runtime | .NET 8.0 |
| Web Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL (Npgsql) |
| Cache | Redis (tùy chọn) — fallback in-memory nếu không cấu hình |
| Authentication | JWT trong HttpOnly Cookie |
| Object Mapping | AutoMapper |
| Password Hashing | BCrypt |
| Config | DotNetEnv (`.env`) |
| API Docs | Swagger / OpenAPI |
| Email | SMTP |
| Chat moderation | Azure Content Safety |

---

## Kiến trúc — 3-Layer Clean Architecture

Dependency hướng vào trong: `API` → `BLL` → `DAL`. Controller **không** truy cập
DAL trực tiếp.

```
Mystic-Journey-BE/
├── Mystic-Journey-API/        # Controllers, Program.cs, Filters (ApiExceptionFilter)
├── BLL/                       # Business Logic Layer
│   ├── DTOs/                  # Request / Response DTOs
│   ├── Services/              # Service + Interfaces/
│   ├── Mappings/              # AutoMapper profiles
│   └── CustomValidations/     # Validators
└── DAL/                       # Data Access Layer
    ├── Models/                # Entity models
    ├── Data/                  # MysticJourneyDbContext
    ├── Migrations/            # EF Core migrations
    └── Repositories/          # Repository pattern + Interfaces/
```

- **Unified response envelope:** mọi endpoint trả về `ApiResponse<T>` =
  `{ success, message, errorCode, data }`.
- **Soft delete:** entity dùng `IsActive = false`, không hard delete.
- **Phân trang:** `PagedResultDto<T>` = `{ totalCount, items }`.

---

## Authentication — JWT trong HttpOnly Cookie

Token **không** trả trong response body và **không** đọc từ header `Authorization`.
`Program.cs` đọc `access_token` từ cookie qua sự kiện `OnMessageReceived`:

```csharp
OnMessageReceived = context => {
    var accessToken = context.Request.Cookies["access_token"];
    if (!string.IsNullOrEmpty(accessToken))
        context.Token = accessToken;
    return Task.CompletedTask;
}
```

Cookie được set `HttpOnly` + `Secure` + `SameSite=None`, nên CORS bắt buộc
`AllowCredentials()` với danh sách origin cụ thể.

### Auth endpoints (`AuthController` — `/api/auth`)

| Method | Route | Mô tả |
|---|---|---|
| POST | `/api/auth/login` | Đăng nhập (kiểm tra tài khoản bị vô hiệu hóa/ban) |
| POST | `/api/auth/register` | Đăng ký (yêu cầu email đã xác thực OTP) |
| GET | `/api/auth/me` | Thông tin tài khoản hiện tại |
| POST | `/api/auth/change-password` | Đổi mật khẩu |
| POST | `/api/auth/logout` | Revoke refresh token + xóa cookie |
| POST | `/api/auth/refresh-token` | Cấp access token mới từ refresh token |
| POST | `/api/auth/forgot-password` | Gửi mã reset qua email |
| POST | `/api/auth/reset-password` | Đặt lại mật khẩu bằng mã |
| POST | `/api/auth/send-verification-code` | Gửi OTP xác thực email |
| POST | `/api/auth/verify-email` | Xác thực OTP |

> **Ban/deactivate:** tài khoản bị ban có `IsActive = false`. Login trả lỗi
> `"Account has been deactivated."` (repository trả cả tài khoản inactive để
> service kiểm tra trạng thái, thay vì báo nhầm "chưa đăng ký").

---

## API Controllers

Route `[Route("api/[controller]")]` → tên controller viết thường, **không có dấu
gạch nối** (vd `AdminAccountsController` → `/api/adminaccounts`).

### Quản trị (Admin / SuperAdmin)

| Controller | Base Path | Mô tả |
|---|---|---|
| `AdminAccountsController` | `/api/adminaccounts` | CRUD tài khoản, ban/unban |
| `PlayerProfilesController` | `/api/playerprofiles` | Profile, stats, currency |
| `ItemsController` | `/api/items` | CRUD item + equipment stats |
| `MonstersController` | `/api/monsters` | CRUD monster + drop table |
| `DungeonsController` | `/api/dungeons` | CRUD dungeon config |
| `ShopItemsController` | `/api/shopitems` | CRUD shop item |
| `GachaBannersController` | `/api/gachabanners` | CRUD banner gacha + tỉ lệ |
| `QuestsController` | `/api/quests` | CRUD quest |
| `AchievementsController` | `/api/achievements` | CRUD achievement |
| `SkillsController` | `/api/skills` | CRUD skill |
| `SkinsController` | `/api/skins` | CRUD skin |
| `MailsController` | `/api/mails` | Gửi/broadcast mail, list phân trang |
| `DailyLoginRewardsController` | `/api/dailyloginrewards` | Phần thưởng đăng nhập |
| `ContentsController` | `/api/contents` | CMS bài viết + block |
| `GameSettingsController` | `/api/gamesettings` | Cấu hình game runtime |
| `PurchaseHistoriesController` | `/api/purchasehistories` | Lịch sử mua hàng |
| `SalesController` | `/api/sales` | Lịch sử bán (per player) |
| `DashboardController` | `/api/dashboard` | Thống kê admin |

### Game client (người chơi)

| Controller | Base Path | Mô tả |
|---|---|---|
| `CharactersController` | `/api/characters` | Tạo & xem nhân vật |
| `PlayerSkillsController` | `/api/player-skills` | Skill của người chơi |
| `PlayerQuestsController` | `/api/playerquests` | Tiến trình quest |
| `InventoryController` | `/api/inventory` | Túi đồ, trang bị, enhance |
| `ShopController` | `/api/shop` | Mua bán phía người chơi |
| `CurrencyController` | `/api/currencies` | Gold / Gems |
| `WorldController` | `/api/world` | Dữ liệu bản đồ/thế giới |
| `FriendController` | `/api/friend` | Bạn bè |
| `ChatController` | `/api/chat` | Chat + kiểm duyệt |
| `PresenceController` | `/api/presence` | Trạng thái online |
| `SeedController` | `/api/seed` | Seed dữ liệu (dev) |

---

## Role System

| Role | Mô tả |
|---|---|
| `Player` | Người chơi thông thường |
| `Admin` | Quản lý tính năng và cài đặt game |
| `SuperAdmin` | Toàn quyền, kể cả quản lý tài khoản |

- Endpoint xác thực: `[Authorize]`
- Endpoint admin: `[Authorize(Roles = "Admin,SuperAdmin")]`

---

## Getting Started

### Yêu cầu

- .NET 8.0 SDK
- PostgreSQL 14+
- (Tùy chọn) Redis

### 1. Cấu hình `.env`

Tạo file `.env` trong `Mystic-Journey-API/` (được `DotNetEnv` nạp trong `Program.cs`):

```env
ConnectionStrings__DefaultConnection=Host=localhost;Database=MysticJourneyDB;Username=postgres;Password=yourpassword
Jwt__Key=your-super-secret-key-min-32-chars
Jwt__Issuer=MysticJourneyAPI
Jwt__Audience=MysticJourneyClient
Smtp__Host=smtp.example.com
Smtp__Port=587
Smtp__Username=your@email.com
Smtp__Password=yourpassword
Smtp__FromEmail=noreply@mysticjourney.com
Smtp__UseSSL=true
# Tùy chọn — nếu bỏ trống sẽ dùng in-memory cache
ConnectionStrings__Redis=localhost:6379
```

### 2. Database Migration

```bash
cd Mystic-Journey-API
dotnet ef migrations add <MigrationName> --project ..\DAL --startup-project .
dotnet ef database update --project ..\DAL --startup-project .
```

### 3. Chạy API

```bash
cd Mystic-Journey-API
dotnet run
```

- **Swagger UI:** `https://localhost:7116/swagger`

### 4. Build toàn bộ solution

```bash
dotnet build Mystic-Journey-BE.sln
```

---

## CORS

Origin được phép: `http://localhost:3000`, `https://localhost:3000`,
`http://localhost:3001`, `https://localhost:3001`.

Bắt buộc `AllowCredentials()` vì FE gửi cookie (`withCredentials: true`) và cookie
dùng `SameSite=None`.
