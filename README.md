# JWTAuthDotNet10

Minimal ASP.NET Core Web API example demonstrating JWT authentication with refresh tokens and role-based authorization. Built for .NET 10.

## Overview

- User registration and login with securely hashed passwords (`PasswordHasher<User>`).
- Issuing JWT access tokens (1 day lifetime) and refresh tokens (stored in DB, 7 day expiry).
- Refresh token endpoint to obtain new access/refresh tokens.
- Role-based authorization (example `Admin` role protected endpoint).

## Requirements

- .NET 10 SDK
- A configured database provider for EF Core (e.g., SQL Server, SQLite, PostgreSQL)
- Visual Studio 2026 or the `dotnet` CLI

## Configuration

Add JWT settings to `appsettings.json` (example):

```json
{
  "AppSettings": {
    "Token": "your-very-long-secret-key-here",
    "Issuer": "YourAppIssuer",
    "Audience": "YourAppAudience"
  }
}
```

- Keep `AppSettings:Token` secret (use environment variables or a secret manager in production).

## Database & Migrations

1. Configure your DB provider in `AppDbContext`.
2. Add a migration: `dotnet ef migrations add <Name>`
3. Apply migrations: `dotnet ef database update`

## Running

- Visual Studio: open the solution and run the project.
- CLI: `dotnet run --project JWTAuthDotNet10`

## API Endpoints

Base path: `api/auth`

- POST `api/auth/register`
  - Body: `{ "username": "alice", "password": "P@ssw0rd" }`
  - Returns created `User` or 400 if username exists.

- POST `api/auth/login`
  - Body: `{ "username": "alice", "password": "P@ssw0rd" }`
  - Returns `TokenResponseDto`: `{ "accessToken": "...", "refreshToken": "..." }`

- POST `api/auth/refresh-token`
  - Body: `{ "userId": "GUID", "refreshToken": "..." }`
  - Returns new `TokenResponseDto` or 401 if invalid.

- GET `api/auth`
  - Requires `Authorization: Bearer <accessToken>` header.

- GET `api/auth/admin-only`
  - Requires authenticated user in role `Admin`.

## Token & Refresh Behavior

- Access tokens are signed with a symmetric key from configuration and expire after 1 day.
- Refresh tokens are generated with a cryptographically secure RNG, stored on the user record and expire after 7 days.

## Notes & Recommendations

- Use a strong secret and rotate keys in production.
- Store secrets in environment variables or secret stores (do not commit secrets).
- Consider refresh token revocation and device/session tracking for improved security.
- Enforce HTTPS in production.

## Next steps

- Add a `.gitignore` (for `bin/`, `obj/`, `secrets`, etc.) and create an initial commit.
- I can add example `appsettings.Development.json`, sample `UserDto` schema, or a Postman collection if desired.
