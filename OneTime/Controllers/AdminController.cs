using Microsoft.AspNetCore.Mvc;
using OneTime.Service;

namespace OneTime.Controllers;
public class AdminController(IFormService service) : Controller
{
    private readonly IFormService service = service;

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GenerateLink()
    {
        var token = await service.GenerateOneTimeLinkAsync();
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        // FIXED: Use query string format instead of route parameter
        var fullLink = $"{baseUrl}/Form/Index?token={token}";

        ViewBag.GeneratedLink = fullLink;
        return View("Index");
    }

    public async Task<IActionResult> ViewResponses()
    {
        var responses = await service.GetAllResponsesAsync();
        return View(responses);
    }

    public async Task<IActionResult> ExportToExcel()
    {
        var excelData = await service.GenerateExcelReportAsync();
        var fileName = $"FormResponses_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost]
    public async Task<IActionResult> CleanupExpiredLinks()
    {
        await service.CleanupExpiredLinksAsync();
        TempData["Message"] = "Expired links have been cleaned up successfully.";
        return RedirectToAction("Index");
    }
}