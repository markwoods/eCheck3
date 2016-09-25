using eCheck3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using eCheck3.Models;
using System.Threading.Tasks;
using System.Security.Claims;



namespace eCheck3.Controllers
{
    public class DevelopmentController : Controller
    {
        private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;

        DevelopmentViewModel DVM = new DevelopmentViewModel();

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

        // GET: DevelopmentHome
        [AllowAnonymous]
        public ActionResult DevelopmentHome() {
            //
            // Set up DD List of available page choices, select whatever the previous was
            //
            // Read Previous Choice from Cookie
            DVM.Choice = "";
            if (Request.Cookies["DevTargetPage"] != null) {
                DVM.Choice = Request.Cookies["DevTargetPage"].Value.ToString();
            }
            DVM.ChoiceList = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "../Home", Text = "Dashboard"},
                new SelectListItem(){ Value = "../Home/About", Text = "About"},
                new SelectListItem(){ Value = "../Home/Contact", Text = "Contact"},
                new SelectListItem(){ Value = "../Admin/Company", Text = "Company"},
                new SelectListItem(){ Value = "../Admin/CompanyEdit/37", Text = "CompanyEdit"},
                new SelectListItem(){ Value = "../Admin/CompanyDetails/37", Text = "CompanyDetails"},
                new SelectListItem(){ Value = "../Admin/Group", Text = "Group" },
                new SelectListItem(){ Value = "../Admin/GroupCreate", Text = "Group Add" },
                new SelectListItem(){ Value = "../Admin/GroupEdit/5", Text = "Group Edit" },
                new SelectListItem(){ Value = "../Admin/GroupEditMembership/5", Text = "Group Membership" },
                new SelectListItem(){ Value = "../Admin/GroupDelete/5", Text = "Group Delete" },
            };
            return View(DVM);
        }

        // POST: DevelopmentHome
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DevelopmentHome(string Choice)
        {
            //
            // Log me on, redirect to selected page, and store selection to use 
            // as default next time
            //

            // logon Me
            var result = await SignInManager.PasswordSignInAsync("markw@criticalcommunication.ca", "W!nter2010", true, false);

            switch (result)
            {
                case SignInStatus.Success:
                    // store specified page
                    ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var user = await userManager.FindByEmailAsync("markw@criticalcommunication.ca");
                    var t = await userManager.AddClaimAsync(user.Id, new Claim("FullName", user.FirstName + " " + user.LastName));
                    var myCookie = new HttpCookie("DevTargetPage", Choice);
                    myCookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(myCookie);
                    // redirect to specified page
                    return RedirectToAction(Choice);
                default:
                    return View();
            }
        }



        // GET: Development/(+)
        [AllowAnonymous]
        //[Authorize(Roles = "canDoDevelopmentTesting")]
        public ActionResult Index(String strPlusMinus)
        {
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            string lnma = user.LastName;
            string ball = user.Id;

            RoleTesting R = new RoleTesting();
            switch (strPlusMinus)
            {
                case "Plus":
                    R.GiveRolesToUser();
                    break;
                case "Minus":
                    R.RemoveRolesFromUser();
                    break;
            }

            return View();
        }


        // GET: Database
        [Authorize(Roles = "canDoDevelopmentTesting")]
        public ActionResult Database()
        {
            return View();
        }
    }
}