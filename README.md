# Mystic Journey - Backend API Documentation

Tài liệu này mô tả chi tiết về các API của game Mystic Journey, bao gồm business logic, DTOs sử dụng, validation, và mục đích của từng endpoint.

## Mục lục

1. [Accounts API](#1-accounts-api---quản-lý-tài-khoản)
2. [PlayerProfiles API](#2-playerprofiles-api---quản-lý-hồ-sơ-người-chơi)
3. [Skills API](#3-skills-api---quản-lý-kỹ-năng)
4. [Inventories API](#4-inventories-api---quản-lý-túi-đồ)
5. [Items API](#5-items-api---quản-lý-vật-phẩm)
6. [Shops API](#6-shops-api---cửa-hàng)
7. [Quests API](#7-quests-api---nhiệm-vụ)
8. [Mails API](#8-mails-api---thư-tin-nhắn)
9. [Friends API](#9-friends-api---bạn-bè)
10. [Gacha API](#10-gacha-api---quay-thưởng)

---

## 1. Accounts API - Quản lý Tài khoản

### Mục đích
Xử lý các chức năng liên quan đến tài khoản người dùng như đăng ký, đăng nhập, quên mật khẩu, và xác thực email.

### Base URL
```
/api/Accounts
```

### Các Endpoint

#### 1.1. POST /api/Accounts/login
**Mục đích:** Đăng nhập người dùng vào hệ thống.

**Business Logic:**
- Xác thực thông tin đăng nhập (email/username và password)
- Kiểm tra tài khoản có tồn tại và đang hoạt động
- Xác minh mật khẩu sử dụng BCrypt
- Tạo JWT Access Token và Refresh Token
- Cập nhật thời gian đăng nhập cuối và refresh token

**DTO Request:**
```json
{
  "EmailOrUsername": "string",
  "Password": "string"
}
```

**DTO Response:**
```json
{
  "Success": true,
  "Message": "string",
  "Account": {
    "AccountId": "guid",
    "FullName": "string",
    "UserName": "string",
    "EmailAddress": "string",
    "Role": "string",
    "AccessToken": "string",
    "AccessTokenExpiresAt": "datetime",
    "RefreshToken": "string",
    "RefreshTokenExpiresAt": "datetime"
  }
}
```

**Validation:**
- EmailOrUsername: bắt buộc, tối đa 255 ký tự
- Password: bắt buộc

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 1.2. POST /api/Accounts/register
**Mục đích:** Đăng ký tài khoản mới.

**Business Logic:**
- Kiểm tra các trường bắt buộc
- So sánh password và confirmPassword
- Kiểm tra email và username chưa tồn tại
- Mã hóa mật khẩu bằng BCrypt
- Tạo tài khoản với role mặc định là Player
- Gửi mã xác thực email tự động
- Tạo JWT tokens sau khi đăng ký thành công

**DTO Request:**
```json
{
  "FullName": "string",
  "UserName": "string",
  "EmailAddress": "string",
  "Password": "string",
  "ConfirmPassword": "string",
  "Gender": "string (Male, Female, Other)",
  "PhoneNumber": "string?",
  "Birthday": "date?"
}
```

**Validation:**
- FullName: bắt buộc, tối đa 200 ký tự
- UserName: bắt buộc, theo custom validation [UserName]
- EmailAddress: bắt buộc, định dạng email hợp lệ
- Password: bắt buộc, theo custom validation [Password]
- ConfirmPassword: phải khớp với Password
- Birthday: tuổi tối thiểu 13 tuổi

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 1.3. POST /api/Accounts/forgot-password
**Mục đích:** Gửi mã xác thực để đặt lại mật khẩu.

**Business Logic:**
- Kiểm tra email tồn tại và tài khoản đang hoạt động
- Yêu cầu email đã được xác thực trước khi reset
- Tạo mã xác thực 6 số
- Lưu mã và thời hạn (15 phút) vào database
- Gửi email chứa mã xác thực

**DTO Request:**
```json
{
  "Email": "string"
}
```

**DTO Response:**
```json
{
  "Success": true,
  "Message": "A verification code has been sent to your email."
}
```

**Validation:**
- Email: bắt buộc, định dạng email hợp lệ

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 1.4. POST /api/Accounts/reset-password
**Mục đích:** Đặt lại mật khẩu mới sau khi xác thực.

**Business Logic:**
- Xác minh email và mã xác thực
- Kiểm tra mã chưa hết hạn (15 phút)
- So sánh password và confirmPassword
- Cập nhật mật khẩu mới (đã mã hóa BCrypt)
- Xóa mã xác thực sau khi sử dụng

**DTO Request:**
```json
{
  "Email": "string",
  "VerificationCode": "string (6 digits)",
  "NewPassword": "string",
  "ConfirmPassword": "string"
}
```

**Validation:**
- Email: bắt buộc, định dạng email
- VerificationCode: bắt buộc, 6 chữ số
- NewPassword: bắt buộc, theo custom validation [Password]
- ConfirmPassword: phải khớp với NewPassword

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 1.5. POST /api/Accounts/send-verification-code
**Mục đích:** Gửi lại mã xác thực email.

**Business Logic:**
- Tạo mã xác thực 6 số ngẫu nhiên
- Lưu mã và thời hạn vào database
- Gửi email với mã xác thực

**DTO Request:**
```json
{
  "Email": "string"
}
```

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 1.6. POST /api/Accounts/verify-email
**Mục đích:** Xác thực email của tài khoản.

**Business Logic:**
- Xác minh email và mã xác thực
- Kiểm tra mã chưa hết hạn
- Cập nhật trạng thái EmailConfirmed = true
- Xóa mã xác thực sau khi sử dụng

**DTO Request:**
```json
{
  "Email": "string",
  "VerificationCode": "string (6 digits)"
}
```

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 1.7. POST /api/Accounts/change-password
**Mục đích:** Thay đổi mật khẩu khi đã đăng nhập.

**Business Logic:**
- Xác minh mật khẩu hiện tại
- Cập nhật mật khẩu mới (đã mã hóa BCrypt)

**DTO Request:**
```json
{
  "CurrentPassword": "string",
  "NewPassword": "string",
  "ConfirmPassword": "string"
}
```

**Validation:**
- CurrentPassword: bắt buộc
- NewPassword: bắt buộc, theo custom validation [Password]
- ConfirmPassword: phải khớp với NewPassword

**Authentication:** Yêu cầu (Authorize)

---

## 2. PlayerProfiles API - Quản lý Hồ sơ Người chơi

### Mục đích
Quản lý hồ sơ người chơi trong game, bao gồm tạo nhân vật, quản lý tiền tệ, năng lượng và kinh nghiệm.

### Base URL
```
/api/PlayerProfiles
```

### Các Endpoint

#### 2.1. POST /api/PlayerProfiles/create
**Mục đích:** Tạo hồ sơ người chơi mới (nhân vật game).

**Business Logic:**
- Kiểm tra tài khoản chưa có hồ sơ
- Tạo hồ sơ với các thông số khởi tạo:
  - Level: 1
  - ExperiencePoints: 0
  - Gold: 100 (tiền khởi đầu)
  - Gems: 10 (đá quý khởi đầu)
  - Energy: 100
- Tạo PlayerStats với các chỉ số ban đầu:
  - Health: 100, Mana: 50
  - Strength: 10, Defense: 10, Agility: 10, Intelligence: 10, Endurance: 10
  - SkillPoints: 5

**DTO Request:**
```json
{
  "DisplayName": "string",
  "AvatarUrl": "string",
  "Class": "string (Knight, Mage, Archer, ...)"
}
```

**DTO Response:**
```json
{
  "Success": true,
  "Message": "Player profile created successfully.",
  "Data": {
    "ProfileId": "guid",
    "AccountId": "guid",
    "DisplayName": "string",
    "AvatarUrl": "string",
    "Class": "string",
    "Level": 1,
    "Gold": 100,
    "Gems": 10,
    "Energy": 100
  }
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 2.2. GET /api/PlayerProfiles
**Mục đích:** Lấy thông tin hồ sơ người chơi cơ bản.

**Business Logic:**
- Truy xuất hồ sơ theo AccountId từ JWT token

**DTO Response:**
```json
{
  "Success": true,
  "Data": { /* PlayerProfileResponseDto */ }
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 2.3. GET /api/PlayerProfiles/details
**Mục đích:** Lấy thông tin hồ sơ chi tiết kèm theo chỉ số nhân vật.

**Business Logic:**
- Truy xuất hồ sơ với đầy đủ thông tin và PlayerStats
- Tính toán ExperienceToNextLevel

**DTO Response:**
```json
{
  "Success": true,
  "Detail": {
    /* PlayerProfileDetailResponseDto với Stats */
  }
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 2.4. PUT /api/PlayerProfiles
**Mục đích:** Cập nhật thông tin hồ sơ người chơi.

**Business Logic:**
- Cập nhật DisplayName, AvatarUrl, Class nếu được cung cấp
- Validate Class nếu có thay đổi

**DTO Request:**
```json
{
  "DisplayName": "string?",
  "AvatarUrl": "string?",
  "Class": "string?"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 2.5. GET /api/PlayerProfiles/stats
**Mục đích:** Lấy chỉ số nhân vật (stats) của người chơi.

**DTO Response:**
```json
{
  "Success": true,
  "Data": {
    "StatsId": "guid",
    "Health": 100,
    "Mana": 50,
    "Strength": 10,
    "Defense": 10,
    "Agility": 10,
    "Intelligence": 10,
    "Endurance": 10,
    "Luck": 0,
    "SkillPoints": 5,
    /* ... các chỉ số khác */
  }
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 2.6. GET /api/PlayerProfiles/currency
**Mục đích:** Lấy thông tin tiền tệ hiện tại của người chơi.

**DTO Response:**
```json
{
  "Success": true,
  "Data": {
    "Gold": 100,
    "Gems": 10,
    "Energy": 100,
    "MaxEnergy": 100
  }
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 2.7. POST /api/PlayerProfiles/currency/add
**Mục đích:** Thêm tiền tệ cho người chơi.

**Business Logic:**
- Kiểm tra Amount > 0
- Cộng vào Gold hoặc Gems tùy CurrencyType

**DTO Request:**
```json
{
  "CurrencyType": "int (0=Gold, 1=Gems)",
  "Amount": "decimal"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 2.8. POST /api/PlayerProfiles/currency/spend
**Mục đích:** Tiêu tiền tệ của người chơi.

**Business Logic:**
- Kiểm tra Amount > 0
- Kiểm tra số dư đủ
- Trừ Gold hoặc Gems tùy CurrencyType

**Authentication:** Yêu cầu (Authorize)

---

#### 2.9. POST /api/PlayerProfiles/energy/{change}
**Mục đích:** Cập nhật năng lượng (tăng/giảm).

**Business Logic:**
- Cộng/trừ năng lượng theo giá trị change
- Giới hạn trong khoảng 0-100

**Authentication:** Yêu cầu (Authorize)

---

#### 2.10. POST /api/PlayerProfiles/experience/{amount}
**Mục đích:** Thêm kinh nghiệm và xử lý level up.

**Business Logic:**
- Cộng kinh nghiệm vào tài khoản
- Kiểm tra nếu đủ kinh nghiệm thì level up
- Khi level up:
  - Cộng 1 SkillPoint
  - Tăng Health +10, Mana +5
  - Tăng Strength/Defense/Agility +2
  - Trừ kinh nghiệm cần thiết
- Công thức kinh nghiệm cần cho level: `100 * level * level`

**Authentication:** Yêu cầu (Authorize)

---

#### 2.11. GET /api/PlayerProfiles/exists
**Mục đích:** Kiểm tra tài khoản đã có hồ sơ chưa.

**Authentication:** Không yêu cầu (AllowAnonymous)

---

## 3. Skills API - Quản lý Kỹ năng

### Mục đích
Quản lý kỹ năng của nhân vật: xem, mở khóa, nâng cấp, trang bị.

### Base URL
```
/api/Skills
```

### Các Endpoint

#### 3.1. GET /api/Skills
**Mục đích:** Lấy danh sách tất cả kỹ năng đang hoạt động.

**DTO Response:**
```json
{
  "Success": true,
  "Skills": [
    {
      "SkillId": "guid",
      "Name": "string",
      "Description": "string",
      "Category": "string",
      "DamageType": "string",
      "TargetType": "string",
      "ClassRequirement": "string",
      "ManaCost": 10,
      "CooldownSeconds": 30,
      "BaseDamage": 50,
      "UnlockLevel": 1
    }
  ],
  "TotalCount": 10
}
```

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 3.2. GET /api/Skills/class/{classType}
**Mục đích:** Lấy kỹ năng theo lớp nhân vật.

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 3.3. GET /api/Skills/available
**Mục đích:** Lấy kỹ năng có thể mở khóa dựa trên level và class của người chơi.

**Business Logic:**
- Lọc kỹ năng theo UnlockLevel <= PlayerLevel
- Lọc theo ClassRequirement (Knight hoặc class hiện tại)

**Authentication:** Yêu cầu (Authorize)

---

#### 3.4. GET /api/Skills/player
**Mục đích:** Lấy danh sách kỹ năng đã sở hữu của người chơi.

**DTO Response:**
```json
{
  "Success": true,
  "PlayerSkills": [
    {
      "PlayerSkillId": "guid",
      "SkillId": "guid",
      "SkillName": "string",
      "Level": 1,
      "Experience": 0,
      "IsEquipped": false,
      /* ... */
    }
  ]
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 3.5. GET /api/Skills/equipped
**Mục đích:** Lấy danh sách kỹ năng đang được trang bị.

**Authentication:** Yêu cầu (Authorize)

---

#### 3.6. POST /api/Skills/unlock
**Mục đích:** Mở khóa kỹ năng mới.

**Business Logic:**
- Kiểm tra người chơi chưa sở hữu kỹ năng này
- Kiểm tra PlayerLevel >= UnlockLevel
- Kiểm tra ClassRequirement phù hợp
- Tạo PlayerSkill mới với Level = 1, Experience = 0

**DTO Request:**
```json
{
  "SkillId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 3.7. POST /api/Skills/upgrade
**Mục đích:** Nâng cấp kỹ năng.

**Business Logic:**
- Kiểm tra người chơi có đủ SkillPoints (>=1)
- Kiểm tra kỹ năng chưa đạt level tối đa (20)
- Thêm kinh nghiệm vào kỹ năng: `Level * 100`
- Trừ 1 SkillPoint
- Kiểm tra level up nếu đủ kinh nghiệm

**DTO Request:**
```json
{
  "PlayerSkillId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 3.8. POST /api/Skills/equip
**Mục đích:** Trang bị kỹ năng.

**Business Logic:**
- Kiểm tra kỹ năng chưa được trang bị
- Cập nhật IsEquipped = true

**DTO Request:**
```json
{
  "PlayerSkillId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 3.9. POST /api/Skills/unequip/{playerSkillId}
**Mục đích:** Gỡ kỹ năng khỏi trang bị.

**Business Logic:**
- Kiểm tra kỹ năng đang được trang bị
- Cập nhật IsEquipped = false

**Authentication:** Yêu cầu (Authorize)

---

## 4. Inventories API - Quản lý Túi đồ

### Mục đích
Quản lý túi đồ của người chơi: xem vật phẩm, thêm, xóa, trang bị, nâng cấp.

### Base URL
```
/api/Inventories
```

### Các Endpoint

#### 4.1. GET /api/Inventories
**Mục đích:** Lấy danh sách vật phẩm trong túi đồ.

**Business Logic:**
- Hỗ trợ phân trang (mặc định 50 item/trang)

**DTO Response:**
```json
{
  "Success": true,
  "Items": [
    {
      "InventoryItemId": "guid",
      "ItemId": "guid",
      "ItemName": "string",
      "ItemType": "string",
      "ItemRarity": "string",
      "Quantity": 5,
      "IsEquipped": false,
      "EnhancementLevel": 0
    }
  ],
  "TotalCount": 100,
  "PageNumber": 1,
  "PageSize": 50
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 4.2. GET /api/Inventories/equipped
**Mục đích:** Lấy danh sách vật phẩm đang được trang bị.

**Authentication:** Yêu cầu (Authorize)

---

#### 4.3. GET /api/Inventories/{inventoryItemId}
**Mục đích:** Lấy chi tiết một vật phẩm trong túi đồ.

**Business Logic:**
- Trả về thông tin chi tiết bao gồm EquipmentStats

**Authentication:** Yêu cầu (Authorize)

---

#### 4.4. POST /api/Inventories/add
**Mục đích:** Thêm vật phẩm vào túi đồ.

**Business Logic:**
- Kiểm tra Item tồn tại
- Xử lý stack: nếu đã có item cùng loại và chưa full stack
- Nếu không stack được, tạo item mới
- Hỗ trợ thêm nhiều items cùng lúc

**DTO Request:**
```json
{
  "ItemId": "guid",
  "Quantity": 1
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 4.5. POST /api/Inventories/remove
**Mục đích:** Xóa vật phẩm khỏi túi đồ.

**Business Logic:**
- Kiểm tra item thuộc về người chơi
- Kiểm tra item không được trang bị
- Giảm số lượng hoặc xóa nếu hết

**DTO Request:**
```json
{
  "InventoryItemId": "guid",
  "Quantity": 1
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 4.6. POST /api/Inventories/equip
**Mục đích:** Trang bị vật phẩm.

**Business Logic:**
- Kiểm tra item là loại có thể trang bị (Weapon, Armor, Accessory)
- Kiểm tra item chưa được trang bị
- Gỡ tất cả items cùng slot trước khi trang bị
- Cập nhật IsEquipped = true

**DTO Request:**
```json
{
  "InventoryItemId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 4.7. POST /api/Inventories/unequip
**Mục đích:** Gỡ vật phẩm khỏi trang bị.

**Business Logic:**
- Kiểm tra item đang được trang bị
- Cập nhật IsEquipped = false

**DTO Request:**
```json
{
  "InventoryItemId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 4.8. POST /api/Inventories/enhance
**Mục đích:** Nâng cấp vật phẩm (enhancement).

**Business Logic:**
- Kiểm tra EnhancementLevel < 15 (max level)
- Tính chi phí: `(EnhancementLevel + 1) * 100 Gold`
- Kiểm tra đủ Gold
- Tính tỷ lệ thành công: `100 - (EnhancementLevel * 5)%`
- Trừ Gold dù thành công hay thất bại
- Nếu thành công: tăng EnhancementLevel

**DTO Request:**
```json
{
  "InventoryItemId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

## 5. Items API - Quản lý Vật phẩm

### Mục đích
Quản lý danh sách vật phẩm trong game (chỉ Admin có quyền tạo/sửa/xóa).

### Base URL
```
/api/Items
```

### Các Endpoint

#### 5.1. GET /api/Items
**Mục đích:** Lấy danh sách tất cả vật phẩm.

**Business Logic:**
- Hỗ trợ phân trang
- Chỉ trả về items đang hoạt động (IsActive = true)

**DTO Response:**
```json
{
  "Success": true,
  "Items": [
    {
      "ItemId": "guid",
      "Name": "string",
      "Description": "string",
      "Type": "string",
      "Rarity": "string",
      "Slot": "string",
      "BaseValue": 100,
      "MaxStack": 99,
      "IsTradable": true
    }
  ],
  "TotalCount": 50
}
```

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 5.2. GET /api/Items/type/{type}
**Mục đích:** Lọc vật phẩm theo loại (Weapon, Armor, Accessory, Consumable, Material).

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 5.3. GET /api/Items/rarity/{rarity}
**Mục đích:** Lọc vật phẩm theo độ hiếm (Common, Uncommon, Rare, Epic, Legendary).

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 5.4. GET /api/Items/search
**Mục đích:** Tìm kiếm vật phẩm theo tên.

**Business Logic:**
- Tìm kiếm không phân biệt hoa thường

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 5.5. GET /api/Items/{id}
**Mục đích:** Lấy thông tin chi tiết một vật phẩm.

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 5.6. GET /api/Items/{id}/detail
**Mục đích:** Lấy thông tin chi tiết với EquipmentStats.

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 5.7. POST /api/Items
**Mục đích:** Tạo vật phẩm mới (Admin only).

**Business Logic:**
- Validate Type và Rarity hợp lệ
- Tạo EquipmentStats nếu là vật phẩm trang bị

**DTO Request:**
```json
{
  "Name": "string",
  "Description": "string?",
  "Type": "int",
  "Rarity": "int",
  "Slot": "int",
  "BaseValue": 100,
  "MaxStack": 99,
  "IsTradable": true,
  "IconUrl": "string?",
  "Stats": {
    "HealthBonus": 10,
    "StrengthBonus": 5,
    /* ... */
  }
}
```

**Authentication:** Yêu cầu (Authorize), Role = Admin

---

#### 5.8. PUT /api/Items/{id}
**Mục đích:** Cập nhật vật phẩm (Admin only).

**Authentication:** Yêu cầu (Authorize), Role = Admin

---

#### 5.9. DELETE /api/Items/{id}
**Mục đích:** Xóa vật phẩm (soft delete - Admin only).

**Business Logic:**
- Không xóa vĩnh viễn mà chỉ đặt IsActive = false

**Authentication:** Yêu cầu (Authorize), Role = Admin

---

## 6. Shops API - Cửa hàng

### Mục đích
Quản lý cửa hàng trong game: xem items, mua hàng, lịch sử mua hàng.

### Base URL
```
/api/Shops
```

### Các Endpoint

#### 6.1. GET /api/Shops
**Mục đích:** Lấy danh sách tất cả items trong cửa hàng.

**DTO Response:**
```json
{
  "Success": true,
  "Items": [
    {
      "ShopItemId": "guid",
      "ItemId": "guid",
      "ItemName": "string",
      "ItemRarity": "string",
      "Currency": "string",
      "Price": 100,
      "Stock": 50,
      "DailyPurchaseLimit": 3
    }
  ]
}
```

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 6.2. GET /api/Shops/available
**Mục đích:** Lấy items đang được bán (trong thời gian hiện tại).

**Business Logic:**
- Kiểm tra IsActive = true
- Kiểm tra AvailableFrom <= Now
- Kiểm tra AvailableTo >= Now

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 6.3. GET /api/Shops/{shopItemId}
**Mục đích:** Lấy thông tin chi tiết một shop item.

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 6.4. POST /api/Shops/purchase
**Mục đích:** Mua vật phẩm từ cửa hàng.

**Business Logic:**
- Kiểm tra item đang hoạt động và trong thời gian bán
- Kiểm tra Stock còn đủ
- Kiểm tra DailyPurchaseLimit (nếu có)
- Tính tổng giá = Price * Quantity
- Kiểm tra số dư (Gold hoặc Gems)
- Trừ tiền từ tài khoản
- Tạo PurchaseHistory
- Giảm Stock
- Thêm item vào Inventory

**DTO Request:**
```json
{
  "ShopItemId": "guid",
  "Quantity": 1
}
```

**DTO Response:**
```json
{
  "Success": true,
  "Message": "Successfully purchased 1x Iron Sword!",
  "Currency": {
    "Gold": 500,
    "Gems": 10
  }
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 6.5. GET /api/Shops/history
**Mục đích:** Lấy lịch sử mua hàng của người chơi.

**Business Logic:**
- Hỗ trợ phân trang

**Authentication:** Yêu cầu (Authorize)

---

## 7. Quests API - Nhiệm vụ

### Mục đích
Quản lý nhiệm vụ: xem quests, nhận quest, cập nhật tiến độ, nhận thưởng.

### Base URL
```
/api/Quests
```

### Các Endpoint

#### 7.1. GET /api/Quests
**Mục đích:** Lấy danh sách tất cả nhiệm vụ.

**DTO Response:**
```json
{
  "Success": true,
  "Quests": [
    {
      "QuestId": "guid",
      "Title": "string",
      "Description": "string",
      "Type": "string",
      "RequiredLevel": 5,
      "RewardExperience": 100,
      "RewardGold": 50,
      "RewardGems": 10,
      "RewardItemId": "guid?",
      "RewardItemName": "string?"
    }
  ]
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 7.2. GET /api/Quests/type/{type}
**Mục đích:** Lọc nhiệm vụ theo loại (Main, Side, Daily, Weekly).

**Authentication:** Yêu cầu (Authorize)

---

#### 7.3. GET /api/Quests/available
**Mục đích:** Lấy nhiệm vụ có thể nhận dựa trên level người chơi.

**Authentication:** Yêu cầu (Authorize)

---

#### 7.4. GET /api/Quests/player
**Mục đích:** Lấy tất cả nhiệm vụ đã nhận của người chơi.

**DTO Response:**
```json
{
  "Success": true,
  "PlayerQuests": [
    {
      "PlayerQuestId": "guid",
      "QuestId": "guid",
      "QuestTitle": "string",
      "Status": "InProgress",
      "Progress": 3,
      "TargetValue": 10,
      "RewardExperience": 100,
      "RewardGold": 50
    }
  ]
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 7.5. GET /api/Quests/active
**Mục đích:** Lấy nhiệm vụ đang thực hiện (InProgress).

**Authentication:** Yêu cầu (Authorize)

---

#### 7.6. GET /api/Quests/completed
**Mục đích:** Lấy nhiệm vụ đã hoàn thành (Completed).

**Authentication:** Yêu cầu (Authorize)

---

#### 7.7. POST /api/Quests/accept
**Mục đích:** Nhận nhiệm vụ.

**Business Logic:**
- Kiểm tra chưa nhận quest này
- Kiểm tra PlayerLevel >= RequiredLevel
- Tạo PlayerQuest với Status = InProgress, Progress = 0

**DTO Request:**
```json
{
  "QuestId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 7.8. POST /api/Quests/progress
**Mục đích:** Cập nhật tiến độ nhiệm vụ.

**Business Logic:**
- Kiểm tra quest đang InProgress
- Cộng Progress theo ProgressAmount
- Nếu Progress >= TargetValue: đặt Status = Completed

**DTO Request:**
```json
{
  "PlayerQuestId": "guid",
  "ProgressAmount": 1
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 7.9. POST /api/Quests/claim
**Mục đích:** Nhận thưởng nhiệm vụ.

**Business Logic:**
- Kiểm tra quest đã Completed
- Cộng Gold, Gems vào tài khoản
- Cộng Experience (sẽ trigger level up nếu đủ)
- Đặt Status = Claimed

**DTO Request:**
```json
{
  "PlayerQuestId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

## 8. Mails API - Thư/Tin nhắn

### Mục đích
Quản lý hệ thống thư trong game: xem thư, đọc, nhận quà, gửi thư.

### Base URL
```
/api/Mails
```

### Các Endpoint

#### 8.1. GET /api/Mails
**Mục đích:** Lấy danh sách thư của người chơi.

**Business Logic:**
- Hỗ trợ phân trang
- Trả về số thư chưa đọc

**DTO Response:**
```json
{
  "Success": true,
  "Mails": [
    {
      "MailId": "guid",
      "Title": "string",
      "Content": "string",
      "Type": "System",
      "AttachedGold": 100,
      "AttachedGems": 10,
      "AttachedItemName": "Iron Sword",
      "IsRead": false,
      "IsClaimed": false
    }
  ],
  "TotalCount": 20,
  "UnreadCount": 5
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 8.2. GET /api/Mails/unread
**Mục đích:** Lấy danh sách thư chưa đọc.

**Authentication:** Yêu cầu (Authorize)

---

#### 8.3. GET /api/Mails/unread/count
**Mục đích:** Lấy số lượng thư chưa đọc.

**Authentication:** Yêu cầu (Authorize)

---

#### 8.4. GET /api/Mails/{mailId}
**Mục đích:** Lấy chi tiết một thư.

**Business Logic:**
- Tự động đánh dấu đã đọc khi xem

**Authentication:** Yêu cầu (Authorize)

---

#### 8.5. POST /api/Mails/{mailId}/read
**Mục đích:** Đánh dấu thư đã đọc.

**Authentication:** Yêu cầu (Authorize)

---

#### 8.6. POST /api/Mails/{mailId}/claim
**Mục đích:** Nhận quà đính kèm trong thư.

**Business Logic:**
- Kiểm tra thư chưa được claim
- Kiểm tra thư chưa hết hạn
- Cộng Gold, Gems vào tài khoản
- Thêm Item vào Inventory
- Đánh dấu IsClaimed = true

**Authentication:** Yêu cầu (Authorize)

---

#### 8.7. POST /api/Mails/send
**Mục đích:** Gửi thư cho người chơi (Admin only).

**Business Logic:**
- Hỗ trợ đính kèm Gold, Gems, Item
- Thời hạn mặc định: 30 ngày

**DTO Request:**
```json
{
  "ReceiverId": "guid",
  "Title": "string",
  "Content": "string",
  "MailType": 0,
  "AttachedGold": 100,
  "AttachedGems": 0,
  "AttachedItemId": "guid?",
  "AttachedItemQuantity": 1
}
```

**Authentication:** Yêu cầu (Authorize), Role = Admin

---

#### 8.8. DELETE /api/Mails/{mailId}
**Mục đích:** Xóa thư.

**Business Logic:**
- Không cho xóa thư còn quà chưa nhận

**Authentication:** Yêu cầu (Authorize)

---

## 9. Friends API - Bạn bè

### Mục đích
Quản lý hệ thống kết bạn: xem bạn bè, gửi/chấp nhận/từ chối lời mời, chặn.

### Base URL
```
/api/Friends
```

### Các Endpoint

#### 9.1. GET /api/Friends
**Mục đích:** Lấy danh sách bạn bè.

**DTO Response:**
```json
{
  "Success": true,
  "Friends": [
    {
      "FriendId": "guid",
      "PlayerProfileId": "guid",
      "PlayerDisplayName": "string",
      "PlayerAvatarUrl": "string",
      "PlayerLevel": 15,
      "PlayerClass": "Knight",
      "Status": "Accepted"
    }
  ]
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 9.2. GET /api/Friends/pending
**Mục đích:** Lấy lời mời kết bạn đang chờ (người khác gửi cho mình).

**Authentication:** Yêu cầu (Authorize)

---

#### 9.3. GET /api/Friends/sent
**Mục đích:** Lấy lời mời đã gửi (đang chờ người khác chấp nhận).

**Authentication:** Yêu cầu (Authorize)

---

#### 9.4. POST /api/Friends/send
**Mục đích:** Gửi lời mời kết bạn.

**Business Logic:**
- Kiểm tra không gửi cho chính mình
- Kiểm tra chưa là bạn bè
- Kiểm tra chưa có lời mời đang chờ

**DTO Request:**
```json
{
  "AddresseeId": "guid"
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 9.5. POST /api/Friends/respond
**Mục đích:** Phản hồi lời mời kết bạn (chấp nhận/từ chối).

**Business Logic:**
- Kiểm tra lời mời thuộc về mình
- Kiểm tra lời mời đang ở trạng thái Pending
- Nếu Accept: đặt Status = Accepted
- Nếu Reject: đặt Status = Rejected

**DTO Request:**
```json
{
  "FriendId": "guid",
  "Accept": true
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 9.6. DELETE /api/Friends/{friendId}
**Mục đích:** Xóa bạn bè.

**Business Logic:**
- Kiểm tra friendship tồn tại và thuộc về mình
- Xóa record friendship

**Authentication:** Yêu cầu (Authorize)

---

#### 9.7. POST /api/Friends/block/{playerId}
**Mục đích:** Chặn người chơi.

**Business Logic:**
- Nếu đã là bạn bè: đặt Status = Blocked
- Nếu chưa có: tạo record mới với Status = Blocked

**Authentication:** Yêu cầu (Authorize)

---

## 10. Gacha API - Quay thưởng

### Mục đích
Quản lý hệ thống gacha (quay thưởng) với các banner và tỷ lệ rơi đồ.

### Base URL
```
/api/Gacha
```

### Các Endpoint

#### 10.1. GET /api/Gacha/banners
**Mục đích:** Lấy danh sách tất cả banner gacha.

**DTO Response:**
```json
{
  "Success": true,
  "Banners": [
    {
      "BannerId": "guid",
      "Name": "Legendary Heroes",
      "Type": "Limited",
      "PullCost": 10,
      "PityLimit": 90,
      "IsActive": true,
      "StartAt": "datetime",
      "EndAt": "datetime"
    }
  ]
}
```

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 10.2. GET /api/Gacha/banners/available
**Mục đích:** Lấy banner đang hoạt động (trong thời gian hiện tại).

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 10.3. GET /api/Gacha/banners/{bannerId}
**Mục đích:** Lấy thông tin chi tiết một banner.

**Authentication:** Không yêu cầu (AllowAnonymous)

---

#### 10.4. POST /api/Gacha/pull
**Mục đích:** Quay gacha (mua vật phẩm ngẫu nhiên).

**Business Logic:**
- Kiểm tra banner đang hoạt động và trong thời gian
- Giới hạn 1-10 lần quay mỗi request
- Tính tổng chi phí = PullCost * PullCount
- Kiểm tra đủ Gems
- Trừ Gems
- Với mỗi lần quay:
  - Kiểm tra pity system (tăng tỷ lệ featured item sau PityLimit - 10 lần)
  - Chọn item ngẫu nhiên dựa trên DropRate
  - Nếu là featured item (pity): reset counter
  - Tạo GachaPullHistory
  - Thêm item vào Inventory
- Trả về danh sách items đã quay được

**DTO Request:**
```json
{
  "BannerId": "guid",
  "PullCount": 10
}
```

**DTO Response:**
```json
{
  "Success": true,
  "Message": "You pulled 10 time(s)!",
  "PullResults": [
    {
      "RewardItemId": "guid",
      "ItemName": "Fire Sword",
      "ItemRarity": "Epic",
      "IsFeatured": true,
      "PullNumber": 1
    }
  ],
  "Currency": {
    "Gold": 500,
    "Gems": 90,
    "Energy": 100
  }
}
```

**Authentication:** Yêu cầu (Authorize)

---

#### 10.5. GET /api/Gacha/history
**Mục đích:** Lấy lịch sử quay gacha của người chơi.

**Business Logic:**
- Hỗ trợ phân trang
- Sắp xếp theo thời gian giảm dần

**Authentication:** Yêu cầu (Authorize)

---

## Thông tin chung

### Cấu trúc Response chuẩn

Tất cả API đều trả về response theo cấu trúc chuẩn:

```json
{
  "Success": true,
  "Message": "Mô tả kết quả",
  "Data": { /* Dữ liệu trả về, tùy API */ }
}
```

### Mã HTTP Response

| Mã | Ý nghĩa |
|----|---------|
| 200 | Thành công |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (chưa đăng nhập) |
| 403 | Forbidden (không có quyền) |
| 404 | Not Found |

### Authentication

- JWT Token được sử dụng để xác thực
- Token được truyền qua header: `Authorization: Bearer <token>`
- Một số endpoint cho phép truy cập ẩn danh (AllowAnonymous)

### Validation

- Sử dụng Data Annotations cho validation
- Custom validators cho Username và Password
- Kiểm tra tuổi tối thiểu 13 tuổi khi đăng ký

### Các Enum quan trọng

**CharacterClass:**
- "Knight"
- "Mage"
- "Archer"

**ItemType:**
- 0: Weapon
- 1: Armor
- 2: Accessory
- 3: Consumable
- 4: Material

**ItemRarity:**
- 0: Common
- 1: Uncommon
- 2: Rare
- 3: Epic
- 4: Legendary

**CurrencyType:**
- 0: Gold
- 1: Gems

**QuestStatus:**
- 0: NotStarted
- 1: InProgress
- 2: Completed
- 3: Claimed

**FriendStatus:**
- 0: Pending
- 1: Accepted
- 2: Rejected
- 3: Blocked

---

## Hướng dẫn Cài đặt & Khởi chạy (Getting Started)

### Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server (hoặc cấu hình CSDL tương ứng)
- Visual Studio 2022 / VS Code

### Các bước cài đặt
1. **Clone repository:**
   ```bash
   git clone <repo-url>
   cd Mystic-Journey-BE
   ```

2. **Cấu hình chuỗi kết nối (Connection String):**
   Mở file `appsettings.json` trong project `Mystic-Journey-API` và cập nhật chuỗi kết nối cho phù hợp với SQL Server của bạn:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=MysticJourneyDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
   }
   ```
   *Lưu ý: Cấu hình thêm SMTP và JWT secret key nếu cần thiết trong `appsettings.json`.*

3. **Cập nhật Database (Migration):**
   Mở terminal, điều hướng vào thư mục chứa dự án API (hoặc chạy từ Package Manager Console trong VS):
   ```bash
   dotnet ef database update --project Mystic-Journey-API
   ```

4. **Khởi chạy ứng dụng:**
   ```bash
   cd Mystic-Journey-API
   dotnet run
   ```
   API sẽ chạy ở địa chỉ mặc định `https://localhost:5001` hoặc `http://localhost:5000`. Bạn có thể truy cập `/swagger` để xem tài liệu API qua Swagger UI.

---

## Công nghệ sử dụng

- **.NET 8.0** - Backend framework
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core** - ORM
- **AutoMapper** - Object mapping
- **BCrypt** - Password hashing
- **JWT** - Authentication
- **SMTP** - Email service
