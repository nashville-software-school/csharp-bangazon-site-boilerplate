    # Welcome to Bangazon!

## Overview

This version of Bangazon implements the Identity framework, and extends the base User object with the `ApplicationUser` model.
It shows how to remove a model's property from the automatic model binding in a controller method by using `ModelState.Remove()`.

## Setup


> **Pick one person from your team to follow these steps. No one else should touch anything at this point.**

### Option 1: Github Classroom

If your instructor chooses to use Github Classroom, you will all be given an invitation link.

1. The chosen person will click the link and create the team.
2. Once that process is complete and the repository is created, everyone else joins their team.
1. Notify your instructor that the repository is created so that your issue tickets can be generated.
1. The chosen person will clone the team's repository to their machine. **No one else should**.
1. Create a branch named `initial-setup`.
1. Open Visual Studio and load the solution file
1. Move on to the **Seeding the Database** section.

### Option 2: Custom Organizations

1. Clone this repository to your machine.
1. Create a new repository on your team's Github organization named `BangazonSite`.
1. Copy the connection string for your repo.
1. From your project directory, execute the following commands
    ```sh
    git remote remove origin
    git remote add origin <paste your new Github URL here>
    ```
1. Push up the master branch to your new remote origin
1. Create a branch named `initial-setup`.
1. Open Visual Studio and load the solution file
1. Move on to the **Seeding the Database** section.

### Seeding the Database

You will want to seed your database with some default values. Open the `ApplicationDbContext.cs` file and scroll all the way to the bottom. You will find the following code.

```cs
modelBuilder.Entity<PaymentType> ().HasData (...)
```

The `HasData()` method lets you create one, or more, instances of a database model. Those instances will be turned into `INSERT INTO` SQL statements when you generate a migration either through the Package Manager Console with

```
Add-Migration MigrationName
```

The boilerplate project has existing seed data. Review that code with your team and if the team decides that they want more seeded data, add the new objects now.

### Generating the Database

Once your appsettings are updated and you're satisfied with your seed data, you should generate your database.

1. Go to the Package Manager Console in Visual Studio.
1. Use the `Add-Migration BangazonTables` command.
1. Once Visual Studio shows you the migration file, execute `Update-Database` to generate your tables.
1. Connect to your database to verify that all tables were created and data was added as expected.

### Submit a PR

Push up your branch and submit a PR that your team lead will review and approve. No one else on the team can merge this PR until your team lead approves it.

## Setup for Everyone

Once the initial setup is completed by the volunteer and the PR is approved by your team lead, the PR will get merged into master and now everyone else can pull the repository.

1. Open Visual Studio and load the solution file
1. Go to the Package Manager Console in Visual Studio.
1. Execute `Update-Database` to generate your tables.
1. Connect to your database to verify that all tables were created and data was added as expected.

## References for Tickets

### Accessing the Authenticated User

In any of your controllers that need to access the currently authenticated user, for example the Order Summary screen or the New Product Form, you need to inject the `UserManager` into the controller. Here's the relevant code that you need.

Add a private field to your controller.

```cs
private readonly UserManager<ApplicationUser> _userManager;
```

In the constructor, inject the `UserManager` service.

```cs
public ProductsController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
{
    _userManager = userManager;
    _context = ctx;
}
```

Then add the following private method to the controller.

```cs
private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
```

Once that is defined, any method that needs to see who the user is can invoke the method. Here's an example of when someone clicks the Purchase button for a product.

```cs
[Authorize]
public async Task<IActionResult> Purchase([FromRoute] int id)
{
    // Find the product requested
    Product productToAdd = await _context.Product.SingleOrDefaultAsync(p => p.ProductId == id);

    // Get the current user
    var user = await GetCurrentUserAsync();

    // See if the user has an open order
    var openOrder = await _context.Order.SingleOrDefaultAsync(o => o.User == user && o.PaymentTypeId == null);


    // If no order, create one, else add to existing order
}
```

### Grouped Products by Product Type

One of the features you need to implement is a view that displays all of the product types as headers, with the first three products in that type listed beneath it. We are providing you a LINQ statement that will get you started.

Whomever tackles that ticket, this is the method that you will need to add to your controller.

```cs

public async Task<IActionResult> Types()
{
    var model = new ProductTypesViewModel();

    model.Types = await _context
        .ProductType
        .Select(pt => new TypeWithProducts()
        {
            TypeId = pt.ProductTypeId,
            TypeName = pt.Label,
            ProductCount = pt.Products.Count(),
            Products = pt.Products.OrderByDescending(p => p.DateCreated).Take(3)
        }).ToListAsync();

    return View(model);
}
```

## Removing Items from Model Validation

One of the features you must implement is allowing customers to add products to sell. You'll need to remove the user from model validation to get it to work. Here's an example of something your team will need to do in `Create()` method in **`ProductsController`**.

```cs
// Remove the user from the model validation because it is
// not information posted in the form
ModelState.Remove("product.User");
```