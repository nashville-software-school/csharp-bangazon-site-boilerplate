using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Bangazon.Models.OrderViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Bangazon.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private ApplicationDbContext _context;
        public OrderController(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Order
        [Authorize]
        public async Task<IActionResult> Index()
        {
            OrderDetailViewModel model = new OrderDetailViewModel();
            
            var user = await GetCurrentUserAsync();

            // TODO: Find out why this is bringing back ALL orders
            model.Order = await _context.Order
                .Where(o => o.User == user)
                .SingleOrDefaultAsync();

            return View(model);
        }

        // POST: Order/Cancel
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lineItems = await _context.LineItem.Where(l => l.OrderId == id).ToListAsync();

            // TODO: Remove all line items from the order

            return RedirectToAction(nameof(ProductsController.Index), "Products");
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
