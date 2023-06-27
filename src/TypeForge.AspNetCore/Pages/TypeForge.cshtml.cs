using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TypeForge.AspNetCore.Pages;

public class TypeForge : PageModel
{
    public string Message { get; set; } = default!;

    public void OnGet()
    {
        Message = "Hello, worl dsadaasdasdasdassd!";
    }
}
