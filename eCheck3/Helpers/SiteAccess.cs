using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eCheck3.Models;

namespace WebRole1.Helpers
{
    //
    // Alter Web.Config in Views Section:
    // Remove <pages pageBaseType="System.Web.Mvc.WebViewPage">
    // And replace with <pages pageBaseType="WebRole1.Helpers.ApplicationViewPage">

    public abstract class ApplicationViewPage<T> : WebViewPage<T>
    {
        protected override void InitializePage()
        {
            SetViewBagMenuAccessProperties();
            base.InitializePage();
        }

        private void SetViewBagMenuAccessProperties()
        {
            //
            // Build Menu Access based on user roles/permissions
            //

            //
            // AdministrationGroup
            //
            bool IsAuthorizedAtLeastOnceInGroup = false;
            if (User.IsInRole("canViewAllCompanies") || User.IsInRole("canEditAllCompanies") || User.IsInRole("canViewMyCompany") || User.IsInRole("canEditMyCompany"))
            {
                ViewBag.MenuLink_Administration_Company = "true";
                IsAuthorizedAtLeastOnceInGroup = true;
            }
            if (User.IsInRole("canViewGroupList") || User.IsInRole("canAddGroups") || User.IsInRole("canDeleteGroups") || User.IsInRole("canEditGroups") || User.IsInRole("canEditGroupsMembership"))
            {
                ViewBag.MenuLink_Administration_Group = "true";
                IsAuthorizedAtLeastOnceInGroup = true;
            }
            if (IsAuthorizedAtLeastOnceInGroup)
            {
                ViewBag.MenuLink_Administration = "true";
            }

            // temporarily authorize anon for development
            //ViewBag.MenuLink_Administration_Company = "true";
            //ViewBag.MenuLink_Administration = "true";
            
            //
            // DevelopmentGroup
            //
            IsAuthorizedAtLeastOnceInGroup = false;

            if (Request.IsLocal == true)
            {
                ViewBag.MenuLink_Local = "true";
                IsAuthorizedAtLeastOnceInGroup = true;
            }

            if (User.IsInRole("canDoDevelopmentTesting")) 
            {
                IsAuthorizedAtLeastOnceInGroup = true;
                ViewBag.MenuLink_Development_Testing = "true";
            }

            if (IsAuthorizedAtLeastOnceInGroup)
            {
                ViewBag.MenuLink_Development = "true";
            }

            // temporarily authorize anon for development
            //ViewBag.MenuLink_Development = "true";

        }
    }

    public class SiteAccess
    {

    }
}