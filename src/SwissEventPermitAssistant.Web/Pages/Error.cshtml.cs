using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SwissEventPermitAssistant.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    [BindProperty(SupportsGet = true)]
    public int? ResponseStatusCode { get; set; }

    public string Heading => ResponseStatusCode switch
    {
        404 => "Page introuvable",
        405 => "Action non disponible",
        _ => "Une erreur est survenue"
    };

    public string Message => ResponseStatusCode switch
    {
        404 => "Cette page n’existe pas ou son adresse a changé.",
        405 => "Cette action ne peut pas être traitée depuis cette page.",
        _ => "La demande n’a pas pu être traitée. Réessayez depuis le questionnaire ou consultez les sources officielles."
    };

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

