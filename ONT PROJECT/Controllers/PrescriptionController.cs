using Microsoft.AspNetCore.Mvc;
using ONT_PROJECT.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

public class PrescriptionController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _context;

    public PrescriptionController(IWebHostEnvironment env, ApplicationDbContext context)
    {
        _env = env;
        _context = context;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        int customerId = GetLoggedInCustomerId();
        if (customerId == 0)
        {
            // Not logged in or invalid user, redirect to login or error
            return RedirectToAction("Login", "CustomerRegister");
        }

        var prescriptions = _context.UnprocessedPrescriptions
            .Where(p => p.CustomerId == customerId)
            .ToList();
        return View(prescriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile PrescriptionFile)
    {
        int customerId = GetLoggedInCustomerId();
        if (customerId == 0)
            return RedirectToAction("Login", "CustomerRegister");

        if (PrescriptionFile != null && PrescriptionFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await PrescriptionFile.CopyToAsync(memoryStream);

            bool isRequested = Request.Form["RequestDispense"].ToString().Contains("true");

            var prescription = new UnprocessedPrescription
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                CustomerId = customerId,
                PrescriptionPhoto = memoryStream.ToArray(),
                Dispense = isRequested ? "Requested" : "Unpocessed",
                Status = isRequested ? "Unprocessed" : "Unprocessed"
            };

            _context.UnprocessedPrescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            // ✅ Set TempData flag to show success message
            TempData["UploadSuccess"] = true;
        }

        return RedirectToAction("Upload");
    }

    [HttpPost]
    public async Task<IActionResult> RequestDispenseAction(int id)
    {
        var prescription = await _context.UnprocessedPrescriptions.FindAsync(id);
        if (prescription != null && prescription.Status == "Unprocessed")
        {
            prescription.Status = "Requested";
            prescription.Dispense = "Requested";
            _context.Update(prescription);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Upload");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var prescription = await _context.UnprocessedPrescriptions.FindAsync(id);
        if (prescription != null)
        {
            _context.UnprocessedPrescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            TempData["DeleteSuccess"] = "Prescription deleted successfully!";
        }

        return RedirectToAction("Upload");
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var prescription = await _context.UnprocessedPrescriptions.FindAsync(id);
        if (prescription == null) return NotFound();
        return View(prescription); // For modal, we won't use this GET view anymore
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, IFormFile PrescriptionFile, string Dispense)
    {
        var prescription = await _context.UnprocessedPrescriptions.FindAsync(id);
        if (prescription == null) return NotFound();

        if (PrescriptionFile != null && PrescriptionFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await PrescriptionFile.CopyToAsync(ms);
            prescription.PrescriptionPhoto = ms.ToArray();
        }

        if (!string.IsNullOrEmpty(Dispense))
        {
            prescription.Dispense = Dispense;
            prescription.Status = Dispense;
        }

        _context.Update(prescription);
        await _context.SaveChangesAsync();

        return RedirectToAction("Upload");
    }

    public async Task<IActionResult> ViewPdf(int id)
    {
        var prescription = await _context.UnprocessedPrescriptions.FindAsync(id);
        if (prescription == null || prescription.PrescriptionPhoto == null)
        {
            return NotFound();
        }

        return File(prescription.PrescriptionPhoto, "application/pdf");
    }

    private int GetLoggedInCustomerId()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        return userId ?? 0;
    }
}
