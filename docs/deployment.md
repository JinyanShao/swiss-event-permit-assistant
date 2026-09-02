# Deployment

How to ship a new release of Swiss Event Permit Assistant to production.

Do not store passwords, tokens, Azure publish profiles, GitHub credentials, or MFA backup codes in this repository. Keep those in a personal password manager.

## Production Environment

| | |
| --- | --- |
| URL | https://sepa-fribourg-jinyan.azurewebsites.net |
| Health endpoint | https://sepa-fribourg-jinyan.azurewebsites.net/healthz |
| Host | Azure App Service |
| Resource group | `rg-swiss-event-permit-assistant-neu` |
| App Service Plan | `asp-sepa-f1-fr` |
| Web App name | `sepa-fribourg-jinyan` |
| Region | France Central |
| SKU | Free |
| Runtime | .NET 10 |
| HTTPS-only | enabled |

Required app setting:

```text
ASPNETCORE_ENVIRONMENT=Production
```

## Prerequisites

- Azure CLI installed and logged in (`az login`)
- Access to the Azure subscription that owns the App Service resources above
- No secrets committed to the repo

## Release Steps

Build and test:

```bash
dotnet test SwissEventPermitAssistant.slnx --configuration Release
dotnet publish src/SwissEventPermitAssistant.Web/SwissEventPermitAssistant.Web.csproj \
  --configuration Release \
  --output /tmp/sepa-azure-publish
```

Package:

```bash
cd /tmp/sepa-azure-publish
zip -qr /tmp/sepa-azure-main.zip .
```

Deploy to the existing App Service:

```bash
az webapp deploy \
  --resource-group rg-swiss-event-permit-assistant-neu \
  --name sepa-fribourg-jinyan \
  --src-path /tmp/sepa-azure-main.zip \
  --type zip \
  --async false
```

## Verify

Confirm app settings and runtime:

```bash
az webapp config appsettings list \
  --resource-group rg-swiss-event-permit-assistant-neu \
  --name sepa-fribourg-jinyan

az webapp config show \
  --resource-group rg-swiss-event-permit-assistant-neu \
  --name sepa-fribourg-jinyan
```

Smoke test:

```bash
curl -i https://sepa-fribourg-jinyan.azurewebsites.net/
curl -i https://sepa-fribourg-jinyan.azurewebsites.net/healthz
```

## If the Production URL Changes

Update the GitHub repository homepage to match:

```bash
gh repo edit JinyanShao/swiss-event-permit-assistant \
  --homepage https://sepa-fribourg-jinyan.azurewebsites.net
```
