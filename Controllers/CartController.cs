using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Northwind.Controllers
{
    public class CartController : Controller
    {
        public ActionResult CartList() => View();
    }
}