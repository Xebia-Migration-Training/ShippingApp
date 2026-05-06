# ShippingApp Repository Training Manual
## Developer Workflow Guide

---

## 📚 Table of Contents

1. [Introduction](#introduction)
2. [Prerequisites](#prerequisites)
3. [Getting Started](#getting-started)
4. [Developer Workflow](#developer-workflow)
5. [Building and Running the Application](#building-and-running-the-application)

---

## Introduction

### About ShippingApp

The **ShippingApp** is a comprehensive shipping rules management system built with .NET 8.0. It provides functionality for managing shipping rules, calculating costs, handling exchange rates, and managing master data for ports, countries, vessels, and principals.

### Purpose of This Manual

This training manual will guide you through the essential developer workflow:
- Setting up Git and cloning the repository
- Creating feature branches
- Making changes and committing code
- Creating and managing pull requests
- Building and running the application locally

---

## Prerequisites

### Required Software

#### 1. Git (Version Control)
- **Version**: 2.40 or higher
- **Download**: https://git-scm.com/downloads

**Verify Installation**:
```bash
git --version
```

#### 2. .NET SDK
- **Version**: .NET 8.0 SDK or higher
- **Download**: https://dotnet.microsoft.com/download

**Verify Installation**:
```bash
dotnet --version
```

#### 3. Code Editor/IDE
Choose one:
- **Visual Studio Code** (Recommended) - https://code.visualstudio.com/
- **Visual Studio 2022** - Version 17.8 or higher
- **JetBrains Rider** - Version 2023.3 or higher

### Required Accounts

#### GitHub Account
- Create account at: https://github.com
- Join the **Xebia-Migration-Training** organization
- Request access to the **ShippingApp** repository

---

## Getting Started

### Step 1: Configure Git

```bash
# Set your name
git config --global user.name "Your Full Name"

# Set your email (use your GitHub email)
git config --global user.email "your.email@example.com"

# Configure line endings
# For Windows:
git config --global core.autocrlf true
# For Mac/Linux:
git config --global core.autocrlf input

# Verify configuration
git config --global --list
```

### Step 2: Set Up SSH Authentication

#### Generate SSH Key

```bash
# Generate new SSH key
ssh-keygen -t ed25519 -C "your.email@example.com"

# Press Enter to accept default location
# Enter a passphrase (recommended) or press Enter to skip
```

#### Start SSH Agent

**Windows (PowerShell)**:
```powershell
Start-Service ssh-agent
ssh-add $env:USERPROFILE\.ssh\id_ed25519
```

**Mac/Linux**:
```bash
eval "$(ssh-agent -s)"
ssh-add ~/.ssh/id_ed25519
```

#### Add SSH Key to GitHub

1. **Copy your public key**:

   **Windows (PowerShell)**:
   ```powershell
   Get-Content $env:USERPROFILE\.ssh\id_ed25519.pub | Set-Clipboard
   ```

   **Mac**:
   ```bash
   pbcopy < ~/.ssh/id_ed25519.pub
   ```

   **Linux**:
   ```bash
   cat ~/.ssh/id_ed25519.pub
   # Copy the output manually
   ```

2. **Add to GitHub**:
   - Go to: https://github.com/settings/keys
   - Click "New SSH key"
   - Title: e.g., "Work Laptop"
   - Paste your key
   - Click "Add SSH key"

3. **Test Connection**:
   ```bash
   ssh -T git@github.com
   ```

### Step 3: Clone the Repository

```bash
# Navigate to your workspace directory
cd ~/workspace
# or on Windows: cd C:\workspace

# Clone the repository
git clone git@github.com:Xebia-Migration-Training/ShippingApp.git

# Navigate into the repository
cd ShippingApp

# Verify
git status
```

### Step 4: Build the Solution

```bash
# Restore dependencies
dotnet restore ShippingRules.sln

# Build the solution
dotnet build ShippingRules.sln

# Verify all projects are listed
dotnet sln ShippingRules.sln list
```

---

## Developer Workflow

### Workflow Overview

```
1. Create feature branch from master
2. Make code changes
3. Commit changes with clear messages
4. Push branch to remote
5. Create Pull Request on GitHub
6. Address review comments
7. Merge PR
8. Delete branch and pull latest
```

---

### Step 1: Create a Feature Branch

```bash
# Switch to master and get latest changes
git checkout master
git pull origin master

# Create and switch to new feature branch
git checkout -b feature/your-feature-name

# Verify you're on the new branch
git branch
```

**Branch Naming Conventions**:
- `feature/description` - New features
- `bugfix/description` - Bug fixes
- `hotfix/description` - Critical production fixes
- `docs/description` - Documentation changes

**Examples**:
```bash
git checkout -b feature/add-exchange-rate-api
git checkout -b bugfix/fix-null-reference-error
git checkout -b docs/update-readme
```

---

### Step 2: Make Your Changes

Open your IDE and make code changes.

**Best Practices**:
- Keep changes focused on one feature/fix
- Follow existing code style
- Write clear, self-documenting code
- Add comments for complex logic

**Example: Adding a New Controller**

```csharp
// src/ShippingRules.API/Controllers/ExampleController.cs
using Microsoft.AspNetCore.Mvc;

namespace ShippingRules.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExampleController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello World" });
        }
    }
}
```

---

### Step 3: Check Status and Stage Changes

```bash
# Check what files have changed
git status

# View detailed changes
git diff

# Stage specific files
git add src/ShippingRules.API/Controllers/ExampleController.cs

# Or stage all changes
git add .

# Verify staged changes
git status
```

**What NOT to stage**:
- Build outputs: `bin/`, `obj/`
- IDE settings: `.vs/`, `.vscode/settings.json`
- User-specific files
- Secrets or credentials

---

### Step 4: Commit Your Changes

```bash
# Commit with a message
git commit -m "Add example API controller"

# For detailed commits (opens editor)
git commit
```

**Writing Good Commit Messages**:

**Format**:
```
Brief summary (50 chars or less)

Detailed description of what changed and why.
Wrap lines at 72 characters.

- List specific changes
- Explain the reasoning
- Reference issue numbers if applicable

Fixes #123
```

**Good Example**:
```bash
git commit -m "Add exchange rate calculation endpoint

- Implement POST /api/exchange-rates/calculate
- Add validation for currency codes
- Include error handling for invalid inputs
- Add unit tests for calculation logic

Fixes #45"
```

**Bad Examples** ❌:
```bash
git commit -m "fixed stuff"
git commit -m "WIP"
git commit -m "updates"
```

**Good Examples** ✅:
```bash
git commit -m "Add pagination to shipping rules API"
git commit -m "Fix null reference exception in rate calculator"
git commit -m "Update API documentation for new endpoints"
```

---

### Step 5: Push to Remote

```bash
# First time pushing this branch
git push -u origin feature/your-feature-name

# Subsequent pushes
git push
```

**If you need to update your commit**:
```bash
# Make additional changes
git add .

# Amend the last commit
git commit --amend --no-edit

# Force push (only on your feature branch!)
git push --force-with-lease
```

---

### Step 6: Create a Pull Request

1. **Go to GitHub**:
   - Navigate to: https://github.com/Xebia-Migration-Training/ShippingApp
   - Click "Pull requests" tab
   - Click "New pull request"

2. **Select branches**:
   - Base: `master` (or `develop`)
   - Compare: `feature/your-feature-name`

3. **Fill PR Template**:

```markdown
## Description
Brief description of what this PR does.

## Changes Made
- Added new exchange rate calculation endpoint
- Implemented validation logic
- Added unit tests
- Updated API documentation

## Testing
- [x] Code builds successfully
- [x] All tests pass
- [x] Tested manually with Postman
- [x] No breaking changes

## Related Issues
Closes #45
```

4. **Request Reviewers**:
   - Add team members as reviewers
   - Assign yourself
   - Add appropriate labels (enhancement, bug, documentation)

---

### Step 7: Address Review Comments

**When you receive feedback**:

1. **Read comments carefully**
2. **Make requested changes**:
   ```bash
   # Make the changes in your editor
   
   # Stage and commit
   git add .
   git commit -m "Address review comments: improve validation logic"
   
   # Push updates
   git push
   ```

3. **Respond to comments**:
   - Acknowledge feedback
   - Ask questions if unclear
   - Mark conversations as resolved after fixing

4. **Request re-review** when ready

---

### Step 8: Merge Pull Request

**After approval**:

1. **Choose merge strategy**:
   - **Squash and Merge** (Recommended) - Combines all commits into one
   - **Merge Commit** - Preserves all commits
   - **Rebase and Merge** - Replays commits on target branch

2. **Click "Squash and Merge"**

3. **Edit commit message if needed**

4. **Confirm merge**

---

### Step 9: Clean Up After Merge

```bash
# Switch back to master
git checkout master

# Pull the latest changes (includes your merged PR)
git pull origin master

# Delete your local feature branch
git branch -d feature/your-feature-name

# Delete remote branch (if not auto-deleted)
git push origin --delete feature/your-feature-name

# Verify
git branch -a
```

---

### Working with an Existing Branch

**Pull latest changes**:
```bash
# Fetch all changes from remote
git fetch origin

# Pull changes for your current branch
git pull origin feature/your-feature-name
```

**Keep your branch up-to-date with master**:
```bash
# Fetch latest
git fetch origin

# Option 1: Merge master into your branch
git checkout feature/your-feature-name
git merge origin/master

# Option 2: Rebase (cleaner history)
git checkout feature/your-feature-name
git rebase origin/master
```

**If you have merge conflicts**:
```bash
# Git will pause and show conflicts
git status

# Open conflicting files and look for:
<<<<<<< HEAD
Your changes
=======
Their changes
>>>>>>> origin/master

# Edit the file to resolve conflicts
# Remove the conflict markers
# Keep the code you want

# Stage resolved files
git add resolved-file.cs

# Continue merge/rebase
git merge --continue
# or
git rebase --continue
```

---

### Common Git Commands

**Viewing Information**:
```bash
# Current status
git status

# Commit history
git log --oneline -10

# Branch list
git branch -a

# Show changes
git diff
git diff --staged
```

**Undoing Changes**:
```bash
# Discard changes in a file
git checkout -- filename.cs

# Unstage a file
git reset HEAD filename.cs

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Undo last commit (discard changes)
git reset --hard HEAD~1
```

**Stashing Changes**:
```bash
# Save changes temporarily
git stash

# List stashes
git stash list

# Apply latest stash
git stash pop

# Apply specific stash
git stash apply stash@{0}
```

---

## Building and Running the Application

### Running the API

#### Using .NET CLI

```bash
# Navigate to API project
cd src/ShippingRules.API

# Run the application
dotnet run

# Run with auto-restart on changes (recommended)
dotnet watch run
```

**Expected Output**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:8080
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Access the API**:
- Swagger UI: http://localhost:8080/swagger
- API Base: http://localhost:8080

#### Using Visual Studio

1. Open `ShippingRules.sln`
2. Set `ShippingRules.API` as startup project
3. Press `F5` to start debugging
4. Browser opens with Swagger

#### Using VS Code

1. Open folder in VS Code
2. Press `F5`
3. Select ".NET Core Launch (web)"

---

### Testing the API

#### Using Swagger UI

1. Navigate to: http://localhost:8080/swagger
2. Expand an endpoint
3. Click "Try it out"
4. Fill in parameters
5. Click "Execute"
6. View response

#### Using curl

```bash
# GET request
curl http://localhost:8080/api/shipping-rules

# POST request
curl -X POST http://localhost:8080/api/shipping-rules \
  -H "Content-Type: application/json" \
  -d '{
    "ruleName": "Test Rule",
    "description": "Test description",
    "baseRate": 100.00
  }'
```

#### Using .http Files (VS Code)

Create a file `requests.http`:

```http
### Get all shipping rules
GET http://localhost:8080/api/shipping-rules

### Get specific rule
GET http://localhost:8080/api/shipping-rules/1

### Create new rule
POST http://localhost:8080/api/shipping-rules
Content-Type: application/json

{
  "ruleName": "Express Shipping",
  "description": "Fast delivery",
  "baseRate": 150.00
}
```

Install "REST Client" extension in VS Code, then click "Send Request" above each request.

---

### Running the Web Application

```bash
# Navigate to Web project
cd src/ShippingRules.Web

# Run the application
dotnet run

# Run with auto-restart
dotnet watch run
```

**Access**: http://localhost:5014

---

### Building the Solution

```bash
# Build all projects
dotnet build ShippingRules.sln

# Build in Release mode
dotnet build ShippingRules.sln --configuration Release

# Clean build outputs
dotnet clean

# Restore + Build
dotnet restore
dotnet build
```

---

### Using Docker

```bash
# Build Docker image
docker build -t shippingrules-api -f src/ShippingRules.API/Dockerfile .

# Run container
docker run -p 8080:8080 shippingrules-api

# Using docker-compose (runs all services)
docker-compose up

# Run in background
docker-compose up -d

# Stop services
docker-compose down
```

---

## Quick Reference

### Daily Workflow

```bash
# 1. Start of day - Get latest code
git checkout master
git pull origin master

# 2. Create feature branch
git checkout -b feature/my-feature

# 3. Make changes, then check status
git status
git diff

# 4. Stage and commit
git add .
git commit -m "Add new feature"

# 5. Push to remote
git push -u origin feature/my-feature

# 6. Create PR on GitHub

# 7. After merge - clean up
git checkout master
git pull origin master
git branch -d feature/my-feature
```

### Essential Git Commands

```bash
# Status and info
git status                    # Check status
git log --oneline -10         # View history
git branch                    # List branches

# Making changes
git checkout -b feature/name  # Create branch
git add .                     # Stage all changes
git commit -m "message"       # Commit
git push                      # Push to remote

# Syncing
git fetch origin              # Fetch changes
git pull origin master        # Pull latest master
git merge origin/master       # Merge master into current

# Undoing
git checkout -- file          # Discard file changes
git reset HEAD file           # Unstage file
git reset --soft HEAD~1       # Undo last commit
```

### .NET Commands

```bash
# Build and run
dotnet restore                # Restore packages
dotnet build                  # Build solution
dotnet run                    # Run project
dotnet watch run              # Run with hot reload

# Solution management
dotnet sln list               # List projects
dotnet clean                  # Clean build outputs
```

---

## Getting Help

**Resources**:
- **Team Chat** - Ask team members
- **GitHub Issues** - Check repository issues
- **Git Documentation** - https://git-scm.com/doc
- **.NET Documentation** - https://docs.microsoft.com/dotnet

**When Asking for Help**:
1. Describe what you're trying to do
2. Show what you've tried
3. Include error messages
4. Mention your environment (OS, versions)

---

**Last Updated**: December 21, 2025  
**Version**: 2.0  
**Focus**: Developer Workflow
├── .github/                      # GitHub-specific files
│   └── workflows/                # CI/CD workflow definitions
│       └── docker-cicd.yml       # Main CI/CD pipeline
├── src/                          # Source code
│   ├── ShippingRules.API/        # REST API project
│   ├── ShippingRules.Application/ # Business logic layer
│   ├── ShippingRules.Domain/     # Domain models and entities
│   ├── ShippingRules.Infrastructure/ # Data access and external services
│   ├── ShippingRules.Web/        # Blazor web application
│   └── ShippingRules.MAUI/       # Mobile application (optional)
├── tests/                        # Test projects (if present)
├── docs/                         # Documentation
├── docker-compose.yml            # Docker composition file
├── Dockerfile                    # Docker build instructions
├── ShippingRules.sln             # Solution file
├── README.md                     # Project overview
├── Git-setup.md                  # Git workflow guide
├── Github.actions.md             # GitHub Actions documentation
└── Repository-Training-Manual.md # This file
```

### Project Layer Architecture

The application follows Clean Architecture principles:

```
┌─────────────────────────────────────────┐
│         ShippingRules.API               │  ← Presentation Layer
│  (REST endpoints, Controllers)          │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      ShippingRules.Application          │  ← Application Layer
│  (Use Cases, Services, DTOs)            │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│        ShippingRules.Domain             │  ← Domain Layer
│  (Entities, Value Objects, Enums)       │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│    ShippingRules.Infrastructure         │  ← Infrastructure Layer
│  (Database, Repositories, External APIs)│
└─────────────────────────────────────────┘
```

### Key Directories Explained

#### `.github/workflows/`
Contains GitHub Actions workflow files for CI/CD automation.

**Key File**: `docker-cicd.yml`
- Builds the application
- Runs tests
- Deploys to Azure (production)

#### `src/ShippingRules.API/`
The REST API layer exposing HTTP endpoints.

**Key Files**:
- `Program.cs` - Application entry point and configuration
- `Controllers/` - API endpoint controllers
- `appsettings.json` - Configuration settings
- `Dockerfile` - Container build instructions

**Responsibilities**:
- HTTP request/response handling
- Authentication and authorization
- Input validation
- API documentation (Swagger)

#### `src/ShippingRules.Application/`
Business logic and use case orchestration.

**Key Components**:
- `Features/` - Feature-based organization (CQRS pattern)
- `Services/` - Business services
- `DTOs/` - Data Transfer Objects
- `Interfaces/` - Abstractions for repositories

**Responsibilities**:
- Implementing business rules
- Orchestrating domain operations
- Managing transactions
- Exception handling

#### `src/ShippingRules.Domain/`
Core domain models and business entities.

**Key Components**:
- `Entities/` - Domain entities (Country, Port, Vessel, etc.)
- `Enums/` - Enumeration types
- `ValueObjects/` - Value objects (if present)

**Responsibilities**:
- Defining core business entities
- Encapsulating business logic in entities
- Domain validation rules

#### `src/ShippingRules.Infrastructure/`
External concerns and data persistence.

**Key Components**:
- `Data/` - Database context and configurations
- `Repositories/` - Data access implementations

**Responsibilities**:
- Database operations (Entity Framework Core)
- External API integrations
- File system operations
- Caching mechanisms

#### `src/ShippingRules.Web/`
Blazor Server web application for UI.

**Key Components**:
- `Pages/` - Razor pages/components
- `Layout/` - Layout components
- `Components/` - Reusable UI components
- `wwwroot/` - Static assets (CSS, JS, images)

#### `src/ShippingRules.MAUI/`
Cross-platform mobile application (optional).

---

## Development Environment Setup

### Step 1: Restore Dependencies

```bash
# Navigate to repository root
cd ShippingApp

# Restore NuGet packages
dotnet restore ShippingRules.sln

# Expected output:
# Determining projects to restore...
# Restored [project names]...
```

### Step 2: Build the Solution

```bash
# Build all projects
dotnet build ShippingRules.sln --configuration Debug

# For optimized build
dotnet build ShippingRules.sln --configuration Release
```

**Expected Output**:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**If build fails**, see [Troubleshooting](#troubleshooting) section.

### Step 3: Verify Project Structure

```bash
# List all projects in solution
dotnet sln ShippingRules.sln list

# Expected output:
# src/ShippingRules.Domain/ShippingRules.Domain.csproj
# src/ShippingRules.Application/ShippingRules.Application.csproj
# src/ShippingRules.Infrastructure/ShippingRules.Infrastructure.csproj
# src/ShippingRules.API/ShippingRules.API.csproj
# src/ShippingRules.Web/ShippingRules.Web.csproj
```

### Step 4: IDE-Specific Setup

#### Visual Studio Code

1. **Open the repository**:
   ```bash
   code .
   ```

2. **Install recommended extensions** when prompted

3. **Trust the workspace** when asked

4. **Configure launch settings**:
   - Press `F5` to start debugging
   - Select ".NET Core" when prompted
   - VS Code will create `.vscode/launch.json`

#### Visual Studio 2022

1. **Open solution**:
   - File → Open → Project/Solution
   - Select `ShippingRules.sln`

2. **Set startup project**:
   - Right-click `ShippingRules.API` in Solution Explorer
   - Select "Set as Startup Project"

3. **Configure debugging**:
   - Debug → Properties → Debug Launch Profiles

#### JetBrains Rider

1. **Open solution**:
   - File → Open → Select `ShippingRules.sln`

2. **Configure run configurations**:
   - Run → Edit Configurations
   - Add new .NET Launch configuration

### Step 5: Configure Environment Variables (Optional)

Create `src/ShippingRules.API/appsettings.Development.json` if it doesn't exist:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=shippingrules.db"
  },
  "AllowedHosts": "*"
}
```

**Note**: Never commit sensitive credentials to the repository!

---

## Working with the Repository

### Understanding Git Branches

#### Branch Structure

```
master (main branch)
  ├── develop (integration branch)
  ├── feature/manuals (feature branch)
  ├── feature/shipping-rules (feature branch)
  ├── bugfix/api-timeout (bugfix branch)
  └── hotfix/security-patch (hotfix branch)
```

#### Branch Types

1. **master** - Production-ready code
2. **develop** - Integration branch for features
3. **feature/** - New features or enhancements
4. **bugfix/** - Bug fixes for develop branch
5. **hotfix/** - Critical fixes for production

### Current Branch: feature/manuals

You're currently working on the `feature/manuals` branch, which contains:
- GitHub Actions documentation
- Training manuals
- Git setup guides

### Viewing Branch Information

```bash
# Show current branch
git branch

# Show all branches (local and remote)
git branch -a

# Show branch with last commit
git branch -v

# Show remote branches
git branch -r
```

### Switching Branches

```bash
# Switch to existing branch
git checkout feature/manuals

# Create and switch to new branch
git checkout -b feature/my-new-feature

# Switch to previous branch
git checkout -

# Modern syntax (Git 2.23+)
git switch feature/manuals
git switch -c feature/my-new-feature
```

### Updating Your Local Repository

```bash
# Fetch all changes from remote
git fetch origin

# View what changed
git log HEAD..origin/feature/manuals --oneline

# Update current branch with remote changes
git pull origin feature/manuals

# Or using rebase (cleaner history)
git pull --rebase origin feature/manuals
```

### Viewing Repository History

```bash
# View commit history
git log

# Concise one-line format
git log --oneline

# Show last 10 commits
git log -10

# Graphical view of branches
git log --graph --oneline --all

# Show changes in each commit
git log -p

# Show commits by specific author
git log --author="Your Name"

# Show commits in date range
git log --since="2 weeks ago"
```

### Checking Status and Changes

```bash
# Check current status
git status

# Short status
git status -s

# Show changes in working directory
git diff

# Show staged changes
git diff --staged

# Show changes in specific file
git diff path/to/file.cs
```

---

## Development Workflow

### Workflow Overview

```
1. Pick a task from project board
2. Create feature branch
3. Make changes locally
4. Test your changes
5. Commit changes
6. Push to remote
7. Create Pull Request
8. Code review
9. Merge to develop/master
```

### Step-by-Step Workflow

#### Step 1: Create a Feature Branch

```bash
# Ensure you're on the latest master/develop
git checkout master
git pull origin master

# Create feature branch
git checkout -b feature/add-shipping-calculator

# Verify you're on new branch
git branch
```

**Branch Naming Conventions**:
- `feature/short-description` - New features
- `bugfix/issue-description` - Bug fixes
- `hotfix/critical-fix` - Production hotfixes
- `docs/what-youre-documenting` - Documentation

**Examples**:
- `feature/add-exchange-rate-api`
- `bugfix/fix-null-reference-error`
- `hotfix/security-vulnerability`
- `docs/api-documentation`

#### Step 2: Make Your Changes

Open your IDE and make code changes.

**Best Practices**:
- Keep changes focused on one feature/fix
- Follow existing code style
- Add comments for complex logic
- Update documentation as needed

#### Step 3: Stage Your Changes

```bash
# View what changed
git status

# Stage specific files
git add src/ShippingRules.API/Controllers/NewController.cs
git add src/ShippingRules.Application/Services/NewService.cs

# Stage all changes in a directory
git add src/ShippingRules.API/

# Stage all changes (use with caution)
git add .

# Stage only modified files (not new/deleted)
git add -u

# Interactive staging (selective staging)
git add -p
```

**What NOT to stage**:
- Build outputs (bin/, obj/)
- User-specific files (.vs/, .vscode/settings.json)
- Secrets and credentials
- Large binary files (unless necessary)

#### Step 4: Commit Your Changes

```bash
# Commit with message
git commit -m "Add shipping cost calculator endpoint"

# Commit with detailed message (opens editor)
git commit

# Amend last commit (if you forgot something)
git add forgotten-file.cs
git commit --amend --no-edit
```

**Good Commit Messages**:
```
Add shipping cost calculator endpoint

- Implement POST /api/shipping/calculate
- Add validation for input parameters
- Include unit tests for calculator service
- Update API documentation

Fixes #123
```

**Commit Message Guidelines**:
- First line: Brief summary (50 chars or less)
- Blank line
- Detailed description (wrap at 72 chars)
- Reference issue numbers if applicable

**Bad Examples**:
- "Fixed stuff" ❌
- "WIP" ❌
- "asdfasdf" ❌
- "Updated files" ❌

**Good Examples**:
- "Add pagination to shipping rules API" ✅
- "Fix null reference exception in rate calculator" ✅
- "Update exchange rate service documentation" ✅

#### Step 5: Push to Remote

```bash
# Push to remote repository
git push origin feature/add-shipping-calculator

# First time pushing a new branch
git push -u origin feature/add-shipping-calculator

# Force push (use with caution!)
git push --force-with-lease origin feature/add-shipping-calculator
```

**When to force push**:
- After rebasing your branch
- After amending commits
- **Never** on shared branches (master, develop)

#### Step 6: Create a Pull Request

1. **Navigate to GitHub**:
   - Go to https://github.com/Xebia-Migration-Training/ShippingApp

2. **Create PR**:
   - Click "Pull requests" tab
   - Click "New pull request"
   - Select your branch: `feature/add-shipping-calculator`
   - Select target branch: usually `develop` or `master`

3. **Fill PR Details**:
   ```markdown
   ## Description
   Adds a new shipping cost calculator endpoint that calculates costs
   based on weight, distance, and shipping method.

   ## Changes Made
   - Added `CalculateShippingCostAsync` method to `ShippingService`
   - Implemented `/api/shipping/calculate` POST endpoint
   - Added input validation
   - Created unit tests for calculator logic
   - Updated Swagger documentation

   ## Testing
   - [x] Unit tests pass
   - [x] Manual testing with Postman
   - [x] Integration tests pass

   ## Related Issues
   Closes #123

   ## Screenshots (if applicable)
   [Attach screenshots if UI changes]

   ## Checklist
   - [x] Code follows project style guidelines
   - [x] Added/updated tests
   - [x] Updated documentation
   - [x] No breaking changes
   ```

4. **Request Reviewers**:
   - Add team members as reviewers
   - Assign yourself
   - Add labels (enhancement, bug, documentation, etc.)

5. **Link to Issues**:
   - Reference issue numbers in description
   - Use keywords: "Fixes #123", "Closes #456", "Resolves #789"

#### Step 7: Code Review Process

**As Author**:
1. Respond to review comments promptly
2. Make requested changes
3. Push updates (no need to create new PR)
4. Mark conversations as resolved
5. Request re-review when ready

**Making Changes After Review**:
```bash
# Make changes based on feedback
# Stage and commit
git add .
git commit -m "Address review comments: fix validation logic"

# Push updates
git push origin feature/add-shipping-calculator
```

#### Step 8: Merge Pull Request

**Options**:
1. **Merge Commit** - Preserves all commits and merge history
2. **Squash and Merge** - Combines all commits into one (cleaner)
3. **Rebase and Merge** - Replays commits on target branch

**Recommended**: Use "Squash and Merge" for feature branches

**After Merging**:
```bash
# Switch back to master
git checkout master

# Pull latest changes
git pull origin master

# Delete local feature branch
git branch -d feature/add-shipping-calculator

# Delete remote branch (if not auto-deleted)
git push origin --delete feature/add-shipping-calculator
```

### Keeping Your Branch Updated

```bash
# Fetch latest changes
git fetch origin

# Option 1: Merge master into your branch
git checkout feature/add-shipping-calculator
git merge origin/master

# Option 2: Rebase on master (cleaner history)
git checkout feature/add-shipping-calculator
git rebase origin/master

# If conflicts occur during rebase
# 1. Resolve conflicts in your editor
# 2. Stage resolved files
git add resolved-file.cs
# 3. Continue rebase
git rebase --continue

# To abort rebase if needed
git rebase --abort
```

---

## Building and Running the Application

### Running the API

#### Method 1: Using .NET CLI (Recommended for Development)

```bash
# Navigate to API project
cd src/ShippingRules.API

# Run the application
dotnet run

# Run with specific profile
dotnet run --launch-profile "https"

# Run with watch (auto-restart on changes)
dotnet watch run
```

**Expected Output**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:8080
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Access Points**:
- API Base: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger
- Health Check: http://localhost:8080/health

#### Method 2: Using IDE

**Visual Studio Code**:
1. Press `F5` to start debugging
2. Select ".NET Core Launch (web)"

**Visual Studio 2022**:
1. Press `F5` or click "Play" button
2. API launches in browser with Swagger

**Rider**:
1. Click "Run" button or press `Shift+F10`

#### Method 3: Using Docker

```bash
# Build Docker image
docker build -t shippingrules-api -f src/ShippingRules.API/Dockerfile .

# Run container
docker run -p 8080:8080 shippingrules-api

# Using docker-compose
docker-compose up
```

### Running the Web Application

```bash
# Navigate to Web project
cd src/ShippingRules.Web

# Run the application
dotnet run

# Run with watch
dotnet watch run
```

**Expected Output**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5014
```

**Access**: http://localhost:5014

### Running Both API and Web Simultaneously

**Option 1: Multiple Terminals**

Terminal 1:
```bash
cd src/ShippingRules.API
dotnet watch run
```

Terminal 2:
```bash
cd src/ShippingRules.Web
dotnet watch run
```

**Option 2: Using Docker Compose**

```bash
# From repository root
docker-compose up

# Run in background
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Testing the API

#### Using Swagger UI

1. Navigate to http://localhost:8080/swagger
2. Expand an endpoint (e.g., `GET /api/shipping-rules`)
3. Click "Try it out"
4. Fill in parameters if needed
5. Click "Execute"
6. View response

#### Using curl

```bash
# Get all shipping rules
curl http://localhost:8080/api/shipping-rules

# Get specific rule
curl http://localhost:8080/api/shipping-rules/{id}

# Create new rule (POST)
curl -X POST http://localhost:8080/api/shipping-rules \
  -H "Content-Type: application/json" \
  -d '{
    "ruleName": "Test Rule",
    "description": "Test description",
    "precedenceLevel": 1,
    "baseRate": 100.00,
    "surchargePercentage": 5.0,
    "ruleType": "Standard",
    "effectiveFrom": "2025-01-01T00:00:00Z"
  }'
```

#### Using .http File

1. Download collection from repository (if available)
2. Import into Postman
3. Set base URL: `http://localhost:8080`
4. Run requests

#### Using .http File

VS Code with REST Client extension:

```http
### Get all shipping rules
GET http://localhost:8080/api/shipping-rules

### Get specific rule
GET http://localhost:8080/api/shipping-rules/{{ruleId}}

### Create new rule
POST http://localhost:8080/api/shipping-rules
Content-Type: application/json

{
  "ruleName": "Express Shipping",
  "description": "Fast delivery service",
  "precedenceLevel": 1,
  "baseRate": 150.00,
  "surchargePercentage": 10.0,
  "ruleType": "Express",
  "effectiveFrom": "2025-01-01T00:00:00Z"
}
```

---



---

## CI/CD Pipeline


Add to README.md:
```markdown
![CI/CD](https://github.com/Xebia-Migration-Training/ShippingApp/workflows/Docker%20CI/CD%20Pipeline/badge.svg?branch=feature/manuals)
```

#### Email Notifications

- Configure in GitHub Settings → Notifications
- Receive emails on workflow failures

#### Webhook Notifications

- Configured for Microsoft Teams
- Sends deployment status updates

### Pipeline Secrets and Variables

**Required Secrets** (configured by DevOps team):
- `AZURE_CREDENTIALS` - Azure service principal
- `DOCKER_USERNAME` - Docker Hub username
- `DOCKER_PASSWORD` - Docker Hub token
- `MS_TEAMS_WEBHOOK_URI` - Teams notification webhook

**Required Variables**:
- `AZURE_WEBAPP_NAME` - Azure App Service name
- `AZURE_RESOURCE_GROUP` - Azure resource group

### Triggering Manual Deployment

1. Go to Actions tab
2. Select "Docker CI/CD Pipeline" workflow
3. Click "Run workflow"
4. Select branch
5. Choose "production" environment
6. Click "Run workflow" button

### Debugging Pipeline Failures

#### View Logs

1. Click on failed workflow run
2. Click on failed job
3. Expand failed step
4. Read error messages and stack traces

#### Common Pipeline Issues

**Build Failures**:
```bash
# Test locally before pushing
dotnet build src/ShippingRules.API/ShippingRules.API.csproj --configuration Release
```

**Test Failures**:
```bash
# Run tests locally
dotnet test --verbosity detailed
```

**Deployment Failures**:
- Check Azure credentials are valid
- Verify Azure resources exist
- Check application health endpoint

---

## Code Review Process

### Reviewer Guidelines

#### What to Look For

**Code Quality**:
- [ ] Code is readable and well-structured
- [ ] Follows C# coding conventions
- [ ] Appropriate use of SOLID principles
- [ ] No code duplication
- [ ] Meaningful variable and method names

**Functionality**:
- [ ] Code does what it's supposed to do
- [ ] Edge cases are handled
- [ ] Error handling is appropriate
- [ ] No obvious bugs

**Testing**:
- [ ] Tests cover main scenarios
- [ ] Tests are meaningful and not trivial
- [ ] Test names are descriptive

**Security**:
- [ ] No hardcoded credentials
- [ ] Input is validated
- [ ] No SQL injection vulnerabilities
- [ ] Sensitive data is not logged

**Performance**:
- [ ] No obvious performance issues
- [ ] Database queries are efficient
- [ ] Appropriate use of async/await

**Documentation**:
- [ ] Complex logic is commented
- [ ] API documentation is updated
- [ ] README updated if needed

#### Providing Feedback

**Good Review Comments**:
```
✅ "Consider extracting this logic into a separate method for better testability."
✅ "This could cause a null reference exception. Consider adding a null check."
✅ "Nice use of LINQ! This is much more readable."
✅ "Could you add a test case for when the list is empty?"
```

**Avoid**:
```
❌ "This is wrong."
❌ "Why did you do it this way?"
❌ "I would have done it differently."
```

**Be Constructive**:
- Explain the "why" behind suggestions
- Offer alternatives
- Ask questions to understand intent
- Praise good practices

#### Review Checklist

```markdown
## Code Review Checklist

### General
- [ ] Code builds without errors or warnings
- [ ] Code follows team conventions
- [ ] No debugging code left in (console.log, etc.)

### Functionality
- [ ] Feature works as expected
- [ ] All acceptance criteria met
- [ ] Edge cases handled

### Tests
- [ ] Tests pass
- [ ] New code has tests
- [ ] Test coverage is adequate

### Documentation
- [ ] Code is self-documenting or commented
- [ ] API changes documented
- [ ] README updated if needed

### Security
- [ ] No secrets in code
- [ ] Input validation present
- [ ] Proper error handling

### Approved
- [ ] Ready to merge
```

### Author Guidelines

**Before Requesting Review**:
1. Review your own code first
2. Run all tests locally
3. Check formatting and linting
4. Ensure build succeeds
5. Write clear PR description
6. Self-review the diff on GitHub

**During Review**:
1. Respond to comments promptly
2. Ask for clarification if needed
3. Be open to feedback
4. Don't take criticism personally
5. Explain your decisions when appropriate

**After Review**:
1. Make requested changes
2. Mark conversations as resolved
3. Request re-review
4. Thank reviewers!

---

## Troubleshooting

### Common Issues and Solutions

#### Issue 1: Build Fails with Missing Dependencies

**Symptoms**:
```
error: The type or namespace name 'X' could not be found
```

**Solution**:
```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

#### Issue 2: Port Already in Use

**Symptoms**:
```
System.IO.IOException: Failed to bind to address http://localhost:8080
```

**Solution**:

**Windows**:
```powershell
# Find process using port
netstat -ano | findstr :8080

# Kill process (replace PID)
taskkill /PID <PID> /F
```

**Mac/Linux**:
```bash
# Find process
lsof -i :8080

# Kill process
kill -9 <PID>
```

Or change port in `appsettings.json` or `launchSettings.json`.

#### Issue 3: Database Migration Errors

**Symptoms**:
```
System.InvalidOperationException: Unable to resolve service for type 'DbContext'
```

**Solution**:
```bash
# Install EF Core tools (one time)
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialCreate --project src/ShippingRules.Infrastructure

# Update database
dotnet ef database update --project src/ShippingRules.Infrastructure
```

#### Issue 4: Git Merge Conflicts

**Symptoms**:
```
CONFLICT (content): Merge conflict in file.cs
```

**Solution**:
```bash
# View conflicts
git status

# Open conflicted files in editor
# Look for conflict markers:
<<<<<<< HEAD
Your changes
=======
Their changes
>>>>>>> branch-name

# Resolve conflicts by editing file
# Keep what you need, remove markers

# Stage resolved files
git add resolved-file.cs

# Continue merge
git commit
```

Or use VS Code's merge conflict resolver (recommended).

#### Issue 5: Push Rejected

**Symptoms**:
```
! [rejected] feature/branch -> feature/branch (non-fast-forward)
```

**Solution**:
```bash
# Pull latest changes first
git pull origin feature/branch

# If conflicts, resolve them
# Then push again
git push origin feature/branch
```

#### Issue 6: Accidentally Committed to Wrong Branch

**Solution**:
```bash
# Undo last commit (keep changes)
git reset --soft HEAD~1

# Switch to correct branch
git checkout correct-branch

# Recommit
git add .
git commit -m "Your message"
```

#### Issue 7: Need to Discard Local Changes

**Solution**:
```bash
# Discard changes to specific file
git checkout -- path/to/file.cs

# Discard all local changes
git reset --hard HEAD

# Clean untracked files
git clean -fd
```

#### Issue 8: Swagger Not Loading

**Symptoms**: 404 error on /swagger

**Solution**:
1. Check `Program.cs` for Swagger configuration
2. Ensure running in Development environment
3. Check `appsettings.Development.json` exists
4. Verify port number is correct

#### Issue 9: Docker Build Fails

**Symptoms**: Docker build errors

**Solution**:
```bash
# Check Dockerfile syntax
docker build -t test -f src/ShippingRules.API/Dockerfile .

# View build output
docker build --progress=plain -t test -f src/ShippingRules.API/Dockerfile .

# Clean Docker cache
docker system prune -a
```

#### Issue 10: Tests Fail on CI but Pass Locally

**Possible Causes**:
- Time zone differences
- Database state
- Environment variables
- File path differences (Windows vs Linux)

**Solution**:
```bash
# Set environment to match CI
export ASPNETCORE_ENVIRONMENT=Test

# Run tests with same parameters as CI
dotnet test --configuration Release --verbosity normal --logger trx
```

### Getting Help

**Resources**:
1. **This Manual** - Most common scenarios covered
2. **Team Chat** - Ask team members
3. **GitHub Issues** - Check existing issues
4. **Stack Overflow** - Search for error messages
5. **Official Docs**:
   - .NET: https://docs.microsoft.com/dotnet
   - Git: https://git-scm.com/doc
   - GitHub Actions: https://docs.github.com/actions

**When Asking for Help**:
1. Describe what you're trying to do
2. Show what you've tried
3. Include error messages (full stack trace)
4. Mention your environment (OS, .NET version, etc.)

---

## Best Practices

### Git Best Practices

#### Commit Often, Push Frequently
```bash
# Small, focused commits
git commit -m "Add validation to shipping rule"
git commit -m "Add unit tests for validation"
git commit -m "Update API documentation"
```

#### Write Meaningful Commit Messages
```
Good: "Fix null reference exception in rate calculator"
Bad: "Fixed bug"
```

#### Keep Your Branch Updated
```bash
# Daily: pull latest from master
git checkout master
git pull origin master

# Merge into your branch
git checkout feature/my-feature
git merge master
```

#### Don't Commit These Files
- Build outputs (bin/, obj/)
- IDE files (.vs/, .vscode/settings.json)
- OS files (.DS_Store, Thumbs.db)
- User-specific settings
- Secrets and credentials

#### Use .gitignore
The repository includes a `.gitignore` file. Don't modify unless necessary.

### Coding Best Practices

#### Follow C# Conventions
```csharp
// ✅ Good
public async Task<ShippingRule> GetRuleByIdAsync(Guid id)
{
    if (id == Guid.Empty)
        throw new ArgumentException("Invalid ID", nameof(id));
    
    return await _repository.GetByIdAsync(id);
}

// ❌ Bad
public async Task<ShippingRule> getrule(Guid id)
{
    return await _repository.GetByIdAsync(id);
}
```

#### Use Async/Await Properly
```csharp
// ✅ Good
public async Task<IActionResult> GetRulesAsync()
{
    var rules = await _service.GetAllRulesAsync();
    return Ok(rules);
}

// ❌ Bad
public IActionResult GetRules()
{
    var rules = _service.GetAllRulesAsync().Result; // Blocking!
    return Ok(rules);
}
```

#### Handle Errors Gracefully
```csharp
// ✅ Good
try
{
    var rule = await _service.GetRuleAsync(id);
    return Ok(rule);
}
catch (NotFoundException ex)
{
    _logger.LogWarning(ex, "Rule not found: {Id}", id);
    return NotFound(new { message = "Rule not found" });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error retrieving rule: {Id}", id);
    return StatusCode(500, new { message = "Internal server error" });
}
```

#### Use Dependency Injection
```csharp
// ✅ Good
public class ShippingRulesController : ControllerBase
{
    private readonly IShippingRuleService _service;
    private readonly ILogger<ShippingRulesController> _logger;
    
    public ShippingRulesController(
        IShippingRuleService service,
        ILogger<ShippingRulesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

### Testing Best Practices

#### Test Coverage Goals
- Unit Tests: 80%+ coverage
- Integration Tests: Critical paths
- E2E Tests: Happy paths

#### Test Naming
```csharp
[Fact]
public void GetRuleById_WithValidId_ReturnsRule() { }

[Fact]
public void GetRuleById_WithInvalidId_ThrowsNotFoundException() { }
```

### Security Best Practices

#### Never Commit Secrets
```csharp
// ❌ Bad
var apiKey = "sk-1234567890abcdef";

// ✅ Good
var apiKey = _configuration["ApiKey"];
```

#### Validate Input
```csharp
// ✅ Good
public IActionResult CreateRule([FromBody] CreateRuleRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    if (request.BaseRate < 0)
        return BadRequest("Base rate must be positive");
    
    // Process request...
}
```

---

## Appendix

### A. Useful Git Commands Reference

#### Configuration
```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
git config --global core.editor "code --wait"
git config --list
```

#### Repository Management
```bash
git clone <url>
git init
git remote add origin <url>
git remote -v
```

#### Branching
```bash
git branch                    # List local branches
git branch -a                 # List all branches
git branch <branch-name>      # Create branch
git checkout <branch-name>    # Switch branch
git checkout -b <branch-name> # Create and switch
git branch -d <branch-name>   # Delete local branch
git push origin --delete <branch-name> # Delete remote branch
```

#### Staging and Committing
```bash
git status
git add <file>
git add .
git commit -m "message"
git commit --amend
```

#### Pushing and Pulling
```bash
git push origin <branch>
git pull origin <branch>
git fetch origin
```

#### Viewing History
```bash
git log
git log --oneline
git log --graph --all
git show <commit-hash>
```

#### Undoing Changes
```bash
git checkout -- <file>        # Discard changes
git reset HEAD <file>         # Unstage
git reset --soft HEAD~1       # Undo commit, keep changes
git reset --hard HEAD~1       # Undo commit, discard changes
git revert <commit-hash>      # Create new commit undoing changes
```

#### Merging and Rebasing
```bash
git merge <branch>
git rebase <branch>
git rebase --continue
git rebase --abort
```

#### Stashing
```bash
git stash                     # Stash changes
git stash list                # List stashes
git stash pop                 # Apply and remove latest stash
git stash apply               # Apply stash
git stash drop                # Delete stash
```

### B. .NET CLI Commands Reference

#### Solution Management
```bash
dotnet new sln                       # Create solution
dotnet sln add <project>             # Add project to solution
dotnet sln list                      # List projects
```

#### Project Management
```bash
dotnet new webapi -n MyApi           # Create Web API project
dotnet add package <package-name>    # Add NuGet package
dotnet remove package <package-name> # Remove package
dotnet list package                  # List packages
```

#### Build and Run
```bash
dotnet restore                       # Restore dependencies
dotnet build                         # Build project
dotnet run                           # Run project
dotnet watch run                     # Run with hot reload
dotnet clean                         # Clean build outputs
```

#### Testing
```bash
dotnet test                          # Run all tests
dotnet test --logger trx             # Run with logger
dotnet test --filter <criteria>      # Run filtered tests
```

#### Publishing
```bash
dotnet publish -c Release            # Publish for deployment
dotnet publish -o ./publish          # Specify output directory
```

### C. Docker Commands Reference

#### Images
```bash
docker build -t <name> .             # Build image
docker images                        # List images
docker rmi <image-id>                # Remove image
docker pull <image>                  # Pull from registry
docker push <image>                  # Push to registry
```

#### Containers
```bash
docker run <image>                   # Run container
docker run -d <image>                # Run in background
docker run -p 8080:80 <image>        # Port mapping
docker ps                            # List running containers
docker ps -a                         # List all containers
docker stop <container-id>           # Stop container
docker rm <container-id>             # Remove container
docker logs <container-id>           # View logs
docker exec -it <container-id> bash  # Enter container
```

#### Docker Compose
```bash
docker-compose up                    # Start services
docker-compose up -d                 # Start in background
docker-compose down                  # Stop services
docker-compose logs                  # View logs
docker-compose ps                    # List services
```

### D. Keyboard Shortcuts

#### Visual Studio Code
- `Ctrl+P` / `Cmd+P` - Quick file open
- `Ctrl+Shift+P` / `Cmd+Shift+P` - Command palette
- `F5` - Start debugging
- `Ctrl+`` / `Cmd+`` - Toggle terminal
- `Ctrl+B` / `Cmd+B` - Toggle sidebar
- `Ctrl+/` / `Cmd+/` - Toggle comment

#### Visual Studio
- `F5` - Start debugging
- `Ctrl+F5` - Start without debugging
- `Ctrl+K, Ctrl+D` - Format document
- `Ctrl+K, Ctrl+C` - Comment selection
- `Ctrl+K, Ctrl+U` - Uncomment selection

### E. Additional Resources

#### Official Documentation
- [.NET Documentation](https://docs.microsoft.com/dotnet)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Git Documentation](https://git-scm.com/doc)
- [GitHub Documentation](https://docs.github.com)
- [Docker Documentation](https://docs.docker.com)

#### Learning Resources
- [Microsoft Learn](https://docs.microsoft.com/learn)
- [Pluralsight](https://www.pluralsight.com)
- [LinkedIn Learning](https://www.linkedin.com/learning)
- [GitHub Learning Lab](https://lab.github.com)

#### Community
- [Stack Overflow](https://stackoverflow.com)
- [.NET Blog](https://devblogs.microsoft.com/dotnet)
- [ASP.NET Community](https://dotnet.microsoft.com/platform/community)

#### Tools
- [Visual Studio Code](https://code.visualstudio.com)
- [Postman](https://www.postman.com)
- [Git Extensions](https://gitextensions.github.io)
- [SourceTree](https://www.sourcetreeapp.com)

### F. Team Contacts

**Project Lead**: [Name] - [Email]  
**DevOps Lead**: [Name] - [Email]  
**Tech Lead**: [Name] - [Email]  

**Support Channels**:
- Slack: #shipping-app-dev
- Teams: ShippingApp Team
- Email: team@example.com

---

## Quick Start Checklist

For new team members, follow this checklist:

### Day 1: Setup
- [ ] Install Git, .NET SDK, IDE
- [ ] Create GitHub account
- [ ] Request repository access
- [ ] Configure Git (name, email)
- [ ] Set up SSH authentication
- [ ] Clone repository
- [ ] Build solution successfully

### Day 2: Familiarization
- [ ] Read README.md
- [ ] Explore repository structure
- [ ] Review existing code
- [ ] Run API locally
- [ ] Test endpoints with Swagger
- [ ] Review open pull requests

### Day 3: First Contribution
- [ ] Pick a small task/issue
- [ ] Create feature branch
- [ ] Make changes
- [ ] Write tests
- [ ] Commit and push
- [ ] Create pull request

### Day 4-5: Team Integration
- [ ] Participate in code reviews
- [ ] Join team meetings
- [ ] Ask questions
- [ ] Document learnings

---

## Feedback and Improvements

This manual is a living document. If you find:
- Errors or outdated information
- Missing sections
- Confusing explanations
- Opportunities for improvement

Please:
1. Create an issue on GitHub
2. Submit a pull request with fixes
3. Contact the documentation team

**Last Updated**: December 21, 2025  
**Version**: 1.0  
**Maintained By**: ShippingApp Development Team

---

## Conclusion

Congratulations! You now have a comprehensive understanding of:
- Setting up your development environment
- Working with the ShippingApp repository
- Following proper Git workflows
- Building and running the application
- Contributing code through pull requests
- Understanding the CI/CD pipeline

Remember:
- 📚 Refer to this manual when needed
- 💬 Ask questions when stuck
- 🤝 Help others learn
- 🔄 Keep learning and improving

Welcome to the ShippingApp team! 🚀
