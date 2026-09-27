# Deployment

How to ship a new release of Swiss Event Permit Assistant to production.

Do not store passwords, tokens, Azure publish profiles, GitHub credentials, or MFA backup codes in this repository. Keep those in a personal password manager.

## Production Environment

| | |
| --- | --- |
| URL | https://sepa-jinyan.azurewebsites.net |
| Health endpoint | https://sepa-jinyan.azurewebsites.net/healthz |
| Host | Azure App Service |
| Resource group | `rg-sepa` |
| App Service Plan | `asp-sepa-free` |
| Web App name | `sepa-jinyan` |
| Region | Italy North |
| SKU | Free (F1, Linux) |
| Runtime | .NET 10 |
| HTTPS-only | enabled |

Required app setting:

```text
ASPNETCORE_ENVIRONMENT=Production
```

## Prerequisites

- Azure CLI installed and logged in (`az login`)
- Access to the Azure subscription that owns the App Service resources above
- Subscription: Azure for Students (HES-SO account). Azure Cloud Shell can be used instead of a local Azure CLI.
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
  --resource-group rg-sepa \
  --name sepa-jinyan \
  --src-path /tmp/sepa-azure-main.zip \
  --type zip \
  --async false
```

## Verify

Confirm app settings and runtime:

```bash
az webapp config appsettings list \
  --resource-group rg-sepa \
  --name sepa-jinyan

az webapp config show \
  --resource-group rg-sepa \
  --name sepa-jinyan
```

Smoke test:

```bash
curl -i https://sepa-jinyan.azurewebsites.net/
curl -i https://sepa-jinyan.azurewebsites.net/healthz
```

## If the Production URL Changes

Update the GitHub repository homepage to match:

```bash
gh repo edit JinyanShao/swiss-event-permit-assistant \
  --homepage https://sepa-jinyan.azurewebsites.net
```
