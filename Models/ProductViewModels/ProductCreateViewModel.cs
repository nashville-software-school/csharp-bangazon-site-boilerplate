using System.Collections.Generic;
using System.Linq;
using Bangazon.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bangazon.Models.ProductViewModels
{
  public class ProductCreateViewModel
  {
    public List<SelectListItem> ProductTypes { get; set; }
    public Product Product { get; set; }

    public string UserRole { get; set; }
    public ProductCreateViewModel(ApplicationDbContext ctx) 
    { 

        this.ProductTypes = ctx.ProductType
                                .OrderBy(l => l.Label)
                                .AsEnumerable()
                                .Select(li => new SelectListItem { 
                                    Text = li.Label,
                                    Value = li.ProductTypeId.ToString()
                                }).ToList();

        this.ProductTypes.Insert(0, new SelectListItem { 
            Text = "Choose category...",
            Value = "0"
        }); 
    }
  }
}