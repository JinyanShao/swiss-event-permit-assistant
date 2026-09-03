namespace SwissEventPermitAssistant.Tests.Web;

public sealed class QuestionnaireContentTests
{
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
