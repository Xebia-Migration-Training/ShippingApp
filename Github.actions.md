# GitHub Actions Manual - EasternShipping

## Overview
This manual provides comprehensive guidance for setting up GitHub Actions CI/CD pipelines for the EasternShipping application, a multi-project .NET solution with API, Web, and MAUI components.

## Workflow Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           GITHUB REPOSITORY                                  │
│                                                                              │
│  ┌─────────────┐      ┌─────────────┐      ┌─────────────┐                │
│  │   Feature   │      │   Develop   │      │     Main    │                 │
│  │   Branch    │─────▶│   Branch    │─────▶│   Branch    │                 │
│  └─────────────┘      └─────────────┘      └─────────────┘                 │
│        │                     │                     │                         │
└────────┼─────────────────────┼─────────────────────┼─────────────────────────┘
         │                     │                     │
         ▼                     ▼                     ▼
┌────────────────────────────────────────────────────────────────────────────┐
│                        GITHUB ACTIONS WORKFLOWS                             │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐  │
│  │  PR VALIDATION (pr-validation.yml)                                  │  │
│  │  ┌──────────┐   ┌──────────┐   ┌──────────┐   ┌──────────┐        │  │
│  │  │  Lint    │──▶│  Build   │──▶│   Test   │──▶│ Coverage │        │  │
│  │  │  Check   │   │  Check   │   │  Check   │   │  Report  │        │  │
│  │  └──────────┘   └──────────┘   └──────────┘   └──────────┘        │  │
│  └─────────────────────────────────────────────────────────────────────┘  │
│                                    │                                        │
│                                    ▼ (on merge)                             │
│  ┌─────────────────────────────────────────────────────────────────────┐  │
│  │  CONTINUOUS INTEGRATION (ci.yml)                                    │  │
│  │  ┌──────────┐   ┌──────────┐   ┌──────────┐   ┌──────────┐        │  │
│  │  │ Checkout │──▶│ Restore  │──▶│  Build   │──▶│   Test   │        │  │
│  │  │   Code   │   │   Deps   │   │ Solution │   │  & Cover │        │  │
│  │  └──────────┘   └──────────┘   └──────────┘   └────┬─────┘        │  │
│  └──────────────────────────────────────────────────────┼──────────────┘  │
│                                                          │                  │
│                                    ┌─────────────────────┴──────────┐      │
│                                    ▼                                 ▼      │
│  ┌──────────────────────────────────────┐   ┌──────────────────────────┐  │
│  │  DOCKER BUILD (docker-build.yml)    │   │  CD-API (cd-api.yml)     │  │
│  │  ┌──────────┐   ┌──────────┐        │   │  ┌──────────┐            │  │
│  │  │  Build   │──▶│   Push   │        │   │  │  Build   │            │  │
│  │  │  Image   │   │  to Hub  │        │   │  │ Artifact │            │  │
│  │  └──────────┘   └──────────┘        │   │  └────┬─────┘            │  │
│  └──────────────────────────────────────┘   │       │                  │  │
│                                              │       ▼                  │  │
│  ┌──────────────────────────────────────┐   │  ┌─────────┐            │  │
│  │  CD-WEB (cd-web.yml)                 │   │  │ Deploy  │            │  │
│  │  ┌──────────┐   ┌──────────┐        │   │  │ Staging │            │  │
│  │  │  Build   │──▶│  Deploy  │        │   │  └────┬────┘            │  │
│  │  │ Blazor   │   │  Static  │        │   │       │                  │  │
│  │  └──────────┘   └──────────┘        │   │       ▼                  │  │
│  └──────────────────────────────────────┘   │  ┌─────────┐            │  │
│                                              │  │ Deploy  │            │  │
│  ┌──────────────────────────────────────┐   │  │  Prod   │            │  │
│  │  RELEASE (release.yml)               │   │  └─────────┘            │  │
│  │  ┌──────────┐   ┌──────────┐        │   └──────────────────────────┘  │
│  │  │  Create  │──▶│  Generate│        │                                  │
│  │  │   Tag    │   │  Notes   │        │                                  │
│  │  └──────────┘   └──────────┘        │                                  │
│  └──────────────────────────────────────┘                                  │
│                                                                             │
└─────────────────────────────────────┬───────────────────────────────────────┘
                                      │
                ┌─────────────────────┼─────────────────────┐
                ▼                     ▼                     ▼
       ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐
       │  AZURE APP      │   │  DOCKER HUB     │   │  AZURE STORAGE  │
       │  SERVICE        │   │  REGISTRY       │   │  (Static Web)   │
       │                 │   │                 │   │                 │
       │  • API Staging  │   │  • API Images   │   │  • Blazor WASM  │
       │  • API Prod     │   │  • Tagged Ver.  │   │  • SPA Assets   │
       └─────────────────┘   └─────────────────┘   └─────────────────┘

                           DEPLOYMENT TARGETS
```

### Workflow Flow Description

1. **Code Push**: Developers push code to feature branches
2. **PR Validation**: Automated checks run on pull requests (lint, build, test, coverage)
3. **CI Pipeline**: On merge to main/develop, full CI pipeline executes
4. **Build Artifacts**: Solution is built and packaged for deployment
5. **Deployment**: 
   - API deployed to Azure App Service (staging → production)
   - Web app deployed as static site to Azure Storage
   - Docker images pushed to container registry
6. **Release Management**: Tagged releases with automated changelog generation

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Workflow Structure](#workflow-structure)
3. [CI/CD Workflows](#cicd-workflows)
4. [Secrets Configuration](#secrets-configuration)
5. [Deployment Strategies](#deployment-strategies)
6. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Secrets
Configure these in your GitHub repository: `Settings > Secrets and variables > Actions`

| Secret Name | Description | Example |
|------------|-------------|---------|
| `AZURE_CREDENTIALS` | Azure Service Principal JSON | `{"clientId":"...","clientSecret":"..."}` |
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Azure Web App publish profile | XML content |
| `DOCKER_USERNAME` | Docker Hub username | `youruser` |
| `DOCKER_PASSWORD` | Docker Hub password/token | `dckr_pat_...` |
| `SQL_CONNECTION_STRING` | Database connection string | `Server=...;Database=...` |
| `NUGET_API_KEY` | NuGet package repository key | Optional for private feeds |

### Required Variables
Configure in `Settings > Secrets and variables > Actions > Variables`

| Variable Name | Description | Example |
|--------------|-------------|---------|
| `AZURE_WEBAPP_NAME` | Azure Web App name | `easternshipping-api` |
| `RESOURCE_GROUP` | Azure resource group | `rg-easternshipping-prod` |
| `DOCKER_IMAGE_NAME` | Docker image name | `easternshipping/api` |
| `DOTNET_VERSION` | .NET SDK version | `8.0.x` or `9.0.x` |

---

## Workflow Structure

Create `.github/workflows/` directory in your repository root:

```
.github/
└── workflows/
    ├── ci.yml                      # Continuous Integration
    ├── cd-api.yml                  # Deploy API
    ├── cd-web.yml                  # Deploy Web App
    ├── docker-build.yml            # Docker Image Build & Push
    ├── pr-validation.yml           # Pull Request Validation
    └── release.yml                 # Release Management
```

---

## CI/CD Workflows

### 1. Continuous Integration (ci.yml)

Build, test, and validate all projects on push/PR.

```yaml
name: CI - Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

env:
  DOTNET_VERSION: '8.0.x'
  SOLUTION_PATH: 'ShippingRules.sln'

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0  # Full history for better analysis
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore ${{ env.SOLUTION_PATH }}
    
    - name: Build solution
      run: dotnet build ${{ env.SOLUTION_PATH }} --configuration Release --no-restore
    
    - name: Run tests
      run: dotnet test ${{ env.SOLUTION_PATH }} --configuration Release --no-build --verbosity normal --logger trx --collect:"XPlat Code Coverage"
    
    - name: Upload test results
      if: always()
      uses: actions/upload-artifact@v4
      with:
        name: test-results
        path: '**/TestResults/**/*.trx'
    
    - name: Upload coverage reports
      if: always()
      uses: actions/upload-artifact@v4
      with:
        name: coverage-reports
        path: '**/TestResults/**/coverage.cobertura.xml'
    
    - name: Code Coverage Report
      uses: irongut/CodeCoverageSummary@v1.3.0
      if: github.event_name == 'pull_request'
      with:
        filename: '**/coverage.cobertura.xml'
        badge: true
        format: markdown
        output: both
    
    - name: Add Coverage PR Comment
      uses: marocchino/sticky-pull-request-comment@v2
      if: github.event_name == 'pull_request'
      with:
        recreate: true
        path: code-coverage-results.md
```

### 2. API Deployment (cd-api.yml)

Deploy API to Azure App Service or container platform.

```yaml
name: CD - Deploy API

on:
  push:
    branches: [ main ]
    paths:
      - 'src/ShippingRules.API/**'
      - 'src/ShippingRules.Application/**'
      - 'src/ShippingRules.Domain/**'
      - 'src/ShippingRules.Infrastructure/**'
  workflow_dispatch:
    inputs:
      environment:
        description: 'Deployment environment'
        required: true
        default: 'staging'
        type: choice
        options:
          - staging
          - production

env:
  DOTNET_VERSION: '8.0.x'
  AZURE_WEBAPP_NAME: ${{ vars.AZURE_WEBAPP_NAME }}
  AZURE_WEBAPP_PACKAGE_PATH: './publish'

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Publish
      run: dotnet publish src/ShippingRules.API/ShippingRules.API.csproj -c Release -o ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
    
    - name: Upload artifact
      uses: actions/upload-artifact@v4
      with:
        name: api-package
        path: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}

  deploy-staging:
    runs-on: ubuntu-latest
    needs: build
    if: github.event.inputs.environment == 'staging' || (github.event_name == 'push' && github.ref == 'refs/heads/main')
    environment:
      name: staging
      url: ${{ steps.deploy.outputs.webapp-url }}
    
    steps:
    - name: Download artifact
      uses: actions/download-artifact@v4
      with:
        name: api-package
        path: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
    
    - name: Deploy to Azure Web App
      id: deploy
      uses: azure/webapps-deploy@v3
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}-staging
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_STAGING }}
        package: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
    
    - name: Health Check
      run: |
        sleep 30
        curl --fail ${{ steps.deploy.outputs.webapp-url }}/health || exit 1

  deploy-production:
    runs-on: ubuntu-latest
    needs: build
    if: github.event.inputs.environment == 'production'
    environment:
      name: production
      url: ${{ steps.deploy.outputs.webapp-url }}
    
    steps:
    - name: Download artifact
      uses: actions/download-artifact@v4
      with:
        name: api-package
        path: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
    
    - name: Deploy to Azure Web App
      id: deploy
      uses: azure/webapps-deploy@v3
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}
    
    - name: Health Check
      run: |
        sleep 30
        curl --fail ${{ steps.deploy.outputs.webapp-url }}/health || exit 1
```

### 3. Blazor Web Deployment (cd-web.yml)

Deploy Blazor web application to static hosting or Azure.

```yaml
name: CD - Deploy Web App

on:
  push:
    branches: [ main ]
    paths:
      - 'src/ShippingRules.Web/**'
  workflow_dispatch:

env:
  DOTNET_VERSION: '8.0.x'
  PUBLISH_PATH: './publish-web'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore src/ShippingRules.Web/ShippingRules.Web.csproj
    
    - name: Build
      run: dotnet build src/ShippingRules.Web/ShippingRules.Web.csproj --configuration Release --no-restore
    
    - name: Publish
      run: dotnet publish src/ShippingRules.Web/ShippingRules.Web.csproj -c Release -o ${{ env.PUBLISH_PATH }}
    
    - name: Deploy to Azure Static Web Apps
      uses: Azure/static-web-apps-deploy@v1
      with:
        azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
        repo_token: ${{ secrets.GITHUB_TOKEN }}
        action: "upload"
        app_location: "${{ env.PUBLISH_PATH }}/wwwroot"
        skip_app_build: true
```

### 4. Docker Build & Push (docker-build.yml)

Build and push Docker images to registry.

```yaml
name: Docker Build & Push

on:
  push:
    branches: [ main, develop ]
    tags:
      - 'v*'
  workflow_dispatch:

env:
  REGISTRY: docker.io
  IMAGE_NAME: ${{ vars.DOCKER_IMAGE_NAME || 'easternshipping/api' }}

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
    
    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3
    
    - name: Log in to Docker Hub
      uses: docker/login-action@v3
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ secrets.DOCKER_USERNAME }}
        password: ${{ secrets.DOCKER_PASSWORD }}
    
    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v5
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
        tags: |
          type=ref,event=branch
          type=ref,event=pr
          type=semver,pattern={{version}}
          type=semver,pattern={{major}}.{{minor}}
          type=sha,prefix={{branch}}-
    
    - name: Build and push Docker image
      uses: docker/build-push-action@v5
      with:
        context: .
        file: ./src/ShippingRules.API/Dockerfile
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
        cache-from: type=registry,ref=${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:buildcache
        cache-to: type=registry,ref=${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:buildcache,mode=max
    
    - name: Run Trivy vulnerability scanner
      uses: aquasecurity/trivy-action@master
      with:
        image-ref: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ steps.meta.outputs.version }}
        format: 'sarif'
        output: 'trivy-results.sarif'
    
    - name: Upload Trivy results to GitHub Security
      uses: github/codeql-action/upload-sarif@v3
      if: always()
      with:
        sarif_file: 'trivy-results.sarif'
```

### 5. Pull Request Validation (pr-validation.yml)

Comprehensive PR checks before merge.

```yaml
name: PR Validation

on:
  pull_request:
    branches: [ main, develop ]

jobs:
  validate:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore and Build
      run: |
        dotnet restore
        dotnet build --configuration Release --no-restore
    
    - name: Run Tests
      run: dotnet test --configuration Release --no-build --verbosity normal
    
    - name: Check Code Format
      run: dotnet format --verify-no-changes --verbosity diagnostic
    
    - name: Dependency Check
      run: dotnet list package --vulnerable --include-transitive
    
    - name: PR Size Check
      uses: actions/github-script@v7
      with:
        script: |
          const pr = context.payload.pull_request;
          if (pr.additions + pr.deletions > 1000) {
            core.setFailed('PR is too large. Please split into smaller PRs.');
          }
```

### 6. Release Management (release.yml)

Automated release creation and changelog generation.

```yaml
name: Release

on:
  push:
    tags:
      - 'v*.*.*'

jobs:
  create-release:
    runs-on: ubuntu-latest
    permissions:
      contents: write
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0
    
    - name: Generate changelog
      id: changelog
      run: |
        PREVIOUS_TAG=$(git describe --abbrev=0 --tags $(git rev-list --tags --skip=1 --max-count=1) 2>/dev/null || echo "")
        if [ -z "$PREVIOUS_TAG" ]; then
          CHANGELOG=$(git log --pretty=format:"- %s (%h)" HEAD)
        else
          CHANGELOG=$(git log --pretty=format:"- %s (%h)" ${PREVIOUS_TAG}..HEAD)
        fi
        echo "changelog<<EOF" >> $GITHUB_OUTPUT
        echo "$CHANGELOG" >> $GITHUB_OUTPUT
        echo "EOF" >> $GITHUB_OUTPUT
    
    - name: Create Release
      uses: actions/create-release@v1
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        tag_name: ${{ github.ref_name }}
        release_name: Release ${{ github.ref_name }}
        body: |
          ## Changes in this Release
          ${{ steps.changelog.outputs.changelog }}
          
          ## Docker Images
          - API: `docker pull ${{ vars.DOCKER_IMAGE_NAME }}:${{ github.ref_name }}`
        draft: false
        prerelease: false
```

---

## Secrets Configuration

### Setting Up Azure Credentials

1. **Create Service Principal:**
```bash
az ad sp create-for-rbac --name "github-actions-easternshipping" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} \
  --sdk-auth
```

2. **Add to GitHub Secrets:**
   - Copy the entire JSON output
   - Go to: `Repository > Settings > Secrets and variables > Actions > New repository secret`
   - Name: `AZURE_CREDENTIALS`
   - Value: Paste the JSON

### Setting Up Azure Web App Publish Profile

1. **Download from Azure Portal:**
   - Navigate to your App Service
   - Click "Get publish profile"
   - Save the XML file

2. **Add to GitHub Secrets:**
   - Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Value: Paste the entire XML content

### Setting Up Docker Credentials

1. **Create Docker Hub Access Token:**
   - Log in to Docker Hub
   - Account Settings > Security > New Access Token
   - Give it a descriptive name: "GitHub Actions"

2. **Add to GitHub Secrets:**
   - `DOCKER_USERNAME`: Your Docker Hub username
   - `DOCKER_PASSWORD`: The access token (not your password)

---

## Deployment Strategies

### Blue-Green Deployment

Use deployment slots in Azure:

```yaml
- name: Deploy to Staging Slot
  uses: azure/webapps-deploy@v3
  with:
    app-name: ${{ env.AZURE_WEBAPP_NAME }}
    slot-name: staging
    publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}

- name: Swap Slots
  run: |
    az webapp deployment slot swap \
      --resource-group ${{ vars.RESOURCE_GROUP }} \
      --name ${{ env.AZURE_WEBAPP_NAME }} \
      --slot staging \
      --target-slot production
```

### Canary Deployment

Deploy to a subset of users:

```yaml
- name: Deploy Canary (10% traffic)
  run: |
    az webapp traffic-routing set \
      --resource-group ${{ vars.RESOURCE_GROUP }} \
      --name ${{ env.AZURE_WEBAPP_NAME }} \
      --distribution staging=10 production=90
```

### Rolling Updates with Docker

```yaml
- name: Update Docker Service
  run: |
    docker service update \
      --image ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }} \
      --update-parallelism 2 \
      --update-delay 30s \
      easternshipping-api
```

---

## Environment-Specific Configuration

### Development
- Trigger: Every push to `develop` branch
- Database: Dev SQL Server instance
- Approvals: None required
- Notifications: Slack channel

### Staging
- Trigger: Every push to `main` branch
- Database: Staging SQL Server instance
- Approvals: None required
- Notifications: Email to team

### Production
- Trigger: Manual workflow dispatch or git tags
- Database: Production SQL Server instance
- Approvals: Required (2 reviewers)
- Notifications: Teams channel + PagerDuty

---

## Monitoring and Notifications

### Slack Notifications

```yaml
- name: Notify Slack
  if: always()
  uses: slackapi/slack-github-action@v1.25.0
  with:
    payload: |
      {
        "text": "Deployment ${{ job.status }}: ${{ github.repository }}",
        "blocks": [
          {
            "type": "section",
            "text": {
              "type": "mrkdwn",
              "text": "*Deployment Status:* ${{ job.status }}\n*Repository:* ${{ github.repository }}\n*Branch:* ${{ github.ref_name }}\n*Commit:* ${{ github.sha }}"
            }
          }
        ]
      }
  env:
    SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

### Microsoft Teams Notifications

```yaml
- name: Notify Teams
  if: always()
  uses: jdcargile/ms-teams-notification@v1.4
  with:
    github-token: ${{ github.token }}
    ms-teams-webhook-uri: ${{ secrets.MS_TEAMS_WEBHOOK_URI }}
    notification-summary: "Deployment ${{ job.status }}"
    notification-color: ${{ job.status == 'success' && '28a745' || 'dc3545' }}
```

---

## Troubleshooting

### Common Issues

#### 1. Build Failures - Missing Dependencies
**Problem:** NuGet restore fails
**Solution:**
```yaml
- name: Clear NuGet Cache
  run: dotnet nuget locals all --clear

- name: Restore with verbose logging
  run: dotnet restore --verbosity detailed
```

#### 2. Test Failures in CI
**Problem:** Tests pass locally but fail in CI
**Solution:**
- Check time zones: Set `TZ: UTC`
- Check culture: Set `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT: false`
- Increase timeouts for integration tests

#### 3. Docker Build Fails
**Problem:** Context size too large
**Solution:**
Create `.dockerignore`:
```
**/bin/
**/obj/
**/node_modules/
**/.git/
**/logs/
```

#### 4. Azure Deployment Timeouts
**Problem:** Deployment takes too long
**Solution:**
```yaml
- name: Deploy with extended timeout
  timeout-minutes: 30
  uses: azure/webapps-deploy@v3
```

#### 5. Permission Denied Errors
**Problem:** Cannot write to directories
**Solution:**
```yaml
- name: Fix Permissions
  run: |
    sudo chown -R $USER:$USER .
    chmod -R 755 .
```

### Debug Mode

Enable debug logging:

```yaml
env:
  ACTIONS_RUNNER_DEBUG: true
  ACTIONS_STEP_DEBUG: true
```

### Manual Workflow Triggers

For testing workflows manually:

```yaml
on:
  workflow_dispatch:
    inputs:
      logLevel:
        description: 'Log level'
        required: true
        default: 'warning'
        type: choice
        options:
          - info
          - warning
          - debug
      environment:
        description: 'Environment to deploy'
        required: true
        type: environment
```

---

## Best Practices

### 1. Use Workflow Templates
Create reusable workflows in `.github/workflows/templates/`:
```yaml
# .github/workflows/reusable-build.yml
on:
  workflow_call:
    inputs:
      dotnet-version:
        required: true
        type: string
```

### 2. Matrix Builds
Test across multiple versions:
```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
    dotnet: ['7.0.x', '8.0.x']
```

### 3. Caching
Speed up builds with caching:
```yaml
- name: Cache NuGet packages
  uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-nuget-
```

### 4. Dependency Updates
Automate dependency updates:
```yaml
# .github/dependabot.yml
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "monthly"
```

### 5. Security Scanning
```yaml
- name: Run CodeQL Analysis
  uses: github/codeql-action/analyze@v3
  with:
    category: "/language:csharp"
```

---

## Performance Optimization

### 1. Parallel Jobs
```yaml
jobs:
  build-api:
    runs-on: ubuntu-latest
  build-web:
    runs-on: ubuntu-latest
  build-maui:
    runs-on: windows-latest
```

### 2. Conditional Execution
```yaml
- name: Deploy only if tests pass
  if: success() && github.ref == 'refs/heads/main'
```

### 3. Skip CI for Docs
Add to commit message: `[skip ci]` or `[ci skip]`

---

## Maintenance

### Regular Tasks
- ✅ Review and update secrets every 90 days
- ✅ Update GitHub Actions versions monthly
- ✅ Review workflow runs for failures weekly
- ✅ Archive old workflow runs quarterly
- ✅ Update .NET SDK versions when released

### Monitoring Dashboard
Set up status badges in README.md:
```markdown
[![CI](https://github.com/YourOrg/EasternShipping/actions/workflows/ci.yml/badge.svg)](https://github.com/YourOrg/EasternShipping/actions/workflows/ci.yml)
[![Deploy API](https://github.com/YourOrg/EasternShipping/actions/workflows/cd-api.yml/badge.svg)](https://github.com/YourOrg/EasternShipping/actions/workflows/cd-api.yml)
```

---

## Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Web Apps Deploy Action](https://github.com/Azure/webapps-deploy)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)
- [Docker Build Push Action](https://github.com/docker/build-push-action)
- [GitHub Actions Marketplace](https://github.com/marketplace?type=actions)

---

**Last Updated:** December 20, 2025  
**Maintained by:** DevOps Team  
**Questions?** Create an issue in the repository
