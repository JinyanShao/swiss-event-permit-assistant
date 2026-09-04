using SwissEventPermitAssistant.Domain.Profiles;
using SwissEventPermitAssistant.Domain.Results;
using SwissEventPermitAssistant.Domain.Rules;
using SwissEventPermitAssistant.Domain.Sources;

namespace SwissEventPermitAssistant.Tests.Sources;

public sealed class OfficialResourceCatalogTests
{
    private readonly EventRulesEvaluator _evaluator = new(new FixedTimeProvider(new DateTimeOffset(2026, 8, 14, 12, 0, 0, TimeSpan.Zero)));

    [Fact]
    public void Police_locale_action_resolves_to_stable_ville_page_without_changing_rule_output()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150));
        var action = Assert.Single(result.Actions, action => action.Id == "ACT-POLICE-LOCALE");

        var resource = Assert.Single(OfficialResourceCatalog.For(action));

        Assert.Equal("Ouvrir la page officielle Ville", resource.Label);
        Assert.Equal("https://www.ville-fribourg.ch/organiser-manifestation/moins-de-200", resource.Url.ToString());
        Assert.Equal(OfficialResourceType.OfficialInformation, resource.Type);
        Assert.Equal("Autorisation Police locale", action.Title);
    }

    [Theory]
    [InlineData(150, "DL-SMART-20", "Ouvrir la page Ville avec le lien Smart Check", "https://www.ville-fribourg.ch/organiser-manifestation/moins-de-200")]
    [InlineData(500, "DL-SMART-30", "Ouvrir la page Ville avec le lien Smart Check Plus", "https://www.ville-fribourg.ch/organiser-manifestation/200-a-1000")]
    [InlineData(1200, "DL-SMART-60", "Ouvrir la page Ville avec le lien Smart Event Plus", "https://www.ville-fribourg.ch/organiser-manifestation/plus-de-1000")]
    public void Smart_action_uses_existing_deadline_id_to_select_resource(int expectedAttendance, string deadlineId, string label, string url)
    {
        var result = Evaluate(DefaultProfile(expectedAttendance) with
        {
            BeverageMode = BeverageMode.BeveragesFree
        });
        var action = Assert.Single(result.Actions, action => action.Id == "ACT-SMART-CHECK");

        var resource = Assert.Single(OfficialResourceCatalog.For(action));

        Assert.Equal(deadlineId, action.Deadline?.Id);
        Assert.Equal(label, resource.Label);
        Assert.Equal(url, resource.Url.ToString());
    }

    [Fact]
    public void Patente_k_action_links_to_official_information_not_old_egov_detail_url()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            BeverageMode = BeverageMode.BeveragesSold
        });
        var action = Assert.Single(result.Actions, action => action.Id == "ACT-PATENTE-K");

        var resources = OfficialResourceCatalog.For(action);

        Assert.Contains(resources, resource => resource.Label == "Voir les informations Patente K");
        Assert.Contains(resources, resource => resource.Label == "Lire le guide de demande en ligne Patente K" && resource.Url.ToString() == "https://www.fr.ch/document/530631");
        Assert.DoesNotContain(resources, resource => resource.Url.ToString().Contains("Detail.aspx?id=1075", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Patente_k_information_items_do_not_repeat_action_resource_links()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            BeverageMode = BeverageMode.BeveragesSold
        });

        var egovInformation = Assert.Single(result.Information, information => information.Id == "INFO-EGOV");
        var hoursInformation = Assert.Single(result.Information, information => information.Id == "INFO-PATENTE-K-HOURS");

        Assert.Empty(OfficialResourceCatalog.For(egovInformation));
        Assert.Empty(OfficialResourceCatalog.For(hoursInformation));
    }

    [Fact]
    public void Ocn_action_resolves_to_application_pdf_and_aide_memoire()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            IsSportCompetitionOnPublicRoad = YesNoUnknown.Yes
        });
        var action = Assert.Single(result.Actions, action => action.Id == "ACT-OCN-SPORT");

        var resources = OfficialResourceCatalog.For(action);

        Assert.Contains(resources, resource => resource.Label == "Télécharger la demande OCN" && resource.Type == OfficialResourceType.Form);
        Assert.Contains(resources, resource => resource.Label == "Lire l’aide-mémoire OCN" && resource.Type == OfficialResourceType.Guide);
    }

    [Fact]
    public void Form_b_resource_is_only_attached_to_existing_confirmation()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 1200));
        var confirmation = Assert.Single(result.Confirmations, confirmation => confirmation.Id == "CONF-FORM-B");

        var resource = Assert.Single(OfficialResourceCatalog.For(confirmation));

        Assert.Equal("Consulter le Formulaire B", resource.Label);
        Assert.DoesNotContain(result.Documents, document => document.Id.Contains("FORM-B", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Documents_without_specific_actionable_resource_do_not_get_vague_links()
    {
        var result = Evaluate(DefaultProfile(expectedAttendance: 150) with
        {
            HasTemporaryInstallations = YesNoUnknown.Yes
        });

        var sitePlan = Assert.Single(result.Documents, document => document.Id == "DOC-SITE-PLAN");
        var installationDescription = Assert.Single(result.Documents, document => document.Id == "DOC-INSTALLATION-DESCRIPTION");

        Assert.Empty(OfficialResourceCatalog.For(sitePlan));
        Assert.Empty(OfficialResourceCatalog.For(installationDescription));
    }

    private AssessmentResult Evaluate(EventProfile profile) => _evaluator.Evaluate(profile);

    private static EventProfile DefaultProfile(int? expectedAttendance) =>
        new(
            Commune.VilleDeFribourg,
            new DateOnly(2026, 10, 10),
            expectedAttendance,
            VenueKind.PublicSpace);

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
