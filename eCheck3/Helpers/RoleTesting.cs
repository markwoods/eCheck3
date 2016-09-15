using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using eCheck3.Models;

namespace eCheck3.Helpers
{
    public class RoleTesting
    {

        public void GiveRolesToUser() {


            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationSignInManager signInManager = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            ApplicationUser user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.Current.User.Identity.GetUserId());

            //  Here's how to get UserID from current logged on user

            // Here's how to get a list of roles by user
            //IList<string> mylist = userManager.GetRoles(user.Id);


            userManager.AddToRole(user.Id, "canViewAllCompanies");
            userManager.AddToRole(user.Id, "canEditAllCompanies");
            userManager.AddToRole(user.Id, "canViewMyCompany");
            userManager.AddToRole(user.Id, "canEditMyCompany");
            userManager.AddToRole(user.Id, "canDoDevelopmentTesting");
            userManager.AddToRole(user.Id, "canViewGroupList");
            userManager.AddToRole(user.Id, "canAddGroups");
            userManager.AddToRole(user.Id, "canDeleteGroups");
            userManager.AddToRole(user.Id, "canEditGroups");
            userManager.AddToRole(user.Id, "canEditGroupMembership");

            // Here's how to re-sign in current user to make role changes immediate
            signInManager.SignIn(user, true, true);

            //userManager.RemoveFromRole(user.Id, "canEditAllCompanies");




        }

        public void RemoveRolesFromUser()
        {


            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationSignInManager signInManager = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            ApplicationUser user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.Current.User.Identity.GetUserId());

            //  Here's how to get UserID from current logged on user

            // Here's how to get a list of roles by user
            //IList<string> mylist = userManager.GetRoles(user.Id);


            userManager.RemoveFromRole(user.Id, "canViewAllCompanies");
            userManager.RemoveFromRole(user.Id, "canEditAllCompanies");
            userManager.RemoveFromRole(user.Id, "canViewMyCompany");
            //userManager.RemoveFromRole(user.Id, "canEditMyCompany");
            //userManager.RemoveFromRole(user.Id, "canDoDevelopmentTesting");

            //userManager.RemoveFromRole(user.Id, "canDoDevelopmentTesting");

            // Here's how to re-sign in current user to make role changes immediate
            signInManager.SignIn(user, true, true);


        }

    }
}