using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace OIDC3.Controllers
{
    public class RedirectController : Controller
    {
        public ActionResult Redirect(string param1, string param2)
        {
            string m1 = param1;
            string m2 = param2;
           
            return View();
        }


    }
    
}