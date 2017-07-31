# Welcome to Bangazon!

## This application consists of:

*   Sample pages using ASP.NET Core MVC and Identity framework
*   [Bower](https://go.microsoft.com/fwlink/?LinkId=518004) for managing client-side libraries
*   Theming using [Bootstrap](https://go.microsoft.com/fwlink/?LinkID=398939)

## Overview

This version of Bangazon implements the Identity framework, and extends the base User object with the `ApplicationUser` model. 
It shows how to remove a model's property from the automatic model binding in a controller method by using `ModelState.Remove()`. 
Make sure you look in the `DbInitializer` class to see the product types that are seeded for you.

Want to see how to group table results together? Check out the `ProductsController.Types()` method.

This project was built in Microsoft [Visual Studio Code](https://code.visualstudio.com/).

## Run & Deploy

1. Clone this repo
1. `cd` to new directory
1. Run following commands
    
    ```
    dotnet restore
    dotnet ef database udpdate
    dotnet run
    ```