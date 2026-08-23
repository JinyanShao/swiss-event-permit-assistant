# Project Continuity

This file preserves the working context needed to understand, run, test, and redeploy Swiss Event Permit Assistant without relying on chat history.

Do not store passwords, tokens, Azure publish profiles, GitHub credentials, recovery codes, or MFA backup codes in this repository. Keep those in a personal password manager.

## Project Goal

Swiss Event Permit Assistant helps a user prepare for temporary event permit requirements in the Ville de Fribourg. It turns questionnaire answers into sourced actions, documents, deadlines, information items, and points that must be confirmed with the competent authority.

The product is independent. It is not an official service of the Ville de Fribourg, Canton de Fribourg, prefectures, Police locale, or OCN.

## V0.1 Scope

V0.1 covers:

- Canton de Fribourg
- Ville de Fribourg
- Temporary public event preparation
- French-first user interface
- Questionnaire -> results workflow
- Official source links and checked dates
- Required actions, documents, useful information, deadlines, and confirmation items
- Browser localStorage draft persistence
- No server-side dossier storage

V0.1 does not:

- Submit official applications
- Log in to egov.fr.ch
- Replace official authorities
- Provide legal advice
- Guarantee approval
- Fill PDFs
- Upload attachments
- Store dossiers on the server
- Create user accounts

## Production

- Production URL: https://sepa-fribourg-jinyan.azurewebsites.net
- Health check: https://sepa-fribourg-jinyan.azurewebsites.net/healthz
- Production host: Azure App Service
- Azure resource group: `rg-swiss-event-permit-assistant-neu`
- Azure App Service Plan: `asp-sepa-f1-fr`
- Azure Web App: `sepa-fribourg-jinyan`
- Azure region: France Central
- Azure SKU: Free
- Runtime: .NET 10
- Environment: `Production`
- HTTPS-only: enabled

West Europe was attempted first but Azure did not accept a new App Service resource there at the time. North Europe was attempted next but the subscription quota blocked it. France Central was the first successful Free-tier European deployment target.

## GitHub

- Repository: https://github.com/JinyanShao/swiss-event-permit-assistant
- Default branch: `main`
- Repository homepage: https://sepa-fribourg-jinyan.azurewebsites.net
- License: MIT
- CI: GitHub Actions at `.github/workflows/ci.yml`

Current workflow habit:

1. Identify one issue.
2. Fix it.
3. Run tests or otherwise verify nothing broke.
4. Commit only intended files.
5. Push to GitHub.
6. Check GitHub Actions.

## Technical Stack

- .NET 10
- ASP.NET Core Razor Pages
- C# domain rules
- xUnit tests
- GitHub Actions CI
- Azure App Service deployment
- No database in V0.1
- Browser localStorage for questionnaire draft state

Main projects:

- `src/SwissEventPermitAssistant.Domain`: domain model, official sources, and rule evaluation
- `src/SwissEventPermitAssistant.Web`: Razor Pages web application
- `tests/SwissEventPermitAssistant.Tests`: xUnit business rule tests

Important runtime details:

- `EventRulesEvaluator` uses injected `TimeProvider`; production uses `TimeProvider.System`.
- Tests use a fixed `TimeProvider` so deadline status assertions remain deterministic.
- Official source IDs and user-facing source links are separate from the natural-language UI.

## Local Run

From the repository root:

```bash
dotnet restore
dotnet run --project src/SwissEventPermitAssistant.Web
```

Then open the local URL printed by `dotnet run`.

## Test

Run all tests:

```bash
dotnet test SwissEventPermitAssistant.slnx --configuration Release
```

Release verification:

```bash
dotnet restore
dotnet build SwissEventPermitAssistant.slnx --configuration Release --no-restore
dotnet test SwissEventPermitAssistant.slnx --configuration Release --no-build
dotnet publish src/SwissEventPermitAssistant.Web/SwissEventPermitAssistant.Web.csproj --configuration Release --output ./artifacts/publish
```

Production smoke checks:

```bash
curl https://sepa-fribourg-jinyan.azurewebsites.net/healthz
```

Expected response:

```text
Healthy
```

## Azure Deployment

Prerequisites:

- Azure CLI installed
- Logged in with `az login`
- Access to the Azure subscription that owns the App Service resources
- No secrets committed to the repo

Publish current source locally:

```bash
dotnet test SwissEventPermitAssistant.slnx --configuration Release
dotnet publish src/SwissEventPermitAssistant.Web/SwissEventPermitAssistant.Web.csproj --configuration Release --output /tmp/sepa-azure-publish
```

Create a zip package:

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

Confirm app settings and runtime:

```bash
az webapp config appsettings list \
  --resource-group rg-swiss-event-permit-assistant-neu \
  --name sepa-fribourg-jinyan

az webapp config show \
  --resource-group rg-swiss-event-permit-assistant-neu \
  --name sepa-fribourg-jinyan
```

Required production setting:

```text
ASPNETCORE_ENVIRONMENT=Production
```

Verify production:

```bash
curl -i https://sepa-fribourg-jinyan.azurewebsites.net/
curl -i https://sepa-fribourg-jinyan.azurewebsites.net/healthz
```

After any URL change, update the GitHub repository homepage:

```bash
gh repo edit JinyanShao/swiss-event-permit-assistant \
  --homepage https://sepa-fribourg-jinyan.azurewebsites.net
```

## Official Sources

Official source inventory and freshness policy live in:

- `docs/sources/README.md`
- `src/SwissEventPermitAssistant.Domain/Sources/OfficialSource.cs`

Last product rule review recorded in the source inventory:

- 2026-08-14

Freshness policy:

- Recheck sources before any public release, policy change, or rule change.
- Treat rules older than 90 days since the last checked date as stale until manually reviewed.
- If a page changes or interpretation is ambiguous, mark the affected result as `Needs Confirmation` instead of inferring a requirement.

## Current Unresolved Rules

V0.1 intentionally keeps these as confirmation items rather than automatic requirements:

- Patente K treatment for some free food or free alcohol cases where public pages do not make the operational path fully clear.
- Formulaire A/B role in the current online Patente K workflow when public sources do not state whether upload is required.
- Police locale authorization deadline where Ville de Fribourg public pages do not provide a precise lead time.
- Formulaire B for `manifestation d'importance`: no objective public threshold is confirmed for automatic triggering.
- Public event on private venue: Police locale authorization may be needed, but V0.1 does not infer it automatically without authority confirmation.

## Current Stage

Current project stage: user validation.

V0.1 development is complete. Stop adding features unless user validation uncovers a concrete V0.1 blocker.

Recent validation focus:

- Whether the questionnaire is understandable.
- Whether result actions and deadlines are actionable.
- Whether `Patente K`, `À confirmer`, deadline status, and official source links are clear to real users.
- Whether mobile layout and long French text remain readable.

## Next Step

Run short usability tests with 3-5 real users.

For each tester, record:

- Scenario used
- Device
- Where the questionnaire caused hesitation
- Whether results were understandable and actionable
- Which rule or term caused confusion
- Whether `À confirmer` was understood correctly
- Whether official source links increased trust

Do not plan V0.2 yet. First collect user validation evidence and identify whether V0.1 needs only wording/polish fixes or has a true usability blocker.
