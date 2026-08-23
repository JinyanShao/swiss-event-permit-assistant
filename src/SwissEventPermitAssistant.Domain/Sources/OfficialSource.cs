namespace SwissEventPermitAssistant.Domain.Sources;

public sealed record OfficialSource(
    string Id,
    string Authority,
    string Title,
    Uri Url,
    DateOnly CheckedDate,
    string Confidence,
    string? Notes = null);

public static class OfficialSources
{
    public static readonly IReadOnlyDictionary<string, OfficialSource> All =
        new Dictionary<string, OfficialSource>
        {
            ["SRC-VDF-LT200"] = new(
                "SRC-VDF-LT200",
                "Ville de Fribourg",
                "Organiser une manifestation - moins de 200 personnes",
                new Uri("https://www.ville-fribourg.ch/organiser-manifestation/moins-de-200"),
                new DateOnly(2026, 8, 14),
                "High"),
            ["SRC-VDF-200-1000"] = new(
                "SRC-VDF-200-1000",
                "Ville de Fribourg",
                "Organiser une manifestation - de 200 à 1000 personnes",
                new Uri("https://www.ville-fribourg.ch/organiser-manifestation/200-a-1000"),
                new DateOnly(2026, 8, 14),
                "High"),
            ["SRC-VDF-GT1000"] = new(
                "SRC-VDF-GT1000",
                "Ville de Fribourg",
                "Organiser une manifestation - plus de 1000 personnes",
                new Uri("https://www.ville-fribourg.ch/organiser-manifestation/plus-de-1000"),
                new DateOnly(2026, 8, 14),
                "High"),
            ["SRC-FR-PATENTE-K"] = new(
                "SRC-FR-PATENTE-K",
                "Canton de Fribourg",
                "Manifestations temporaires - Patente K",
                new Uri("https://www.fr.ch/vie-quotidienne/demarches-et-documents/manifestations-temporaires-patente-k"),
                new DateOnly(2026, 8, 14),
                "High"),
            ["SRC-FR-FORM-B"] = new(
                "SRC-FR-FORM-B",
                "Canton de Fribourg",
                "Formulaire complémentaire B - manifestation d’importance",
                new Uri("https://www.fr.ch/vie-quotidienne/demarches-et-documents/formulaires-des-prefectures"),
                new DateOnly(2026, 8, 14),
                "Medium"),
            ["SRC-OCN-SPORT"] = new(
                "SRC-OCN-SPORT",
                "Office de la circulation et de la navigation",
                "Compétitions sportives sur voie publique",
                new Uri("https://www.ocn.ch/fr/conduire/autorisations/competitions-sportives"),
                new DateOnly(2026, 8, 14),
                "High")
        };
}
