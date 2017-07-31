using System.Collections.Generic;
using Bangazon.Models;
using Bangazon.Data;

namespace Bangazon.Models.ProductViewModels
{
  public class ProductTypeDetailViewModel
  {
    public ProductType ProductType { get; set; }
    public List<Product> Products { get; set; }
  }
}