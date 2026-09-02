# Product Validation

Swiss Event Permit Assistant has been tested with external users during the V0.1 validation phase.

Validation focuses on:

- questionnaire clarity
- French wording
- usefulness of generated actions and deadlines
- interpretation of confirmation states
- usability of official-source references
- desktop and mobile readability

Feedback that results in concrete product changes is documented in the individual validation notes in this directory.

Personal contact details, outreach lists, communication history, and internal follow-up planning are intentionally kept outside the public repository.

## Analytics Baseline

Plausible Analytics is used only for privacy-preserving aggregate product validation. Questionnaire answers, event names, email addresses, and permit details are not recorded in analytics.

The first analytics readings include internal setup and QA traffic, so they must not be treated as confirmed external user behaviour.

Baseline recorded on 2026-09-02 for the previous 7 days:

| Metric | Value | Validation note |
| --- | ---: | --- |
| Unique visitors | 4 | Includes possible internal test traffic |
| Total visits | 5 | Includes possible internal test traffic |
| Total pageviews | 13 | Includes possible internal test traffic |
| Source: `benevolat` | 2 visitors | Possible external signal, not confirmed |
| Source: `agef` | 1 visitor | Likely includes internal UTM test traffic |
| Source: Direct / None | 1 visitor | Unknown origin |
| Page: `/Assessment` | 2 visitors | May include internal QA |
| Page: `/Results` | 1 visitor | May include internal QA |
| Goal: Assessment started | 2 | May include internal QA |
| Goal: Results completed | 1 | May include internal QA |
| Goal: Feedback Click | 1 | Likely includes internal analytics test traffic |
| Goal: Outbound Link Click | 1 | Unknown origin |

For future outreach evaluation, use 2026-09-02 as the baseline date and focus on new activity after each message is sent. New UTM sources from later outreach are more useful than the initial mixed setup data.
