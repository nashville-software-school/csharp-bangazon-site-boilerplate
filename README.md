# Welcome to Bangazon!

## Overview

This version of Bangazon implements the Identity framework, and extends the base User object with the `ApplicationUser` model.
It shows how to remove a model's property from the automatic model binding in a controller method by using `ModelState.Remove()`.
Make sure you look in the `DbInitializer` class to see the product types that are seeded for you.

## Setup

After cloning this repository, use the following commands to get everything installed.

```sh
cd Bangazon
dotnet restore
cp appsettings.json.template appsettings.json
```

Now go back up to the main directory and start Visual Studio with the solution file.

```sh
cd ..
start Bangazon.sln
```

Once your IDE is running, you'll have to update your new `appsettings.json` file with the following content. Update to your SQL Server name.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YourServerHere\\SQLEXPRESS;Database=BangazonSite;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

One of the features you need to implement is a view that displays all of the product types as headers, with the first three products in that type listed beneath it. We are providing you a LINQ statement that will get you started.

Whomever tackles that ticket, this is the method that you will need to add to your `ProductsController.cs`.

```cs
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
```

In addition to that, add the following custom route to the bottom of your `Startup.cs` file.

```cs
routes.MapRoute ("types", "types",
    defaults : new { controller = "Products", action = "Types" });
```

## Removing Items from Model Validation

One of the features you must implement is allowing customers to add products to sell. You'll need to remove the user from model validation to get it to work. Here's an example of something your team will need to do in `Create()` method in **`ProductsController`**.

```cs
// Remove the user from the model validation because it is
// not information posted in the form
ModelState.Remove("product.User");
```

## Seeding the Database

If you want to seed your database with some default values, open the `ApplicationDbContext.cs` file and scroll all the way to the bottom. You will the the following code.

```cs
modelBuilder.Entity<PaymentType> ().HasData (...)
```

The `HasData()` method lets you create one, or more, instances of a database model. Those instances will be turned into `INSERT INTO` SQL statements when you generate a migration either through the Package Manager Console with `Add-Migration MigrationName` or through the command line with `dotnet ef migrations add MigrationName`.

You've been given a user and two payment types for that user to start with. Feel free to generate other users, some product types, products, and orders. Once you defined some, generate a migration and udpate the database.

## Generating the Database

Once your appsettings are updated and you've entered in some seed data, you should generate your database.

### From Visual Studio

1. Go to the Package Manager Console in Visual Studio.
1. Use the `Add-Migration BangazonTables` command.
1. Once Visual Studio shows you the migration file, execute `Update-Database` to generate your tables.
1. Use the SQL Server Object Explorer to verify that everything worked as expected.

### From Command Line

```sh
cd Bangazon
dotnet ef migrations add BangazonTables -o Data/Migrations
dotnet ef database update
```
