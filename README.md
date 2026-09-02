# Swiss Event Permit Assistant

[![CI](https://github.com/JinyanShao/swiss-event-permit-assistant/actions/workflows/ci.yml/badge.svg)](https://github.com/JinyanShao/swiss-event-permit-assistant/actions/workflows/ci.yml)
[![Latest release](https://img.shields.io/github/v/release/JinyanShao/swiss-event-permit-assistant)](https://github.com/JinyanShao/swiss-event-permit-assistant/releases/latest)

**[Live demo](https://sepa-fribourg-jinyan.azurewebsites.net)** · [License: MIT](LICENSE)

V0.1 pilot for preparing temporary event permit dossiers for the Ville de Fribourg.

This project is an independent tool. It is not an official service of the Ville de Fribourg or the Canton de Fribourg. It helps users organize publicly available official requirements, deadlines, source links, and points that must be confirmed with the competent authority.

## Current Scope

- Canton de Fribourg
- Ville de Fribourg
- Temporary public event preparation
- French-first user interface
- Razor Pages web application
- Domain rules tested with xUnit

## V0.1 Product Boundary

V0.1 helps a user answer a questionnaire and receive:

- required actions
- documents to prepare
- information to gather
- deadlines
- items that need confirmation
- official source references

V0.1 does not:

- submit official applications
- log in to egov.fr.ch
- replace the Ville, Canton, Prefecture, Police locale, or OCN
- provide legal advice
- guarantee approval
- fill PDFs
- store dossiers on the server
- upload attachments
- create user accounts

## Technical Stack

- .NET 10
- ASP.NET Core Razor Pages
- C# domain rules
- xUnit tests
- No database in V0.1
- Optional browser localStorage for questionnaire draft state

## Repository Structure

```text
src/
  SwissEventPermitAssistant.Domain/
    Domain model and rule evaluation
  SwissEventPermitAssistant.Web/
    Razor Pages application
tests/
  SwissEventPermitAssistant.Tests/
    xUnit tests for domain rules and deadline behavior
docs/
  operations.md
    Local development, testing, and maintenance
  deployment.md
    Azure production deployment
  sources/
    Official source inventory and checked dates
  product/
    Product and UI specifications
```

## Development

Restore and test:

```bash
dotnet restore
dotnet test
```

Run the web app:

```bash
dotnet run --project src/SwissEventPermitAssistant.Web
```

## Deployment and Operations

The current public demo runs on Azure App Service. See [docs/deployment.md](docs/deployment.md) for production deployment and release procedures, and [docs/operations.md](docs/operations.md) for local development, testing, health checks, and maintenance.

GitHub Actions runs restore, build, test, and publish checks on every push and pull request to `main` ([`.github/workflows/ci.yml`](.github/workflows/ci.yml)).

## Documentation

- [docs/deployment.md](docs/deployment.md) — production deployment and release procedures
- [docs/operations.md](docs/operations.md) — local development, testing, health checks, and maintenance
- [docs/sources/README.md](docs/sources/README.md) — official source inventory and freshness policy

## Disclaimer And Source Freshness

Swiss Event Permit Assistant is not affiliated with the Ville de Fribourg, Canton de Fribourg, prefectures, Police locale, or OCN. It organizes public information for preparation only. Users must confirm final requirements, forms, fees, deadlines, and submission instructions with the competent authority.

Rule sources were last reviewed on 2026-08-23. Source freshness policy and unresolved interpretations are recorded in `docs/sources/README.md`.

## Design Direction

The interface follows a Swiss Civic Editorial visual system focused on restrained typography, grid-based layouts, source transparency, and clear administrative information.

## License

MIT — see [LICENSE](LICENSE).
