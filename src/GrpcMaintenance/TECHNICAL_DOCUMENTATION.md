# Technical Documentation - GrpcMaintenance

## 📋 Table of Contents
1. [Overview](#overview)
2. [Project Architecture](#project-architecture)
3. [Environment Configuration](#environment-configuration)
4. [Dependencies and Packages](#dependencies-and-packages)
5. [Layer Structure](#layer-structure)
6. [Database Configuration](#database-configuration)
7. [gRPC and Protobuf](#grpc-and-protobuf)
8. [Main Commands](#main-commands)
9. [Development Flow](#development-flow)

---

## 🎯 Overview

**GrpcMaintenance** is a backend microservice developed in **.NET 8** that provides high-performance communication via **gRPC** between microservices. The project implements a complete equipment maintenance management system with full CRUD operations.

### Key Features
- ✅ Microservice-to-microservice communication via gRPC
- ✅ SQLite database with Entity Framework Core
- ✅ JSON Transcoding for HTTP/JSON compatibility
- ✅ Complete CRUD operations
- ✅ Automated database migrations

### Important Note
> This project uses **gRPC** as the communication protocol between microservices, not for frontend-backend communication. It is a backend-to-backend architecture.

---

## 🏗️ Project Architecture

### Layered Architecture

```
┌─────────────────────────────────────────────────┐
│    Presentation Layer (Controllers)             │
│    (gRPC Endpoints + JSON Transcoding)          │
└───────────────────┬─────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────┐
│    Services Layer (GRPC Services)               │
│  - UserGrpcService                              │
│  - MaintenanceGrpcService                       │
│  - DetailMaintenanceGrpcService                 │
│  - EquipmentGrpcService                         │
│  - GreeterService (Example)                     │
└───────────────────┬─────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────┐
│    Business Logic Layer                         │
│  (Entity Framework Core, DbContext)             │
└───────────────────┬─────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────┐
│    Data Access Layer                            │
│  - AppDbContext (Entity Framework)              │
│  - SQLite Database (Mantenimiento.db)           │
│  - Migrations (Entity Framework)                │
└─────────────────────────────────────────────────┘
```

---

## ⚙️ Environment Configuration

### Prerequisites
- **.NET 8 SDK** installed
- **Visual Studio Code** or **Visual Studio** 2022+
- **PowerShell** or any compatible terminal
- Administrator permissions to install global tools

### Versions Used
```
.NET Framework:              net8.0
Framework Target:            8.0.310
Entity Framework Core:        8.0.0 / 8.0.310
gRPC Tools:                   2.58.0
gRPC AspNetCore:             2.76.0
JsonTranscoding:             8.0.310
Google API CommonProtos:     2.17.0
SQLite:                      8.0.310
```

---

## 📦 Dependencies and Packages

### Installation of Dependencies

#### 1. **Entity Framework Core - Design**
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design --version="8.0.310"
```
**Purpose:** Development tools for Entity Framework, necessary for creating and managing migrations.

#### 2. **Entity Framework Core - SQLite**
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version="8.0.310"
```
**Purpose:** SQLite database provider for Entity Framework Core.

#### 3. **gRPC Tools**
```bash
dotnet add package Grpc.tools --version="2.58.0"
```
**Purpose:** Tools to compile Protobuf definitions (.proto) to C# code.

#### 4. **JSON Transcoding**
```bash
dotnet add package Microsoft.AspNetCore.Grpc.JsonTranscoding --version="8.0.310"
```
**Purpose:** Enables calling gRPC services via HTTP/REST with JSON, not just pure gRPC.

### Installation of Global Tools

#### Entity Framework CLI Tool
```bash
# Uninstall previous version (if exists)
dotnet tool uninstall dotnet-ef --global

# Install specific version
dotnet tool install --global dotnet-ef --version 8.0.0-preview.7.23375.4
```
**Purpose:** Command-line tool for managing database migrations.

---

## 🗂️ Layer Structure

### 1. **Models Layer (`/Models`)**

Contains domain entities representing database tables.

#### User.cs
```csharp
public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public ICollection<MaintenanceOrder> Orders { get; set; }
}
```

#### Equipment.cs
Represents available equipment in the system.

#### MaintenanceOrder.cs
Represents maintenance orders assigned to equipment.

#### DetailMaintenance.cs
Contains specific details for each maintenance order.

### 2. **Data Layer (`/Data`)**

**AppDbContext.cs** - Entity Framework context that manages:
- Entity configuration
- Table relationships
- Automatic migrations

```csharp
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<MaintenanceOrder> MaintenanceOrders { get; set; }
    public DbSet<DetailMaintenance> DetailMaintenances { get; set; }
}
```

**Database:** SQLite
- **File:** `Mantenimiento.db`
- **Location:** Project root
- **Advantages:** Server-less, portable, ideal for development

### 3. **gRPC Services Layer (`/Services`)**

Implementation of services defined in `.proto` files.

#### Main Services

1. **UserGrpcService**
   - `CreateUser()` - POST /v1/user
   - `GetUserById()` - GET /v1/user/{id}
   - `GetAllUsers()` - GET /v1/users
   - `DeleteUser()` - DELETE /v1/user

2. **MaintenanceGrpcService**
   - Management of maintenance orders

3. **DetailMaintenanceGrpcService**
   - Management of maintenance details

4. **EquipmentGrpcService**
   - Equipment management

5. **GreeterService**
   - Demo service

### 4. **Protobuf Definitions (`/Protos`)**

`.proto` files that define gRPC service contracts:

- **user.proto** - User service
- **equipment.proto** - Equipment service
- **maintenance.proto** - Maintenance service
- **detail_maintenance.proto** - Maintenance details
- **greet.proto** - Demo service

---

## 🗄️ Database Configuration

### AppDbContext

```csharp
// In Program.cs
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlite("Data Source=Mantenimiento.db")
);
```

### Entity Relationships

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // One User has many Maintenance Orders
    modelBuilder.Entity<MaintenanceOrder>()
        .HasOne(o => o.User)
        .WithMany(u => u.Orders)
        .HasForeignKey(o => o.UserId);

    // One Equipment has many Maintenance Orders
    modelBuilder.Entity<MaintenanceOrder>()
        .HasOne(o => o.Equipment)
        .WithMany(e => e.Orders)
        .HasForeignKey(o => o.EquipmentId);

    // One Order has many Maintenance Details
    modelBuilder.Entity<DetailMaintenance>()
        .HasOne(d => d.MaintenanceOrder)
        .WithMany(o => o.Details)
        .HasForeignKey(d => d.MaintenanceOrderId);
}
```

### Migrations

Migrations allow versioning and evolution of the database.

**Location:** `/Migrations`

**Generated Files:**
- `20260103235731_InitialCreate.cs` - Initial migration
- `20260103235731_InitialCreate.Designer.cs` - Metadata
- `AppDbContextModelSnapshot.cs` - Current snapshot

---

## 🔌 gRPC and Protobuf

### What is gRPC?

gRPC (gRPC Remote Procedure Call) is a modern open-source framework that enables efficient definition of remote services using Protobuf.

**Advantages:**
- Superior performance to REST/JSON
- Bidirectional communication
- Strong typing
- Automatic compression
- Streaming support

### What is Protobuf?

Protocol Buffers is a language-agnostic data serialization method.

### Example: user.proto

```protobuf
syntax = "proto3";
package users;
option csharp_namespace = "GrpcMaintenance.Protos.User";

service UserService {
  rpc CreateUser (CreateUserRequest) returns (UserResponse) {
    option(google.api.http)={
      post:"/v1/user",
      body:"*"
    };
  }
  
  rpc GetUserById (GetUserByIdRequest) returns (UserResponse) {
    option(google.api.http)={
      get:"/v1/user/{id}",
    };
  }
  
  rpc GetAllUsers (google.protobuf.Empty) returns (UsersResponse) {
    option(google.api.http)={
      get:"/v1/users",
    };
  }
  
  rpc DeleteUser (DeleteUserRequest) returns (DeleteUserResponse) {
    option(google.api.http)={
      delete:"/v1/user/{id}",
      body:"*"
    };
  }
}

message CreateUserRequest {
  string name = 1;
  string role = 2;
}

message GetUserByIdRequest {
  int32 id = 1;
}

message UserResponse {
  int32 id = 1;
  string name = 2;
  string role = 3;
}

message UsersResponse {
  repeated UserResponse users = 1;
}

message DeleteUserRequest {
  int32 id = 1;
}

message DeleteUserResponse {
  int32 id = 1;
}
```

### Protobuf Compilation

`.proto` files are automatically compiled during `dotnet build`:
1. The gRPC compiler processes files in `/Protos`
2. Generates C# classes in `obj/Debug/net8.0/` folder
3. Services inherit from generated base classes

---

## 📋 Program.cs Configuration

```csharp
using GrpcMaintenance.Services;
using GrpcMaintenance.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== SERVICES CONFIGURATION =====

// 1. Configure Database
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlite("Data Source=Mantenimiento.db")
);

// 2. Configure gRPC with JSON Transcoding
builder.Services.AddGrpc().AddJsonTranscoding();

var app = builder.Build();

// ===== PIPELINE CONFIGURATION =====

// 3. Map gRPC Services
app.MapGrpcService<GreeterService>();
app.MapGrpcService<MaintenanceGrpcService>();
app.MapGrpcService<DetailMaintenanceGrpcService>();
app.MapGrpcService<UserGrpcService>();
app.MapGrpcService<EquipmentGrpcService>();

// 4. Test endpoint
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();
```

---

## 🚀 Main Commands

### 1. **Build the Project**
```bash
dotnet build
```
**When to use:** After changes to code or models  
**What it does:** 
- Compiles C# code
- Processes `.proto` files
- Generates automatic gRPC classes
- Verifies compilation errors

### 2. **Run the Server**
```bash
dotnet run --project .\src\GrpcMaintenance\
```
**When to use:** To start the gRPC server  
**Expected output:**
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5246
```

### 3. **Create Database Migrations**
```bash
dotnet ef migrations add CreateMigration
```
**When to use:** ONLY when models in `/Models` change  
**What it does:**
- Detects entity changes
- Generates migration script
- Creates files in `/Migrations`

### 4. **Update Database**
```bash
dotnet ef database update
```
**When to use:** After creating a migration  
**What it does:**
- Applies changes to `Mantenimiento.db`
- Updates table schema

### 5. **Clean Build**
```bash
dotnet clean
```
**When to use:** When persistent compilation errors occur  
**What it does:** Removes `/bin` and `/obj` folders

### 6. **View Project Structure**
```bash
tree /F (Windows)
ls -R (Linux/Mac)
```

---

## 📈 Development Flow

### Standard Flow

#### 1. **Make code changes**
```bash
# Edit files in /Services, /Models, /Protos
```

#### 2. **Compile project**
```bash
dotnet build
```
✅ Verifies errors  
✅ Processes protobuf  
✅ Generates gRPC code  

#### 3. **If models changed: Create migration**
```bash
dotnet ef migrations add DescribeMigration
```

#### 4. **Run server**
```bash
dotnet run --project .\src\GrpcMaintenance\
```

#### 5. **Test endpoints**
- Use gRPC client (grpcurl, BloomRPC)
- Or consume via JSON Transcoding:
```bash
# GET Users
curl http://localhost:5246/v1/users

# CREATE User
curl -X POST http://localhost:5246/v1/user \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe","role":"Admin"}'
```

### Model Change Flow

```
┌─────────────────────────────┐
│ Edit Model (User.cs)        │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│ dotnet build                │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│ dotnet ef migrations add    │
│ "DescribeChange"            │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│ Review file in              │
│ /Migrations/*.cs            │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│ dotnet ef database update   │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│ dotnet run                  │
│ (Updated Server)            │
└─────────────────────────────┘
```

---

## 🔧 .csproj File Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <RootNamespace>GrpcMaintenance</RootNamespace>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <!-- Protobuf files compilation -->
  <ItemGroup>
   <Protobuf Include="Protos\**\*.proto"
            GrpcServices="Server"
            AdditionalImportDirs="Protos" />
  </ItemGroup>

  <!-- NuGet dependencies -->
  <ItemGroup>
    <PackageReference Include="Grpc.AspNetCore" Version="2.76.0" />
    <PackageReference Include="Microsoft.AspNetCore.Grpc.JsonTranscoding" Version="8.0.0" />
    <PackageReference Include="Google.Api.CommonProtos" Version="2.17.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```

---

## 📌 Summary of Key Features

| Feature | Technology | Purpose |
|---|---|---|
| Framework | .NET 8 | Modern and high-performance runtime |
| Protocol | gRPC | Backend-to-backend communication |
| Serialization | Protocol Buffers | Contract definition and serialization |
| Database | SQLite | Persistent storage |
| ORM | Entity Framework Core | Data access with LINQ |
| REST/JSON | Json Transcoding | HTTP client compatibility |
| Migrations | EF Core Migrations | Database schema versioning |

---

## ✅ Development Checklist

- [ ] `.NET 8 SDK` installed
- [ ] `dotnet-ef` global tool installed
- [ ] NuGet dependencies installed (`dotnet restore`)
- [ ] Database created (`dotnet ef database update`)
- [ ] Server builds without errors (`dotnet build`)
- [ ] Server starts correctly (`dotnet run`)
- [ ] Endpoints respond to HTTP/gRPC
- [ ] Migrations in version control

---

## 🔗 Useful References

- [Official gRPC Documentation](https://grpc.io/docs/)
- [Protocol Buffers - Google](https://developers.google.com/protocol-buffers)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core gRPC](https://learn.microsoft.com/en-us/aspnet/core/grpc/)
- [JSON Transcoding for gRPC](https://github.com/grpc/grpc-dotnet/tree/master/src/Grpc.AspNetCore.JsonTranscoding)

---

**Last Updated:** January 2026  
**Status:** Project in development  
**Documentation Version:** 1.0
