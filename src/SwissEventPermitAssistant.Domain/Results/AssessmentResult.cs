using SwissEventPermitAssistant.Domain.Sources;

namespace SwissEventPermitAssistant.Domain.Results;

public enum RequirementStatus
{
    Required,
    Information,
    NeedsConfirmation
}

public enum DeadlineStatus
{
    Confirmed,
    Unconfirmed,
    Passed,
    Approaching
}

public sealed record Deadline(
    string Id,
    string Label,
    DateOnly? Date,
    string Basis,
    DeadlineStatus Status,
    string SourceId);

public sealed record ActionRequirement(
    string Id,
    string Title,
    RequirementStatus Status,
    string Authority,
    string Reason,
    string SourceId,
    Deadline? Deadline = null);

public sealed record DocumentRequirement(
    string Id,
    string Title,
    RequirementStatus Status,
    string Reason,
    IReadOnlyList<string> TriggeredByRuleIds,
    string SourceId);

public sealed record InformationItem(
    string Id,
    string Title,
    string Text,
    string SourceId);

public sealed record ConfirmationItem(
    string Id,
    string Title,
    string Reason,
    string WhatToDo,
    string Authority,
    string SourceId);

public sealed record AssessmentResult(
    IReadOnlyList<ActionRequirement> Actions,
    IReadOnlyList<DocumentRequirement> Documents,
    IReadOnlyList<InformationItem> Information,
    IReadOnlyList<ConfirmationItem> Confirmations,
    IReadOnlyList<Deadline> Deadlines,
    IReadOnlyList<OfficialSource> Sources)
{
    public Deadline? NextImportantDeadline =>
        Deadlines
            .Where(deadline => deadline.Date is not null)
            .OrderBy(deadline => deadline.Date)
            .FirstOrDefault();
}
