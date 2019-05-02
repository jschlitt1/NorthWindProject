using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Models;

namespace Northwind.Controllers
{
    public class CartController : Controller
    {
        // this controller depends on the NorthwindRepository
        private INorthwindRepository repository;
        public CartController(INorthwindRepository repo) => repository = repo;

        public ActionResult CartList()
        {
            int customerId = repository.Customers.FirstOrDefault(c => c.Email == User.Identity.Name).CustomerID;
            return View(repository.CartItems.Include("Product").Where(u => u.CustomerId == customerId));
        }

        //[HttpPost]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    AppUser user = await userManager.FindByIdAsync(id);
        //    if (user != null)
        //    {
        //        IdentityResult result = await userManager.DeleteAsync(user);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            AddErrorsFromResult(result);
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "User Not Found");
        //    }
        //    return View("Index", userManager.Users);
        //}
    }
}