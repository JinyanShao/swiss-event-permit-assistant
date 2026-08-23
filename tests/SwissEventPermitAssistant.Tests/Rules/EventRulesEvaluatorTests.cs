using SwissEventPermitAssistant.Domain.Profiles;
using SwissEventPermitAssistant.Domain.Results;
using SwissEventPermitAssistant.Domain.Rules;

namespace SwissEventPermitAssistant.Tests.Rules;

public sealed class EventRulesEvaluatorTests
{
    private readonly EventRulesEvaluator _evaluator = new(new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 12, 0, 0, TimeSpan.Zero)));

    [Fact]
    public void Small_public_event_without_food_or_drinks_requires_police_only_with_20_day_deadline()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150));

        AssertAction(result, "ACT-POLICE-LOCALE");
        AssertNoAction(result, "ACT-PATENTE-K");
        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-POLICE-20" && deadline.Date == new DateOnly(2026, 9, 20));
    }

    [Fact]
    public void Beverages_sold_require_patente_k_and_make_60_day_deadline_earliest()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            BeverageMode = BeverageMode.BeveragesSold
        });

        AssertAction(result, "ACT-PATENTE-K");
        AssertAction(result, "ACT-SMART-CHECK");
        Assert.Equal("DL-PATENTE-K-60", result.NextImportantDeadline?.Id);
        Assert.Equal(new DateOnly(2026, 8, 11), result.NextImportantDeadline?.Date);
    }

    [Fact]
    public void Free_beverages_do_not_auto_require_patente_k()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            BeverageMode = BeverageMode.BeveragesFree
        });

        AssertNoAction(result, "ACT-PATENTE-K");
        AssertAction(result, "ACT-SMART-CHECK");
        AssertConfirmation(result, "CONF-FREE-BEVERAGES-PATENTE");
    }

    [Fact]
    public void Mid_size_cooked_food_sold_requires_patente_k_and_police_deadline_is_30_days()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 500) with
        {
            FoodMode = FoodMode.CookedFoodSold
        });

        AssertAction(result, "ACT-PATENTE-K");
        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-POLICE-30" && deadline.Date == new DateOnly(2026, 9, 10));
        Assert.DoesNotContain(result.Deadlines, deadline => deadline.Id == "DL-POLICE-UNCONFIRMED");
    }

    [Fact]
    public void Mid_size_free_beverages_require_smart_check_plus_with_30_day_deadline()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 500) with
        {
            BeverageMode = BeverageMode.BeveragesFree
        });

        var smart = Assert.Single(result.Actions, action => action.Id == "ACT-SMART-CHECK");
        Assert.Equal("Smart Check Plus", smart.Title);
        Assert.Equal(new DateOnly(2026, 9, 10), smart.Deadline?.Date);
        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-POLICE-30");
    }

    [Fact]
    public void Large_public_event_with_beverages_sold_requires_smart_event_plus_and_confirmation()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 1200) with
        {
            BeverageMode = BeverageMode.BeveragesSold
        });

        AssertAction(result, "ACT-PATENTE-K");
        Assert.Contains(result.Actions, action => action.Id == "ACT-SMART-CHECK" && action.Title == "Smart Event Plus");
        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-SMART-60" && deadline.Date == new DateOnly(2026, 8, 11));
        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-POLICE-60" && deadline.Date == new DateOnly(2026, 8, 11));
        AssertConfirmation(result, "CONF-FORM-B");
        Assert.DoesNotContain(result.Deadlines, deadline => deadline.Id == "DL-POLICE-UNCONFIRMED");
    }

    [Fact]
    public void Private_public_event_without_sales_needs_police_confirmation_but_no_patente_k()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 80) with
        {
            VenueKind = VenueKind.PrivateVenue,
            IsPublicEvent = YesNoUnknown.Yes
        });

        AssertNoAction(result, "ACT-PATENTE-K");
        AssertNoAction(result, "ACT-POLICE-LOCALE");
        AssertConfirmation(result, "CONF-PRIVATE-POLICE");
    }

    [Fact]
    public void Private_venue_with_beverages_sold_requires_owner_authorization()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 80) with
        {
            VenueKind = VenueKind.PrivateVenue,
            BeverageMode = BeverageMode.BeveragesSold
        });

        AssertAction(result, "ACT-PATENTE-K");
        AssertDocument(result, "DOC-OWNER-AUTHORIZATION");
        AssertNoAction(result, "ACT-POLICE-LOCALE");
    }

    [Fact]
    public void Free_food_only_is_confirmation_for_patente_k_not_required_action()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 80) with
        {
            FoodMode = FoodMode.FoodFree
        });

        AssertNoAction(result, "ACT-PATENTE-K");
        AssertAction(result, "ACT-SMART-CHECK");
        AssertConfirmation(result, "CONF-FREE-FOOD-PATENTE");
        AssertInfo(result, "INFO-REUSABLE-TABLEWARE");
    }

    [Fact]
    public void Food_only_sold_in_public_space_triggers_sustainability_action()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            FoodMode = FoodMode.CookedFoodSold
        });

        var smart = Assert.Single(result.Actions, action => action.Id == "ACT-SMART-CHECK");
        Assert.Equal("Smart Check", smart.Title);
        Assert.Equal("DL-SMART-20", smart.Deadline?.Id);
        Assert.Equal(new DateOnly(2026, 9, 20), smart.Deadline?.Date);
        AssertInfo(result, "INFO-REUSABLE-TABLEWARE");
    }

    [Fact]
    public void Alcohol_sold_requires_patente_k_and_alcohol_information()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 80) with
        {
            AlcoholMode = AlcoholMode.AlcoholSold
        });

        AssertAction(result, "ACT-PATENTE-K");
        AssertInfo(result, "INFO-ALCOHOL");
        AssertInfo(result, "INFO-PATENTE-K-HOURS");
    }

    [Fact]
    public void Alcohol_free_needs_confirmation_and_alcohol_information()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 80) with
        {
            AlcoholMode = AlcoholMode.AlcoholFree
        });

        AssertNoAction(result, "ACT-PATENTE-K");
        AssertConfirmation(result, "CONF-FREE-ALCOHOL-PATENTE");
        AssertInfo(result, "INFO-ALCOHOL");
    }

    [Fact]
    public void Large_attendance_does_not_automatically_require_form_b_document()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 1200));

        Assert.DoesNotContain(result.Documents, document => document.Id.Contains("FORM-B", StringComparison.OrdinalIgnoreCase));
        AssertConfirmation(result, "CONF-FORM-B");
    }

    [Fact]
    public void Liability_insurance_false_adds_confirmation_without_claiming_upload_requirement()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            HasLiabilityInsurance = YesNoUnknown.No
        });

        var document = Assert.Single(result.Documents, item => item.Id == "DOC-LIABILITY-INSURANCE");
        Assert.DoesNotContain("upload", document.Reason, StringComparison.OrdinalIgnoreCase);
        AssertConfirmation(result, "CONF-RC");
    }

    [Fact]
    public void Sport_competition_on_public_road_requires_ocn_authorization_two_calendar_months_before()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            IsSportCompetitionOnPublicRoad = YesNoUnknown.Yes
        });

        AssertAction(result, "ACT-OCN-SPORT");
        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-OCN-2M" && deadline.Date == new DateOnly(2026, 8, 10));
    }

    [Fact]
    public void Patente_k_deadline_passed_is_marked_as_passed()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150, eventDate: new DateOnly(2026, 9, 23)) with
        {
            BeverageMode = BeverageMode.BeveragesSold
        });

        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-PATENTE-K-60" && deadline.Status == DeadlineStatus.Passed);
    }

    [Fact]
    public void Deadline_status_uses_injected_clock()
    {
        var evaluator = new EventRulesEvaluator(new FixedTimeProvider(new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.Zero)));

        var result = evaluator.Evaluate(DefaultProfile(expectedAttendance: 150, eventDate: new DateOnly(2026, 10, 10)));

        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-POLICE-20" && deadline.Status == DeadlineStatus.Approaching);
    }

    [Fact]
    public void Temporary_installations_create_site_plan_and_installation_description()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            HasTemporaryInstallations = YesNoUnknown.Yes
        });

        AssertDocument(result, "DOC-SITE-PLAN");
        AssertDocument(result, "DOC-INSTALLATION-DESCRIPTION");
    }

    [Fact]
    public void Traffic_and_route_require_distinct_documents_and_confirmations()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            AffectsTrafficOrParking = YesNoUnknown.Yes,
            HasProcessionOrRoute = YesNoUnknown.Yes
        });

        AssertDocument(result, "DOC-TRAFFIC-PARKING");
        AssertDocument(result, "DOC-ROUTE-PLAN");
        AssertConfirmation(result, "CONF-TRAFFIC");
        AssertConfirmation(result, "CONF-ROAD-CLOSURE");
    }

    [Fact]
    public void Municipal_material_and_advertising_have_separate_deadlines()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            NeedsMunicipalMaterialOrDecorations = true,
            NeedsAdvertisingBannerOrPublicPosting = true
        });

        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-MATERIAL-30" && deadline.Date == new DateOnly(2026, 9, 10));
        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-ADVERTISING-20" && deadline.Date == new DateOnly(2026, 9, 20));
    }

    [Fact]
    public void Unknown_attendance_does_not_guess_attendance_band()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: null));

        AssertNoAction(result, "ACT-POLICE-LOCALE");
        AssertConfirmation(result, "CONF-ATTENDANCE");
    }

    [Fact]
    public void Other_commune_is_out_of_scope_and_does_not_apply_ville_rules()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            Commune = Commune.Other,
            BeverageMode = BeverageMode.BeveragesSold
        });

        Assert.Empty(result.Actions);
        AssertConfirmation(result, "CONF-SCOPE");
    }

    [Fact]
    public void Multiple_triggers_deduplicate_site_plan_document_and_merge_rule_reasons()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            HasTemporaryInstallations = YesNoUnknown.Yes,
            AffectsTrafficOrParking = YesNoUnknown.Yes
        });

        var sitePlan = Assert.Single(result.Documents, document => document.Id == "DOC-SITE-PLAN");
        Assert.Contains("R-INSTALLATION-001", sitePlan.TriggeredByRuleIds);
        Assert.Contains("R-TRAFFIC-001", sitePlan.TriggeredByRuleIds);
    }

    private AssessmentResult Evaluate(EventProfile profile) => _evaluator.Evaluate(profile);

    private static EventProfile DefaultProfile(int? expectedAttendance, DateOnly? eventDate = null) =>
        new(
            Commune.VilleDeFribourg,
            eventDate ?? new DateOnly(2026, 10, 10),
            expectedAttendance,
            VenueKind.PublicSpace);

    private static void AssertAction(AssessmentResult result, string id) =>
        Assert.Contains(result.Actions, action => action.Id == id);

    private static void AssertNoAction(AssessmentResult result, string id) =>
        Assert.DoesNotContain(result.Actions, action => action.Id == id);

    private static void AssertDocument(AssessmentResult result, string id) =>
        Assert.Contains(result.Documents, document => document.Id == id);

    private static void AssertInfo(AssessmentResult result, string id) =>
        Assert.Contains(result.Information, item => item.Id == id);

    private static void AssertConfirmation(AssessmentResult result, string id) =>
        Assert.Contains(result.Confirmations, item => item.Id == id);

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
