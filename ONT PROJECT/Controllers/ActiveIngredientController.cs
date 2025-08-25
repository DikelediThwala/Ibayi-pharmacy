using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var ingredients = _context.ActiveIngredient.ToList();
            return View(ingredients);
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
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var ingredient = _context.ActiveIngredient.Find(id);
            if (ingredient != null)
            {
                _context.ActiveIngredient.Remove(ingredient);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ingredient deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
