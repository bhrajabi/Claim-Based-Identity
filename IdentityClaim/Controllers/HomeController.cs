using IdentityClaim.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace IdentityClaim.Controllers
{
    public class HomeController : Controller
    {
        #region User Manager
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }
        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion


        public ActionResult Index()
        {
            return View();
        }

        [ClaimsAuthorize(claimType: "Permission", claimValue: "Home.About")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [ClaimsAuthorize(claimType: "Permission", claimValue: "Home.Permissions")]
        [HttpGet]
        public ActionResult Permissions(string userId = null)
        {

            Assembly asm = Assembly.GetAssembly(typeof(MvcApplication));

            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type) || typeof(AsyncController).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                    .Select(x => new { Controller = x.DeclaringType.Name.Replace("Controller", ""), Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();


            var List = controlleractionlist.Where(c => c.ReturnType.Equals("ActionResult")).GroupBy(c => c.Controller);

            var model = new PermissionViewModel();

            foreach (var controller in List)
            {
                List<CheckPermision> actions = new List<CheckPermision>();
                foreach (var item in controller)
                {
                    bool check = false;
                    if (!String.IsNullOrEmpty(userId) && UserManager.GetClaims(userId).SingleOrDefault(c => c.Value == controller.Key + "." + item.Action) != null)
                        check = true;

                    actions.Add(new CheckPermision { Permission = item.Action, Allow = check,Type= item.Attributes.Contains("HttpGet") ? "HttpGet" : "" });
                }

                model.Dic.Add(controller.Key, actions);
            }
            var db = new ApplicationDbContext();

            ViewData["UserId"] = new SelectList(db.Users, "Id", "Email");

            return View(model);
        }

        [ClaimsAuthorize(claimType: "Permission", claimValue: "Home.Permissions")]
        [HttpPost]
        public ActionResult Permissions(PermissionViewModel model)
        {
            foreach (var item in UserManager.GetClaims(model.UserId))
                UserManager.RemoveClaim(model.UserId, item);

            for (int i = 0; i < model.Values.Count; i++)
            {
                if (UserManager.GetClaims(model.UserId).SingleOrDefault(c => c.Type.Equals("Permission") && c.Value.Equals(model.Values[i])) == null)
                    UserManager.AddClaim(model.UserId, new Claim("Permission", model.Values[i]));
            }

            return RedirectToAction("Permissions");
        }

    }
}