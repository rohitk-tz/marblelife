# API/Properties/PublishProfiles - AI Context

## Purpose

This folder contains Web Deploy publish profiles for deploying the API to different environments (QA, Production, etc.).

## Contents

Publish profiles (.pubxml files):
- **WebDeploy_QA.pubxml**: QA environment deployment configuration
- **Production.pubxml**: Production environment deployment configuration
- Other environment-specific profiles

## For AI Agents & Human Developers

**Publish profiles** configure deployment settings:
- Target server URL
- Authentication method
- Database connection strings
- File publish options
- Pre/post-deployment actions

### Publishing from Visual Studio:
1. Right-click project → Publish
2. Select publish profile
3. Verify settings
4. Click Publish

### Publishing from Command Line:
```bash
msbuild API.csproj /p:DeployOnBuild=true /p:PublishProfile=WebDeploy_QA
```

**Security Note**: Never commit credentials in publish profiles. Use parameters or secure configuration.
