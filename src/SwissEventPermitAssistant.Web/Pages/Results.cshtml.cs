using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwissEventPermitAssistant.Domain.Rules;
using SwissEventPermitAssistant.Domain.Results;
using SwissEventPermitAssistant.Web.Models;

namespace SwissEventPermitAssistant.Web.Pages;

public sealed class ResultsModel(EventRulesEvaluator evaluator, TimeProvider timeProvider) : PageModel
{
    [BindProperty]
    public string? AssessmentJson { get; set; }

    public AssessmentInput? Input { get; private set; }

    public AssessmentResult? Result { get; private set; }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Assessment");
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(AssessmentJson))
        {
            return RedirectToPage("/Assessment");
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        try
        {
            Input = JsonSerializer.Deserialize<AssessmentInput>(AssessmentJson, jsonOptions);
        }
        catch (JsonException)
        {
            ModelState.AddModelError(string.Empty, "Les réponses n’ont pas pu être lues. Retournez au questionnaire.");
            return Page();
        }

        if (Input is null)
        {
            ModelState.AddModelError(string.Empty, "Les réponses n’ont pas pu être lues. Retournez au questionnaire.");
            return Page();
        }

        if (Input.Commune == Domain.Profiles.Commune.VilleDeFribourg && Input.EventDate is null)
        {
            ModelState.AddModelError(string.Empty, "La date de la manifestation est requise.");
            return Page();
        }

        if (Input.Commune == Domain.Profiles.Commune.VilleDeFribourg && Input.ExpectedAttendance is null)
        {
            ModelState.AddModelError(string.Empty, "Une estimation du nombre de personnes attendues est requise.");
            return Page();
        }

        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        Result = evaluator.Evaluate(Input.ToEventProfile(today));
        return Page();
    }
}
