using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Tdlr.Utils;

namespace Tdlr.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [Authorize]
        public ActionResult Users()
        {
            ViewData["tenant"] = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
            return View();
        }
    }
}