using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Collections.Concurrent;
using System.ComponentModel;
using LicenseContext = System.ComponentModel.LicenseContext;

namespace OneTime.Service;

public class FormService(AppDbcontext dbcontext) : IFormService
{

    private static readonly ConcurrentDictionary<string, OneTimeLink> _activeLinks = new();
    private static readonly ConcurrentBag<FormResponse> _formResponses = [];
    private readonly AppDbcontext dbcontext = dbcontext;

    public async Task<string> GenerateOneTimeLinkAsync()
    {
        // Clean up expired links first
        await CleanupExpiredLinksAsync();

        var token = Guid.NewGuid().ToString("N");
        var link = new OneTimeLink
        {
            Token = token,
            CreatedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddHours(24), // Link expires in 24 hours
            IsUsed = false
        };

        dbcontext.OneTimeLinks.Add(link);
        await dbcontext.SaveChangesAsync();

        return token;
    }

    public async Task<bool> ValidateAndUseTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var link = await dbcontext.OneTimeLinks
            .FirstOrDefaultAsync(l => l.Token == token);

        if (link == null || link.IsUsed || DateTime.Now > link.ExpiresAt)
            return false;

        // Mark as used and save
        link.IsUsed = true;
        await dbcontext.SaveChangesAsync();

        return true;
    }

    public async Task SaveFormResponseAsync(FormResponse response)
    {
        dbcontext.FormResponses.Add(response);
        await dbcontext.SaveChangesAsync();
    }

    public async Task<List<FormResponse>> GetAllResponsesAsync()
    {
        return await dbcontext.FormResponses
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task<byte[]> GenerateExcelReportAsync()
    {

        var responses = await GetAllResponsesAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Form Responses");

        // Headers
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Full Name";
        worksheet.Cells[1, 3].Value = "Email";
        worksheet.Cells[1, 4].Value = "Phone";
        worksheet.Cells[1, 5].Value = "Company";
        worksheet.Cells[1, 6].Value = "Message";
        worksheet.Cells[1, 7].Value = "Submitted At";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 7])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        // Data
        for (int i = 0; i < responses.Count; i++)
        {
            var row = i + 2;
            var response = responses[i];

            worksheet.Cells[row, 1].Value = response.Id;
            worksheet.Cells[row, 2].Value = response.TUserName;
            worksheet.Cells[row, 3].Value = response.IsWorking;
            worksheet.Cells[row, 7].Value = response.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }

    public async Task CleanupExpiredLinksAsync()
    {
        var expiredLinks = await dbcontext.OneTimeLinks
            .Where(l => DateTime.Now > l.ExpiresAt || l.IsUsed)
            .ToListAsync();

        if (expiredLinks.Any())
        {
            dbcontext.OneTimeLinks.RemoveRange(expiredLinks);
            await dbcontext.SaveChangesAsync();
        }
    }
}

