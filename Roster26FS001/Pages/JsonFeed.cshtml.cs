using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Roster26FS001.Services;

namespace Roster26FS001.Pages;

public sealed class JsonFeedModel : PageModel
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IRosterStore rosterStore;

    public JsonFeedModel(IRosterStore rosterStore)
    {
        this.rosterStore = rosterStore;
    }

    public JsonResult OnGet()
    {
        return new JsonResult(rosterStore.GetSnapshot(), JsonOptions);
    }
}
