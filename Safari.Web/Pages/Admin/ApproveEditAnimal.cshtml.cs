﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Safari.Data;

namespace Safari.Web.Pages.Admin
{
    public class EditModel : PageModel
    {
        private readonly WildlifeDataContext _context;
        private readonly IWildlifeRepository _repository;

        public EditModel(WildlifeDataContext context, IWildlifeRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

        [BindProperty]
        public AnimalDescription AnimalDescription { get; set; } = default!;

        private async Task<IActionResult> RepopulateViewDataAsync(int? id)
        {
            var animal = await _context.Animal.FirstOrDefaultAsync(m => m.AnimalId == id);
            if (animal == null)
            {
                return NotFound();
            }
            Animal = animal;
            ViewData["AnimalTypeId"] = new SelectList(
                               _context.AnimalType, "AnimalTypeId", "Name");
            ViewData["DietTypeId"] = new SelectList(
                               _context.DietType, "DietTypeId", "Name");
            return Page();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Animal == null)
            {
                return NotFound();
            }

            await RepopulateViewDataAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await RepopulateViewDataAsync(Animal.AnimalId);
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _repository.UpdateAnimalAsync(Animal);
                await _repository.UpdateAnimalDescriptionAsync(AnimalDescription);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                await transaction.RollbackAsync();
                await RepopulateViewDataAsync(Animal.AnimalId);
                return Page();
            }

            await transaction.CommitAsync();
            TempData["SuccessMessage"] = "Animal updated successfully!";
            await RepopulateViewDataAsync(Animal.AnimalId);
            return Page();
        }
    }
}
