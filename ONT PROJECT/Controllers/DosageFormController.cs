using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;

namespace ONT_PROJECT.Controllers
{
    public class DosageFormController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DosageFormController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var forms = await _context.DosageForms.ToListAsync();
            return View(forms);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DosageForm form)
        {
            if (form == null)
                return BadRequest();

            ////Console.WriteLine($"Received form.Form: '{form.FormName}'");

            if (ModelState.IsValid)
            {
                bool exists = _context.DosageForms
                   .Any(d => d.FormName.ToLower() == form.FormName.ToLower());


                if (exists)
                {
                    TempData["ErrorMessage"] = "Form already exists in the system!";
                    return RedirectToAction(nameof(Create));
                }

                _context.Add(form);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dosage form added successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"ModelState Error for {state.Key}: {error.ErrorMessage}");
                    }
                }
            }

            return View(form);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var form = await _context.DosageForms.FindAsync(id);
            if (form == null)
                return NotFound();

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DosageForm form)
        {
            if (id != form.FormId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(form);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dosage form updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var form = await _context.DosageForms.FindAsync(id);
            if (form != null)
            {
                _context.DosageForms.Remove(form);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dosage form deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
