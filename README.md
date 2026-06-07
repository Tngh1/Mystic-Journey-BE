# Mystic Journey — Backend API

RESTful API cho **Mystic Journey**, một MMORPG dark fantasy, xây dựng bằng .NET 8 và PostgreSQL.

---

## Tech Stack

| Hạng mục | Công nghệ |
|---|---|
| Runtime | .NET 8.0 |
| Web Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL (Npgsql) |
| Authentication | JWT Bearer — HttpOnly Cookie |
| Object Mapping | AutoMapper |
| Password Hashing | BCrypt |
| API Documentation | Swagger / OpenAPI |
| Email | SMTP |

---

## Authentication — HttpOnly Cookie

Token **không được trả về response body** và **không được lưu phía client**. Toàn bộ được quản lý qua `HttpOnly Secure Cookie`.

```
POST /api/accounts/login
  ↓  Backend validate email/password
  ↓  Set-Cookie: access_token=<jwt>; HttpOnly; Secure; SameSite=None
  ↓  Set-Cookie: refresh_token=<jwt>; HttpOnly; Secure; SameSite=None
  ↓  Response: { accountId, userName, email, role }
```

### Cookie Options (`AccountsController.cs`)

```csharp
new CookieOptions {
    HttpOnly = true,         // JavaScript không đọc được
    Secure   = true,         // Chỉ gửi qua HTTPS
    SameSite = SameSiteMode.None,  // Cho phép cross-origin (cần Secure=true)
    Path     = "/",
    Expires  = expiry
}
```

### JWT từ Cookie (`Program.cs`)

Backend đọc token trực tiếp từ cookie trong mỗi request — không yêu cầu header `Authorization`:

```csharp
OnMessageReceived = context => {
    var accessToken = context.Request.Cookies["access_token"];
    if (!string.IsNullOrEmpty(accessToken))
        context.Token = accessToken;
    return Task.CompletedTask;
}
```

### Refresh Token

```
POST /api/accounts/refresh-token
  ↓  Backend đọc refresh_token cookie
  ↓  Phát hành access_token mới
  ↓  Set lại cookie tự động
```

### Logout

```
POST /api/accounts/logout
  ↓  Backend revoke refresh token trong DB
  ↓  Xoá cả hai cookie (Set-Cookie với expires quá khứ)
```

---

## Architecture — 3-Layer Clean Architecture

```
Mystic-Journey-BE/
├── Mystic-Journey-API/      # Controllers, Program.cs, Filters
├── BLL/                     # Business Logic Layer
│   ├── DTOs/                # Request / Response DTOs
│   ├── Services/            # Service implementations
│   ├── Services/Interfaces/ # Service interfaces
│   ├── Mappings/            # AutoMapper profiles
│   └── CustomValidations/   # Password, UserName validators
└── DAL/                     # Data Access Layer
    ├── Models/              # Entity models
    ├── Data/                # DbContext
    └── Repositories/        # Repository pattern + interfaces
```

---

## API Controllers

| Controller | Base Path | Mô tả |
|---|---|---|
| `AccountsController` | `/api/accounts` | Login, Register, Logout, `/me`, Password reset, Email verify, Refresh token |
| `AdminAccountsController` | `/api/admin-accounts` | CRUD tài khoản, ban/unban |
| `PlayerProfilesController` | `/api/player-profiles` | Profile, stats, currency, experience |
| `ItemsController` | `/api/items` | Quản lý item (Admin CRUD) |
| `MonstersController` | `/api/monsters` | Quản lý monster (Admin CRUD) |
| `DungeonsController` | `/api/dungeons` | Dungeon config (Admin CRUD) |
| `ShopItemsController` | `/api/shop-items` | Quản lý shop (Admin CRUD) |
| `GachaBannersController` | `/api/gacha-banners` | Hệ thống gacha (Admin CRUD + Pull) |
| `QuestsController` | `/api/quests` | Quản lý quest (Admin CRUD) |
| `AchievementsController` | `/api/achievements` | Quản lý achievement (Admin CRUD) |
| `MailsController` | `/api/mails` | Hệ thống mail (Send/Read/Claim) |
| `DailyLoginRewardsController` | `/api/daily-login-rewards` | Phần thưởng đăng nhập hàng ngày |
| `PurchaseHistoriesController` | `/api/purchase-histories` | Lịch sử giao dịch |
| `DashboardController` | `/api/dashboard` | Thống kê admin |
| `GameSettingsController` | `/api/game-settings` | Cấu hình game runtime |
| `ContentsController` | `/api/contents` | CMS bài viết |
| `SalesController` | `/api/sales` | Báo cáo doanh thu |

---

## Role System

| Role | Mô tả |
|---|---|
| `Player` | Người chơi thông thường |
| `Admin` | Quản lý tính năng và cài đặt game |
| `SuperAdmin` | Toàn quyền hệ thống kể cả quản lý tài khoản |

- Endpoint yêu cầu xác thực: `[Authorize]`
- Endpoint công khai: `[AllowAnonymous]`
- Endpoint admin: `[Authorize(Roles = "Admin,SuperAdmin")]`

---

## Getting Started

### Yêu cầu

- .NET 8.0 SDK
- PostgreSQL 14+
- Visual Studio 2022 hoặc VS Code

### 1. Cấu hình

Tạo file `.env` trong thư mục `Mystic-Journey-API/`:

```env
DATABASE_URL=Host=localhost;Database=MysticJourneyDB;Username=postgres;Password=yourpassword
JWT__KEY=your-super-secret-key-min-32-chars
JWT__ISSUER=MysticJourneyAPI
JWT__AUDIENCE=MysticJourneyClient
SMTP__HOST=smtp.example.com
SMTP__USERNAME=your@email.com
SMTP__PASSWORD=yourpassword
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
- **HTTP:** `http://localhost:5176`

### 4. CORS

Các origin được phép: `http://localhost:3000`, `https://localhost:3000`, `http://localhost:3001`, `https://localhost:3001`.

Bắt buộc dùng `AllowCredentials()` vì FE gửi cookie (`withCredentials: true`).

---

## Business Rules

| Rule | Chi tiết |
|---|---|
| Level Up | `ExperienceRequired = 100 × level²` — mỗi level: +1 SkillPoint, +10 HP, +5 MP, +2 stat |
| Item Enhancement | Max level 15, success rate = `100 - (level × 5)%`, cost = `(level + 1) × 100 Gold` |
| Gacha Pity | Guaranteed featured item trong `PityLimit - 10` pulls |
| Daily Login | Streak-based, reset nếu bỏ lỡ một ngày |
| Soft Delete | Tất cả entity dùng `IsActive = false` thay vì hard delete |
