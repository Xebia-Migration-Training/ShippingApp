# Shipping Rules Application

## Professional .NET 9.0 Architecture

This is a complete enterprise-grade application using **Clean Architecture** with **.NET 9.0**, **ASP.NET Core**, and **.NET MAUI**.

## Architecture Overview

### 🏗️ Solution Structure

```
ShippingRules.sln
├── src/
│   ├── ShippingRules.Domain         - Core business entities & enums
│   ├── ShippingRules.Application    - Use cases, CQRS, validation
│   ├── ShippingRules.Infrastructure - EF Core, repositories
│   ├── ShippingRules.API            - ASP.NET Core Web API
│   └── ShippingRules.MAUI           - .NET MAUI cross-platform UI
```

### 🎯 Key Features

#### Business Rules & Precedence Logic
- **5-Level Precedence Hierarchy**:
  1. Vessel-specific (Highest)
  2. Port-specific
  3. Principal-specific
  4. Country-specific
  5. Global (Lowest)

- **Date-based Validation**: Effective from/to dates with overlap detection
- **Active Status Management**: Rules can be activated/deactivated
- **Approval Workflow**: Support for rules requiring approval
- **Conflict Detection**: Automatic detection of overlapping rules

#### Technical Stack
- **.NET 9.0** with C# 13
- **EF Core 9.0** with SQL Server
- **MediatR** for CQRS pattern
- **FluentValidation** for business rules
- **Serilog** for structured logging
- **Swagger/OpenAPI** for API documentation
- **.NET MAUI** for cross-platform UI
- **CommunityToolkit.Mvvm** for MVVM pattern

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server (or LocalDB)
- Visual Studio 2022 or VS Code
- (Optional) .NET MAUI workload for mobile/desktop apps

### Run the API

```powershell
cd src/ShippingRules.API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7046`
- HTTP: `http://localhost:5046`
- Swagger UI: `https://localhost:7046` (root)

### Run the MAUI App

**Note**: .NET MAUI workload is required. Install with:
```powershell
dotnet workload install maui
```

Then run:
```powershell
cd src/ShippingRules.MAUI
dotnet run
```

## 📡 API Endpoints

### Shipping Rules
- `GET /api/ShippingRules` - Get all rules
- `GET /api/ShippingRules?activeOnly=true` - Get active rules only
- `POST /api/ShippingRules` - Create new rule
- `GET /api/ShippingRules/applicable` - Find applicable rule by precedence
- `POST /api/ShippingRules/calculate-cost` - Calculate cost with rule

### Example API Request

```json
POST /api/ShippingRules
{
  "ruleName": "US West Coast Freight",
  "description": "Freight charges for US West Coast ports",
  "precedenceLevel": 2,
  "portId": "44444444-4444-4444-4444-444444444444",
  "effectiveFrom": "2025-01-01",
  "effectiveTo": null,
  "baseRate": 500.00,
  "surchargePercentage": 15.00,
  "ruleType": "FreightCharge",
  "ruleCategory": "Standard",
  "requiresApproval": false
}
```

## 📱 MAUI Application Features

### Navigation Structure
1. **Rules List Page** - View all shipping rules with filters
2. **Create Rule Page** - Add new rules with validation
3. **Validate Rule Page** - Test precedence logic and find applicable rules

### UI Features
- Modern Fluent Design
- Real-time validation
- Responsive layouts
- Dark/Light theme support
- Cross-platform (Windows, Android, iOS, macOS)

## 🗄️ Database Schema

### Core Tables
- `Countries` - Country master data
- `Ports` - Port master data
- `Principals` - Shipping line/carriers
- `Vessels` - Vessel/ship data
- `ShippingRules` - Business rules with precedence

### Seeded Data
The database includes sample data:
- Countries: USA, China, India
- Ports: Los Angeles, Shanghai, Mumbai
- Principals: Maersk, MSC
- Vessels: Sample container ships

## 🔒 Security (Future Enhancement)

Ready for:
- Azure AD / Entra ID authentication
- JWT Bearer tokens
- Role-based authorization
- API rate limiting
- HTTPS enforcement

## 📊 Precedence Logic Example

Given criteria:
- Country: USA
- Port: Los Angeles
- Vessel: Maersk Explorer

The system will search in order:
1. ✅ Vessel-specific rule (if exists) - **Selected**
2. Port-specific rule (if no vessel rule)
3. Principal-specific rule
4. Country-specific rule
5. Global default rule

## 🛠️ Development Commands

```powershell
# Build entire solution
dotnet build

# Run all tests
dotnet test

# Create EF Core migration
cd src/ShippingRules.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../ShippingRules.API

# Update database
dotnet ef database update --startup-project ../ShippingRules.API

# Clean solution
dotnet clean
```

## 📝 License

Microsoft Consulting Project - 2025

## 📞 Support

For support, contact the development team.

---

**Built with ❤️ using Microsoft .NET 9.0 Stack**
