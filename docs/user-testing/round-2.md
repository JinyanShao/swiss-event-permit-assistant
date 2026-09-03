# User Testing Round 2

## Context

- Stage: V0.1 real-world validation
- Source: Bénévolat Fribourg Freiburg
- Feedback date: 2026-09-03
- Feedback type: external association-support intermediary

This note records real external feedback without storing personal data. It paraphrases the feedback and does not include the individual sender's name, email address, signature, physical address, or full correspondence.

## Feedback Received

An external representative from Bénévolat Fribourg Freiburg tested the tool and described it as an interesting tool.

They reported that the product handled poorly or unclearly events held in a public-facing venue managed by a private organisation, with examples such as a concert hall or restaurant.

They also stated that they plan to make the tool available to associations that contact them and may send further feedback.

## Interpretation

This is a referral and distribution signal.

It is not a partnership, endorsement, recommendation, formal adoption, validation by an authority, or proof that associations have already used the tool.

## Confirmed Defect

The feedback exposed a real questionnaire clarity issue:

- the wording conflated public accessibility with `domaine public communal`;
- a private restaurant or concert hall open to the public could therefore be misunderstood as public space.

## Response Implemented

- Explicitly separated venue status (`domaine public communal` vs private venue/fonds privé) from event access (`ouverte au public`).
- Removed the dead `PrivateVenueOwnerAuthorizationAvailable` questionnaire input.
- Preserved existing rules and document requirements.
- Added regression coverage ensuring that a private venue open to the public is not treated as public domain.

## Related Commit

- `8e15d819cf53cb5afc5955c3f080a6932118c310` - production fix for private-venue/public-access clarity.

## Unresolved Follow-Up Questions

- When an existing restaurant, bar, hotel, or concert-hall patente or authorisation can cover an event.
- Whether it matters who sells or serves food or drinks: the establishment or the event organiser.
- When Patente K remains necessary.
- Operational Police locale process for a public event on private land.

## Result

The reported wording defect was fixed in production. No business rule, deadline, source interpretation, or document requirement was changed as part of this response.
