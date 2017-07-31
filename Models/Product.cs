using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bangazon.Models
{
  public class Product
  {
    [Key]
    public int ProductId {get;set;}

    [NotMapped]
    public int Quantity { get; set; }


    [Required]
    [DataType(DataType.Date)]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime DateCreated {get;set;}

    [Required]
    [StringLength(255)]
    public string Description { get; set; }

    [Required]
    [StringLength(55)]
    public string Title { get; set; }

    [Required]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public double Price { get; set; }

    [Required]
    public virtual ApplicationUser User { get; set; }

    [Required]
    public int ProductTypeId { get; set; }
    
    [Display(Name="Category")]
    public ProductType ProductType { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Price > 10000)
        {
            yield return new ValidationResult("Please contact our customer service department to sell something of this value.");
        }
    }
  }
}
