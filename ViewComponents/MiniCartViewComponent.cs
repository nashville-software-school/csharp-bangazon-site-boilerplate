using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.OrderViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.ViewComponents
{
    public class MiniCartViewModel
    {
        public IEnumerable<OrderLineItem> LineItems { get; set; } = new List<OrderLineItem>();
    }

    /*
        ViewComponent for displaying the cart widget in the navigation bar.
     */
    public class MiniCartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
 
        public MiniCartViewComponent(ApplicationDbContext c, UserManager<ApplicationUser> userManager)
        {
            _context = c;
            _userManager = userManager;
        }
 
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get the current, authenticated user
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            // Instantiate view model
            MiniCartViewModel model = new MiniCartViewModel();

            // Determine if there is an active order
            var order = _context.Order
                .Include("LineItems.Product")
                .Where(o => o.User == user && o.PaymentType == null);

            // If there is an open order, query appropriate values
            if (order != null)
            {
                model.LineItems = await order
                    .SelectMany(o => o.LineItems)
                    .GroupBy(l => l.Product)
                    .Select(g => new OrderLineItem {
                        Product = g.Key,
                        Units = g.Select(l => l.ProductId).Count(),
                        Cost = g.Key.Price * g.Select(l => l.ProductId).Count()
                    }).ToListAsync()
                    ;
            }

            // Render template bound to MiniCartViewModel
            return View(model);
        }
    }
}