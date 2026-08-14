using SwissEventPermitAssistant.Domain.Profiles;
using SwissEventPermitAssistant.Domain.Results;
using SwissEventPermitAssistant.Domain.Sources;

namespace SwissEventPermitAssistant.Domain.Rules;

public sealed class EventRulesEvaluator
{
    private static readonly DateOnly Today = new(2026, 8, 14);

    public AssessmentResult Evaluate(EventProfile profile)
    {
        var actions = new List<ActionRequirement>();
        var documents = new List<DocumentRequirement>();
        var information = new List<InformationItem>();
        var confirmations = new List<ConfirmationItem>();
        var deadlines = new List<Deadline>();
        var sourceIds = new HashSet<string>();

        if (profile.Commune != Commune.VilleDeFribourg)
        {
            AddConfirmation(confirmations, "CONF-SCOPE", "Commune non couverte", "V0.1 couvre uniquement la Ville de Fribourg.", "Verifier les exigences aupres de la commune competente.", "Commune competente", "SRC-VDF-LT200", sourceIds);
            return Build(actions, documents, information, confirmations, deadlines, sourceIds);
        }

        AddPoliceLocaleRule(profile, actions, confirmations, deadlines, sourceIds);
        AddPatenteKRules(profile, actions, documents, information, confirmations, deadlines, sourceIds);
        AddSmartAndReuseRules(profile, actions, information, confirmations, deadlines, sourceIds);
        AddSetupRules(profile, documents, information, confirmations, sourceIds);
        AddMobilityRules(profile, actions, documents, confirmations, deadlines, sourceIds);
        AddInsuranceRules(profile, documents, confirmations, sourceIds);
        AddOptionalVilleRules(profile, actions, deadlines, sourceIds);

        return Build(actions, documents, information, confirmations, deadlines, sourceIds);
    }

    private static void AddPoliceLocaleRule(EventProfile profile, List<ActionRequirement> actions, List<ConfirmationItem> confirmations, List<Deadline> deadlines, HashSet<string> sourceIds)
    {
        if (profile.ExpectedAttendance is null)
        {
            AddConfirmation(confirmations, "CONF-ATTENDANCE", "Nombre de personnes a confirmer", "Le nombre attendu est necessaire pour orienter la demande a la Police locale.", "Indiquer une estimation avant de deposer le dossier.", "Ville de Fribourg - Police locale", "SRC-VDF-LT200", sourceIds);
            return;
        }

        var (sourceId, deadline) = profile.ExpectedAttendance < 200
            ? ("SRC-VDF-LT200", CreateDaysBefore("DL-POLICE-20", "Autorisation Police locale", profile.EventDate, 20, "SRC-VDF-LT200"))
            : profile.ExpectedAttendance <= 1000
                ? ("SRC-VDF-200-1000", CreateUnconfirmed("DL-POLICE-UNCONFIRMED", "Autorisation Police locale", "SRC-VDF-200-1000"))
                : ("SRC-VDF-GT1000", CreateUnconfirmed("DL-POLICE-UNCONFIRMED", "Autorisation Police locale", "SRC-VDF-GT1000"));

        Use(sourceIds, sourceId);
        deadlines.AddUnique(deadline);
        actions.AddUnique(new ActionRequirement("ACT-POLICE-LOCALE", "Autorisation Police locale", RequirementStatus.Required, "Ville de Fribourg - Police locale", "La manifestation se trouve dans le perimetre Ville de Fribourg V0.1.", sourceId, deadline));
    }

    private static void AddPatenteKRules(EventProfile profile, List<ActionRequirement> actions, List<DocumentRequirement> documents, List<InformationItem> information, List<ConfirmationItem> confirmations, List<Deadline> deadlines, HashSet<string> sourceIds)
    {
        var patenteRequired = profile.BeverageMode == BeverageMode.BeveragesSold
            || profile.FoodMode is FoodMode.CookedFoodSold or FoodMode.OtherFoodSoldOrUnsure
            || profile.AlcoholMode == AlcoholMode.AlcoholSold;

        if (patenteRequired)
        {
            var deadline = CreateDaysBefore("DL-PATENTE-K-60", "Patente K", profile.EventDate, 60, "SRC-FR-PATENTE-K");
            Use(sourceIds, "SRC-FR-PATENTE-K");
            deadlines.AddUnique(deadline);
            actions.AddUnique(new ActionRequirement("ACT-PATENTE-K", "Patente K", RequirementStatus.Required, "Prefecture du district", "Vous avez indique une vente de boissons, d'alcool ou de nourriture.", "SRC-FR-PATENTE-K", deadline));
            information.AddUnique(new InformationItem("INFO-EGOV", "Demarche en ligne", "La Patente K se depose via egov.fr.ch avec inscription.", "SRC-FR-PATENTE-K"));

            if (profile.VenueKind == VenueKind.PrivateVenue)
            {
                documents.AddMerge(new DocumentRequirement("DOC-OWNER-AUTHORIZATION", "Autorisation signee du proprietaire", RequirementStatus.Required, "Necessaire pour une Patente K sur fonds prive.", ["R-OWNER-001"], "SRC-FR-PATENTE-K"));
            }
        }

        if (profile.BeverageMode is BeverageMode.BeveragesFree or BeverageMode.NotSure)
        {
            AddConfirmation(confirmations, "CONF-FREE-BEVERAGES-PATENTE", "Patente K pour boissons gratuites", "Les sources publiques consultées confirment la vente de boissons, mais pas clairement la distribution gratuite.", "Confirmer avec la Prefecture si des boissons sont offertes.", "Prefecture du district", "SRC-FR-PATENTE-K", sourceIds);
        }

        if (profile.FoodMode is FoodMode.FoodFree or FoodMode.NotSure)
        {
            AddConfirmation(confirmations, "CONF-FREE-FOOD-PATENTE", "Patente K pour nourriture gratuite", "Les sources publiques consultées confirment la vente de mets, mais pas clairement la distribution gratuite.", "Confirmer avec la Prefecture si de la nourriture est offerte.", "Prefecture du district", "SRC-FR-PATENTE-K", sourceIds);
        }

        if (profile.AlcoholMode is AlcoholMode.AlcoholFree or AlcoholMode.NotSure)
        {
            AddConfirmation(confirmations, "CONF-FREE-ALCOHOL-PATENTE", "Patente K pour alcool offert", "Les sources publiques consultées ne permettent pas de confirmer automatiquement si l'alcool offert gratuitement exige une Patente K.", "Confirmer avec la Prefecture avant de deposer le dossier.", "Prefecture du district", "SRC-FR-PATENTE-K", sourceIds);
        }

        if (profile.AlcoholMode is AlcoholMode.AlcoholSold or AlcoholMode.AlcoholFree or AlcoholMode.NotSure)
        {
            information.AddUnique(new InformationItem("INFO-ALCOHOL", "Boissons alcoolisees", "Prevoir les informations et obligations de prevention liees aux boissons alcoolisees.", "SRC-FR-PATENTE-K"));
            Use(sourceIds, "SRC-FR-PATENTE-K");
        }

        if (patenteRequired || profile.ExpectedAttendance > 1000 || profile.AffectsTrafficOrParking == YesNoUnknown.Yes || profile.HasProcessionOrRoute == YesNoUnknown.Yes)
        {
            AddConfirmation(confirmations, "CONF-FORM-B", "Formulaire B", "Le Formulaire B concerne les manifestations d'importance, mais aucun seuil objectif public n'a ete confirme pour V0.1.", "Ne pas le considerer automatiquement requis; verifier si l'autorite le demande.", "Prefecture du district", "SRC-FR-FORM-B", sourceIds);
        }
    }

    private static void AddSmartAndReuseRules(EventProfile profile, List<ActionRequirement> actions, List<InformationItem> information, List<ConfirmationItem> confirmations, List<Deadline> deadlines, HashSet<string> sourceIds)
    {
        var beveragesServed = profile.BeverageMode is BeverageMode.BeveragesSold or BeverageMode.BeveragesFree or BeverageMode.NotSure;
        var foodServed = profile.FoodMode is FoodMode.CookedFoodSold or FoodMode.OtherFoodSoldOrUnsure or FoodMode.FoodFree or FoodMode.NotSure;

        if ((beveragesServed || foodServed) && profile.VenueKind == VenueKind.PublicSpace)
        {
            information.AddUnique(new InformationItem("INFO-REUSABLE-TABLEWARE", "Vaisselle reutilisable", "Les manifestations qui servent des mets ou boissons peuvent etre soumises aux exigences de durabilite de la Ville.", SourceForAttendance(profile.ExpectedAttendance)));
            Use(sourceIds, SourceForAttendance(profile.ExpectedAttendance));
        }

        if (profile.VenueKind == VenueKind.PublicSpace && beveragesServed)
        {
            var smartAction = SmartAction(profile.ExpectedAttendance);
            var deadline = CreateUnconfirmed("DL-SMART-UNCONFIRMED", smartAction, SourceForAttendance(profile.ExpectedAttendance));
            deadlines.AddUnique(deadline);
            actions.AddUnique(new ActionRequirement("ACT-SMART-CHECK", smartAction, RequirementStatus.Required, "Ville de Fribourg", "Vous avez indique des boissons dans l'espace public.", SourceForAttendance(profile.ExpectedAttendance), deadline));
            Use(sourceIds, SourceForAttendance(profile.ExpectedAttendance));
        }

        if (profile.VenueKind == VenueKind.NotSure)
        {
            AddConfirmation(confirmations, "CONF-VENUE", "Lieu a confirmer", "Le type de lieu determine certaines obligations liees a l'espace public.", "Confirmer si la manifestation utilise l'espace public.", "Ville de Fribourg - Police locale", "SRC-VDF-LT200", sourceIds);
        }

        if (profile.VenueKind == VenueKind.PrivateVenue && profile.IsPublicEvent == YesNoUnknown.Yes)
        {
            AddConfirmation(confirmations, "CONF-PRIVATE-POLICE", "Autorisation Police locale sur terrain prive", "Les sources publiques mentionnent les manifestations publiques sur fonds prive, mais ne permettent pas de confirmer automatiquement le besoin d'autorisation.", "Contacter la Police locale avant de deposer le dossier.", "Ville de Fribourg - Police locale", "SRC-VDF-LT200", sourceIds);
        }
    }

    private static void AddSetupRules(EventProfile profile, List<DocumentRequirement> documents, List<InformationItem> information, List<ConfirmationItem> confirmations, HashSet<string> sourceIds)
    {
        if (profile.HasTemporaryInstallations == YesNoUnknown.Yes)
        {
            documents.AddMerge(new DocumentRequirement("DOC-SITE-PLAN", "Plan du site", RequirementStatus.Required, "Nécessaire pour decrire les installations et les acces.", ["R-INSTALLATION-001"], "SRC-FR-FORM-B"));
            documents.AddMerge(new DocumentRequirement("DOC-INSTALLATION-DESCRIPTION", "Description des installations temporaires", RequirementStatus.Required, "Tente, scene, bar, cuisine, WC ou installations similaires.", ["R-INSTALLATION-001"], "SRC-FR-FORM-B"));
            Use(sourceIds, "SRC-FR-FORM-B");
        }
        else if (profile.HasTemporaryInstallations == YesNoUnknown.Unknown)
        {
            AddConfirmation(confirmations, "CONF-INSTALLATIONS", "Installations temporaires", "Cette reponse peut changer les documents a preparer.", "Confirmer si des installations temporaires sont prevues.", "Ville de Fribourg - Police locale", "SRC-FR-FORM-B", sourceIds);
        }

        if (profile.HasAmplifiedMusicOrSound != YesNoUnknown.No)
        {
            information.AddUnique(new InformationItem("INFO-SOUND", "Musique et sonorisation", "La musique amplifiee ou la sonorisation peut entrainer des conditions liees au bruit ou aux horaires.", SourceForAttendance(profile.ExpectedAttendance)));
            Use(sourceIds, SourceForAttendance(profile.ExpectedAttendance));

            if (profile.HasAmplifiedMusicOrSound == YesNoUnknown.Unknown || profile.EventEndTime is not null)
            {
                AddConfirmation(confirmations, "CONF-SOUND", "Conditions liees au son", "Les conditions exactes dependent du lieu, de l'horaire et de l'autorite competente.", "Verifier les conditions applicables avec la Police locale ou l'autorite competente.", "Ville de Fribourg - Police locale", SourceForAttendance(profile.ExpectedAttendance), sourceIds);
            }
        }

        if (profile.UsesGasGrillOrHeater != YesNoUnknown.No)
        {
            information.AddUnique(new InformationItem("INFO-GAS", "Gril ou chauffage a gaz", "Prevoir le respect des normes GPL mentionnees par la Ville.", SourceForAttendance(profile.ExpectedAttendance)));
            Use(sourceIds, SourceForAttendance(profile.ExpectedAttendance));
        }
    }

    private static void AddMobilityRules(EventProfile profile, List<ActionRequirement> actions, List<DocumentRequirement> documents, List<ConfirmationItem> confirmations, List<Deadline> deadlines, HashSet<string> sourceIds)
    {
        if (profile.AffectsTrafficOrParking == YesNoUnknown.Yes)
        {
            documents.AddMerge(new DocumentRequirement("DOC-SITE-PLAN", "Plan du site", RequirementStatus.Required, "Utile pour documenter les acces, la circulation et l'organisation du site.", ["R-TRAFFIC-001"], "SRC-FR-FORM-B"));
            documents.AddMerge(new DocumentRequirement("DOC-TRAFFIC-PARKING", "Concept de circulation ou stationnement", RequirementStatus.Required, "Nécessaire si la manifestation influence la circulation ou le stationnement.", ["R-TRAFFIC-001"], "SRC-FR-FORM-B"));
            AddConfirmation(confirmations, "CONF-TRAFFIC", "Mesures de circulation", "Les mesures exactes doivent etre validees par l'autorite competente.", "Confirmer les besoins de signalisation, fermeture ou stationnement.", "Police cantonale / Police locale", "SRC-FR-FORM-B", sourceIds);
        }
        else if (profile.AffectsTrafficOrParking == YesNoUnknown.Unknown)
        {
            AddConfirmation(confirmations, "CONF-TRAFFIC-UNKNOWN", "Impact circulation a confirmer", "Un impact sur la circulation peut changer les documents et autorisations.", "Verifier si la manifestation influence la circulation ou le stationnement.", "Police locale", "SRC-FR-FORM-B", sourceIds);
        }

        if (profile.HasProcessionOrRoute == YesNoUnknown.Yes)
        {
            documents.AddMerge(new DocumentRequirement("DOC-ROUTE-PLAN", "Plan du parcours", RequirementStatus.Required, "Nécessaire pour un cortege, parcours ou occupation de route.", ["R-ROUTE-001"], "SRC-FR-FORM-B"));
            AddConfirmation(confirmations, "CONF-ROAD-CLOSURE", "Fermeture ou restriction de route", "Une fermeture ou restriction peut necessiter une validation specifique.", "Confirmer avec l'autorite competente.", "Police locale / Police cantonale", "SRC-FR-FORM-B", sourceIds);
        }
        else if (profile.HasProcessionOrRoute == YesNoUnknown.Unknown)
        {
            AddConfirmation(confirmations, "CONF-ROUTE-UNKNOWN", "Parcours a confirmer", "Un parcours ou cortege peut changer les documents a preparer.", "Confirmer si un parcours est prevu.", "Police locale", "SRC-FR-FORM-B", sourceIds);
        }

        if (profile.IsSportCompetitionOnPublicRoad == YesNoUnknown.Yes)
        {
            var deadline = CreateMonthsBefore("DL-OCN-2M", "Autorisation OCN", profile.EventDate, 2, "SRC-OCN-SPORT");
            deadlines.AddUnique(deadline);
            actions.AddUnique(new ActionRequirement("ACT-OCN-SPORT", "Autorisation OCN", RequirementStatus.Required, "Office de la circulation et de la navigation", "Vous avez indique une competition sportive sur la voie publique.", "SRC-OCN-SPORT", deadline));
            Use(sourceIds, "SRC-OCN-SPORT");
        }
    }

    private static void AddInsuranceRules(EventProfile profile, List<DocumentRequirement> documents, List<ConfirmationItem> confirmations, HashSet<string> sourceIds)
    {
        documents.AddMerge(new DocumentRequirement("DOC-LIABILITY-INSURANCE", "Assurance responsabilite civile valable", RequirementStatus.Required, "La Ville indique que l'organisateur doit pouvoir presenter une RC valable.", ["R-RC-001"], SourceForAttendance(profile.ExpectedAttendance)));
        Use(sourceIds, SourceForAttendance(profile.ExpectedAttendance));

        if (profile.HasLiabilityInsurance != YesNoUnknown.Yes)
        {
            AddConfirmation(confirmations, "CONF-RC", "Assurance responsabilite civile", "L'assurance RC n'est pas confirmee dans vos reponses.", "Prevoir une RC valable ou verifier ce point avant de deposer la demande.", "Ville de Fribourg", SourceForAttendance(profile.ExpectedAttendance), sourceIds);
        }
    }

    private static void AddOptionalVilleRules(EventProfile profile, List<ActionRequirement> actions, List<Deadline> deadlines, HashSet<string> sourceIds)
    {
        if (profile.NeedsMunicipalMaterialOrDecorations)
        {
            var deadline = CreateDaysBefore("DL-MATERIAL-30", "Materiel ou decorations communales", profile.EventDate, 30, SourceForAttendance(profile.ExpectedAttendance));
            deadlines.AddUnique(deadline);
            actions.AddUnique(new ActionRequirement("ACT-MUNICIPAL-MATERIAL", "Materiel ou decorations communales", RequirementStatus.Required, "Ville de Fribourg", "Vous avez indique un besoin de materiel ou decorations communales.", SourceForAttendance(profile.ExpectedAttendance), deadline));
            Use(sourceIds, SourceForAttendance(profile.ExpectedAttendance));
        }

        if (profile.NeedsAdvertisingBannerOrPublicPosting)
        {
            var deadline = CreateDaysBefore("DL-ADVERTISING-20", "Affichage public ou banderole", profile.EventDate, 20, SourceForAttendance(profile.ExpectedAttendance));
            deadlines.AddUnique(deadline);
            actions.AddUnique(new ActionRequirement("ACT-ADVERTISING", "Affichage public ou banderole", RequirementStatus.Required, "Ville de Fribourg - Police locale", "Vous avez indique un besoin d'affichage public ou de banderole.", SourceForAttendance(profile.ExpectedAttendance), deadline));
            Use(sourceIds, SourceForAttendance(profile.ExpectedAttendance));
        }
    }

    private static AssessmentResult Build(List<ActionRequirement> actions, List<DocumentRequirement> documents, List<InformationItem> information, List<ConfirmationItem> confirmations, List<Deadline> deadlines, HashSet<string> sourceIds) =>
        new(
            actions.OrderBy(action => action.Id).ToArray(),
            documents.OrderBy(document => document.Id).ToArray(),
            information.OrderBy(item => item.Id).ToArray(),
            confirmations.OrderBy(item => item.Id).ToArray(),
            deadlines.OrderBy(deadline => deadline.Date ?? DateOnly.MaxValue).ThenBy(deadline => deadline.Id).ToArray(),
            sourceIds.Select(id => OfficialSources.All[id]).OrderBy(source => source.Id).ToArray());

    private static Deadline CreateDaysBefore(string id, string label, DateOnly eventDate, int days, string sourceId)
    {
        var date = eventDate.AddDays(-days);
        return new Deadline(id, label, date, $"Au moins {days} jours avant la manifestation.", DeadlineStatusFor(date), sourceId);
    }

    private static Deadline CreateMonthsBefore(string id, string label, DateOnly eventDate, int months, string sourceId)
    {
        var date = eventDate.AddMonths(-months);
        return new Deadline(id, label, date, $"Au moins {months} mois avant la manifestation.", DeadlineStatusFor(date), sourceId);
    }

    private static Deadline CreateUnconfirmed(string id, string label, string sourceId) =>
        new(id, label, null, "Delai minimum non confirme dans la source publique consultee.", DeadlineStatus.Unconfirmed, sourceId);

    private static DeadlineStatus DeadlineStatusFor(DateOnly date)
    {
        if (date < Today)
        {
            return DeadlineStatus.Passed;
        }

        return date <= Today.AddDays(14) ? DeadlineStatus.Approaching : DeadlineStatus.Confirmed;
    }

    private static string SourceForAttendance(int? attendance) =>
        attendance is null || attendance < 200
            ? "SRC-VDF-LT200"
            : attendance <= 1000
                ? "SRC-VDF-200-1000"
                : "SRC-VDF-GT1000";

    private static string SmartAction(int? attendance) =>
        attendance is null || attendance < 200
            ? "Smart Check"
            : attendance <= 1000
                ? "Smart Check Plus"
                : "Smart Event Plus";

    private static void AddConfirmation(List<ConfirmationItem> confirmations, string id, string title, string reason, string whatToDo, string authority, string sourceId, HashSet<string> sourceIds)
    {
        Use(sourceIds, sourceId);
        confirmations.AddUnique(new ConfirmationItem(id, title, reason, whatToDo, authority, sourceId));
    }

    private static void Use(HashSet<string> sourceIds, string sourceId) => sourceIds.Add(sourceId);
}

internal static class RuleCollectionExtensions
{
    public static void AddUnique(this List<ActionRequirement> items, ActionRequirement item)
    {
        if (items.All(existing => existing.Id != item.Id))
        {
            items.Add(item);
        }
    }

    public static void AddUnique(this List<InformationItem> items, InformationItem item)
    {
        if (items.All(existing => existing.Id != item.Id))
        {
            items.Add(item);
        }
    }

    public static void AddUnique(this List<ConfirmationItem> items, ConfirmationItem item)
    {
        if (items.All(existing => existing.Id != item.Id))
        {
            items.Add(item);
        }
    }

    public static void AddUnique(this List<Deadline> items, Deadline item)
    {
        if (items.All(existing => existing.Id != item.Id))
        {
            items.Add(item);
        }
    }

    public static void AddMerge(this List<DocumentRequirement> items, DocumentRequirement item)
    {
        var existing = items.FirstOrDefault(document => document.Id == item.Id);
        if (existing is null)
        {
            items.Add(item);
            return;
        }

        items.Remove(existing);
        items.Add(existing with
        {
            TriggeredByRuleIds = existing.TriggeredByRuleIds.Concat(item.TriggeredByRuleIds).Distinct().Order().ToArray(),
            Reason = existing.Reason == item.Reason ? existing.Reason : $"{existing.Reason} {item.Reason}"
        });
    }
}
