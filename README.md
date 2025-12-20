## How to Run (Local)

Open **2 terminals**: one for the API and one for the Web app.

### 1) Run the API
```bash
cd src/ShippingRules.API
dotnet run
```

API base URL:
- `http://localhost:8080`
- Swagger UI: `http://localhost:8080` (root)

Port config:
- [src/ShippingRules.API/Properties/launchSettings.json](src/ShippingRules.API/Properties/launchSettings.json)

### 2) Run the Web App (Blazor)
```bash
cd src/ShippingRules.Web
dotnet run
```

Web URL:
- `http://localhost:5014`

API base for Web:
- [src/ShippingRules.Web/Program.cs](src/ShippingRules.Web/Program.cs)

### 3) Run the MAUI App (Optional)
Install workload (one time):
```bash
dotnet workload install maui
```

Run:
```bash
cd src/ShippingRules.MAUI
dotnet run
```

MAUI API base URL:
- Android emulator: `http://10.0.2.2:8080/api`
- Desktop: `http://localhost:8080/api`

Config:
- [src/ShippingRules.MAUI/Services/ShippingRulesApiService.cs](src/ShippingRules.MAUI/Services/ShippingRulesApiService.cs)

---

## How to Use (Business Flow)

### Step A — Create a Rule
Web:
- Page: `http://localhost:5014/create-rule`
- File: [src/ShippingRules.Web/Pages/CreateRule.razor](src/ShippingRules.Web/Pages/CreateRule.razor)

MAUI:
- Page: Create Rule
- File: [src/ShippingRules.MAUI/Views/CreateRulePage.xaml](src/ShippingRules.MAUI/Views/CreateRulePage.xaml)

API used:
- `POST /api/ShippingRules`
- Controller: [`ShippingRules.API.Controllers.ShippingRulesController.Create`](src/ShippingRules.API/Controllers/ShippingRulesController.cs)

Important inputs:
- **Rule Type** (string): must match exactly (example: `PortCharge`, `FreightCharge`)
- **Precedence Level**: 1=Vessel, 2=Port, 3=Principal, 4=Country, 5=Global
- **Effective From/To**: date window for when the rule applies

### Step B — Validate (Find Applicable Rule)
Web:
- Page: `http://localhost:5014/validate-rule`
- File: [src/ShippingRules.Web/Pages/ValidateRule.razor](src/ShippingRules.Web/Pages/ValidateRule.razor)

Pick:
- Country / Port / Principal / Vessel (optional, depending on rule)
- Rule Type
- Effective Date

API used:
- `GET /api/ShippingRules/applicable`
- Controller: [`ShippingRules.API.Controllers.ShippingRulesController.GetApplicableRule`](src/ShippingRules.API/Controllers/ShippingRulesController.cs)

### Step C — Calculate Cost (from Validate page)
In the same Validate page:
- Enter **Base Amount**
- Click **Calculate Cost**

API used:
- `POST /api/ShippingRules/calculate-cost`
- Controller: [`ShippingRules.API.Controllers.ShippingRulesController.CalculateCost`](src/ShippingRules.API/Controllers/ShippingRulesController.cs)

---

## Core Business Logic (Precedence)

Rule selection is handled by:
- [`ShippingRules.Application.Services.RulePrecedenceService.GetApplicableRuleAsync`](src/ShippingRules.Application/Services/RulePrecedenceService.cs)

The system filters rules by:
- `RuleType`
- Effective range: `EffectiveFrom <= EffectiveDate <= EffectiveTo (if set)`
- `IsActive == true`

Then it picks the most specific match by precedence:
1. Vessel-specific
2. Port-specific
3. Principal-specific
4. Country-specific
5. Global default

Precedence consistency validation:
- [`ShippingRules.Application.Features.ShippingRules.Commands.CreateShippingRule.CreateShippingRuleCommandValidator`](src/ShippingRules.Application/Features/ShippingRules/Commands/CreateShippingRule/CreateShippingRuleCommandValidator.cs)

---

## Cost Calculation

Formula (as implemented):
$$\text{TotalCost}=\text{BaseRate}+(\text{BaseAmount}\times\frac{\text{Surcharge\%}}{100})$$

Calculation helper:
- [`ShippingRules.Application.Services.RulePrecedenceService.CalculateCost`](src/ShippingRules.Application/Services/RulePrecedenceService.cs)

---

## Approval Workflow

If `RequiresApproval = true` on creation:
- The rule is created as **Pending / Inactive**
- It will not be applied until approved

Approve a rule:
- `POST /api/ShippingRules/{id}/approve`
- Controller: [`ShippingRules.API.Controllers.ShippingRulesController.ApproveRule`](src/ShippingRules.API/Controllers/ShippingRulesController.cs)

Entity fields:
- [`ShippingRules.Domain.Entities.ShippingRule.RequiresApproval`](src/ShippingRules.Domain/Entities/ShippingRule.cs)
- [`ShippingRules.Domain.Entities.ShippingRule.ApprovedBy`](src/ShippingRules.Domain/Entities/ShippingRule.cs)
- [`ShippingRules.Domain.Entities.ShippingRule.ApprovedAt`](src/ShippingRules.Domain/Entities/ShippingRule.cs)

---

## Conflict Detection (Overlap)

On create, the system checks for overlapping rules (same criteria + same RuleType + overlapping effective dates).  
If conflicts exist, the API returns **409 Conflict** with conflict details.

Repository support:
- [`ShippingRules.Application.Interfaces.IShippingRuleRepository.GetConflictingRulesAsync`](src/ShippingRules.Application/Interfaces/IShippingRuleRepository.cs)

---

## Multi-currency (FX Conversion)

Exchange rates are stored in:
- [`ShippingRules.Infrastructure.Data.ShippingRulesDbContext.ExchangeRates`](src/ShippingRules.Infrastructure/Data/ShippingRulesDbContext.cs)

API:
- `GET /api/master/fx-rate?from=USD&to=INR&at=2025-12-20T00:00:00Z`

Cost conversion (optional):
- `POST /api/ShippingRules/calculate-cost` with `TargetCurrency`

---

## Bulk Simulation / Batch Validation

Batch cost calculation:
- `POST /api/ShippingRules/batch-calculate-cost`

Implementation:
- [`ShippingRules.API.Controllers.ShippingRulesController.BatchCalculateCost`](src/ShippingRules.API/Controllers/ShippingRulesController.cs)