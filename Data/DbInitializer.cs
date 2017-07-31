using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bangazon.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any products.
                if (context.ProductType.Any())
                {
                    return;   // DB has been seeded
                }

                var productTypes = new ProductType[]
                {
                    new ProductType { 
                        Label = "Electronics"
                    },
                    new ProductType { 
                        Label = "Appliances"
                    },
                    new ProductType { 
                        Label = "Housewares"
                    },
                };

                foreach (ProductType i in productTypes)
                {
                    context.ProductType.Add(i);
                }
                context.SaveChanges();
            }
       }
    }
}