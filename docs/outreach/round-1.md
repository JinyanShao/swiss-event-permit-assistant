# Outreach Round 1

## Status

- Stage: V0.1 user validation and first real outreach
- Product URL: https://sepa-fribourg-jinyan.azurewebsites.net
- Outreach status: not sent
- Prepared on: 2026-08-24

Do not store private personal data, mailing-list exports, credentials, tokens, or MFA backup codes in this file.

## Contact List

| Priority | Object | Contact | Outreach URL | Source checked | Why this object fits |
| --- | --- | --- | --- | --- | --- |
| 1 | Bénévolat Fribourg Freiburg | info@benevolat-fr.ch / 026 422 37 07 | https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=benevolat&utm_medium=email&utm_campaign=outreach_round1 | 2026-08-24 | Local association hub with broad access to volunteer-led organizations that may organize public events. |
| 2 | AGEF | agef@unifr.ch / +41 026 300 73 10 | https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=agef&utm_medium=email&utm_campaign=outreach_round1 | 2026-08-24 | Central student association body; can validate whether the assistant helps student associations understand event preparation. |
| 3 | DI'VIN | divin.unifr@gmail.com | https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=divin&utm_medium=email&utm_campaign=outreach_round1 | 2026-08-24 | Oenology association; events may involve alcohol, tastings, public/private venues, and Patente K questions. |
| 4 | ESN Fribourg | esnfribourg@gmail.com | https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=esn&utm_medium=email&utm_campaign=outreach_round1 | 2026-08-24 | International student events often involve public attendance, venues, food or drinks, and users who need clear French wording. |
| 5 | Association des quartiers Jura Torry Miséricorde | contact@jtm-fribourg.ch | https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=jtm&utm_medium=email&utm_campaign=outreach_round1 | 2026-08-24 | Ville de Fribourg quartier association that organizes neighborhood events and is close to the civic use case. |
| 6 | Vélo Club Fribourg | contact@veloclubfribourg.ch / chenaux.adrien@veloclubfribourg.ch for race organization context | https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=velo_club_fribourg&utm_medium=email&utm_campaign=outreach_round1 | 2026-08-24 | Cycling club with public sport events and route/traffic/OCN-adjacent scenarios. |

## Analytics Setup

- Provider: Plausible Analytics 30-day trial.
- Page views: automatic Plausible page views.
- Assessment started: configure as a Plausible pageview goal for `/Assessment`.
- Results completed: configure as a Plausible pageview goal for `/Results`.
- Official source clicks: use Plausible outbound link tracking.
- Feedback clicked: custom event `Feedback Click`.
- Attribution: standard UTM parameters only; do not use custom properties for `utm_source`, source IDs, or questionnaire values.

## Uptime Monitoring Setup

- Provider: UptimeRobot Free.
- Monitor 1: HTTP monitor for `https://sepa-fribourg-jinyan.azurewebsites.net/healthz`; expected HTTP 200.
- Monitor 2: homepage keyword monitor for `https://sepa-fribourg-jinyan.azurewebsites.net/`; expected keyword `Préparer une manifestation à Fribourg`.

## Outreach Log

| Date | Object | Channel | Message | Response | Follow-up |
| --- | --- | --- | --- | --- | --- |
| Not sent | Bénévolat Fribourg Freiburg | Email | Not sent | Pending | Pending |
| Not sent | AGEF | Email | Not sent | Pending | Pending |
| Not sent | DI'VIN | Email | Not sent | Pending | Pending |
| Not sent | ESN Fribourg | Email | Not sent | Pending | Pending |
| Not sent | Association des quartiers Jura Torry Miséricorde | Email | Not sent | Pending | Pending |
| Not sent | Vélo Club Fribourg | Email | Not sent | Pending | Pending |

## Notes

- No outreach has been sent yet.
- This round is for learning whether V0.1 is understandable and useful before considering V0.2.
- Do not add product features from outreach ideas until they are validated as real V0.1 blockers.
