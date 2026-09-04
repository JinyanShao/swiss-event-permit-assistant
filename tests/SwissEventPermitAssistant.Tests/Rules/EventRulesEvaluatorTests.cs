using SwissEventPermitAssistant.Domain.Profiles;
using SwissEventPermitAssistant.Domain.Results;
using SwissEventPermitAssistant.Domain.Rules;

namespace SwissEventPermitAssistant.Tests.Rules;

public sealed class EventRulesEvaluatorTests
{
    private readonly EventRulesEvaluator _evaluator = new(new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 12, 0, 0, TimeSpan.Zero)));

    [Fact]
    public void Event_profile_defaults_are_conservative_for_unanswered_fields()
    {
        var profile = new EventProfile(
            Commune.VilleDeFribourg,
            new DateOnly(2026, 10, 10),
            ExpectedAttendance: 150,
            VenueKind.PublicSpace);

        Assert.Equal(YesNoUnknown.Unknown, profile.IsPublicEvent);
        Assert.Equal(BeverageMode.NotSure, profile.BeverageMode);
        Assert.Equal(FoodMode.NotSure, profile.FoodMode);
        Assert.Equal(AlcoholMode.NotSure, profile.AlcoholMode);
        Assert.Equal(YesNoUnknown.Unknown, profile.HasLiabilityInsurance);
    }

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
    public void Private_public_event_is_not_treated_as_public_space()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 80) with
        {
            VenueKind = VenueKind.PrivateVenue,
            IsPublicEvent = YesNoUnknown.Yes,
            BeverageMode = BeverageMode.BeveragesFree
        });

        AssertNoAction(result, "ACT-PATENTE-K");
        AssertNoAction(result, "ACT-POLICE-LOCALE");
        AssertNoAction(result, "ACT-SMART-CHECK");
        AssertConfirmation(result, "CONF-PRIVATE-POLICE");
    }

    [Fact]
    public void Public_space_public_event_still_triggers_public_domain_rules()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 80) with
        {
            VenueKind = VenueKind.PublicSpace,
            IsPublicEvent = YesNoUnknown.Yes,
            BeverageMode = BeverageMode.BeveragesFree
        });

        AssertAction(result, "ACT-POLICE-LOCALE");
        AssertAction(result, "ACT-SMART-CHECK");
        Assert.DoesNotContain(result.Confirmations, confirmation => confirmation.Id == "CONF-PRIVATE-POLICE");
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
            BeverageMode = BeverageMode.NoBeverages,
            AlcoholMode = AlcoholMode.AlcoholSold
        });

        AssertAction(result, "ACT-PATENTE-K");
        AssertAction(result, "ACT-SMART-CHECK");
        AssertInfo(result, "INFO-ALCOHOL");
        AssertInfo(result, "INFO-REUSABLE-TABLEWARE");
        AssertInfo(result, "INFO-PATENTE-K-HOURS");
    }

    [Fact]
    public void Alcohol_free_needs_confirmation_and_alcohol_information()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 80) with
        {
            BeverageMode = BeverageMode.NoBeverages,
            AlcoholMode = AlcoholMode.AlcoholFree
        });

        AssertNoAction(result, "ACT-PATENTE-K");
        AssertAction(result, "ACT-SMART-CHECK");
        AssertConfirmation(result, "CONF-FREE-ALCOHOL-PATENTE");
        AssertInfo(result, "INFO-ALCOHOL");
        AssertInfo(result, "INFO-REUSABLE-TABLEWARE");
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
    public void Unknown_attendance_with_confirmed_drinks_does_not_guess_smart_band()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: null) with
        {
            BeverageMode = BeverageMode.BeveragesFree
        });

        AssertNoAction(result, "ACT-SMART-CHECK");
        Assert.DoesNotContain(result.Deadlines, deadline => deadline.Id.StartsWith("DL-SMART-", StringComparison.Ordinal));
        AssertConfirmation(result, "CONF-ATTENDANCE");
        AssertInfo(result, "INFO-REUSABLE-TABLEWARE");
    }

    [Theory]
    [InlineData(150, "Smart Check", "DL-SMART-20")]
    [InlineData(500, "Smart Check Plus", "DL-SMART-30")]
    [InlineData(1200, "Smart Event Plus", "DL-SMART-60")]
    public void Smart_check_thresholds_remain_20_30_60_days(int attendance, string expectedTitle, string expectedDeadlineId)
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: attendance) with
        {
            BeverageMode = BeverageMode.BeveragesFree
        });

        var smart = Assert.Single(result.Actions, action => action.Id == "ACT-SMART-CHECK");
        Assert.Equal(expectedTitle, smart.Title);
        Assert.Equal(expectedDeadlineId, smart.Deadline?.Id);
        Assert.Contains(result.Deadlines, deadline => deadline.Id == expectedDeadlineId);
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
        AssertConfirmation(result, "CONF-SCOPE-OUTSIDE");
        Assert.DoesNotContain(result.Confirmations, confirmation => confirmation.Id == "CONF-SCOPE-UNKNOWN");
    }

    [Fact]
    public void Unknown_commune_is_scope_to_confirm_and_does_not_apply_ville_rules()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            Commune = Commune.Unknown,
            BeverageMode = BeverageMode.BeveragesSold
        });

        Assert.Empty(result.Actions);
        AssertConfirmation(result, "CONF-SCOPE-UNKNOWN");
        Assert.DoesNotContain(result.Confirmations, confirmation => confirmation.Id == "CONF-SCOPE-OUTSIDE");
    }

    [Fact]
    public void Confirmed_ville_commune_applies_normal_ville_rules()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            Commune = Commune.VilleDeFribourg
        });

        AssertAction(result, "ACT-POLICE-LOCALE");
        Assert.Contains(result.Deadlines, deadline => deadline.Id == "DL-POLICE-20");
    }

    [Fact]
    public void Alcohol_sold_counts_as_confirmed_beverage_service_for_sustainability()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            BeverageMode = BeverageMode.NoBeverages,
            AlcoholMode = AlcoholMode.AlcoholSold
        });

        AssertAction(result, "ACT-SMART-CHECK");
        AssertInfo(result, "INFO-REUSABLE-TABLEWARE");
    }

    [Fact]
    public void Alcohol_free_counts_as_confirmed_beverage_service_for_sustainability()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            BeverageMode = BeverageMode.NoBeverages,
            AlcoholMode = AlcoholMode.AlcoholFree
        });

        AssertAction(result, "ACT-SMART-CHECK");
        AssertInfo(result, "INFO-REUSABLE-TABLEWARE");
    }

    [Theory]
    [InlineData("beverage")]
    [InlineData("food")]
    [InlineData("alcohol")]
    public void Food_drink_or_alcohol_not_sure_alone_creates_confirmation_not_required_smart_action(string uncertainField)
    {
        var profile = DefaultProfile(expectedAttendance: 150) with
        {
            BeverageMode = BeverageMode.NoBeverages,
            FoodMode = FoodMode.NoFood,
            AlcoholMode = AlcoholMode.NoAlcohol
        };

        profile = uncertainField switch
        {
            "beverage" => profile with { BeverageMode = BeverageMode.NotSure },
            "food" => profile with { FoodMode = FoodMode.NotSure },
            "alcohol" => profile with { AlcoholMode = AlcoholMode.NotSure },
            _ => profile
        };

        var result = Evaluate(profile);

        AssertNoAction(result, "ACT-SMART-CHECK");
        Assert.DoesNotContain(result.Information, item => item.Id == "INFO-REUSABLE-TABLEWARE");
        AssertConfirmation(result, "CONF-SMART-REUSE");
    }

    [Fact]
    public void Confirmed_food_or_drink_still_requires_smart_when_another_answer_is_not_sure()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            BeverageMode = BeverageMode.BeveragesFree,
            FoodMode = FoodMode.NotSure,
            AlcoholMode = AlcoholMode.NoAlcohol
        });

        AssertAction(result, "ACT-SMART-CHECK");
        AssertInfo(result, "INFO-REUSABLE-TABLEWARE");
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
            VenueKind.PublicSpace,
            YesNoUnknown.Yes,
            BeverageMode.NoBeverages,
            FoodMode.NoFood,
            AlcoholMode.NoAlcohol,
            YesNoUnknown.No,
            null,
            YesNoUnknown.No,
            YesNoUnknown.No,
            YesNoUnknown.No,
            YesNoUnknown.No,
            false,
            false,
            YesNoUnknown.No,
            YesNoUnknown.Yes);

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
