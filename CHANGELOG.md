# Changelog

All notable changes to Swiss Event Permit Assistant are documented in this file.

## [Unreleased]

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
