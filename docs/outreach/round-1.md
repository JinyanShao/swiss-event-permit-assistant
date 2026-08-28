# Outreach Round 1

## Status

- Stage: V0.1 user validation and first real outreach
- Product URL: https://sepa-fribourg-jinyan.azurewebsites.net
- Outreach status: first three messages sent; second batch prepared
- Prepared on: 2026-08-24
- First send date: 2026-08-28

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
| 2026-08-28 | Bénévolat Fribourg Freiburg | Email | Sent initial V0.1 user-validation message with `utm_source=benevolat`. | Pending | If no response after 5 working days |
| 2026-08-28 | AGEF | Email | Sent initial V0.1 user-validation message with `utm_source=agef`. | Pending | If no response after 5 working days |
| 2026-08-28 | DI'VIN | Email | Sent initial V0.1 user-validation message with `utm_source=divin`. | Pending | If no response after 5 working days |
| Not sent | ESN Fribourg | Email | Not sent | Pending | Pending |
| Not sent | Association des quartiers Jura Torry Miséricorde | Email | Not sent | Pending | Pending |
| Not sent | Vélo Club Fribourg | Email | Not sent | Pending | Pending |

## Follow-up Template

Use only if there is no response after 5 working days.

Subject: Petit suivi - outil pour préparer une manifestation à Fribourg

Bonjour,

Je me permets de revenir brièvement vers vous au sujet du petit outil gratuit que je vous ai envoyé pour préparer une manifestation en Ville de Fribourg :

[OUTREACH_URL]

Je cherche surtout à vérifier si le questionnaire est compréhensible et si les résultats sont utiles pour des associations ou organisateurs locaux.

Si vous pensez que ce n'est pas pertinent pour vous, aucun souci. Et si vous connaissez une association pour laquelle cela pourrait être utile, n'hésitez pas à lui transmettre le lien.

Merci beaucoup et bonne journée,

Jinyan Shao

jinyanshao.ch

## Round 1 Data Summary Template

Period reviewed: [START_DATE] to [END_DATE]

### Outreach Sent

| Object | Sent date | Follow-up date | Reply status |
| --- | --- | --- | --- |
| Bénévolat Fribourg Freiburg | 2026-08-28 | [DATE_OR_NOT_SENT] | [Pending / Replied / Not relevant] |
| AGEF | 2026-08-28 | [DATE_OR_NOT_SENT] | [Pending / Replied / Not relevant] |
| DI'VIN | 2026-08-28 | [DATE_OR_NOT_SENT] | [Pending / Replied / Not relevant] |
| ESN Fribourg | [DATE_OR_NOT_SENT] | [DATE_OR_NOT_SENT] | [Pending / Replied / Not relevant] |
| Association des quartiers Jura Torry Miséricorde | [DATE_OR_NOT_SENT] | [DATE_OR_NOT_SENT] | [Pending / Replied / Not relevant] |
| Vélo Club Fribourg | [DATE_OR_NOT_SENT] | [DATE_OR_NOT_SENT] | [Pending / Replied / Not relevant] |

### Plausible Signals

| Metric | Observation |
| --- | --- |
| Visits by UTM source | [benevolat / agef / divin / esn / jtm / velo_club_fribourg] |
| Assessment started | [count / qualitative note] |
| Results completed | [count / qualitative note] |
| Official source clicks | [count / qualitative note] |
| Feedback clicks | [count / qualitative note] |

### Qualitative Feedback

| Source | Feedback | Severity | V0.1 action |
| --- | --- | --- | --- |
| [Object/person, anonymized if needed] | [Feedback summary] | [Blocker / Important / Nice to have] | [Fix / Watch / Defer] |

### Decision

- Keep V0.1 unchanged unless feedback identifies a real blocker.
- Fix only concrete correctness, clarity, French wording, or usability issues.
- Do not start V0.2 from speculative ideas.

## Second Batch Email Drafts

### ESN Fribourg

Subject: Un outil pour préparer une manifestation à Fribourg

Bonjour,

Je me permets de vous contacter car j'ai développé un petit outil gratuit et indépendant pour aider à préparer une manifestation en Ville de Fribourg.

Il permet, à partir de quelques questions, d'identifier les démarches qui peuvent être nécessaires, les délais à prévoir, les documents à préparer et les liens vers les sources officielles de la Ville et du Canton.

https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=esn&utm_medium=email&utm_campaign=outreach_round1

Je suis développeuse logiciel basée à Fribourg et je travaille notamment sur des outils numériques qui simplifient des démarches concrètes. Comme ESN Fribourg organise et accompagne des activités étudiantes et internationales, j'ai pensé que cet outil pourrait éventuellement être utile pour préparer certains événements.

Si vous voyez une information incorrecte ou peu claire, je serais également très intéressée par votre retour. Et si vous pensez que l'outil peut être utile à une association de votre réseau, n'hésitez pas à le lui transmettre.

Merci et bonne journée,

Jinyan Shao

jinyanshao.ch

### Association des quartiers Jura Torry Miséricorde

Subject: Un outil pour préparer une manifestation à Fribourg

Bonjour,

Je me permets de vous contacter car j'ai développé un petit outil gratuit et indépendant pour aider à préparer une manifestation en Ville de Fribourg.

Il permet, à partir de quelques questions, d'identifier les démarches qui peuvent être nécessaires, les délais à prévoir, les documents à préparer et les liens vers les sources officielles de la Ville et du Canton.

https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=jtm&utm_medium=email&utm_campaign=outreach_round1

Je suis développeuse logiciel basée à Fribourg et je travaille notamment sur des outils numériques qui simplifient des démarches concrètes. Comme votre association est proche de la vie de quartier et des activités locales, j'ai pensé que cet outil pourrait éventuellement être utile à des personnes qui préparent une manifestation.

Si vous voyez une information incorrecte ou peu claire, je serais également très intéressée par votre retour. Et si vous pensez que l'outil peut être utile à une association de votre réseau, n'hésitez pas à le lui transmettre.

Merci et bonne journée,

Jinyan Shao

jinyanshao.ch

### Vélo Club Fribourg

Subject: Un outil pour préparer une manifestation à Fribourg

Bonjour,

Je me permets de vous contacter car j'ai développé un petit outil gratuit et indépendant pour aider à préparer une manifestation en Ville de Fribourg.

Il permet, à partir de quelques questions, d'identifier les démarches qui peuvent être nécessaires, les délais à prévoir, les documents à préparer et les liens vers les sources officielles de la Ville, du Canton et de l'OCN lorsque la circulation peut être concernée.

https://sepa-fribourg-jinyan.azurewebsites.net/?utm_source=velo_club_fribourg&utm_medium=email&utm_campaign=outreach_round1

Je suis développeuse logiciel basée à Fribourg et je travaille notamment sur des outils numériques qui simplifient des démarches concrètes. Comme le Vélo Club Fribourg peut être concerné par des événements sportifs, des parcours ou des questions liées à l'espace public, j'ai pensé que cet outil pourrait éventuellement être utile pour une première préparation.

Si vous voyez une information incorrecte ou peu claire, je serais également très intéressée par votre retour. Et si vous pensez que l'outil peut être utile à une association de votre réseau, n'hésitez pas à le lui transmettre.

Merci et bonne journée,

Jinyan Shao

jinyanshao.ch

## Notes

- Bénévolat Fribourg Freiburg, AGEF, and DI'VIN were sent on 2026-08-28.
- ESN Fribourg, Association des quartiers Jura Torry Miséricorde, and Vélo Club Fribourg are prepared but not sent.
- This round is for learning whether V0.1 is understandable and useful before considering V0.2.
- Do not add product features from outreach ideas until they are validated as real V0.1 blockers.
