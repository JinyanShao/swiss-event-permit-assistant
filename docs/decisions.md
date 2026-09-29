# Decision Log

Swiss Event Permit Assistant (SEPA), V0.1. Lightweight ADRs reconstructed from the code, the docs and the git history (git history on `main` from 2026-08-14). Each entry cites the files it rests on. Where the repository does not record *why* an alternative was rejected, the entry says so rather than inventing a reason.

Status legend: **Accepted** = reflected in current `main`. **Superseded** = replaced by a later decision, still visible in history.

---

## ADR-001 — Keep permit rules in a framework-free Domain project

**Status:** Accepted (since `c30c6a5` "Implement core event permit rules", 2026-08-14)

**Context.** The product value is the rule logic: which actions, documents, deadlines and confirmations follow from a questionnaire. That logic has to be testable without a web server and readable by someone checking it against official pages.

**Decision.**
- `src/SwissEventPermitAssistant.Domain` has no package references and no ASP.NET dependency (`SwissEventPermitAssistant.Domain.csproj` is a plain `Microsoft.NET.Sdk` library).
- One entry point: `EventRulesEvaluator.Evaluate(EventProfile) -> AssessmentResult` (`Rules/EventRulesEvaluator.cs`).
- Input is an immutable `sealed record EventProfile` (`Profiles/EventProfile.cs`). Output is immutable records: `ActionRequirement`, `DocumentRequirement`, `InformationItem`, `ConfirmationItem`, `Deadline`, collected in `AssessmentResult` (`Results/AssessmentResult.cs`).
- Rules are grouped by topic in private methods: `AddPoliceLocaleRule`, `AddPatenteKRules`, `AddSmartAndReuseRules`, `AddSetupRules`, `AddMobilityRules`, `AddInsuranceRules`, `AddOptionalVilleRules`.
- Every output item has a stable ID with a prefix: `ACT-`, `DOC-`, `INFO-`, `CONF-`, `DL-`, `SRC-`, and documents carry `TriggeredByRuleIds` (`R-INSTALLATION-001`, `R-TRAFFIC-001`, …).
- The web layer only maps input: `AssessmentInput.ToEventProfile(...)` (`Web/Models/AssessmentInput.cs`) and calls the evaluator, registered as a singleton in `Program.cs`.

**Alternatives considered.** Not documented in the repo. Obvious alternatives would be rules in the PageModel, a data-driven rule table (JSON/DB), or a rules engine.

**Consequences / trade-offs.**
- + Rules are unit-tested directly (37 test cases in `EventRulesEvaluatorTests`) with no web host.
- + Stable IDs let tests assert on `"ACT-PATENTE-K"` rather than on French text, and let `OfficialResourceCatalog` attach links by ID.
- + Output is deterministic: `Build(...)` sorts every list by ID and deadlines by date then ID.
- + Duplicates are handled explicitly: `RuleCollectionExtensions.AddUnique` (first wins) and `AddMerge` (merges `TriggeredByRuleIds` and reasons for `DOC-SITE-PLAN`, which both setup and traffic rules can trigger; covered by `Multiple_triggers_deduplicate_site_plan_document_and_merge_rule_reasons`).
- − User-facing French strings live inside the domain (titles, reasons). The domain is not language-neutral; adding German would mean refactoring (see ADR-005).
- − Changing a rule means a code change and a redeploy. That is acceptable for 8 sources and one commune, but would not scale to many communes.

---

## ADR-002 — "Confirm, don't infer": unknowns become confirmation items, not requirements

**Status:** Accepted. Strengthened in `15861b3` "Fix assessment scope and safe defaults" (2026-09-03).

**Context.** Official pages leave some cases ambiguous: free drinks or food, free alcohol, the Formulaire B threshold, public events on private land. A wrong "Required" could send an organiser to the wrong office. A missing requirement could cause a refusal. `docs/sources/README.md` (Freshness Policy): *"mark the affected result as `Needs Confirmation` instead of inferring a requirement."*

**Decision.**
- Three kinds of output: `RequirementStatus.Required` actions and documents, `InformationItem`, and `ConfirmationItem` (with `WhatToDo` and `Authority`).
- Every question keeps an explicit "I don't know" value (`YesNoUnknown.Unknown`, `BeverageMode.NotSure`, `VenueKind.NotSure`, `Commune.Unknown`). `EventProfile` defaults unanswered fields to those values (`Event_profile_defaults_are_conservative_for_unanswered_fields`).
- Examples in `EventRulesEvaluator`:
  - Patente K is `Required` only for *sold* drinks, food or alcohol (`patenteRequired` in `AddPatenteKRules`). Free or unsure cases produce `CONF-FREE-BEVERAGES-PATENTE`, `CONF-FREE-FOOD-PATENTE` or `CONF-FREE-ALCOHOL-PATENTE`.
  - Formulaire B is **never** a required document. It is always `CONF-FORM-B` ("aucun seuil objectif public n'a été confirmé"). Tests: `Large_attendance_does_not_automatically_require_form_b_document`, `Form_b_resource_is_only_attached_to_existing_confirmation`.
  - Private venue + public event produces `CONF-PRIVATE-POLICE`, not a Police locale action (`Private_public_event_is_not_treated_as_public_space`).
  - Unknown attendance produces `CONF-ATTENDANCE`. The code does not guess a band (`Unknown_attendance_does_not_guess_attendance_band`, `Unknown_attendance_with_confirmed_drinks_does_not_guess_smart_band`).
- **Scope boundary as a rule.** `Evaluate` returns early for `Commune.Other` (`CONF-SCOPE-OUTSIDE`) and `Commune.Unknown` (`CONF-SCOPE-UNKNOWN`) before any Ville rule runs.

**What changed in history.** Before `15861b3`, `NotSure` for drinks or food counted as "served" and triggered the Smart Check action, and a null attendance fell back to the smallest band (`attendance is null || attendance < 200`). `Commune` had only `VilleDeFribourg | Other`, so "don't know" was treated as "outside". That commit split `confirmedFoodOrDrinkService` from `foodOrDrinkUncertain`, added `CONF-SMART-REUSE`, made `SmartAction/SmartDeadlineDays/SmartDeadlineId` take a non-nullable `int`, and added `Commune.Unknown`. `ResultsModel.OnPost` now requires date and attendance only for `VilleDeFribourg`.

**Alternatives considered.** Treating unknowns as "worst case" (require everything), which is the pre-`15861b3` behaviour for Smart Check. It was replaced because it produced required actions the user had not confirmed.

**Consequences / trade-offs.**
- + The tool never claims a requirement that the sources don't support.
- − Results can contain many "À confirmer" items. Users must still contact authorities. This is by design (README: "does not provide legal advice").
- − The UI forces an explicit radio answer (`required`, no preselection; `Food_drink_and_alcohol_questions_require_explicit_answers_without_preselection`, `Questionnaire_radio_answers_are_not_preselected_after_navigation`), so defaults mainly protect against tampered or partial JSON.

---

## ADR-003 — Compute deadlines from the event date, with an injected clock

**Status:** Accepted. Clock injection in `ca8ca56` (2026-08-15). Police deadlines reconciled in `607403f` (2026-08-23).

**Context.** Each authority states a minimum lead time: Police locale 20/30/60 days by attendance band, Smart Check 20/30/60 days, Patente K 60 days, OCN 2 months, municipal material 30 days, posting/banner 20 days. Users need the date and whether it has already passed.

**Decision.**
- `CreateDaysBefore(id, label, eventDate, days, sourceId)` uses `DateOnly.AddDays(-days)`. `CreateMonthsBefore` uses `DateOnly.AddMonths(-months)`, for calendar months (OCN). Test: `Sport_competition_on_public_road_requires_ocn_authorization_two_calendar_months_before` (10.10 → 10.08).
- Bands: `< 200`, `200..1000` inclusive, `> 1000` (`AddPoliceLocaleRule`, `SmartAction`, `SmartDeadlineDays`, `SmartDeadlineId`, `SourceForAttendance`).
- `DeadlineStatusFor(date)` returns `Passed` if the date is before today, `Approaching` if it is within 14 days, else `Confirmed`.
- "Today" comes from an injected `TimeProvider` (constructor `EventRulesEvaluator(TimeProvider? timeProvider = null)`, registered with `AddSingleton(TimeProvider.System)` in `Program.cs`). Before `ca8ca56`, the evaluator had `private static readonly DateOnly Today = new(2026, 8, 14);`.
- `AssessmentResult.NextImportantDeadline` is the earliest dated deadline. `Results.cshtml` shows it first and adds a warning when its status is `Passed`.

**What changed in history.** Originally the 200–1000 and >1000 Police deadlines were `CreateUnconfirmed("DL-POLICE-UNCONFIRMED", …)` and Smart Check was `DL-SMART-UNCONFIRMED`. After re-reading the Ville pages and adding the durability directive `SRC-VDF-DURABILITY` (`607403f`), they became `DL-POLICE-30/60` and `DL-SMART-20/30/60`. Tests still check that `DL-POLICE-UNCONFIRMED` is gone.

**Alternatives considered.** A hard-coded date (the first version). It was replaced because it froze deadline status in production and made tests implicit. `Deadline_status_uses_injected_clock` now pins the behaviour.

**Consequences / trade-offs.**
- + Deterministic tests: every test uses `FixedTimeProvider(2026-08-14 12:00 UTC)`.
- The earlier `CreateUnconfirmed` helper became unused after these changes and was removed in v0.1.5. `DeadlineStatus.Unconfirmed` remains available for future rules.
- − `GetLocalNow()` uses the server's time zone (UTC on Azure Linux), not Europe/Zurich. Near midnight, the "today" used for status can be off by a day. Known limitation.
- − No test covers the exact band edges (199/200/1000/1001) or the 14-day "Approaching" edge. Tests use 80/150/500/1200.

---

## ADR-004 — Version official sources in code, with checked dates, separate from action links

**Status:** Accepted. Sources in `607403f`. Resource links in `e81b2f6` / `4f15fcd` / `f47bc4c` (2026-09-02 to 2026-09-04).

**Context.** Trust depends on showing *where* each requirement comes from and *when* it was checked. Official URLs also break: the Ville form and SmartEvent deep links returned errors on 2026-09-04 (`docs/sources/README.md`).

**Decision.**
- `OfficialSources.All` (`Domain/Sources/OfficialSource.cs`) is a dictionary of 8 `OfficialSource` records (`SRC-VDF-ENTRY`, `SRC-VDF-LT200`, `SRC-VDF-200-1000`, `SRC-VDF-GT1000`, `SRC-VDF-DURABILITY`, `SRC-FR-PATENTE-K`, `SRC-FR-FORM-B`, `SRC-OCN-SPORT`). Each has `CheckedDate` (2026-08-23) and `Confidence` ("High"/"Medium").
- Every rule output carries a `SourceId`. The evaluator collects used IDs (`Use(sourceIds, …)`) and returns only the sources that were cited.
- `docs/sources/README.md` mirrors the table and lists the unresolved interpretations kept as confirmations.
- `Results.cshtml` renders "Vérifiée le dd.MM.yyyy" per source (`SourceDisclosure`) and a "Sources officielles" rail.
- **Separation of concerns:** `OfficialResourceCatalog.For(...)` (`Domain/Sources/OfficialResourceCatalog.cs`, `ResourceCheckedDate = 2026-09-04`) adds *action links* (forms, guides) by result ID. The docs state these links "do not change rule triggers, deadlines, or requirement status". Tests lock this in: `Police_locale_action_resolves_to_stable_ville_page_without_changing_rule_output`, `Smart_action_uses_existing_deadline_id_to_select_resource`, `Documents_without_specific_actionable_resource_do_not_get_vague_links`.
- Broken deep links were replaced by stable Ville pages (`f47bc4c`). `Patente_k_action_links_to_official_information_not_old_egov_detail_url` guards against the old `Detail.aspx?id=1075` URL.

**Alternatives considered.** Fetching sources at runtime. It is explicitly avoided: threat model, "no runtime fetch; source URLs are displayed as references only".

**Consequences / trade-offs.**
- + A rule re-check changes code and docs in one reviewable commit (see `607403f`).
- − The 90-day staleness policy is **manual**. No code or CI job compares `CheckedDate` to today or checks links.
- The "last checked" date on the results page is derived from the `CheckedDate` values of the official sources (earliest date), so it cannot drift from the source catalogue (since v0.1.5).

---

## ADR-005 — Server-rendered Razor Pages, no database, browser-only draft, French-first UI

**Status:** Accepted (skeleton `644c295`, flow `e89ea25`)

**Context.** V0.1 is a one-shot questionnaire → result. There are no accounts and no dossiers to keep. The README boundary: "store dossiers on the server", "create user accounts" are out of scope.

**Decision.**
- ASP.NET Core Razor Pages: `Index`, `Assessment`, `Results`, `Privacy`, `Error`.
- The questionnaire (`Assessment.cshtml`, 7 steps `data-step="0..6"`) is a single form. `wwwroot/js/site.js` handles steps, validation and conditional sections. On submit it serialises answers into the hidden `AssessmentJson` and POSTs to `/Results`.
- `ResultsModel.OnPost` deserialises with `JsonStringEnumConverter`, handles `JsonException` and null, validates date and attendance only for Ville, then evaluates. `OnGet` redirects to `/Assessment`.
- No database. Draft state is stored only in `localStorage` (`sepa.assessmentDraft.v1`, `sepa.lastAssessment.v1`) plus `sessionStorage` for the current step. A corrupt draft is discarded (`readStoredPayload` → `localStorage.removeItem`; commit `44ede89`).
- The UI copy is French and was validated with French-speaking testers (`docs/user-testing/round-1.md`, `round-2.md`).

**Alternatives considered.** SPA + API, MVC. Reason visible in the code: one stateless POST, server-side rendering of a result page, and no client framework or build pipeline.

**Consequences / trade-offs.**
- + Tiny attack surface: the threat model lists no accounts, uploads, DB or runtime fetch.
- + The result is plain server HTML, readable without JS once rendered.
- − The questionnaire depends on JS: the form posts only the hidden JSON.
- − The client can send any JSON. The server treats it as untrusted (typed deserialisation, safe enum defaults), but there are no request-size limits (listed as a recommended control in `docs/security/threat-model.md`).
- − French-only text in the domain and views. There is no localisation layer, although Fribourg is bilingual FR/DE.
- − The "Résultat indisponible" block in `Results.cshtml` always says the *date* is missing, even when the error is attendance or unreadable JSON. The `ModelState` message is not shown.

---

## ADR-006 — Testing strategy: rule-level unit tests + a fixed clock + source-content regression tests

**Status:** Accepted

**Context.** Wrong rules are the main risk ("Rule integrity" in the threat model). UX regressions found in user testing (preselected radios, private-venue wording) should not come back.

**Decision.** There are 65 xUnit test cases in `tests/SwissEventPermitAssistant.Tests`, counted statically: 56 `[Fact]` plus 9 `[InlineData]` rows.

| File | Cases | What it pins |
| --- | ---: | --- |
| `Rules/EventRulesEvaluatorTests.cs` | 37 | Each rule family, attendance bands (Theory `Smart_check_thresholds_remain_20_30_60_days`), deadline dates, `Passed` / `Approaching` status, scope exits, dedup/merge, "not sure" handling |
| `Sources/OfficialResourceCatalogTests.cs` | 9 | Links attach by result ID and never alter rule output. The old eGov URL is not used |
| `Web/ResultsModelTests.cs` | 3 | `ResultsModel.OnPost` with real JSON: out-of-scope communes need no date/attendance, Ville still does |
| `Web/QuestionnaireContentTests.cs` | 16 | String assertions on `Assessment.cshtml`, `Results.cshtml`, `site.js`, `sitemap.xml`: required markers, no preselected radios, scope-only payload, corrupt-draft handling, the private-venue wording from round 2, and removal of `PrivateVenueOwnerAuthorizationAvailable` |

- `FixedTimeProvider : TimeProvider` (overrides `GetUtcNow`) is used in the rule, catalog and ResultsModel tests.
- CI (`.github/workflows/ci.yml`) runs restore → build → test → publish on every push and PR to `main`, with `permissions: contents: read`.

**Alternatives considered.** Browser E2E tests. `round-1.md` mentions "production Playwright QA", but no Playwright project or script is in the repo.

**Consequences / trade-offs.**
- + The rule tests read like a specification (for example `Free_beverages_do_not_auto_require_patente_k`).
- − The 16 content tests assert on literal markup and JS source. They are cheap regression guards but brittle: renaming a variable breaks them without changing behaviour. They do not execute JS.
- − There are no edge-value tests at 200/1000 and no integration test through the HTTP pipeline (`WebApplicationFactory`).
- Deadline behaviour is tested in `Rules/` together with the rules that produce the deadlines.

---

## ADR-007 — Deploy to Azure App Service Free (F1, Linux) by manual zip deploy, with a `/healthz` endpoint

**Status:** Accepted. Host moved in `a2b4c8f` (2026-09-27).

**Context.** This is a student project with no budget. The app is stateless and needs no DB or secrets at runtime.

**Decision.**
- `Program.cs`: `AddHealthChecks()` + `MapHealthChecks("/healthz")`, `UseExceptionHandler("/Error")` + `UseHsts()` outside Development, `UseStatusCodePagesWithReExecute("/Error", "?responseStatusCode={0}")`, `UseHttpsRedirection()` (added in `3f1a1af` "Prepare app for production release").
- `docs/deployment.md`: `dotnet publish` → `zip` → `az webapp deploy --type zip --async false` to `sepa-jinyan` (RG `rg-sepa`, plan `asp-sepa-free`, F1 Linux, Italy North, HTTPS-only, `ASPNETCORE_ENVIRONMENT=Production`), then smoke test `/` and `/healthz`.
- CI builds and publishes an artifact but does **not** deploy. Deployment is a deliberate manual step.
- Secrets policy: no publish profiles or credentials in the repo (`deployment.md`, `operations.md`).

**History.** The deleted `docs/PROJECT_CONTINUITY.md` (removed in `ee1acca`) records the first host, `sepa-fribourg-jinyan` in France Central: West Europe did not accept a new App Service, and North Europe hit a quota. On 2026-09-27 production moved to `sepa-jinyan.azurewebsites.net` under an Azure for Students subscription (HES-SO).

**Alternatives considered.** Other regions (above). No evidence that other hosts or containers were considered.

**Consequences / trade-offs.**
- + Zero cost, simple, reproducible from the docs.
- − F1 has cold starts. On 2026-09-28, `/healthz` took about 20 s on the first request (one probe timed out at 40 s). There is no SLA and no always-on.
- − `/healthz` is a liveness check only (no custom checks), which is enough since there are no dependencies.
- − Deploys are manual, so production can drift from `main`. There is no deployed-version marker.

---

## ADR-008 — Remove analytics; keep validation evidence qualitative

**Status:** Accepted (`91e6a78`, `3b8a54b`, `ec5a62e`, 2026-09-27). Supersedes `30dca7c` "Add minimal anonymous analytics hooks" (2026-08-24).

**Context.** For outreach validation, Plausible Analytics (cookie-less) was added, with `data-analytics-event` hooks in `_Layout.cshtml` and `site.js`. A baseline was recorded in `docs/validation/README.md` (`a89a90f`). That baseline itself noted that the numbers (4 unique visitors) mostly included internal QA traffic.

**Decision.** Remove the Plausible script, the `data-analytics-event` attributes and the `window.plausible` click hooks. Privacy page and validation README now say "No analytics, no tracking cookies". Validation relies on documented user feedback (`docs/user-testing/round-1.md`, `round-2.md`).

**Alternatives considered.** Keeping Plausible. The commit message says it was "no longer used". The validation README says the account is no longer active.

**Consequences / trade-offs.**
- + Simpler privacy story, consistent with "Rien n'est envoyé aux autorités" and no server storage.
- − No usage data to support adoption claims. Evidence is limited to two feedback rounds (1 anonymous tester; 1 association intermediary, explicitly "not a partnership, endorsement…").
- The privacy page states what is (not) collected; the former UTM section was removed together with analytics (v0.1.5).
