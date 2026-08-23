# User Testing Round 1

## Context

- Stage: V0.1 user validation
- Tester count: 1
- Tester identity: anonymous
- Language of feedback: French
- Feedback date: 2026-08-23
- Product URL: https://sepa-fribourg-jinyan.azurewebsites.net

This note records real user feedback without storing personal data.

## Feedback Received

The tester reported that the questionnaire should ask for the planned end time near the event date. They also suggested clearer French wording for the beverage and food questions:

- `Qu’en est-il des boissons ?`
- `Qu’en est-il de la nourriture ?`

The tester also corrected the municipal material wording:

- `Matériel ou décorations communales`

Finally, the tester noted that visible French copy should be reviewed because some accents were missing.

## Changes Made

- Moved the optional planned end time field to the first questionnaire step, close to the event date.
- Kept start time out of V0.1 because it does not change current rule evaluation.
- Updated the beverage and food question labels.
- Updated the municipal material label.
- Reviewed visible French copy and corrected accents, apostrophes, and wording in questionnaire, results, error text, and rule output.
- Kept business rule IDs, action IDs, document IDs, and deadline logic unchanged for the V0.1.1 polish.

## Follow-Up Correction

During production Playwright QA, the municipal material wording was visible only inside a checkbox sentence. A small follow-up commit made `Matériel ou décorations communales` a clear question label.

## Related Commits

- `da4ba25` - `Polish French questionnaire copy`
- `005b7f4` - `Clarify municipal material question`

## Result

- Local xUnit tests passed.
- GitHub CI passed.
- Production deployment completed.
- Production Playwright QA passed on desktop and mobile.

## Remaining Notes

This round produced French/UX polish only. It did not introduce V0.2 planning, accounts, database storage, PDF handling, or new permit scope.
