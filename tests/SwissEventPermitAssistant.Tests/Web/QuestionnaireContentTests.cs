namespace SwissEventPermitAssistant.Tests.Web;

public sealed class QuestionnaireContentTests
{
    [Fact]
    public void Commune_question_is_required_and_keeps_unknown_distinct_from_other()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Assessment.cshtml"));
        var script = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/wwwroot/js/site.js"));

        Assert.Contains("data-step=\"0\" data-title=\"Périmètre\"", content);
        Assert.Contains("<legend>Périmètre</legend>", content);
        Assert.Contains("La manifestation aura-t-elle lieu sur le territoire de la Ville de Fribourg ?", content);
        Assert.Contains("name=\"commune\" value=\"VilleDeFribourg\" required", content);
        Assert.Contains("name=\"commune\" value=\"Other\"", content);
        Assert.Contains("name=\"commune\" value=\"Unknown\"", content);
        Assert.Contains("data-step=\"1\" data-title=\"Manifestation\"", content);
        Assert.Contains("commune: text(\"commune\") || \"Unknown\"", script);
        Assert.DoesNotContain("commune: \"VilleDeFribourg\"", script);
        Assert.Contains("currentStep === 0 && selectedCommune() !== \"VilleDeFribourg\"", script);
    }

    [Fact]
    public void Draft_restoration_keeps_old_or_out_of_scope_drafts_on_scope_step()
    {
        var script = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/wwwroot/js/site.js"));

        Assert.Contains("let currentStep = 0;", script);
        Assert.Contains("restoreDraft();", script);
        Assert.Contains("if (selectedCommune() === \"VilleDeFribourg\")", script);
        Assert.Contains("currentStep = Number(sessionStorage.getItem(\"sepa.currentStep\") || \"0\");", script);
    }

    [Fact]
    public void Scope_only_submission_does_not_send_stale_event_details()
    {
        var script = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/wwwroot/js/site.js"));

        Assert.Contains("currentStep === 0 && selectedCommune() !== \"VilleDeFribourg\"", script);
        Assert.Contains("? collectScopePayload()", script);
        Assert.Contains("function collectScopePayload()", script);
        Assert.Contains("commune: selectedCommune()", script);
    }

    [Fact]
    public void Scope_result_uses_concise_notice_instead_of_normal_permit_layout()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Results.cshtml"));

        Assert.Contains("else if (input.Commune != Commune.VilleDeFribourg)", content);
        Assert.Contains("Périmètre V0.1", content);
        Assert.Contains("Modifier ma réponse", content);
    }

    [Fact]
    public void Empty_information_results_do_not_render_a_blank_information_section()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Results.cshtml"));

        Assert.Contains("@if (result.Information.Count > 0)", content);
        Assert.Contains("<h2>Informations utiles</h2>", content);
    }

    [Fact]
    public void Expected_attendance_is_required_as_an_estimate()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Assessment.cshtml"));

        Assert.Contains("Une estimation suffit pour orienter les démarches et les délais applicables.", content);
        Assert.Contains("name=\"expectedAttendance\" type=\"number\" min=\"1\" inputmode=\"numeric\" required", content);
        Assert.Contains("Indiquez une estimation du nombre de personnes attendues.", content);
    }

    [Fact]
    public void Food_drink_and_alcohol_questions_require_explicit_answers_without_preselection()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Assessment.cshtml"));
        var script = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/wwwroot/js/site.js"));

        Assert.Contains("name=\"beverageMode\" value=\"NoBeverages\" required", content);
        Assert.Contains("name=\"foodMode\" value=\"NoFood\" required", content);
        Assert.Contains("name=\"alcoholMode\" value=\"NoAlcohol\" required", content);
        Assert.DoesNotContain("name=\"beverageMode\" value=\"NoBeverages\" checked", content);
        Assert.DoesNotContain("name=\"foodMode\" value=\"NoFood\" checked", content);
        Assert.DoesNotContain("name=\"alcoholMode\" value=\"NoAlcohol\" checked", content);
        Assert.Contains("beverageMode: text(\"beverageMode\") || \"NotSure\"", script);
        Assert.Contains("foodMode: text(\"foodMode\") || \"NotSure\"", script);
        Assert.Contains("alcoholMode: text(\"alcoholMode\") || \"NotSure\"", script);
    }

    [Fact]
    public void Questionnaire_radio_answers_are_not_preselected_after_navigation()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Assessment.cshtml"));
        var script = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/wwwroot/js/site.js"));

        Assert.Contains("novalidate autocomplete=\"off\"", content);
        Assert.DoesNotContain(" checked", content);
        Assert.Contains("const restoredPayload = restoreDraft();", script);
        Assert.Contains("if (!restoredPayload)", script);
        Assert.Contains("clearRadioState();", script);
        Assert.Contains("isPublicEvent: text(\"isPublicEvent\") || \"Unknown\"", script);
        Assert.Contains("hasAmplifiedMusicOrSound: text(\"hasAmplifiedMusicOrSound\") || \"Unknown\"", script);
        Assert.Contains("hasTemporaryInstallations: text(\"hasTemporaryInstallations\") || \"Unknown\"", script);
        Assert.Contains("affectsTrafficOrParking: text(\"affectsTrafficOrParking\") || \"Unknown\"", script);
        Assert.Contains("hasProcessionOrRoute: text(\"hasProcessionOrRoute\") || \"Unknown\"", script);
        Assert.Contains("isSportCompetitionOnPublicRoad: text(\"isSportCompetitionOnPublicRoad\") || \"Unknown\"", script);
        Assert.Contains("usesGasGrillOrHeater: text(\"usesGasGrillOrHeater\") || \"Unknown\"", script);
    }

    [Fact]
    public void Corrupt_browser_draft_is_discarded_instead_of_blocking_questionnaire_startup()
    {
        var script = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/wwwroot/js/site.js"));

        Assert.Contains("const payload = readStoredPayload(draftKey) ?? readStoredPayload(resultKey);", script);
        Assert.Contains("function readStoredPayload(key)", script);
        Assert.Contains("try {", script);
        Assert.Contains("return JSON.parse(saved);", script);
        Assert.Contains("localStorage.removeItem(key);", script);
        Assert.Contains("return null;", script);
    }

    [Fact]
    public void Required_questions_show_visible_asterisk_marker()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Assessment.cshtml"));

        Assert.Contains("La manifestation aura-t-elle lieu sur le territoire de la Ville de Fribourg ? *</p>", content);
        Assert.Contains("Date de la manifestation *</span>", content);
        Assert.Contains("Nombre de personnes attendues *</span>", content);
        Assert.Contains("Où la manifestation aura-t-elle lieu ? *</p>", content);
        Assert.Contains("La manifestation est-elle ouverte au public ? *</p>", content);
        Assert.Contains("Qu’en est-il des boissons ? *</p>", content);
        Assert.Contains("Qu’en est-il de la nourriture ? *</p>", content);
        Assert.Contains("Y aura-t-il des boissons alcoolisées ? *</p>", content);
        Assert.Contains("Avez-vous une assurance responsabilité civile valable ? *</p>", content);
    }

    [Fact]
    public void Client_validation_surfaces_all_required_radio_errors_and_focuses_first_invalid_field()
    {
        var script = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/wwwroot/js/site.js"));

        Assert.Contains("let firstInvalid = null;", script);
        Assert.Contains("firstInvalid ??= field;", script);
        Assert.Contains("firstInvalid?.focus();", script);
        Assert.DoesNotContain("field.focus();", script);
    }

    [Fact]
    public void Venue_question_distinguishes_public_domain_from_private_venue_open_to_public()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Assessment.cshtml"));

        Assert.Contains("Où la manifestation aura-t-elle lieu ?", content);
        Assert.Contains("Par exemple, une rue, une place ou un parc lorsqu’ils relèvent du domaine public communal. Un lieu privé peut aussi être ouvert au public. En cas de doute, choisissez « Je ne sais pas ».", content);
        Assert.Contains("Sur le domaine public communal", content);
        Assert.Contains("Dans un lieu ou sur un fonds privé, par exemple un restaurant, un bar, une salle de concert ou un local associatif", content);
        Assert.Contains("Cette réponse concerne l’accès à l’événement, pas le statut public ou privé du lieu.", content);
        Assert.DoesNotContain("espace accessible au public", content);
    }

    [Fact]
    public void Removed_owner_authorization_answer_is_not_collected_as_dead_input()
    {
        Assert.DoesNotContain("privateVenueOwnerAuthorizationAvailable", File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Assessment.cshtml")));
        Assert.DoesNotContain("privateVenueOwnerAuthorizationAvailable", File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/wwwroot/js/site.js")));
        Assert.DoesNotContain("PrivateVenueOwnerAuthorizationAvailable", File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Models/AssessmentInput.cs")));
        Assert.DoesNotContain("PrivateVenueOwnerAuthorizationAvailable", File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Domain/Profiles/EventProfile.cs")));
    }

    [Fact]
    public void Results_summary_names_venue_status_and_event_access_separately()
    {
        var content = File.ReadAllText(ProjectFile("src/SwissEventPermitAssistant.Web/Pages/Results.cshtml"));

        Assert.Contains("Statut du lieu :", content);
        Assert.Contains("Accès :", content);
        Assert.Contains("domaine public communal", content);
        Assert.Contains("lieu ou fonds privé", content);
        Assert.Contains("ouverte au public", content);
        Assert.Contains("non ouverte au public", content);
    }

    private static string ProjectFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "SwissEventPermitAssistant.slnx")))
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new DirectoryNotFoundException("Could not locate the project root.");
        }

        return Path.Combine(directory.FullName, relativePath);
    }
}
