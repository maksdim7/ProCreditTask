# Swift MT103 Web API

## Overview

This project is a .NET 10 Web API that accepts a SWIFT MT103 message as a text file, parses the message fields, and stores the extracted data in a SQLite database.

The solution uses a custom MT103 parser and does not rely on any external SWIFT message parsing libraries.

## Features

- Upload SWIFT MT103 message file
- Custom parsing of MT103 fields
- Support for multiline SWIFT fields
- Store parsed data in SQLite
- Database communication without Entity Framework
- Retrieve stored messages through API
- Swagger UI for API testing
- Logging with NLog
- No authentication or authorization

## Technologies

- .NET 10
- ASP.NET Core Web API
- SQLite
- Microsoft.Data.Sqlite
- NLog
- Swagger / OpenAPI

## Parsed MT103 Fields

The parser extracts the following fields:

| Tag | Description |
|---|---|
| `20` | Transaction Reference |
| `23B` | Bank Operation Code |
| `32A` | Value Date, Currency, Amount |
| `33B` | Currency, Amount |
| `50K` | Ordering Customer |
| `57A` | Account With Institution |
| `59` | Beneficiary Customer |
| `70` | Remittance Information |
| `71A` | Details of Charges |

## Project Structure

```text
SwiftMt103.Api/
├── Data/
│   └── Database.cs
├── Services/
│   └── Mt103Parser.cs
├── Program.cs
├── nlog.config
├── appsettings.json
└── SwiftMt103.Api.csproj
```

## How to Run
- git clone <repository-url>
- cd <repository-folder>/SwiftMt103.Api
- dotnet restore
- dotnet run
- Open swagger UI ->http://localhost:5278/swagger
  
After uploading a .txt file containing swift message, the message is parsed and stored in SQlite database returning the parsed fields. The database file is created automatically when the application starts. Logs are written to logs/app.log.
