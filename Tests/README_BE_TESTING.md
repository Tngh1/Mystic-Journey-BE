# Mystic Journey Backend API Testing Guide (22 Features)

This directory contains test collections and scripts for validating all **22 Web Features (44 Sub-Functions)** defined in `Report5_Web_Test_Cases.xlsx`.

---

## 1. Swagger UI Testing (Interactive Browser)

1. **Start Backend Web API**:
   ```bash
   cd d:\DHFPT\Đồ Án\Project\Mystic-Journey-BE\Mystic-Journey-API
   dotnet run
   ```
2. **Open Swagger UI in your browser**:
   Navigate to: [https://localhost:7116/swagger](https://localhost:7116/swagger)
3. **Login & Authorize**:
   - Scroll to **Auth** -> `POST /api/Auth/login`.
   - Click **Try it out** and enter request body:
     ```json
     {
       "emailOrUsername": "admin@mysticjourney.com",
       "password": "AdminPassword123!",
       "clientType": "Web"
     }
     ```
   - Click **Execute** and copy the `accessToken` from the JSON response.
   - Click the **Authorize 🔓** button at top right.
   - Enter `Bearer <your_access_token>` and click **Authorize**.
4. **Run Endpoint Tests**:
   - Test endpoints under all controllers corresponding to features F01 through F22.

---

## 2. VS Code / Visual Studio `.http` File Execution

1. Open `web_features_test_suite.http` in Visual Studio or VS Code (with **REST Client** extension installed).
2. Set `@host = https://localhost:7116` (or your running API port).
3. Paste your token into `@authToken = <your_token>`.
4. Click **Send Request** above any section (F01 to F22) to execute API requests directly.

---

## 3. Postman Collection

1. Launch Postman.
2. Click **Import** -> Select `Mystic_Journey_Web_API.postman_collection.json`.
3. Set environment variable `baseUrl` = `https://localhost:7116`.
4. Run `F02 - Login` -> `F02.1 Login to Account`. The test script automatically saves `accessToken` to `{{token}}`.
5. Run tests for any feature folder (`F01` to `F22`).
