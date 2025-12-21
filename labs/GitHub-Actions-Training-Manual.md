# GitHub Actions Training Manual
## Complete Guide to CI/CD with docker-cicd.yml

---

## Table of Contents

1. [Introduction to GitHub Actions](#introduction-to-github-actions)
2. [Understanding the Workflow File](#understanding-the-workflow-file)
3. [Workflow Components Deep Dive](#workflow-components-deep-dive)
4. [Practical Examples and Use Cases](#practical-examples-and-use-cases)
5. [Troubleshooting Guide](#troubleshooting-guide)
6. [Best Practices](#best-practices)
7. [Advanced Topics](#advanced-topics)
8. [Hands-On Exercises](#hands-on-exercises)

---

## Introduction to GitHub Actions

### What is GitHub Actions?

GitHub Actions is a continuous integration and continuous delivery (CI/CD) platform that allows you to automate your build, test, and deployment pipeline. You can create workflows that build and test every pull request to your repository, or deploy merged pull requests to production.

### Key Concepts

#### 1. **Workflows**
- YAML files stored in `.github/workflows/` directory
- Define automated processes
- Triggered by events (push, pull request, schedule, etc.)
- Can contain one or more jobs

#### 2. **Events**
- Triggers that start a workflow
- Examples: `push`, `pull_request`, `schedule`, `workflow_dispatch`
- Can be filtered by branches, tags, or paths

#### 3. **Jobs**
- Set of steps that execute on the same runner
- Run in parallel by default
- Can have dependencies on other jobs

#### 4. **Steps**
- Individual tasks within a job
- Can run commands or actions
- Execute sequentially

#### 5. **Actions**
- Reusable units of code
- Can be created by GitHub, the community, or yourself
- Used with the `uses` keyword

#### 6. **Runners**
- Servers that run workflows
- GitHub-hosted or self-hosted
- Available OS: Ubuntu Linux, Windows, macOS

---

## Understanding the Workflow File

### Location and Structure

Our workflow file is located at:
```
.github/workflows/docker-cicd.yml
```

### Workflow Overview

```yaml
name: Docker CI/CD Pipeline
```

**Purpose**: This workflow automates the build, test, and deployment process for the ShippingRules application, a .NET-based shipping management system.

**Pipeline Stages**:
1. **Build & Test** - Compiles the .NET application and runs tests
2. **Deploy to Production** - Deploys to Azure App Service (conditional)

---

## Workflow Components Deep Dive

### 1. Workflow Triggers (on)

```yaml
on:
  push:
    branches: [ feature/manuals ]
```

**Explanation**:
- **Event**: `push` - Triggers when code is pushed to the repository
- **Branch Filter**: Only triggers when pushing to `feature/manuals` branch
- This is a focused trigger for development work on the manuals feature

**Alternative Trigger Configurations**:

```yaml
# Multiple branches
on:
  push:
    branches: [ main, master, develop ]

# All feature branches
on:
  push:
    branches: 
      - main
      - 'feature/**'

# Pull requests
on:
  pull_request:
    branches: [ main ]

# Manual trigger
on:
  workflow_dispatch:
    inputs:
      environment:
        description: 'Target environment'
        required: true
        type: choice
        options:
          - staging
          - production

# Scheduled runs
on:
  schedule:
    - cron: '0 2 * * *'  # Daily at 2 AM UTC
```

### 2. Environment Variables (env)

```yaml
env:
  DOCKER_REGISTRY: docker.io
  IMAGE_NAME: easternshipping/api
  DOTNET_VERSION: '8.0.x'
```

**Purpose**: Define global variables accessible across all jobs and steps.

**Benefits**:
- **Centralized Configuration**: Change version in one place
- **Reusability**: Reference with `${{ env.VARIABLE_NAME }}`
- **Maintainability**: Easy to update and manage

**Accessing Environment Variables**:
```yaml
# In workflow
${{ env.DOCKER_REGISTRY }}

# In shell commands
$DOCKER_REGISTRY (Linux/Mac)
$env:DOCKER_REGISTRY (Windows PowerShell)
```

### 3. Job 1: Build & Test

#### Job Configuration

```yaml
build-and-test:
  name: 🏗️ Build & Test Application
  runs-on: ubuntu-latest
```

**Components**:
- **Job ID**: `build-and-test` (used for dependencies and references)
- **Display Name**: Shown in GitHub UI with emoji for visual identification
- **Runner**: `ubuntu-latest` - GitHub-hosted Ubuntu Linux runner

**Runner Options**:
- `ubuntu-latest`, `ubuntu-22.04`, `ubuntu-20.04`
- `windows-latest`, `windows-2022`, `windows-2019`
- `macos-latest`, `macos-13`, `macos-12`

#### Step-by-Step Breakdown

##### Step 1: Checkout Code

```yaml
- name: 📥 Checkout code
  uses: actions/checkout@v4
```

**Purpose**: Downloads repository code to the runner.

**Action Details**:
- **Action**: `actions/checkout@v4` - Official GitHub action
- **Version**: `v4` - Using version 4 for latest features and security
- **Default Behavior**: 
  - Checks out the commit that triggered the workflow
  - Fetches only the latest commit (shallow clone)

**Advanced Options**:
```yaml
- uses: actions/checkout@v4
  with:
    fetch-depth: 0        # Fetch all history for all branches
    submodules: true      # Checkout submodules
    token: ${{ secrets.PAT }}  # Use Personal Access Token
```

##### Step 2: Setup .NET

```yaml
- name: 🔧 Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: ${{ env.DOTNET_VERSION }}
```

**Purpose**: Installs the specified .NET SDK version on the runner.

**Configuration**:
- Uses environment variable for version flexibility
- `.NET 8.0.x` - Latest patch version of .NET 8

**Why This Matters**:
- Ensures consistent build environment
- Allows testing against specific .NET versions
- Can install multiple versions if needed

**Multiple Versions Example**:
```yaml
- uses: actions/setup-dotnet@v4
  with:
    dotnet-version: |
      6.0.x
      7.0.x
      8.0.x
```

##### Step 3: Restore Dependencies

```yaml
- name: 📦 Restore dependencies
  run: dotnet restore src/ShippingRules.API/ShippingRules.API.csproj
```

**Purpose**: Downloads all NuGet packages required by the project.

**Key Points**:
- Targets specific project file: `ShippingRules.API.csproj`
- Creates packages cache for faster subsequent builds
- Validates package references and compatibility

**Best Practices**:
- Restore before building to separate concerns
- Use `--locked-mode` for reproducible builds in production
- Consider caching packages for faster runs

**With Caching**:
```yaml
- uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-nuget-

- name: 📦 Restore dependencies
  run: dotnet restore src/ShippingRules.API/ShippingRules.API.csproj
```

##### Step 4: Build Solution

```yaml
- name: 🏗️ Build solution
  run: dotnet build src/ShippingRules.API/ShippingRules.API.csproj --configuration Release --no-restore
```

**Purpose**: Compiles the application code.

**Build Options**:
- `--configuration Release`: Optimized build for production
- `--no-restore`: Skip restore (already done in previous step)

**Configuration Types**:
- **Debug**: Includes debugging symbols, no optimization
- **Release**: Optimized code, minimal debugging info

**Additional Options**:
```bash
--verbosity detailed      # More build output
--no-incremental          # Clean build
--output ./publish        # Specify output directory
```

##### Step 5: Run Tests

```yaml
- name: 🧪 Run tests
  run: |
    if [ -d "tests" ]; then
      dotnet test tests/**/*.csproj --configuration Release --verbosity normal --logger trx
    else
      echo "No tests found, skipping test step"
    fi
```

**Purpose**: Executes unit and integration tests.

**Script Breakdown**:
- **Conditional Check**: `if [ -d "tests" ]` - Checks if tests directory exists
- **Pattern Matching**: `tests/**/*.csproj` - Finds all test projects
- **Logger**: `trx` - Generates Visual Studio Test Results file
- **Graceful Handling**: Skips if no tests exist (doesn't fail workflow)

**Test Options**:
```bash
--no-build                        # Use already built assemblies
--collect:"XPlat Code Coverage"   # Generate code coverage
--filter "Category=Unit"          # Run specific test categories
--blame-crash                     # Collect crash dumps
```

**Test Results Formats**:
- `trx` - Visual Studio Test Results
- `junit` - JUnit XML format
- `html` - HTML test report

##### Step 6: Upload Test Results

```yaml
- name: 📊 Upload test results
  if: always()
  uses: actions/upload-artifact@v4
  with:
    name: test-results
    path: '**/TestResults/**/*.trx'
```

**Purpose**: Saves test results as workflow artifacts.

**Key Features**:
- `if: always()`: Runs even if previous steps failed
- Preserves test results for 90 days (default)
- Available for download from workflow run page

**Artifact Use Cases**:
- Share test reports with team
- Debug failed tests
- Compliance and audit trails
- Integration with external tools

**Advanced Artifact Configuration**:
```yaml
- uses: actions/upload-artifact@v4
  with:
    name: test-results-${{ github.run_number }}
    path: |
      **/TestResults/**/*.trx
      **/coverage/**/*.xml
    retention-days: 30
    if-no-files-found: warn
```

##### Step 7: Build Summary

```yaml
- name: ✅ Build Summary
  run: |
    echo "### 🏗️ Build Complete" >> $GITHUB_STEP_SUMMARY
    echo "- .NET Version: ${{ env.DOTNET_VERSION }}" >> $GITHUB_STEP_SUMMARY
    echo "- Build Status: ✅ Success" >> $GITHUB_STEP_SUMMARY
```

**Purpose**: Creates a rich summary on the workflow run page.

**$GITHUB_STEP_SUMMARY**:
- Special environment file
- Supports Markdown formatting
- Appears on workflow summary page
- Visible to all collaborators

**Enhanced Summary Examples**:
```bash
echo "## Build Results" >> $GITHUB_STEP_SUMMARY
echo "" >> $GITHUB_STEP_SUMMARY
echo "| Property | Value |" >> $GITHUB_STEP_SUMMARY
echo "|----------|-------|" >> $GITHUB_STEP_SUMMARY
echo "| Branch | ${{ github.ref_name }} |" >> $GITHUB_STEP_SUMMARY
echo "| Commit | ${{ github.sha }} |" >> $GITHUB_STEP_SUMMARY
echo "| Author | ${{ github.actor }} |" >> $GITHUB_STEP_SUMMARY
```

### 4. Job 2: Deploy to Production

#### Job Configuration

```yaml
deploy-production:
  name: 🚀 Deploy to Production
  runs-on: ubuntu-latest
  needs: build-and-test
  if: github.event.inputs.environment == 'production' || startsWith(github.ref, 'refs/tags/v')
  environment:
    name: production
    url: ${{ steps.deploy.outputs.url }}
```

**Components Explained**:

##### Job Dependencies

```yaml
needs: build-and-test
```

**Purpose**: Ensures deployment only runs after successful build.

**Dependency Patterns**:
```yaml
# Single dependency
needs: build

# Multiple dependencies
needs: [build, test, security-scan]

# Matrix dependencies
needs: [build-${{ matrix.os }}]
```

##### Conditional Execution

```yaml
if: github.event.inputs.environment == 'production' || startsWith(github.ref, 'refs/tags/v')
```

**Purpose**: Deploy only when specific conditions are met.

**Conditions**:
1. Manual workflow trigger with production environment selected
2. Code is pushed to a tag starting with 'v' (e.g., v1.0.0)

**Common Conditionals**:
```yaml
# Branch-based
if: github.ref == 'refs/heads/main'

# Event-based
if: github.event_name == 'push'

# Actor-based
if: github.actor != 'dependabot[bot]'

# Combined conditions
if: github.ref == 'refs/heads/main' && github.event_name == 'push'

# Success/Failure
if: success()
if: failure()
if: always()
```

##### Environment Protection

```yaml
environment:
  name: production
  url: ${{ steps.deploy.outputs.url }}
```

**Purpose**: Links deployment to GitHub Environment with protection rules.

**Environment Features**:
- **Required Reviewers**: Mandate approvals before deployment
- **Wait Timer**: Delay deployment for a specified period
- **Deployment Branches**: Restrict which branches can deploy
- **Secrets**: Environment-specific secrets
- **Deployment History**: Track all deployments

**Configuring Environments**:
1. Go to Repository Settings → Environments
2. Create "production" environment
3. Configure protection rules:
   - Add required reviewers
   - Set wait timer (e.g., 5 minutes)
   - Restrict to specific branches

#### Deployment Steps

##### Step 1: Azure Login

```yaml
- name: 🔐 Azure Login
  uses: azure/login@v1
  with:
    creds: ${{ secrets.AZURE_CREDENTIALS }}
```

**Purpose**: Authenticates with Azure for deployment.

**Secrets Required**:
```json
{
  "clientId": "<GUID>",
  "clientSecret": "<SECRET>",
  "subscriptionId": "<GUID>",
  "tenantId": "<GUID>"
}
```

**Setting Up Azure Credentials**:
```bash
# Create service principal
az ad sp create-for-rbac --name "GitHubActions" \
  --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --sdk-auth

# Add to GitHub Secrets as AZURE_CREDENTIALS
```

##### Step 2: Deploy to Azure

```yaml
- name: 🚀 Deploy to Azure App Service
  id: deploy
  uses: azure/webapps-deploy@v3
  with:
    app-name: ${{ vars.AZURE_WEBAPP_NAME }}
    images: ${{ env.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
```

**Key Points**:
- **Step ID**: `deploy` - Used to reference outputs in later steps
- **Variables**: `vars.AZURE_WEBAPP_NAME` - Repository or environment variable
- **Image Tag**: `github.sha` - Unique identifier for each deployment

**GitHub Context Variables**:
- `github.sha`: Commit SHA that triggered workflow
- `github.ref`: Branch or tag reference
- `github.actor`: User who triggered workflow
- `github.repository`: Repository name (owner/repo)
- `github.run_number`: Unique workflow run number

##### Step 3: Health Check

```yaml
- name: 🏥 Health Check
  run: |
    sleep 30
    curl --fail --retry 5 --retry-delay 10 ${{ steps.deploy.outputs.webapp-url }}/health || exit 1
```

**Purpose**: Verifies deployment was successful and app is responsive.

**Script Breakdown**:
- `sleep 30`: Wait for app to start (30 seconds)
- `curl --fail`: Return error code if HTTP status indicates failure
- `--retry 5`: Retry up to 5 times on failure
- `--retry-delay 10`: Wait 10 seconds between retries
- `|| exit 1`: Fail the step if curl ultimately fails

**Advanced Health Checks**:
```bash
# Check multiple endpoints
for endpoint in /health /api/health /ready; do
  curl --fail --retry 3 "$APP_URL$endpoint" || exit 1
done

# Validate response content
RESPONSE=$(curl -s "$APP_URL/health")
if [[ "$RESPONSE" != *"Healthy"* ]]; then
  echo "Health check failed: $RESPONSE"
  exit 1
fi
```

##### Step 4: Notifications

```yaml
- name: 📢 Notify Teams
  if: always()
  uses: jdcargile/ms-teams-notification@v1.4
  with:
    github-token: ${{ github.token }}
    ms-teams-webhook-uri: ${{ secrets.MS_TEAMS_WEBHOOK_URI }}
    notification-summary: "Production Deployment ${{ job.status }}"
    notification-color: ${{ job.status == 'success' && '28a745' || 'dc3545' }}
```

**Purpose**: Sends deployment status to Microsoft Teams channel.

**Features**:
- `if: always()`: Runs regardless of previous step results
- `job.status`: success, failure, or cancelled
- Conditional color: green for success, red for failure

**Alternative Notification Methods**:
```yaml
# Slack
- uses: slackapi/slack-github-action@v1
  with:
    webhook: ${{ secrets.SLACK_WEBHOOK }}
    payload: |
      {
        "text": "Deployment ${{ job.status }}",
        "username": "GitHub Actions",
        "icon_emoji": ":rocket:"
      }

# Email
- uses: dawidd6/action-send-mail@v3
  with:
    server_address: smtp.gmail.com
    server_port: 587
    username: ${{ secrets.EMAIL_USERNAME }}
    password: ${{ secrets.EMAIL_PASSWORD }}
    subject: Deployment ${{ job.status }}
    body: Deployment to production has ${{ job.status }}
    to: team@example.com

# Discord
- uses: sarisia/actions-status-discord@v1
  with:
    webhook: ${{ secrets.DISCORD_WEBHOOK }}
    status: ${{ job.status }}
    title: Production Deployment
```

---

## Practical Examples and Use Cases

### Example 1: Adding Code Quality Checks

```yaml
  code-quality:
    name: 🔍 Code Quality Analysis
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Install SonarScanner
        run: dotnet tool install --global dotnet-sonarscanner
      
      - name: Run SonarQube Analysis
        env:
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
        run: |
          dotnet sonarscanner begin /k:"ShippingApp" /d:sonar.host.url="${{ vars.SONAR_HOST }}" /d:sonar.token="$SONAR_TOKEN"
          dotnet build src/ShippingRules.API/ShippingRules.API.csproj
          dotnet sonarscanner end /d:sonar.token="$SONAR_TOKEN"
```

### Example 2: Matrix Strategy for Multi-Platform Builds

```yaml
  build-matrix:
    name: Build on ${{ matrix.os }}
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        dotnet-version: ['6.0.x', '7.0.x', '8.0.x']
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET ${{ matrix.dotnet-version }}
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ matrix.dotnet-version }}
      
      - name: Build
        run: dotnet build src/ShippingRules.API/ShippingRules.API.csproj
```

### Example 3: Database Migrations

```yaml
  database-migration:
    name: 🗄️ Database Migration
    runs-on: ubuntu-latest
    needs: build-and-test
    if: github.ref == 'refs/heads/main'
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Install EF Tools
        run: dotnet tool install --global dotnet-ef
      
      - name: Run Migrations
        env:
          CONNECTION_STRING: ${{ secrets.DB_CONNECTION_STRING }}
        run: |
          dotnet ef database update --project src/ShippingRules.Infrastructure \
            --connection "$CONNECTION_STRING"
```

### Example 4: Docker Build and Push

```yaml
  docker-build:
    name: 🐳 Build and Push Docker Image
    runs-on: ubuntu-latest
    needs: build-and-test
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3
      
      - name: Login to Docker Hub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}
      
      - name: Build and Push
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./src/ShippingRules.API/Dockerfile
          push: true
          tags: |
            ${{ env.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:latest
            ${{ env.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

---

## Troubleshooting Guide

### Common Issues and Solutions

#### Issue 1: Workflow Not Triggering

**Symptoms**: Push code but workflow doesn't run

**Solutions**:
1. Check branch name matches trigger configuration
2. Verify workflow file is in `.github/workflows/` directory
3. Ensure YAML syntax is valid (use YAML validator)
4. Check if Actions are enabled in repository settings

```yaml
# Check your trigger configuration
on:
  push:
    branches: [ feature/manuals ]  # Must match exactly
```

#### Issue 2: Build Fails - Package Restore Errors

**Symptoms**: Cannot restore NuGet packages

**Solutions**:
```yaml
# Add explicit NuGet sources
- name: Restore with sources
  run: |
    dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
    dotnet restore src/ShippingRules.API/ShippingRules.API.csproj
```

#### Issue 3: Tests Fail on CI but Pass Locally

**Symptoms**: Tests succeed on local machine, fail in Actions

**Common Causes**:
- Different time zones
- Missing environment variables
- File path differences (Windows vs Linux)
- Database connection issues

**Solutions**:
```yaml
- name: Run tests with detailed output
  run: |
    dotnet test --verbosity diagnostic --logger "console;verbosity=detailed"
  env:
    TZ: 'UTC'  # Set timezone
    ASPNETCORE_ENVIRONMENT: 'Test'
```

#### Issue 4: Secrets Not Working

**Symptoms**: Cannot access secrets, authentication fails

**Checklist**:
1. Verify secret name matches exactly (case-sensitive)
2. Check secret is defined at correct level (repo, environment, org)
3. Ensure secret doesn't contain trailing spaces or newlines
4. For JSON secrets, validate JSON format

```yaml
# Test secret availability (without exposing value)
- name: Check secret
  run: |
    if [ -z "${{ secrets.AZURE_CREDENTIALS }}" ]; then
      echo "Secret AZURE_CREDENTIALS is not set"
      exit 1
    fi
    echo "Secret is available"
```

#### Issue 5: Deployment Job Skipped

**Symptoms**: Build succeeds but deployment doesn't run

**Check**:
```yaml
if: github.event.inputs.environment == 'production' || startsWith(github.ref, 'refs/tags/v')
```

**Solutions**:
- Push a tag: `git tag v1.0.0 && git push origin v1.0.0`
- Use workflow_dispatch with production input
- Modify condition to match your needs

#### Issue 6: Timeout Errors

**Symptoms**: Job cancelled due to timeout

**Solutions**:
```yaml
jobs:
  build-and-test:
    runs-on: ubuntu-latest
    timeout-minutes: 30  # Default is 360 (6 hours)
    
    steps:
      - name: Long running step
        timeout-minutes: 10  # Per-step timeout
        run: ./long-script.sh
```

### Debugging Techniques

#### Enable Debug Logging

Set repository secrets:
- `ACTIONS_RUNNER_DEBUG`: `true`
- `ACTIONS_STEP_DEBUG`: `true`

#### Add Debug Steps

```yaml
- name: Debug Information
  run: |
    echo "Event name: ${{ github.event_name }}"
    echo "Ref: ${{ github.ref }}"
    echo "Actor: ${{ github.actor }}"
    echo "Run number: ${{ github.run_number }}"
    echo "Working directory:"
    pwd
    echo "Directory contents:"
    ls -la
    echo "Environment variables:"
    env | sort
```

#### Use act for Local Testing

```bash
# Install act (GitHub Actions runner for local)
brew install act  # macOS
choco install act-cli  # Windows

# Run workflow locally
act push

# Run specific job
act -j build-and-test

# Use specific runner image
act -P ubuntu-latest=catthehacker/ubuntu:full-latest
```

---

## Best Practices

### 1. Security Best Practices

#### Use Secrets for Sensitive Data

```yaml
# ✅ Good
- name: Deploy
  env:
    API_KEY: ${{ secrets.API_KEY }}
  run: ./deploy.sh

# ❌ Bad
- name: Deploy
  env:
    API_KEY: "hardcoded-key-12345"  # Never do this!
  run: ./deploy.sh
```

#### Restrict Permissions

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    permissions:
      contents: read      # Read repository contents
      packages: write     # Write to GitHub Packages
      security-events: write  # Write security events
```

#### Pin Action Versions

```yaml
# ✅ Good - Pinned to specific SHA
- uses: actions/checkout@b4ffde65f46336ab88eb53be808477a3936bae11  # v4.1.1

# ⚠️ Acceptable - Major version
- uses: actions/checkout@v4

# ❌ Avoid - Can break unexpectedly
- uses: actions/checkout@main
```

### 2. Performance Optimization

#### Use Caching

```yaml
- name: Cache NuGet packages
  uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-nuget-

- name: Cache Docker layers
  uses: actions/cache@v3
  with:
    path: /tmp/.buildx-cache
    key: ${{ runner.os }}-buildx-${{ github.sha }}
    restore-keys: |
      ${{ runner.os }}-buildx-
```

#### Parallelize Jobs

```yaml
jobs:
  unit-tests:
    runs-on: ubuntu-latest
    steps:
      - run: dotnet test --filter Category=Unit
  
  integration-tests:
    runs-on: ubuntu-latest
    steps:
      - run: dotnet test --filter Category=Integration
  
  # Both run in parallel
  
  deploy:
    needs: [unit-tests, integration-tests]
    runs-on: ubuntu-latest
    steps:
      - run: ./deploy.sh
```

#### Use Matrix Strategically

```yaml
strategy:
  matrix:
    project: 
      - ShippingRules.API
      - ShippingRules.Infrastructure
      - ShippingRules.Application
  fail-fast: false  # Continue other jobs if one fails
  max-parallel: 3   # Limit concurrent jobs
```

### 3. Maintainability

#### Use Composite Actions

Create `.github/actions/setup-dotnet-app/action.yml`:
```yaml
name: 'Setup .NET Application'
description: 'Checkout, setup .NET, and restore dependencies'
inputs:
  dotnet-version:
    description: '.NET version'
    required: false
    default: '8.0.x'
runs:
  using: 'composite'
  steps:
    - uses: actions/checkout@v4
    - uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ inputs.dotnet-version }}
    - run: dotnet restore
      shell: bash
```

Use in workflow:
```yaml
- name: Setup Application
  uses: ./.github/actions/setup-dotnet-app
  with:
    dotnet-version: '8.0.x'
```

#### Use Reusable Workflows

Create `.github/workflows/build-template.yml`:
```yaml
name: Build Template

on:
  workflow_call:
    inputs:
      dotnet-version:
        required: true
        type: string
      project-path:
        required: true
        type: string

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ inputs.dotnet-version }}
      - run: dotnet build ${{ inputs.project-path }}
```

Use template:
```yaml
jobs:
  build-api:
    uses: ./.github/workflows/build-template.yml
    with:
      dotnet-version: '8.0.x'
      project-path: 'src/ShippingRules.API/ShippingRules.API.csproj'
```

### 4. Documentation

#### Add Workflow Comments

```yaml
# ============================================================
# JOB: Build and Test
# Purpose: Compile application and run automated tests
# Triggers: On every push to feature/manuals branch
# Dependencies: None
# ============================================================
build-and-test:
  name: 🏗️ Build & Test Application
  runs-on: ubuntu-latest
```

#### Create Workflow Status Badges

Add to README.md:
```markdown
![CI/CD Pipeline](https://github.com/Xebia-Migration-Training/ShippingApp/workflows/Docker%20CI/CD%20Pipeline/badge.svg)
```

---

## Advanced Topics

### 1. Dynamic Matrix Generation

```yaml
jobs:
  prepare-matrix:
    runs-on: ubuntu-latest
    outputs:
      matrix: ${{ steps.set-matrix.outputs.matrix }}
    steps:
      - id: set-matrix
        run: |
          PROJECTS=$(find src -name "*.csproj" | jq -R -s -c 'split("\n")[:-1]')
          echo "matrix={\"project\":$PROJECTS}" >> $GITHUB_OUTPUT
  
  build:
    needs: prepare-matrix
    strategy:
      matrix: ${{ fromJson(needs.prepare-matrix.outputs.matrix) }}
    runs-on: ubuntu-latest
    steps:
      - run: dotnet build ${{ matrix.project }}
```

### 2. Custom Actions with Docker

Create `.github/actions/custom-action/Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
```

Create `entrypoint.sh`:
```bash
#!/bin/bash
echo "Running custom action"
dotnet build $1
```

Create `action.yml`:
```yaml
name: 'Custom Build Action'
description: 'Custom Docker-based build action'
inputs:
  project-path:
    description: 'Path to project file'
    required: true
runs:
  using: 'docker'
  image: 'Dockerfile'
  args:
    - ${{ inputs.project-path }}
```

### 3. Deployment Slots and Blue-Green Deployment

```yaml
  deploy-to-slot:
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to Staging Slot
        uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ vars.AZURE_WEBAPP_NAME }}
          slot-name: staging
          images: ${{ env.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
      
      - name: Run Smoke Tests
        run: |
          STAGING_URL="https://${{ vars.AZURE_WEBAPP_NAME }}-staging.azurewebsites.net"
          curl --fail "$STAGING_URL/health"
      
      - name: Swap Slots
        run: |
          az webapp deployment slot swap \
            --resource-group ${{ vars.AZURE_RESOURCE_GROUP }} \
            --name ${{ vars.AZURE_WEBAPP_NAME }} \
            --slot staging \
            --target-slot production
```

### 4. Handling Concurrent Deployments

```yaml
jobs:
  deploy:
    runs-on: ubuntu-latest
    concurrency:
      group: production-deployment
      cancel-in-progress: false  # Wait for current deployment to finish
    steps:
      - name: Deploy
        run: ./deploy.sh
```

### 5. Custom GitHub App for Enhanced Permissions

```yaml
jobs:
  create-pr:
    runs-on: ubuntu-latest
    steps:
      - name: Generate App Token
        id: generate-token
        uses: actions/create-github-app-token@v1
        with:
          app-id: ${{ secrets.APP_ID }}
          private-key: ${{ secrets.APP_PRIVATE_KEY }}
      
      - name: Create Pull Request
        env:
          GH_TOKEN: ${{ steps.generate-token.outputs.token }}
        run: |
          gh pr create --title "Auto PR" --body "Automated changes"
```

---

## Hands-On Exercises

### Exercise 1: Add Linting Step

**Task**: Add a step to run code linting before building.

**Solution**:
```yaml
- name: 🔍 Run Code Linter
  run: |
    dotnet tool install -g dotnet-format
    dotnet format --verify-no-changes --verbosity diagnostic \
      src/ShippingRules.API/ShippingRules.API.csproj
```

### Exercise 2: Add Code Coverage

**Task**: Generate and upload code coverage reports.

**Solution**:
```yaml
- name: 🧪 Run Tests with Coverage
  run: |
    dotnet test tests/**/*.csproj \
      --configuration Release \
      --logger trx \
      --collect:"XPlat Code Coverage" \
      --results-directory ./coverage

- name: 📊 Upload Coverage to Codecov
  uses: codecov/codecov-action@v3
  with:
    files: ./coverage/**/coverage.cobertura.xml
    flags: unittests
    name: codecov-umbrella
```

### Exercise 3: Multi-Environment Deployment

**Task**: Create separate deployment jobs for staging and production.

**Solution**:
```yaml
  deploy-staging:
    name: 🚀 Deploy to Staging
    runs-on: ubuntu-latest
    needs: build-and-test
    if: github.ref == 'refs/heads/develop'
    environment:
      name: staging
    steps:
      - uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ vars.STAGING_WEBAPP_NAME }}
          images: ${{ env.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
  
  deploy-production:
    name: 🚀 Deploy to Production
    runs-on: ubuntu-latest
    needs: [build-and-test, deploy-staging]
    if: github.ref == 'refs/heads/main'
    environment:
      name: production
    steps:
      - uses: azure/webapps-deploy@v3
        with:
          app-name: ${{ vars.PRODUCTION_WEBAPP_NAME }}
          images: ${{ env.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
```

### Exercise 4: Scheduled Dependency Updates

**Task**: Create a workflow that runs weekly to check for outdated dependencies.

**Solution**:
```yaml
name: Dependency Check

on:
  schedule:
    - cron: '0 9 * * 1'  # Every Monday at 9 AM UTC
  workflow_dispatch:

jobs:
  check-dependencies:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Check Outdated Packages
        run: |
          dotnet list src/ShippingRules.API/ShippingRules.API.csproj package --outdated > outdated.txt
          cat outdated.txt
      
      - name: Create Issue if Outdated
        if: contains(fileRead('outdated.txt'), 'Top-level Package')
        uses: actions/github-script@v7
        with:
          script: |
            github.rest.issues.create({
              owner: context.repo.owner,
              repo: context.repo.repo,
              title: 'Outdated Dependencies Found',
              body: 'Automated check found outdated dependencies. Please review.',
              labels: ['dependencies']
            })
```

### Exercise 5: Release Automation

**Task**: Automatically create a GitHub release when a tag is pushed.

**Solution**:
```yaml
name: Create Release

on:
  push:
    tags:
      - 'v*'

jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Build Release Assets
        run: |
          dotnet publish src/ShippingRules.API/ShippingRules.API.csproj \
            -c Release -o ./publish
          tar -czf ShippingRules-${{ github.ref_name }}.tar.gz -C ./publish .
      
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: ShippingRules-${{ github.ref_name }}.tar.gz
          generate_release_notes: true
          draft: false
          prerelease: false
```

---

## Quick Reference

### GitHub Context Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `github.sha` | Commit SHA | `ffac537e6cbbf934b08745a378932722df287a53` |
| `github.ref` | Full ref | `refs/heads/feature/manuals` |
| `github.ref_name` | Short ref | `feature/manuals` |
| `github.actor` | User who triggered | `octocat` |
| `github.repository` | Repo name | `Xebia-Migration-Training/ShippingApp` |
| `github.run_number` | Workflow run number | `42` |
| `github.event_name` | Event that triggered | `push` |

### Common Conditionals

```yaml
# Branch checks
if: github.ref == 'refs/heads/main'
if: startsWith(github.ref, 'refs/heads/feature/')

# Event checks
if: github.event_name == 'pull_request'
if: github.event_name == 'push'

# Job status
if: success()
if: failure()
if: always()
if: cancelled()

# File changes
if: contains(github.event.head_commit.message, '[deploy]')

# Actor checks
if: github.actor != 'dependabot[bot]'
```

### Useful Actions

| Action | Purpose | Usage |
|--------|---------|-------|
| `actions/checkout` | Checkout code | `uses: actions/checkout@v4` |
| `actions/setup-dotnet` | Setup .NET | `uses: actions/setup-dotnet@v4` |
| `actions/cache` | Cache dependencies | `uses: actions/cache@v3` |
| `actions/upload-artifact` | Upload artifacts | `uses: actions/upload-artifact@v4` |
| `actions/download-artifact` | Download artifacts | `uses: actions/download-artifact@v4` |
| `docker/build-push-action` | Build Docker image | `uses: docker/build-push-action@v5` |

---

## Additional Resources

### Official Documentation
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Workflow Syntax Reference](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)
- [GitHub Actions Marketplace](https://github.com/marketplace?type=actions)

### Community Resources
- [Awesome Actions](https://github.com/sdras/awesome-actions) - Curated list of actions
- [GitHub Actions Toolkit](https://github.com/actions/toolkit) - Build your own actions
- [Act](https://github.com/nektos/act) - Run actions locally

### Learning Paths
- GitHub Learning Lab: GitHub Actions Course
- Microsoft Learn: Automate your workflow with GitHub Actions
- LinkedIn Learning: Learning GitHub Actions

---

## Glossary

**Action**: A reusable unit of code that performs a specific task.

**Artifact**: Files produced by a workflow run (build outputs, test reports, logs).

**Concurrency**: Control over how multiple workflow runs execute simultaneously.

**Context**: Information about workflow runs, variables, and runner environments.

**Environment**: Deployment target with protection rules and secrets.

**Event**: Activity that triggers a workflow (push, pull request, schedule).

**Expression**: `${{ }}` syntax to evaluate variables and conditions.

**Job**: Set of steps that execute on the same runner.

**Matrix**: Strategy to run multiple job variations with different configurations.

**Runner**: Server that runs workflows (GitHub-hosted or self-hosted).

**Secret**: Encrypted environment variable for sensitive data.

**Step**: Individual task within a job (run command or action).

**Workflow**: Automated process defined by YAML file in `.github/workflows/`.

---

## Conclusion

This manual provides comprehensive coverage of GitHub Actions using the ShippingApp CI/CD pipeline as a practical example. By understanding the concepts, components, and best practices outlined here, you can:

1. ✅ Create robust CI/CD pipelines
2. ✅ Automate build, test, and deployment processes
3. ✅ Implement security best practices
4. ✅ Optimize workflow performance
5. ✅ Troubleshoot common issues
6. ✅ Extend workflows with advanced features

### Next Steps

1. **Practice**: Try the hands-on exercises
2. **Customize**: Modify the workflow for your specific needs
3. **Expand**: Add additional jobs (security scanning, performance testing)
4. **Share**: Document your workflows for team members
5. **Iterate**: Continuously improve based on feedback and learnings

### Support and Feedback

For questions or issues:
- Review this manual's troubleshooting section
- Check GitHub Actions documentation
- Open an issue in the repository
- Consult with your DevOps team

---

**Document Version**: 1.0  
**Last Updated**: December 21, 2025  
**Workflow File**: `.github/workflows/docker-cicd.yml`  
**Repository**: Xebia-Migration-Training/ShippingApp  
**Branch**: feature/manuals
