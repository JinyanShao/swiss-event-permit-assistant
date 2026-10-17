# Official Source Inventory

V0.1 uses public official pages for the Ville de Fribourg, Canton de Fribourg, and OCN. The application is independent and does not guarantee that the sources remain current after the checked date.

## Freshness Policy

- Last product rule review: 2026-10-17.
- Recheck official sources before any public release, policy change, or rule change.
- Treat rules older than 90 days since the last checked date as stale until manually reviewed.
- If a page changes or an interpretation is ambiguous, mark the affected result as `Needs Confirmation` instead of inferring a requirement.

## V0.1 Sources

| Source ID | Authority | Scope | URL | Checked | Confidence |
| --- | --- | --- | --- | --- | --- |
| `SRC-VDF-ENTRY` | Ville de Fribourg | General event process, Police locale and durability entry points | https://www.ville-fribourg.ch/organiser-manifestation | 2026-10-17 | High |
| `SRC-VDF-LT200` | Ville de Fribourg | Police locale deadline for fewer than 200 people | https://www.ville-fribourg.ch/sites/default/files/2026-09/Conditions%20generales%20pour%20les%20manifestations.pdf | 2026-10-17 | High |
| `SRC-VDF-200-1000` | Ville de Fribourg | Police locale deadline for 200 to 1000 people | https://www.ville-fribourg.ch/sites/default/files/2026-09/Conditions%20generales%20pour%20les%20manifestations.pdf | 2026-10-17 | High |
| `SRC-VDF-GT1000` | Ville de Fribourg | Police locale deadline for more than 1000 people | https://www.ville-fribourg.ch/sites/default/files/2026-09/Conditions%20generales%20pour%20les%20manifestations.pdf | 2026-10-17 | High |
| `SRC-VDF-DURABILITY` | Ville de Fribourg | Directive sur la durabilité, Smart Check deadlines, reusable tableware exemptions | https://www.ville-fribourg.ch/reglements-tarifs/300-13 | 2026-10-17 | High |
| `SRC-FR-PATENTE-K` | Canton de Fribourg | Manifestations temporaires, Patente K | https://www.fr.ch/vie-quotidienne/demarches-et-documents/manifestations-temporaires-patente-k | 2026-10-17 | High |
| `SRC-FR-FORM-B` | Canton de Fribourg | Formulaires des préfectures | https://www.fr.ch/vie-quotidienne/demarches-et-documents/formulaires-des-prefectures | 2026-10-17 | Medium |
| `SRC-OCN-SPORT` | OCN Fribourg | Compétitions sportives sur route | https://www.ocn.ch/fr/conduire/autorisations/competitions-sportives | 2026-10-17 | High |

## Unresolved Items Kept As Confirmation

- Smart Check scope where no food/drink is served, and reusable-tableware exemptions for small events.
- Attendance bands for multi-day events with different daily attendance, and official-holiday treatment when calculating Police locale working-day cut-offs.
- Patente K for free food/drink/alcohol and where an existing venue operator's permit may cover an event.
- Formulaire A/B roles in the current online workflow, the trigger for a `manifestation d'importance`, and whether installation/traffic documents are mandatory for all sizes.
- Whether RC insurance is universally required, and Police locale authorization for public events on privately owned land; the current app leaves these as confirmations.
- OCN's definition of a competition, and permit coordination for routes across communes.

## Actionable Official Resources

These links are presentation resources that help users reach the relevant official form, guide, or information page after the existing V0.1 rules have produced a result. They do not change rule triggers, deadlines, or requirement status.

Resource links were checked on 2026-10-17. The itemized review, observed page dates and unresolved interpretations are recorded in [the 2026-10-17 source review](review-2026-10-17.md). The three Ville attendance-band URLs previously used by V0.1 now return HTTP 410; the attendance rules now cite the current Conditions générales PDF, the Police locale action links to the live official form, and Smart Check actions link to the Ville overview.

| Result ID | Resource | Type | URL | Checked |
| --- | --- | --- | --- | --- |
| `ACT-POLICE-LOCALE` | Ville Police locale announcement form | Online application | https://www.ville-fribourg.ch/form/po-manifestation | 2026-10-17 |
| `ACT-SMART-CHECK` + `DL-SMART-20` | Ville page with Smart Check entry point | Official information | https://www.ville-fribourg.ch/organiser-manifestation | 2026-10-17 |
| `ACT-SMART-CHECK` + `DL-SMART-30` | Ville page with Smart Check Plus entry point | Official information | https://www.ville-fribourg.ch/organiser-manifestation | 2026-10-17 |
| `ACT-SMART-CHECK` + `DL-SMART-60` | Ville page with Smart Event Plus entry point | Official information | https://www.ville-fribourg.ch/organiser-manifestation | 2026-10-17 |
| `ACT-PATENTE-K` | Patente K official information | Official information | https://www.fr.ch/vie-quotidienne/demarches-et-documents/manifestations-temporaires-patente-k | 2026-10-17 |
| `ACT-PATENTE-K` | Patente K online-application guide | Guide | https://www.fr.ch/document/530631 | 2026-10-17 |
| `CONF-FORM-B` | Formulaire B | Form | https://www.fr.ch/document/104241 | 2026-10-17 |
| `ACT-OCN-SPORT` | OCN sport event application | Form | https://www.ocn.ch/sites/default/files/2025-10/2025_Demande_autorisation_manifestation_sportive.pdf | 2026-10-17 |
| `ACT-OCN-SPORT` | OCN sport event aide-memoire | Guide | https://www.ocn.ch/sites/default/files/2024-08/Aidememoire_manifestations_sportives_20240819.pdf | 2026-10-17 |
