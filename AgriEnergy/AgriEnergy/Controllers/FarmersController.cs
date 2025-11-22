using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgriEnergy.Data;
using AgriEnergy.Models;
using Microsoft.AspNetCore.Authorization;

namespace AgriEnergy.Controllers
{
    // This controller is restricted to users in the "Employee" role
    [Authorize(Roles = "Employee")]
    public class FarmersController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor that injects the application's database context
        public FarmersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Farmers
        // Retrieves and displays a list of all farmers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Farmers.ToListAsync());
        }

        // GET: Farmers/Details/5
        // Retrieves and displays detailed information for a single farmer
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Returns 404 if no ID is provided
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (farmer == null)
            {
                return NotFound(); // Returns 404 if the farmer is not found
            }

            return View(farmer); // Passes the farmer to the view
        }

        // GET: Farmers/Create
        // Displays the form to create a new farmer
        public IActionResult Create()
        {
            return View();
        }

        // POST: Farmers/Create
        // Handles form submission for creating a new farmer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FarmerName,FarmerLastName,PhoneNumber,Address,City,EmailAddress")] Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(farmer); // Adds the new farmer to the context
                await _context.SaveChangesAsync(); // Saves the new farmer to the database
                return RedirectToAction(nameof(Index)); // Redirects to the list of farmers
            }
            return View(farmer); // If validation fails, redisplay the form
        }

        // GET: Farmers/Edit/5
        // Retrieves a farmer's data for editing
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Returns 404 if no ID is provided
            }

            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                return NotFound(); // Returns 404 if the farmer is not found
            }
            return View(farmer); // Passes the farmer to the edit view
        }

        // POST: Farmers/Edit/5
        // Handles form submission for editing a farmer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FarmerName,FarmerLastName,PhoneNumber,Address,City,EmailAddress")] Farmer farmer)
        {
            if (id != farmer.Id)
            {
                return NotFound(); // Returns 404 if the route ID does not match the farmer's ID
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmer); // Updates the farmer's data
                    await _context.SaveChangesAsync(); // Saves changes to the database
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerExists(farmer.Id))
                    {
                        return NotFound(); // If farmer no longer exists, return 404
                    }
                    else
                    {
                        throw; // Otherwise, rethrow the exception
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirects to the list of farmers
            }
            return View(farmer); // If validation fails, redisplay the form
        }

        // GET: Farmers/Delete/5
        // Displays the confirmation page for deleting a farmer
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Returns 404 if no ID is provided
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (farmer == null)
            {
                return NotFound(); // Returns 404 if the farmer is not found
            }

            return View(farmer); // Passes the farmer to the delete view
        }

        // POST: Farmers/Delete/5
        // Handles the confirmation to delete a farmer
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer != null)
            {
                _context.Farmers.Remove(farmer); // Removes the farmer from the context
            }

            await _context.SaveChangesAsync(); // Saves the deletion to the database
            return RedirectToAction(nameof(Index)); // Redirects to the list of farmers
        }

        // Checks whether a farmer with the given ID exists
        private bool FarmerExists(int id)
        {
            return _context.Farmers.Any(e => e.Id == id);
        }
    }
}