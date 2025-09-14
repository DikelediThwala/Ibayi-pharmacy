using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Helpers;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class MedicineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicineController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var medicines = await _context.Medicines
                .Include(m => m.Form)
                .Include(m => m.MedIngredients)
                    .ThenInclude(mi => mi.ActiveIngredient)
                .ToListAsync();

            return View(medicines);
        }

        public IActionResult Create()
        {
            ViewBag.Forms = new SelectList(
                    _context.DosageForms
                     .Where(f => f.Status == "Active") 
                    .OrderBy(f => f.FormName),
                    "FormId",
                    "FormName"
                );

            ViewBag.Suppliers = _context.Suppliers
                .Where(s => s.Status == "Active")
                 .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.Name
                }).ToList();

            ViewBag.Ingredients = _context.ActiveIngredient
                 .Where(ai => ai.Status == "Active")
                .OrderBy(i => i.Ingredients)
                .Select(ai => new SelectListItem
                {
                    Value = ai.ActiveIngredientId.ToString(),
                    Text = ai.Ingredients
                }).ToList();

            ViewBag.ScheduleList = Enumerable.Range(1, 8)
                .Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = $"Schedule {i}"
                }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine, List<int> selectedIngredients, List<string> strengths)
        {
            if (_context.Medicines.Any(m => m.MedicineName == medicine.MedicineName))
            {
                TempData["ErrorMessage"] = "This medicine already exists!";

                ViewBag.Forms = new SelectList(_context.DosageForms.OrderBy(f => f.FormName), "FormId", "FormName");

                ViewBag.Suppliers = _context.Suppliers.OrderBy(s => s.Name)
                    .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.Name }).ToList();

                ViewBag.Ingredients = _context.ActiveIngredient.OrderBy(i => i.Ingredients)
                    .Select(ai => new SelectListItem { Value = ai.ActiveIngredientId.ToString(), Text = ai.Ingredients }).ToList();

                ViewBag.ScheduleList = Enumerable.Range(1, 8)
                    .Select(i => new SelectListItem { Value = i.ToString(), Text = $"Schedule {i}" }).ToList();

                return View(medicine);
            }

            if (ModelState.IsValid)
            {
                _context.Add(medicine);
                await _context.SaveChangesAsync();

                for (int i = 0; i < selectedIngredients.Count; i++)
                {
                    var medIngredient = new MedIngredient
                    {
                        MedicineId = medicine.MedicineId,
                        ActiveIngredientId = selectedIngredients[i],
                        Strength = strengths[i]
                    };
                    _context.MedIngredients.Add(medIngredient);
                }

                await _context.SaveChangesAsync();

                ActivityLogger.LogActivity(_context, "Create Medicine", $"Medicine {medicine.MedicineName} was added.");
 
                // Set success message
                TempData["SuccessMessage"] = "Medicine created successfully!";
                return RedirectToAction(nameof(Index));
            }

           
            ViewBag.Forms = new SelectList(
                              _context.DosageForms.OrderBy(f => f.FormName),
                              "FormId",
                              "FormName"
                          );

            ViewBag.Suppliers = _context.Suppliers
     .Where(s => s.Status == "Active")
     .OrderBy(s => s.Name)
     .Select(s => new SelectListItem
     {
         Value = s.SupplierId.ToString(),
         Text = s.Name,
         Selected = s.SupplierId == medicine.SupplierId
     }).ToList();




            ViewBag.Ingredients = _context.ActiveIngredient
                 .OrderBy(i => i.Ingredients)
                .Select(ai => new SelectListItem
                {
                    Value = ai.ActiveIngredientId.ToString(),
                    Text = ai.Ingredients
                }).ToList();

            ViewBag.ScheduleList = Enumerable.Range(1, 8)
                .Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = $"Schedule {i}"
                }).ToList();

            return View(medicine);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var medicine = await _context.Medicines
                .Include(m => m.MedIngredients)
                    .ThenInclude(mi => mi.ActiveIngredient)
                .FirstOrDefaultAsync(m => m.MedicineId == id);

            if (medicine == null)
                return NotFound();

            // Only active ingredients
            ViewBag.Ingredients = _context.ActiveIngredient
                .Where(ai => ai.Status == "Active")
                .OrderBy(i => i.Ingredients)
                .Select(i => new SelectListItem
                {
                    Value = i.ActiveIngredientId.ToString(),
                    Text = i.Ingredients
                }).ToList();

            // Only active dosage forms
            ViewBag.Forms = new SelectList(
                _context.DosageForms
                    .Where(f => f.Status == "Active")
                    .OrderBy(f => f.FormName),
                "FormId",
                "FormName"
            );

            // Only active suppliers
            ViewBag.Suppliers = _context.Suppliers
                .Where(s => s.Status == "Active")
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.Name
                }).ToList();

            return View(medicine);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine medicine, int[] selectedIngredientIds, List<string> strengths)
        {
            if (id != medicine.MedicineId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicine);
                    await _context.SaveChangesAsync();

                    // Remove old ingredients
                    var existingIngredients = _context.MedIngredients.Where(mi => mi.MedicineId == id);
                    _context.MedIngredients.RemoveRange(existingIngredients);

                    // Add selected ingredients with correct strengths
                    if (selectedIngredientIds != null && selectedIngredientIds.Any())
                    {
                        for (int i = 0; i < selectedIngredientIds.Length; i++)
                        {
                            var medIngredient = new MedIngredient
                            {
                                MedicineId = id,
                                ActiveIngredientId = selectedIngredientIds[i],
                                Strength = (strengths != null && strengths.Count > i) ? strengths[i] : "N/A"
                            };
                            _context.MedIngredients.Add(medIngredient);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Medicine updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Medicines.Any(e => e.MedicineId == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.Ingredients = _context.ActiveIngredient.OrderBy(i => i.Ingredients)
                .Select(i => new SelectListItem { Value = i.ActiveIngredientId.ToString(), Text = i.Ingredients })
                .ToList();

            ViewBag.Forms = new SelectList(
                                         _context.DosageForms.OrderBy(f => f.FormName),
                                         "FormId",
                                         "FormName"
                                     );
            ViewBag.Suppliers = _context.Suppliers
                 .OrderBy(s => s.Name)
                .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.Name })
                .ToList();

            return View(medicine);
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();

            medicine.Status = "Deactivated";
            _context.Update(medicine);
            await _context.SaveChangesAsync();

            ActivityLogger.LogActivity(_context, "Deactivate Medicine", $"Medicine {medicine.MedicineName} was deactivated.");


            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();

            medicine.Status = "Active";
            _context.Update(medicine);
            await _context.SaveChangesAsync();

            ActivityLogger.LogActivity(_context, "Activate Medicine", $"Medicine {medicine.MedicineName} was activated.");

            return Json(new { success = true });
        }

    }
}


