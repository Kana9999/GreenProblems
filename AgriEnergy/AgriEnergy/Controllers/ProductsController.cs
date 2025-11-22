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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor that injects the database context
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display only the products that belong to the currently logged-in farmer
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Index1()
        {
            string currentFarmer = User.Identity.Name;

            var farmerProducts = await _context.Products
                .Where(p => p.FarmerName == currentFarmer)
                .ToListAsync();

            return View(farmerProducts); // Return farmer-specific product list
        }

        // Display all products with optional filtering by search string, category, and date range
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index(string searchString, string category, DateTime? startDate, DateTime? endDate)
        {
            var products = _context.Products.AsQueryable();

            // Filter by search keyword in farmer name or product name
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p =>
                    p.FarmerName.Contains(searchString) ||
                    p.ProductName.Contains(searchString));
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.category.Contains(category));
            }

            // Filter by start date
            if (startDate.HasValue)
            {
                products = products.Where(p => p.DateTime >= startDate.Value);
            }

            // Filter by end date
            if (endDate.HasValue)
            {
                products = products.Where(p => p.DateTime <= endDate.Value);
            }

            // Preserve filters for form repopulation
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = category;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View("Index", await products.ToListAsync());
        }

        // Show details of a specific product by ID
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID not provided
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            return View(product);
        }

        // Display the product creation form
        public IActionResult Create()
        {
            return View();
        }

        // Handle product creation submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create([Bind("Id,ProductName,category,DateTime")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.FarmerName = User.Identity.Name; // Assign current logged-in farmer
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index1)); // Redirect to farmer's product list
            }

            // Pass validation errors to view
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(product);
        }

        // Display the product editing form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID not provided
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            return View(product);
        }

        // Handle product update submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FarmerName,ProductName,category,DateTime")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound(); // ID mismatch
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency conflict
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index)); // Redirect to full product list
            }

            return View(product); // Show validation errors
        }

        // Display the confirmation page for deleting a product
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // ID not provided
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            return View(product);
        }

        // Handle product deletion confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product); // Remove product from database
            }

            await _context.SaveChangesAsync(); // Save changes
            return RedirectToAction(nameof(Index)); // Redirect to product list
        }

        // Check if a product with the given ID exists in the database
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}