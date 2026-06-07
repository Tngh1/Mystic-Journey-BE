# Mystic Journey — Backend API

A RESTful API backend for **Mystic Journey**, a dark fantasy MMORPG, built with .NET 8 and PostgreSQL.

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8.0 |
| Web Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL (`Npgsql`) |
| Authentication | JWT Bearer (Access + Refresh tokens) |
| Object Mapping | AutoMapper |
| Password Hashing | BCrypt |
| API Documentation | Swagger / OpenAPI (Swashbuckle) |
| Email | SMTP (MailKit) |
| Validation | Data Annotations + Custom Validators |

## Architecture — 3-Layer Clean Architecture

```
Mystic-Journey-BE/
├── Mystic-Journey-API/     # Controllers, Program.cs, Middleware
├── BLL/                    # Business Logic Layer
│   ├── DTOs/               # 28 DTO files (Request / Response)
│   ├── Services/           # 16 service implementations
│   ├── Services/Interfaces/ # Service interfaces
│   ├── Mappings/           # AutoMapper profiles
│   └── CustomValidations/  # Password, UserName, MinimumAge attributes
└── DAL/                    # Data Access Layer
    ├── Models/             # 43 entity models
    ├── Data/               # DbContext
    └── Repositories/       # 12 repositories + interfaces
```

## Database Models (43 entities)

| Category | Entities |
|---|---|
| **Auth** | `Account`, `Role` |
| **Player** | `PlayerProfile`, `PlayerStat` |
| **Equipment** | `Item`, `EquipmentStats`, `InventoryItem` |
| **Skills** | `Skill`, `PlayerSkill` |
| **Combat** | `Monster`, `MonsterDrop` |
| **Dungeon** | `DungeonConfig`, `Chest`, `ChestItem`, `PlayerChest` |
| **Economy** | `ShopItem`, `PurchaseHistory`, `PlayerCurrencyLog` |
| **Gacha** | `GachaBanner`, `GachaBannerItem`, `GachaPullHistory` |
| **Quests** | `Quest`, `PlayerQuest` |
| **Achievements** | `Achievement`, `PlayerAchievement` |
| **Social** | `Friend`, `ChatMessage`, `Guild`, `GuildMember`, `GuildInvitation` |
| **Content** | `Skin`, `PlayerSkin`, `NPC`, `NPCDialogue` |
| **Systems** | `Mail`, `GameAnnouncement`, `PlayerAnnouncement` |
| **Config** | `GameSetting`, `DailyLoginReward`, `PlayerDailyLogin`, `Content`, `CategoryContent`, `BlockContent` |

## API Controllers (17)

| Controller | Base Path | Purpose |
|---|---|---|
| `AccountsController` | `/api/Accounts` | Login, Register, Password reset, Email verification |
| `AdminAccountsController` | `/api/AdminAccounts` | Admin CRUD for accounts |
| `PlayerProfilesController` | `/api/PlayerProfiles` | Player profile, stats, currency, experience |
| `ItemsController` | `/api/Items` | Item catalog (Admin CRUD) |
| `MonstersController` | `/api/Monsters` | Monster management (Admin CRUD) |
| `DungeonsController` | `/api/Dungeons` | Dungeon config (Admin CRUD) |
| `ShopItemsController` | `/api/ShopItems` | Shop management (Admin CRUD) |
| `GachaBannersController` | `/api/GachaBanners` | Gacha system (Admin CRUD + Pull) |
| `QuestsController` | `/api/Quests` | Quest management (Admin CRUD) |
| `AchievementsController` | `/api/Achievements` | Achievement management (Admin CRUD) |
| `MailsController` | `/api/Mails` | Mail system (Send/Read/Claim) |
| `DailyLoginRewardsController` | `/api/DailyLoginRewards` | Daily login rewards (Admin CRUD) |
| `PurchaseHistoriesController` | `/api/PurchaseHistories` | Purchase history |
| `DashboardController` | `/api/Dashboard` | Admin dashboard statistics |
| `GameSettingsController` | `/api/GameSettings` | Game config (Admin CRUD) |
| `ContentsController` | `/api/Contents` | CMS content management |
| `SalesController` | `/api/Sales` | Sales reports |

## Authentication & Authorization

- **JWT Bearer tokens** with symmetric key signing
- Access token + Refresh token pattern
- Role-based authorization: `Player` (1), `Admin` (2), `Super Admin` (3)
- Unauthenticated endpoints use `[AllowAnonymous]`
- SMTP email service for verification codes

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL 14+
- Visual Studio 2022 or VS Code

### 1. Clone & Configure

```bash
cd Mystic-Journey-BE
```

Create a `.env` file (or set environment variables directly):

```
DATABASE_URL=Host=localhost;Database=MysticJourneyDB;Username=postgres;Password=yourpassword
JWT__KEY=your-super-secret-key-min-32-chars
JWT__ISSUER=MysticJourneyAPI
JWT__AUDIENCE=MysticJourneyClient
SMTP__HOST=smtp.example.com
SMTP__USERNAME=your@email.com
SMTP__PASSWORD=yourpassword
```

Or update `appsettings.json` / `appsettings.Development.json` directly.

### 2. Database Migrations

```bash
# Navigate to API project
cd Mystic-Journey-API

# Create migration (if needed)
dotnet ef migrations add InitialCreate --project ..\DAL --startup-project .

# Apply migrations
dotnet ef database update --project ..\DAL --startup-project .
```

### 3. Run

```bash
cd Mystic-Journey-API
dotnet run
```

API is available at:
- **Swagger UI:** `https://localhost:5001/swagger`
- **HTTP:** `http://localhost:5000`

### 4. CORS

Frontend origin `http://localhost:3000` and `http://localhost:3001` (both http/https) are allowed by default.

## DTOs — 28 Separate Files

All DTOs live in `BLL/DTOs/` and are organized by domain:

| File | Domain |
|---|---|
| `AccountDTO.cs` | Login, Register, Password, Token |
| `AccountAdminDTO.cs` | Admin account management |
| `PlayerProfileDTO.cs` | Player profile & stats |
| `ItemDTO.cs` | Item catalog |
| `MonsterDTO.cs` | Monster & monster drops |
| `DungeonDTO.cs` | Dungeon configuration |
| `ShopDTO.cs` | Shop items & purchase history |
| `GachaDTO.cs` | Gacha banners |
| `GachaHistoryDTO.cs` | Gacha pull history |
| `QuestDTO.cs` | Quest definitions |
| `AchievementDTO.cs` | Achievement definitions |
| `GameSettingDTO.cs` | Game settings |
| `ContentDTO.cs` | CMS: Content, Category, Block |
| `MailDTO.cs` | Mail system |
| `DailyLoginRewardDTO.cs` | Daily login rewards |
| `GuildDTO.cs` | Guild, GuildMember, Invitation |
| `ChatDTO.cs` | Chat messages & Friends |
| `ChestDTO.cs` | Chest & chest items |
| `SkillDTO.cs` | Skill & player skill |
| `SkinDTO.cs` | Skin & player skin |
| `PlayerQuestDTO.cs` | Player quest progress |
| `PlayerLoginRewardDTO.cs` | Player daily login |
| `CurrencyLogDTO.cs` | Currency transaction log |
| `PlayerAchievementDTO.cs` | Player achievement progress |
| `NPCDTO.cs` | NPC & dialogues |
| `AnnouncementDTO.cs` | Game announcements |
| `InventoryDTO.cs` | Inventory items |
| `EquipmentStatsDTO.cs` | Equipment stat definitions |
| `CommonDTO.cs` | Dashboard stats, Paginated response |

## Key Business Rules

- **Level Up:** `ExperienceRequired = 100 * level * level`. Each level grants +1 SkillPoint, +10 HP, +5 MP, +2 primary stats.
- **Item Enhancement:** Max level 15. Success rate = `100 - (level * 5)%`. Cost = `(level + 1) * 100 Gold`.
- **Gacha Pity:** Guaranteed featured item within `PityLimit - 10` pulls.
- **Daily Login:** Streak-based reward cycle. Resets if a day is missed.
- **Soft Delete:** All entities use `IsActive` flag instead of hard delete. No hard-delete endpoints exist.
