# Swiss Event Permit Assistant

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

## Confirmed Reuse Plan

- UI design and adjustment: `frontend-design`
- UI review after implementation: `web-design-guidelines`
- Real browser flow testing: `playwright-skill`
- Core business rule testing: xUnit
- Pre-deployment security review: `security-threat-model` and `insecure-defaults`
- PDF automation: deferred to V0.2

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

## Design Direction

The locked visual direction is Swiss Civic Editorial: restrained, precise, editorial, typographic, and source-forward. The current generated Razor Pages template is only an engineering skeleton; the final UI must not keep a default Bootstrap or generic template appearance.
