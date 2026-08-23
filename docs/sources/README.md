# Official Source Inventory

V0.1 uses public official pages for the Ville de Fribourg, Canton de Fribourg, and OCN. The application is independent and does not guarantee that the sources remain current after the checked date.

## Freshness Policy

- Last product rule review: 2026-08-23.
- Recheck official sources before any public release, policy change, or rule change.
- Treat rules older than 90 days since the last checked date as stale until manually reviewed.
- If a page changes or an interpretation is ambiguous, mark the affected result as `Needs Confirmation` instead of inferring a requirement.

## V0.1 Sources

| Source ID | Authority | Scope | URL | Checked | Confidence |
| --- | --- | --- | --- | --- | --- |
| `SRC-VDF-ENTRY` | Ville de Fribourg | General event process, Police locale and durability entry points | https://www.ville-fribourg.ch/organiser-manifestation | 2026-08-23 | High |
| `SRC-VDF-LT200` | Ville de Fribourg | Manifestations de moins de 200 personnes | https://www.ville-fribourg.ch/organiser-manifestation/moins-de-200 | 2026-08-23 | High |
| `SRC-VDF-200-1000` | Ville de Fribourg | Manifestations de 200 à 1000 personnes | https://www.ville-fribourg.ch/organiser-manifestation/200-a-1000 | 2026-08-23 | High |
| `SRC-VDF-GT1000` | Ville de Fribourg | Manifestations de plus de 1000 personnes | https://www.ville-fribourg.ch/organiser-manifestation/plus-de-1000 | 2026-08-23 | High |
| `SRC-VDF-DURABILITY` | Ville de Fribourg | Vaisselle réutilisable, Smart Check deadlines, exemptions | https://www.ville-fribourg.ch/reglements-tarifs/300-13 | 2026-08-23 | High |
| `SRC-FR-PATENTE-K` | Canton de Fribourg | Manifestations temporaires, Patente K | https://www.fr.ch/vie-quotidienne/demarches-et-documents/manifestations-temporaires-patente-k | 2026-08-23 | High |
| `SRC-FR-FORM-B` | Canton de Fribourg | Formulaires des préfectures | https://www.fr.ch/vie-quotidienne/demarches-et-documents/formulaires-des-prefectures | 2026-08-23 | Medium |
| `SRC-OCN-SPORT` | OCN Fribourg | Compétitions sportives sur route | https://www.ocn.ch/fr/conduire/autorisations/competitions-sportives | 2026-08-23 | High |

## Unresolved Items Kept As Confirmation

- Patente K treatment for some free food or free alcohol cases where public pages do not make the operational path fully clear.
- Formulaire A/B role in the current online Patente K workflow when the public service page does not state whether upload is required.
- Formulaire B for `manifestation d'importance`: no objective public threshold is confirmed for automatic triggering.
- Public event on private venue: Police locale authorization may be needed, but V0.1 does not infer it automatically without authority confirmation.
