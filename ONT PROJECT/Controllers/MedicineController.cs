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
            var medicines = await _context.Medicines.ToListAsync();
            return View(medicines);
        }

        public IActionResult Create()
        {
            ViewBag.Ingredients = _context.ActiveIngredient
                .Select(i => new SelectListItem { Value = i.ActiveIngredientId.ToString(), Text = i.Ingredients })
                .ToList();

            ViewBag.Forms = _context.DosageForms
                .Select(f => new SelectListItem { Value = f.FormId.ToString(), Text = f.FormName })
                .ToList();

            ViewBag.Suppliers = _context.Suppliers
                .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.Name })
                .ToList();

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine)
        {
            Console.WriteLine($"Medicine.FormId = {medicine.FormId}");
            Console.WriteLine($"Medicine.Schedule = {medicine.Schedule}");
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"ModelState error on {key}: {error.ErrorMessage}");
                }
            }


            if (!ModelState.IsValid)
            {
                
                ViewBag.Ingredients = _context.ActiveIngredient
                    .Select(i => new SelectListItem { Value = i.ActiveIngredientId.ToString(), Text = i.Ingredients })
                    .ToList();

                ViewBag.Forms = _context.DosageForms
                    .Select(f => new SelectListItem { Value = f.FormId.ToString(), Text = f.FormName })
                    .ToList();

                ViewBag.Suppliers = _context.Suppliers
                    .Select(s => new SelectListItem { Value = s.SupplierId.ToString(), Text = s.Name })
                    .ToList();

                return View(medicine);
            }

            _context.Add(medicine);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medicine added successfully.";
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
                return NotFound();

            return View(medicine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine medicine)
        {
            if (id != medicine.MedicineId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicine);
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
            return View(medicine);
        }
    }
}
