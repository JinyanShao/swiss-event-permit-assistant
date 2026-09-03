using System.Text.Json;
using System.Text.Json.Serialization;
using SwissEventPermitAssistant.Domain.Profiles;
using SwissEventPermitAssistant.Domain.Rules;
using SwissEventPermitAssistant.Web.Models;
using SwissEventPermitAssistant.Web.Pages;

namespace SwissEventPermitAssistant.Tests.Web;

public sealed class ResultsModelTests
{
    [Fact]
    public void Other_commune_can_reach_scope_result_without_event_date_or_attendance()
    {
        var model = CreateModel(new AssessmentInput
        {
            Commune = Commune.Other
        });

        model.OnPost();

        Assert.True(model.ModelState.IsValid);
        Assert.NotNull(model.Result);
        Assert.Contains(model.Result.Confirmations, confirmation => confirmation.Id == "CONF-SCOPE-OUTSIDE");
        Assert.Empty(model.Result.Actions);
        Assert.Null(model.Input?.EventDate);
        Assert.Null(model.Input?.ExpectedAttendance);
    }

    [Fact]
    public void Unknown_commune_can_reach_scope_result_without_event_date_or_attendance()
    {
        var model = CreateModel(new AssessmentInput
        {
            Commune = Commune.Unknown
        });

        model.OnPost();

        Assert.True(model.ModelState.IsValid);
        Assert.NotNull(model.Result);
        Assert.Contains(model.Result.Confirmations, confirmation => confirmation.Id == "CONF-SCOPE-UNKNOWN");
        Assert.Empty(model.Result.Actions);
        Assert.Null(model.Input?.EventDate);
        Assert.Null(model.Input?.ExpectedAttendance);
    }

    [Fact]
    public void Ville_commune_still_requires_event_date_and_attendance()
    {
        var model = CreateModel(new AssessmentInput
        {
            Commune = Commune.VilleDeFribourg
        });

        model.OnPost();

        Assert.False(model.ModelState.IsValid);
        Assert.Null(model.Result);
    }

    private static ResultsModel CreateModel(AssessmentInput input)
    {
        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        return new ResultsModel(
            new EventRulesEvaluator(new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 12, 0, 0, TimeSpan.Zero))),
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 12, 0, 0, TimeSpan.Zero)))
        {
            AssessmentJson = JsonSerializer.Serialize(input, jsonOptions)
        };
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
