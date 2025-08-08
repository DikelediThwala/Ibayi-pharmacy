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

        var prescriptions = _context.Prescriptions
            .Where(p => p.CustomerId == customerId)
            .ToList();
        return View(prescriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile PrescriptionFile, bool RequestDispense)
    {
        int customerId = GetLoggedInCustomerId();
        if (customerId == 0)
        {
            return RedirectToAction("Login", "CustomerRegister");
        }

        if (PrescriptionFile != null && PrescriptionFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await PrescriptionFile.CopyToAsync(memoryStream);

            var prescription = new Prescription
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                CustomerId = customerId,
                DoctorId = 1, // TODO: get real doctor ID if applicable
                PharmacistId = 1, // TODO: get real pharmacist ID if applicable
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
    public async Task<IActionResult> ViewPdf(int id)
    {
        var prescription = await _context.Prescriptions.FindAsync(id);
        if (prescription == null || prescription.PrescriptionPhoto == null)
        {
            return NotFound();
        }

        return File(prescription.PrescriptionPhoto, "application/pdf");
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
        // Safely get UserId from session, return 0 if not found
        int? userId = HttpContext.Session.GetInt32("UserId");
        return userId ?? 0;
    }




    // =======================
    // PRESCRIPTION LINE ACTIONS
    // =======================

    // GET: Prescription/Lines/5
    public IActionResult Manage(int prescriptionId)
    {
        var prescription = _context.Prescriptions
            .Include(p => p.PrescriptionLines)
            .ThenInclude(pl => pl.Medicine)
            .FirstOrDefault(p => p.PrescriptionId == prescriptionId);

        if (prescription == null)
            return NotFound();

        ViewBag.PrescriptionId = prescriptionId;
        ViewBag.Medicines = _context.Medicines.ToList(); // <-- Add this

        return View(prescription);
    }


    [HttpPost]
    public IActionResult AddLine(PrescriptionLine line)
    {
        if (ModelState.IsValid)
        {
            _context.PrescriptionLines.Add(line);
            _context.SaveChanges();
        }
        return RedirectToAction("Manage", new { prescriptionId = line.PrescriptionId });
    }

    [HttpPost]
    public IActionResult EditLine(PrescriptionLine line)
    {
        if (ModelState.IsValid)
        {
            _context.PrescriptionLines.Update(line);
            _context.SaveChanges();
        }
        return RedirectToAction("Manage", new { prescriptionId = line.PrescriptionId });
    }

    [HttpPost]
    public IActionResult DeleteLine(int id, int prescriptionId)
    {
        var line = _context.PrescriptionLines.Find(id);
        if (line != null)
        {
            _context.PrescriptionLines.Remove(line);
            _context.SaveChanges();
        }
        return RedirectToAction("Manage", new { prescriptionId });
    }

}
