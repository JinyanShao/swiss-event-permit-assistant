# Changelog

All notable changes to Swiss Event Permit Assistant are documented in this file.

## [v0.1.5] - 2026-09-29

### External validation

- Fixed private-venue questionnaire wording based on feedback from Bénévolat Fribourg Freiburg; hardened assessment scope handling and safe defaults; recorded the round-2 validation feedback. (`8e15d81`, `15861b3`, `0533bc8`)

### Results page: official resource links

- Added official resource links (forms, guides) to results, deduplicated Patente K links, and replaced broken official deep links with stable Ville pages. (`e81b2f6`, `4f15fcd`, `f47bc4c`)

### Questionnaire: validation and draft-restore hardening

- Polished first-paint copy and the step counter; required explicit radio answers and marked required questions; prevented a restored draft from leaving a default radio selection; handled corrupt assessment drafts; hardened P1 validation and result polish; polished assessment navigation and sitemap. (`c000e1f`, `43ee72c`, `ce911ff`, `f34c011`, `13f877a`, `44ede89`, `59feadb`, `3377645`)

### Privacy: removed Plausible analytics and tracking code

- Recorded, then fully removed, a Plausible-based analytics validation baseline; removed the Plausible script, the `data-analytics-event` click hooks, and all remaining references now that analytics are no longer used. (`a89a90f`, `91e6a78`, `3b8a54b`, `ec5a62e`)

### Deployment

- Migrated production to `sepa-jinyan.azurewebsites.net` under an Azure for Students subscription. (`a2b4c8f`)

### Documentation

- Updated round 1 outreach tracking, then removed it (and other internal continuity notes) from the public repo; split operations documentation into `deployment.md` and `operations.md`; aligned the README with the new deployment docs. (`44b30cb`, `ee1acca`, `871f31d`, `6a5d8c9`)

### Outreach & SEO

- Added a meta description, Open Graph tags, canonical URL, `robots.txt`, and `sitemap.xml`; polished homepage copy for first-time visitors. (`c26cfb8`)

### Housekeeping

- Fixed stale commit references in `docs/user-testing/round-1.md` (the original hashes predated a history rewrite and no longer resolve on `main`).
- Removed the "Liens de campagne" (UTM) section from the privacy page; the site no longer generates or reads UTM parameters.
- Replaced the hardcoded "sources checked" date on the Results page with a value computed from `OfficialSources` and `OfficialResourceCatalog`.
- Removed the unused `CreateUnconfirmed` deadline helper from `EventRulesEvaluator`.
- Removed the empty `tests/SwissEventPermitAssistant.Tests/Deadlines/.gitkeep` placeholder.
- Added a `Project status` section to the README.
- Added this changelog.
- Added docs/decisions.md (architecture decision log).

## [v0.1.4] - 2026-08-24

Minimal analytics and outreach attribution.

- Added minimal anonymous analytics hooks: automatic page views and outbound-link tracking, plus a single "Feedback Click" custom event.
- Documented UTM outreach URLs for the first outreach round.
- Updated the privacy disclosure for the added analytics and localStorage usage.

## [v0.1.3] - 2026-08-23

Low-friction feedback and outreach prep.

- Replaced the primary user feedback entry with a mailto link; kept GitHub Issues for technical feedback.
- Added the first outreach tracking document.

## [v0.1.2] - 2026-08-23

Distribution-ready Fribourg pilot.

- Reconciled official event rule deadlines against source pages.
- Polished French questionnaire copy and clarified the municipal material question.
- Added a lightweight public feedback entry point.
- Recorded the first round of anonymous user testing.

## [v0.1.0] - 2026-08-16

Ville de Fribourg pilot — initial release.

- Implemented the core event permit domain rules and the questionnaire-to-results flow.
- Applied the Swiss Civic Editorial visual system.
- Addressed UI accessibility and browser-flow issues found before release.
- Injected a clock abstraction into rule evaluation for deterministic deadline testing.
- Added the MIT license and prepared the app for production release.
