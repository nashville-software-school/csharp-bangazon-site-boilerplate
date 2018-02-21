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
    public class OrderCountViewModel
    {
        public int OrderCount { get; set; } = 0;
    }

    /*
        ViewComponent for displaying the shopping cart link & cart
        widget in the navigation bar.
     */
    public class OrderCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
 
        public OrderCountViewComponent(ApplicationDbContext c, UserManager<ApplicationUser> userManager)
        {
            _context = c;
            _userManager = userManager;
        }
 
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get the current, authenticated user
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            // Instantiate view model
            OrderCountViewModel model = new OrderCountViewModel();

            // Determine if there is an active order
            var order = await _context.Order
                .Include("LineItems.Product")
                .Where(o => o.User == user && o.PaymentType == null)
                .SingleOrDefaultAsync()
                ;

            // If there is an open order, query appropriate values
            if (order != null)
            {
                model.OrderCount = order.LineItems.Count;
            }

            // Render template bound to OrderCountViewModel
            return View(model);
        }
    }
}
