using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            ViewBag.Forms = new SelectList(_context.DosageForms, "FormId", "FormName");

            ViewBag.Suppliers = _context.Suppliers
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.Name
                }).ToList();

            ViewBag.Ingredients = _context.ActiveIngredient
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

        // POST: Medicine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine, List<int> selectedIngredients, List<string> strengths)
        {
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
                return RedirectToAction(nameof(Index));
            }

            // Reload form data if model state is invalid
            ViewBag.Forms = new SelectList(_context.DosageForms, "FormId", "FormName");

            ViewBag.Suppliers = _context.Suppliers
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.Name
                }).ToList();

            ViewBag.Ingredients = _context.ActiveIngredient
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

            ViewBag.Ingredients = _context.ActiveIngredient
                .Select(i => new SelectListItem { Value = i.ActiveIngredientId.ToString(), Text = i.Ingredients })
                .ToList();

            ViewBag.Forms = new SelectList(_context.DosageForms, "FormId", "FormName", medicine.FormId);

            ViewBag.Suppliers = _context.Suppliers
                .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.Name })
                .ToList();

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

            ViewBag.Ingredients = _context.ActiveIngredient
                .Select(i => new SelectListItem { Value = i.ActiveIngredientId.ToString(), Text = i.Ingredients })
                .ToList();

            ViewBag.Forms = new SelectList(_context.DosageForms, "FormId", "FormName", medicine.FormId);

            ViewBag.Suppliers = _context.Suppliers
                .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.Name })
                .ToList();

            return View(medicine);
        }


    }
}


