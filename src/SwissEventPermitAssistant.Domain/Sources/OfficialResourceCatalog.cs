using SwissEventPermitAssistant.Domain.Results;

namespace SwissEventPermitAssistant.Domain.Sources;

public enum OfficialResourceType
{
    OnlineApplication,
    Form,
    Guide,
    OfficialInformation
}

public sealed record OfficialResource(
    string Label,
    Uri Url,
    OfficialResourceType Type,
    DateOnly CheckedDate);

public static class OfficialResourceCatalog
{
    private static readonly DateOnly ResourceCheckedDate = new(2026, 9, 4);

    private static readonly OfficialResource VilleLessThan200 = new(
        "Ouvrir la page officielle Ville",
        new Uri("https://www.ville-fribourg.ch/organiser-manifestation/moins-de-200"),
        OfficialResourceType.OfficialInformation,
        ResourceCheckedDate);

    private static readonly OfficialResource VilleFrom200To1000 = new(
        "Ouvrir la page officielle Ville",
        new Uri("https://www.ville-fribourg.ch/organiser-manifestation/200-a-1000"),
        OfficialResourceType.OfficialInformation,
        ResourceCheckedDate);

    private static readonly OfficialResource VilleMoreThan1000 = new(
        "Ouvrir la page officielle Ville",
        new Uri("https://www.ville-fribourg.ch/organiser-manifestation/plus-de-1000"),
        OfficialResourceType.OfficialInformation,
        ResourceCheckedDate);

    private static readonly OfficialResource SmartCheck = new(
        "Ouvrir la page Ville avec le lien Smart Check",
        new Uri("https://www.ville-fribourg.ch/organiser-manifestation/moins-de-200"),
        OfficialResourceType.OfficialInformation,
        ResourceCheckedDate);

    private static readonly OfficialResource SmartCheckPlus = new(
        "Ouvrir la page Ville avec le lien Smart Check Plus",
        new Uri("https://www.ville-fribourg.ch/organiser-manifestation/200-a-1000"),
        OfficialResourceType.OfficialInformation,
        ResourceCheckedDate);

    private static readonly OfficialResource SmartEventPlus = new(
        "Ouvrir la page Ville avec le lien Smart Event Plus",
        new Uri("https://www.ville-fribourg.ch/organiser-manifestation/plus-de-1000"),
        OfficialResourceType.OfficialInformation,
        ResourceCheckedDate);

    private static readonly OfficialResource PatenteKInformation = new(
        "Voir les informations Patente K",
        new Uri("https://www.fr.ch/vie-quotidienne/demarches-et-documents/manifestations-temporaires-patente-k"),
        OfficialResourceType.OfficialInformation,
        ResourceCheckedDate);

    private static readonly OfficialResource PatenteKOnlineGuide = new(
        "Lire le guide de demande en ligne Patente K",
        new Uri("https://www.fr.ch/document/530631"),
        OfficialResourceType.Guide,
        ResourceCheckedDate);

    private static readonly OfficialResource FormB = new(
        "Consulter le Formulaire B",
        new Uri("https://www.fr.ch/sites/default/files/2024-02/manifestation-temporaire--formulaire-complementaire-b-manifestation-d-importance.pdf"),
        OfficialResourceType.Form,
        ResourceCheckedDate);

    private static readonly OfficialResource OcnApplication = new(
        "Télécharger la demande OCN",
        new Uri("https://www.ocn.ch/sites/default/files/2025-10/2025_Demande_autorisation_manifestation_sportive.pdf"),
        OfficialResourceType.Form,
        ResourceCheckedDate);

    private static readonly OfficialResource OcnGuide = new(
        "Lire l’aide-mémoire OCN",
        new Uri("https://www.ocn.ch/sites/default/files/2024-08/Aidememoire_manifestations_sportives_20240819.pdf"),
        OfficialResourceType.Guide,
        ResourceCheckedDate);

    public static IReadOnlyList<OfficialResource> For(ActionRequirement action) =>
        action.Id switch
        {
            "ACT-POLICE-LOCALE" => [VillePageFor(action.SourceId)],
            "ACT-SMART-CHECK" => SmartResourcesFor(action),
            "ACT-PATENTE-K" => [PatenteKInformation, PatenteKOnlineGuide],
            "ACT-OCN-SPORT" => [OcnApplication, OcnGuide],
            _ => []
        };

    public static IReadOnlyList<OfficialResource> For(InformationItem information) =>
        information.Id switch
        {
            _ => []
        };

    public static IReadOnlyList<OfficialResource> For(DocumentRequirement document) =>
        document.Id switch
        {
            _ => []
        };

    public static IReadOnlyList<OfficialResource> For(ConfirmationItem confirmation) =>
        confirmation.Id switch
        {
            "CONF-FORM-B" => [FormB],
            _ => []
        };

    private static OfficialResource VillePageFor(string sourceId) =>
        sourceId switch
        {
            "SRC-VDF-200-1000" => VilleFrom200To1000,
            "SRC-VDF-GT1000" => VilleMoreThan1000,
            _ => VilleLessThan200
        };

    private static IReadOnlyList<OfficialResource> SmartResourcesFor(ActionRequirement action) =>
        action.Deadline?.Id switch
        {
            "DL-SMART-20" => [SmartCheck],
            "DL-SMART-30" => [SmartCheckPlus],
            "DL-SMART-60" => [SmartEventPlus],
            _ => []
        };
}
