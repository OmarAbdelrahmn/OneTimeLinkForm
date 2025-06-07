using Microsoft.AspNetCore.Mvc;
using OneTime.Service;

namespace OneTime.Controllers;
public class FormController(IFormService service) : Controller
{
    private readonly IFormService service = service;

    [HttpGet]
    public async Task<IActionResult> Index(string token)  // This will automatically capture ?token=abc123
    {
        if (string.IsNullOrEmpty(token))
        {
            return View("InvalidLink");
        }

        // Only validate the token, don't consume it yet
        if (!await service.ValidateTokenAsync(token))
        {
            return View("InvalidLink");
        }

        // Store the token in ViewBag so we can use it in the POST
        ViewBag.Token = token;
        return View(new FormResponse());
    }

    [HttpPost]
    public async Task<IActionResult> Index(FormResponse model, string token)
    {
        // Validate the token again and consume it only if form is valid
        if (string.IsNullOrEmpty(token) || !await service.ValidateTokenAsync(token))
        {
            return View("InvalidLink");
        }

        if (!ModelState.IsValid)
        {
            // If form validation fails, keep the token for retry
            ViewBag.Token = token;
            return View(model);
        }

        // Form is valid, now consume the token and save the response
        if (!await service.ValidateAndUseTokenAsync(token))
        {
            return View("InvalidLink");
        }

        await service.SaveFormResponseAsync(model);
        return View("Success");
    }
}