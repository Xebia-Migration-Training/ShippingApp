# ShippingApp Developer Workflow Manual
## Quick Start Guide for Developers

---

## 📚 Table of Contents

1. [Introduction](#introduction)
2. [Prerequisites](#prerequisites)
3. [Getting Started](#getting-started)
4. [Developer Workflow](#developer-workflow)
5. [Building and Running the Application](#building-and-running-the-application)
6. [Quick Reference](#quick-reference)

---

## Introduction

### About ShippingApp

The **ShippingApp** is a comprehensive shipping rules management system built with .NET 8.0. It provides functionality for managing shipping rules, calculating costs, handling exchange rates, and managing master data for ports, countries, vessels, and principals.

### Purpose of This Manual

This manual focuses on the essential developer workflow:
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
**Version**: 1.0  
**Repository**: Xebia-Migration-Training/ShippingApp
