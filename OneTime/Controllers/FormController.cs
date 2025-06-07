using Microsoft.AspNetCore.Mvc;
using OneTime.Service;

namespace OneTime.Controllers;
public class FormController(IFormService service) : Controller
{
    private readonly IFormService service = service;

    [HttpGet]
    public async Task<IActionResult> Index(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return View("InvalidLink");
        }

        if (!await service.ValidateAndUseTokenAsync(token))
        {
            return View("InvalidLink");
        }

        return View(new FormResponse());
    }

    [HttpPost]
    public async Task<IActionResult> Index(FormResponse model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await service.SaveFormResponseAsync(model);
        return View("Success");
    }
}
