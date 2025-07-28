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
        // Get logged-in customer ID from session or identity
        int customerId = GetLoggedInCustomerId();
        var prescriptions = _context.Prescriptions
            .Where(p => p.CustomerId == customerId)
            .ToList();
        return View(prescriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile PrescriptionFile, bool RequestDispense)
    {
        if (PrescriptionFile != null && PrescriptionFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await PrescriptionFile.CopyToAsync(memoryStream);

            var prescription = new Prescription
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                CustomerId = GetLoggedInCustomerId(),
                DoctorId = 1, // Set default or get from session
                PharmacistId = 1, // Set default or assign later
                PrescriptionPhoto = memoryStream.ToArray(),
                Status = RequestDispense ? "Requested" : "Uploaded"
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Upload");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var prescription = await _context.Prescriptions.FindAsync(id);
        if (prescription != null)
        {
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Upload");
    }

    [HttpPost]
    public async Task<IActionResult> Request(int id)
    {
        var prescription = await _context.Prescriptions.FindAsync(id);
        if (prescription != null && prescription.Status == "Uploaded")
        {
            prescription.Status = "Requested";
            _context.Update(prescription);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Upload");
    }

    private int GetLoggedInCustomerId()
    {
        // Replace with actual logic to get logged-in customer's ID
        return int.Parse(HttpContext.User.Identity?.Name ?? "0");
    }
}
