using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Models;
using Bangazon.Data;
using Bangazon.Models.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Configuration;

namespace Bangazon.Controllers
{
    public class ProductsController : Controller
    {
        // Stores private reference to Identity Framework user manager
        private readonly UserManager<ApplicationUser> _userManager;

        // Stores private reference to EF-created database context
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext ctx,
                                  UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        // This task retrieves the currently authenticated user
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Index()
        {
            // Create new instance of the view model
            ProductListViewModel model = new ProductListViewModel();

            // Set the properties of the view model
            model.Products = await _context.Product.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Detail([FromRoute]int? id)
        {
            // If no id was in the route, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Create new instance of view model
            ProductDetailViewModel model = new ProductDetailViewModel();

            // Set the `Product` property of the view model
            model.Product = await _context.Product
                    .Include(prod => prod.User)
                    .SingleOrDefaultAsync(prod => prod.ProductId == id);

            // If product not found, return 404
            if (model.Product == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Purchase([FromRoute] int id)
        {
            // Find the product requested
            Product productToAdd = await _context.Product.SingleOrDefaultAsync(p => p.ProductId == id);

            // Get the current user
            var user = await GetCurrentUserAsync();

            // Get open order, if exists, otherwise null
            var openOrder = await _context.Order.SingleOrDefaultAsync(o => o.User == user && o.PaymentTypeId == null);

            // Didn't find an open order
            if (openOrder == null)
            {

                // Create new order
                Order newOrder = new Order(){ User = user };
                _context.Add(newOrder);

                // Create new line item
                LineItem li = new LineItem(){
                    Order = newOrder,
                    Product = productToAdd
                };
                _context.Add(li);

            // Open order exists
            } else {

                // Create new line item
                LineItem li = new LineItem(){
                    Order = openOrder,
                    Product = productToAdd
                };
                _context.Add(li);

            }

            // Save all items in the db context
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderController.Index), "Order");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            ProductCreateViewModel model = new ProductCreateViewModel(_context);

            // Get roles assigned to user (should only be one)
            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            // Attach the user role to the view model
            foreach (var role in roles) {
                model.UserRole = role.ToString();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            // Remove the user from the model validation because it is
            // not information posted in the form
            ModelState.Remove("product.User");

            if (ModelState.IsValid)
            {
                /*
                    If all other properties validation, then grab the
                    currently authenticated user and assign it to the
                    product before adding it to the db _context
                */
                var user = await GetCurrentUserAsync();
                product.User = user;

                _context.Add(product);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ProductCreateViewModel model = new ProductCreateViewModel(_context);
            return View(model);
        }

        public async Task<IActionResult> Types()
        {
            var model = new ProductTypesViewModel();

            // Build list of Product instances for display in view
            // LINQ is awesome
            model.GroupedProducts = await (
                from t in _context.ProductType
                join p in _context.Product
                on t.ProductTypeId equals p.ProductTypeId
                group new { t, p } by new { t.ProductTypeId, t.Label } into grouped
                select new GroupedProducts
                {
                    TypeId = grouped.Key.ProductTypeId,
                    TypeName = grouped.Key.Label,
                    ProductCount = grouped.Select(x => x.p.ProductId).Count(),
                    Products = grouped.Select(x => x.p).Take(3)
                }).ToListAsync();

            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
