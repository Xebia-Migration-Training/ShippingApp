# 🚀 Git Workflow Setup Guide
## Your Journey to Version Control Mastery

> **Welcome Aboard!** This comprehensive guide will transform you from a Git beginner to a confident version control navigator. Think of Git as your time machine for code - let's learn how to pilot it! ⏰

---

## 📋 Table of Contents
1. [Prerequisites & Initial Setup](#prerequisites--initial-setup)
2. [Git Installation](#git-installation)
3. [GitHub Account Configuration](#github-account-configuration)
4. [SSH Key Setup (The Secure Way)](#ssh-key-setup-the-secure-way)
5. [Repository Workflow](#repository-workflow)
6. [Branching Strategy](#branching-strategy)
7. [Daily Developer Workflow](#daily-developer-workflow)
8. [Troubleshooting Common Issues](#troubleshooting-common-issues)
9. [Pro Tips & Best Practices](#pro-tips--best-practices)

---

## 🎯 Prerequisites & Initial Setup

Before we dive in, ensure you have:
- [ ] A computer with admin/sudo access
- [ ] Internet connection (obviously! 🌐)
- [ ] A curious mind and willingness to learn
- [ ] Coffee ☕ (optional but recommended)

---

## 📦 Git Installation

### Windows Installation

#### Step 1: Download Git
1. Navigate to [git-scm.com](https://git-scm.com/download/win)
2. Download the latest version (64-bit recommended)
3. Run the installer `Git-x.xx.x-64-bit.exe`

#### Step 2: Installation Wizard
During installation, these settings are recommended:

```plaintext
✅ Use Git from Git Bash only (or from Command Prompt)
✅ Use the OpenSSL library
✅ Checkout Windows-style, commit Unix-style line endings
✅ Use MinTTY (the default terminal of MSYS2)
✅ Default (fast-forward or merge)
✅ Git Credential Manager
✅ Enable file system caching
```

#### Step 3: Verify Installation
Open PowerShell or Command Prompt and type:
```bash
git --version
```
Expected output: `git version 2.xx.x`

🎉 **Success!** Git is now installed on your system.

### macOS Installation

```bash
# Using Homebrew (recommended)
brew install git

# Or download from git-scm.com
# https://git-scm.com/download/mac
```

### Linux Installation

```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install git

# Fedora
sudo dnf install git

# Arch Linux
sudo pacman -S git
```

---

## 🔧 GitHub Account Configuration

### Step 1: Create Your GitHub Identity

Open your terminal and configure your global Git settings:

```bash
# Set your username (use your real name for professional projects)
git config --global user.name "Your Full Name"

# Set your email (use the email associated with your GitHub account)
git config --global user.email "your.email@example.com"

# Set default branch name to 'main'
git config --global init.defaultBranch main

# Enable colored output (makes everything prettier!)
git config --global color.ui auto

# Set your default editor (choose your favorite)
git config --global core.editor "code --wait"  # For VS Code
# git config --global core.editor "vim"        # For Vim lovers
# git config --global core.editor "nano"       # For simplicity
```

### Step 2: Verify Your Configuration

```bash
# View all your git configurations
git config --list

# View specific configurations
git config user.name
git config user.email
```

📝 **Pro Tip:** Your `user.email` should match the email on your GitHub account to properly link commits to your profile!

---

## 🔐 SSH Key Setup (The Secure Way)

SSH keys are like a VIP backstage pass - once you have them, you never need to enter your password again! 🎫

### Step 1: Check for Existing SSH Keys

```bash
# List existing SSH keys
ls -la ~/.ssh
```

Look for files like `id_rsa.pub`, `id_ecdsa.pub`, or `id_ed25519.pub`

### Step 2: Generate a New SSH Key

```bash
# Generate SSH key with your GitHub email
ssh-keygen -t ed25519 -C "your.email@example.com"

# If your system doesn't support ed25519, use RSA
ssh-keygen -t rsa -b 4096 -C "your.email@example.com"
```

**Follow the prompts:**
- Press `Enter` to accept the default file location
- Enter a secure passphrase (recommended) or press `Enter` for no passphrase
- Re-enter the passphrase to confirm

### Step 3: Start the SSH Agent

**Windows (PowerShell):**
```powershell
# Start the SSH agent
Start-Service ssh-agent

# Add your SSH key
ssh-add ~/.ssh/id_ed25519
```

**macOS/Linux:**
```bash
# Start the SSH agent
eval "$(ssh-agent -s)"

# Add your SSH key
ssh-add ~/.ssh/id_ed25519
```

### Step 4: Copy Your Public Key

**Windows (PowerShell):**
```powershell
Get-Content ~/.ssh/id_ed25519.pub | Set-Clipboard
```

**macOS:**
```bash
pbcopy < ~/.ssh/id_ed25519.pub
```

**Linux:**
```bash
cat ~/.ssh/id_ed25519.pub
# Then manually copy the output
```

### Step 5: Add SSH Key to GitHub

1. Go to [GitHub.com](https://github.com) and log in
2. Click your profile photo → **Settings**
3. In the sidebar, click **SSH and GPG keys**
4. Click **New SSH key**
5. Add a descriptive title (e.g., "Work Laptop - Windows")
6. Paste your key into the "Key" field
7. Click **Add SSH key**

### Step 6: Test Your Connection

```bash
ssh -T git@github.com
```

Expected response:
```
Hi username! You've successfully authenticated, but GitHub does not provide shell access.
```

🎊 **Congratulations!** You're now securely connected to GitHub!

---

## 📁 Repository Workflow

### Starting Fresh: Creating a New Repository

#### Option A: Create on GitHub First (Recommended for Teams)

1. **On GitHub.com:**
   - Click the `+` icon → **New repository**
   - Name your repository (e.g., `eastern-shipping`)
   - Add a description
   - Choose Public or Private
   - ✅ Initialize with README
   - Choose a `.gitignore` template (e.g., VisualStudio)
   - Choose a license (e.g., MIT)
   - Click **Create repository**

2. **Clone to Your Local Machine:**
```bash
# Clone via SSH (recommended)
git clone git@github.com:username/eastern-shipping.git

# Navigate into the directory
cd eastern-shipping

# Verify the remote
git remote -v
```

#### Option B: Start Locally (Recommended for Solo Projects)

```bash
# Navigate to your project directory
cd /path/to/your/project

# Initialize Git repository
git init

# Create a README
echo "# Eastern Shipping Project" > README.md

# Add all files to staging
git add .

# Create your first commit
git commit -m "🎉 Initial commit: Project setup"

# Create repository on GitHub (via browser)
# Then add the remote:
git remote add origin git@github.com:username/eastern-shipping.git

# Push to GitHub
git push -u origin main
```

---

## 🌿 Branching Strategy

Think of branches as parallel universes where you can experiment without breaking the main timeline! 🌌

### Main Branch Protection

The `main` branch should always be production-ready:
- ✅ All tests pass
- ✅ Code is reviewed
- ✅ Features are complete

### Branch Naming Convention

Use descriptive names that follow this pattern:

```plaintext
feature/    → New features          → feature/user-authentication
bugfix/     → Bug fixes             → bugfix/login-error
hotfix/     → Urgent production fix → hotfix/critical-security-patch
docs/       → Documentation         → docs/api-documentation
refactor/   → Code refactoring      → refactor/database-layer
test/       → Adding tests          → test/unit-tests-api
```

### Creating and Using Branches

```bash
# View all branches
git branch -a

# Create a new branch
git branch feature/shipping-rules

# Switch to the new branch
git checkout feature/shipping-rules

# Or create and switch in one command (⚡ Fast way!)
git checkout -b feature/shipping-rules

# Push the new branch to GitHub
git push -u origin feature/shipping-rules
```

---

## 💼 Daily Developer Workflow

This is your bread-and-butter routine! 🍞🧈

### Morning Routine: Start Your Day Right

```bash
# 1. Switch to main branch
git checkout main

# 2. Pull latest changes from remote
git pull origin main

# 3. Create a new feature branch
git checkout -b feature/add-vessel-tracking

# 4. Verify you're on the correct branch
git branch
```

### During Development: The Commit Dance

```bash
# 1. Check status frequently
git status

# 2. View changes you've made
git diff

# 3. Add specific files to staging
git add src/ShippingRules.API/Controllers/VesselController.cs

# Or add all changed files
git add .

# Or add interactively (power user move! 💪)
git add -p

# 4. Commit with a meaningful message
git commit -m "✨ feat: Add vessel tracking endpoint"

# 5. Push to remote branch
git push origin feature/add-vessel-tracking
```

### Commit Message Best Practices

Use the **Conventional Commits** standard:

```plaintext
✨ feat:      New feature
🐛 fix:       Bug fix
📝 docs:      Documentation changes
💄 style:     Formatting, missing semicolons, etc.
♻️  refactor:  Code restructuring
✅ test:      Adding tests
🔧 chore:     Build process or tools
⚡ perf:      Performance improvements
```

**Examples:**
```bash
git commit -m "✨ feat: Add shipping rule validation"
git commit -m "🐛 fix: Resolve null reference in port lookup"
git commit -m "📝 docs: Update API documentation with examples"
git commit -m "♻️ refactor: Extract rule precedence logic to service"
```

### End of Day: Clean Up and Push

```bash
# 1. Ensure all changes are committed
git status

# 2. Push your branch to remote
git push origin feature/add-vessel-tracking

# 3. Create a Pull Request on GitHub
# (Do this via GitHub web interface)
```

---

## 🔄 Creating Pull Requests

### Step 1: Prepare Your Branch

```bash
# Ensure your branch is up-to-date with main
git checkout main
git pull origin main
git checkout feature/add-vessel-tracking
git merge main

# Resolve any conflicts if they arise
```

### Step 2: Create Pull Request on GitHub

1. Navigate to your repository on GitHub
2. Click **Pull requests** tab
3. Click **New pull request**
4. Select `main` as base and your feature branch as compare
5. Fill in the PR template:

```markdown
## Description
Brief description of what this PR does

## Type of Change
- [ ] Bug fix
- [x] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- Tested locally with sample data
- All unit tests pass
- Integration tests pass

## Screenshots (if applicable)
[Add screenshots here]

## Checklist
- [x] Code follows project style guidelines
- [x] Self-review completed
- [x] Comments added for complex logic
- [x] Documentation updated
- [x] No new warnings generated
```

6. Click **Create pull request**
7. Request reviewers
8. Wait for approval and merge!

---

## 🆘 Troubleshooting Common Issues

### Issue 1: "Permission Denied (publickey)"

**Solution:**
```bash
# Test SSH connection
ssh -T git@github.com

# If it fails, add your key again
ssh-add ~/.ssh/id_ed25519

# Verify the key is loaded
ssh-add -l
```

### Issue 2: "Failed to Push - Rejected"

**Solution:**
```bash
# Someone pushed before you! Pull first
git pull origin main --rebase

# Then push again
git push origin main
```

### Issue 3: "Merge Conflict"

**Solution:**
```bash
# 1. Don't panic! 😌
# 2. Open the conflicted files
# 3. Look for conflict markers:
#    <<<<<<< HEAD
#    Your changes
#    =======
#    Their changes
#    >>>>>>> branch-name

# 4. Edit the file to resolve conflicts
# 5. Remove conflict markers
# 6. Add and commit
git add .
git commit -m "🔀 merge: Resolve conflicts with main"
```

### Issue 4: "Accidentally Committed to Main"

**Solution:**
```bash
# Create a new branch with current changes
git branch feature/oops-forgot-to-branch

# Reset main to origin
git reset --hard origin/main

# Switch to the new branch
git checkout feature/oops-forgot-to-branch
```

### Issue 5: "Need to Undo Last Commit"

**Solution:**
```bash
# Undo commit but keep changes
git reset --soft HEAD~1

# Undo commit and discard changes (⚠️ Careful!)
git reset --hard HEAD~1
```

---

## 🎓 Pro Tips & Best Practices

### 1. **Commit Often, Push Daily**
Small, frequent commits are better than large, infrequent ones.

```bash
# Good: Small, focused commits
git commit -m "Add vessel validation"
git commit -m "Add unit tests for vessel validation"
git commit -m "Update documentation"

# Bad: One massive commit
git commit -m "Add everything"
```

### 2. **Write Meaningful Commit Messages**

```bash
# ❌ Bad
git commit -m "fixed stuff"
git commit -m "update"
git commit -m "changes"

# ✅ Good
git commit -m "🐛 fix: Resolve null pointer in vessel lookup"
git commit -m "✨ feat: Add pagination to shipping rules API"
git commit -m "📝 docs: Add examples to README"
```

### 3. **Use `.gitignore` Wisely**

The `.gitignore` file tells Git which files to ignore. This is crucial for keeping your repository clean and secure! 🛡️

#### How to Create a .gitignore File

```bash
# In your repository root
touch .gitignore

# Or on Windows
New-Item .gitignore
```

---

#### Example 1: .NET / C# Projects (Recommended for ShippingRules)

```gitignore
# ============================================
# .NET Core / ASP.NET Core / C# .gitignore
# ============================================

## Build Results
bin/
obj/
out/
[Dd]ebug/
[Rr]elease/
[Bb]uild[Ll]og.*
*.dll
*.exe
*.pdb

## User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates
*.userprefs

## Visual Studio / Rider
.vs/
.vscode/
.idea/
*.swp
*.swo
*~
.DS_Store

## ReSharper
_ReSharper*/
*.[Rr]e[Ss]harper
*.DotSettings.user

## NuGet Packages
*.nupkg
*.snupkg
**/packages/*
!**/packages/build/
*.nuget.props
*.nuget.targets
project.lock.json
project.fragment.lock.json
artifacts/

## Test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*
*.trx
*.coverage
*.coveragexml
TestResults/

## Environment & Secrets
.env
.env.local
.env.*.local
appsettings.Development.json
appsettings.*.json
!appsettings.json
secrets.json
*.pfx
*.cer

## Logs
logs/
*.log
log.txt
npm-debug.log*
yarn-debug.log*
yarn-error.log*

## Database
*.db
*.sqlite
*.sqlite3
*.mdf
*.ldf

## OS Files
.DS_Store
Thumbs.db
desktop.ini
*.bak
*.tmp

## MAUI / Xamarin Specific
*.apk
*.aab
*.ipa
*.dSYM
*.mobileprovision

## Azure Functions
local.settings.json
__blobstorage__/
__queuestorage__/
__azurite_db*__.json
```

---

#### Example 2: Node.js / JavaScript Projects

```gitignore
# ============================================
# Node.js / JavaScript / TypeScript
# ============================================

## Dependencies
node_modules/
npm-debug.log*
yarn-debug.log*
yarn-error.log*
.pnpm-debug.log*
package-lock.json  # Optional, depends on team preference
yarn.lock          # Optional, depends on team preference

## Build outputs
dist/
build/
.next/
out/
.nuxt/
.cache/
.parcel-cache/

## Environment variables
.env
.env*.local
.env.development.local
.env.test.local
.env.production.local

## IDE
.vscode/
.idea/
*.swp
*.swo

## Testing
coverage/
.nyc_output/
*.lcov

## OS
.DS_Store
Thumbs.db

## Misc
*.log
.eslintcache
.stylelintcache
```

---

#### Example 3: Python Projects

```gitignore
# ============================================
# Python
# ============================================

## Byte-compiled / optimized
__pycache__/
*.py[cod]
*$py.class
*.so

## Virtual environments
venv/
env/
ENV/
.venv
.conda/

## Distribution / packaging
dist/
build/
*.egg-info/
*.egg
wheels/

## PyCharm / VS Code
.idea/
.vscode/
*.swp

## Jupyter Notebooks
.ipynb_checkpoints/
*.ipynb

## Environment
.env
.env.local
*.env

## Testing
.pytest_cache/
.coverage
htmlcov/
.tox/

## Logs
*.log

## OS
.DS_Store
Thumbs.db
```

---

#### Example 4: Java / Spring Boot Projects

```gitignore
# ============================================
# Java / Spring Boot / Maven / Gradle
# ============================================

## Compiled class files
*.class
*.jar
*.war
*.ear
target/
build/

## Maven
.mvn/
mvnw
mvnw.cmd

## Gradle
.gradle/
gradle/
gradlew
gradlew.bat

## IDE
.idea/
*.iml
*.iws
*.ipr
.vscode/
.settings/
.classpath
.project

## Spring Boot
application-*.yml
application-*.properties
!application.yml
!application.properties

## Logs
*.log
logs/

## OS
.DS_Store
Thumbs.db
```

---

#### Example 5: Docker Projects

```gitignore
# ============================================
# Docker
# ============================================

## Docker
.dockerignore
docker-compose.override.yml
.docker/

## Environment
.env
.env.local
*.env

## Logs
logs/
*.log
```

---

#### Example 6: Universal (Multi-Language) Template

```gitignore
# ============================================
# Universal .gitignore Template
# Use this as a starting point for any project
# ============================================

## Dependencies
node_modules/
vendor/
packages/
bower_components/

## Build outputs
dist/
build/
out/
bin/
obj/
target/
*.dll
*.exe

## Environment & Secrets
.env
.env*.local
*.env
secrets/
config/secrets.yml
*.key
*.pem
*.p12

## IDE & Editors
.vscode/
.idea/
*.swp
*.swo
*~
.DS_Store
*.sublime-*

## Logs
logs/
*.log
npm-debug.log*
yarn-debug.log*

## Testing & Coverage
coverage/
.nyc_output/
test-results/
*.lcov

## OS Files
.DS_Store
Thumbs.db
desktop.ini
*.bak
*.tmp
*.cache

## Databases
*.db
*.sqlite
*.sqlite3

## Archives
*.zip
*.tar
*.gz
*.rar
*.7z
```

---

#### Pro Tips for .gitignore:

**1. Use gitignore.io:**
Visit [gitignore.io](https://www.toptal.com/developers/gitignore) to generate custom `.gitignore` files.

```bash
# Example: Generate .gitignore for Visual Studio, C#, and Windows
curl -L https://www.toptal.com/developers/gitignore/api/visualstudio,csharp,windows > .gitignore
```

**2. Check if a file is ignored:**
```bash
git check-ignore -v filename.txt
```

**3. Add exceptions with `!`:**
```gitignore
# Ignore all .json files
*.json

# But track this specific file
!important-config.json
```

**4. Ignore files already tracked:**
```bash
# If you accidentally committed a file that should be ignored
git rm --cached filename.txt
# Then add it to .gitignore
echo "filename.txt" >> .gitignore
git commit -m "Remove and ignore filename.txt"
```

**5. Global .gitignore (for OS/IDE files):**
```bash
# Create a global gitignore
git config --global core.excludesfile ~/.gitignore_global

# Add OS-specific files
echo ".DS_Store" >> ~/.gitignore_global
echo "Thumbs.db" >> ~/.gitignore_global
echo ".vscode/" >> ~/.gitignore_global
```

**6. View ignored files:**
```bash
# See all ignored files in your repository
git status --ignored
```

### 4. **Pull Before You Push**

Always pull the latest changes before starting work:

```bash
# Morning routine
git checkout main
git pull origin main
git checkout -b feature/new-feature
```

### 5. **Use Aliases for Common Commands**

Add these to your `.gitconfig`:

```bash
git config --global alias.co checkout
git config --global alias.br branch
git config --global alias.ci commit
git config --global alias.st status
git config --global alias.lg "log --oneline --graph --all --decorate"
```

Now you can use:
```bash
git co main          # Instead of git checkout main
git st               # Instead of git status
git lg               # Beautiful commit graph!
```

### 6. **Review Before You Commit**

```bash
# See what you're about to commit
git diff --staged

# Review each change interactively
git add -p
```

### 7. **Keep Your Branches Short-Lived**

Feature branches should live for days, not weeks:
- Create branch
- Develop feature
- Create PR
- Get reviewed
- Merge
- Delete branch

```bash
# Delete local branch after merge
git branch -d feature/old-feature

# Delete remote branch
git push origin --delete feature/old-feature
```

### 8. **Learn These Power Commands**

```bash
# Stash changes temporarily
git stash save "Work in progress"
git stash pop

# Cherry-pick a commit from another branch
git cherry-pick <commit-hash>


---

## 🎯 Workflow Diagram

```plaintext
┌─────────────────────────────────────────────────────────┐
│                    DEVELOPMENT CYCLE                     │
└─────────────────────────────────────────────────────────┘

    START
      │
      ▼
┌─────────────┐
│ git pull    │  Update local main branch
│ origin main │
└──────┬──────┘
       │
       ▼
┌─────────────────────┐
│ git checkout -b     │  Create feature branch
│ feature/new-feature │
└──────┬──────────────┘
       │
       ▼
┌─────────────┐
│  Write Code │  ✍️ Code, test, debug
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  git add .  │  Stage changes
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ git commit  │  Commit with message
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  git push   │  Push to remote
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ Create PR   │  📋 On GitHub.com
└──────┬──────┘
       │
       ▼
┌─────────────┐
│Code Review  │  👀 Team reviews
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ Merge PR    │  🎉 Into main
└──────┬──────┘
       │
       ▼
┌─────────────┐
│Delete Branch│  🧹 Clean up
└──────┬──────┘
       │
       ▼
    REPEAT
```
---

**Happy Coding! May your merges be conflict-free and your commits be meaningful!** 🚀✨

*Last Updated: December 19, 2025*
*Version: 1.0.0*
*Author: GitHub Consultant Team*


