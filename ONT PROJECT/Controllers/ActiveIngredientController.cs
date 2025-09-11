using IBayiLibrary.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Helpers;
using ONT_PROJECT.Models;
using System.Linq;

namespace ONT_PROJECT.Controllers
{
    public class ActiveIngredientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActiveIngredientController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activeIngredients = await _context.ActiveIngredient
                .Where(a => a.Status == "Active")
                .ToListAsync();
            var deactivatedIngredients = await _context.ActiveIngredient
                .Where(a => a.Status == "Inactive")
                .ToListAsync();

            ViewBag.DeactivatedIngredients = deactivatedIngredients;
            return View(activeIngredients);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ActiveIngredient ingredient)
        {
            if (ModelState.IsValid)
            {

                bool exists = _context.ActiveIngredient
           .Any(a => a.Ingredients.ToLower() == ingredient.Ingredients.ToLower());

                if (exists)
                {
                    TempData["ErrorMessage"] = "Ingredient already exists in the system!";
                    return RedirectToAction(nameof(Create));
                }

                _context.Add(ingredient);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ingredient added successfully.";

                ActivityLogger.LogActivity(_context, "Create Ingredient", $"Ingredient {ingredient.Ingredients} added.");

                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var ingredient = _context.ActiveIngredient.Find(id);
            if (ingredient == null) return NotFound();

            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ActiveIngredient ingredient)
        {
            if (id != ingredient.ActiveIngredientId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingredient);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Ingredient updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(ingredient);
                }
            }
            return View(ingredient);
        }


        [HttpPost]
        public IActionResult Deactivate(int id)
        {
            var ingredient = _context.ActiveIngredient.FirstOrDefault(a => a.ActiveIngredientId == id);
            if (ingredient == null) return NotFound();

            ingredient.Status = "Inactive";
            _context.SaveChanges();
            TempData["SuccessMessage"] = $"Ingredient '{ingredient.Ingredients}' deactivated.";

            ActivityLogger.LogActivity(_context, "Deactivate Ingredient", $"Ingredient {ingredient.Ingredients} was deactivated.");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Activate(int id)
        {
            var ingredient = _context.ActiveIngredient.FirstOrDefault(a => a.ActiveIngredientId == id);
            if (ingredient == null) return NotFound();

            ingredient.Status = "Active";
            _context.SaveChanges();
            TempData["SuccessMessage"] = $"Ingredient '{ingredient.Ingredients}' activated.";

            ActivityLogger.LogActivity(_context, "Activate Ingredient", $"Ingredient {ingredient.Ingredients} was activated.");

            return RedirectToAction(nameof(Index));
        }

    }
}
