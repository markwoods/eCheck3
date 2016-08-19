using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Web.Mvc;
using eCheck3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using PagedList;
using eCheck3.Helpers;

namespace eCheck3.Controllers
{
    public class AdminController : Controller
    {
        private EMSDataCompanyEntities dbCompany = new EMSDataCompanyEntities();
        private EMSDataAccessEntities dbAccess = new EMSDataAccessEntities();

        private async Task<string> CreateUser(int CompanyID)
        {
            var user = new ApplicationUser { UserName = "NewAdmin", Email = "new@admin.com" };
            user.CompanyID = CompanyID;
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            IdentityResult result = await userManager.CreateAsync(user, "NewPassword");
            if (result.Succeeded == true)
            {
                return user.Id;
            }
            else {
                return null;
            }
        }
            
            //
        // GET: Admin
        //
        public ActionResult Index()
        {
            return View(); // at this point no view exists
        }

        //
        // GET: Admin/Company
        //
        [Authorize(Roles="canViewMyCompany, canEditMyCompany, canViewAllCompanies, canEditAllCompanies")]
        public ActionResult Company(string sortOrder, string hfSortOrder, string currentFilter, string searchString, int? page, string pageSizeList)
        {
            //
            // Direct user to appropriate page based on roles
            //
            var user = System.Web.HttpContext.Current.User;
            if (!user.IsInRole("canViewAllCompanies") && !user.IsInRole("canEditAllCompanies"))
            {
                // User must be in a view/edit my companies only role, redirect as appropriate
                // Get actual user's company ID
                ApplicationUser AppUser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.User.Identity.GetUserId());
                string strCompanyID = AppUser.CompanyID.ToString();
                // Send to view or edit page as appropriate
                if (user.IsInRole("canViewMyCompany"))
                {
                    // Send User to view page, even if also allowed to edit
                    return RedirectToAction("CompanyDetails/" + strCompanyID, "Admin");
                }
                if (user.IsInRole("canEditMyCompany"))
                {
                    // Send User to edit page
                    return RedirectToAction("CompanyEdit/" + strCompanyID, "Admin");
                }
            }

            // Build page size select list (Default = 10)
            if (String.IsNullOrEmpty(pageSizeList))
            {
                pageSizeList = "10";
            }
            List<SelectListItem> items = ViewHelper.PageSizeList(pageSizeList);
            ViewBag.PageSizeList = items;
            ViewBag.PageSizeListValue = pageSizeList;

            // Get company from model
            var tbCompany_Company = from s in dbCompany.tbCompany_Company.Include(t => t.tbSysReference_PwdComplexity).Include(t => t.tbSysReference_PwdExpiry)
                                    select s;

            // Handle search
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                tbCompany_Company = tbCompany_Company.Where(s => s.CompanyName.Contains(searchString));
            }

            // Handle sort item / order
            if (string.IsNullOrEmpty(sortOrder)) { sortOrder = hfSortOrder; }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ActiveSortParm = sortOrder == "active" ? "active_desc" : "active";
            switch (sortOrder)
            {
                case "name_desc":
                    tbCompany_Company = tbCompany_Company.OrderByDescending(s => s.CompanyName);
                    break;
                case "active_desc":
                    tbCompany_Company = tbCompany_Company.OrderByDescending(s => s.IsActive);
                    break;
                case "active":
                    tbCompany_Company = tbCompany_Company.OrderBy(s => s.IsActive);
                    break;
                default:
                    tbCompany_Company = tbCompany_Company.OrderBy(s => s.CompanyName);
                    break;
            }

            // Set PageSize
           int pageSize = 10;
            switch (pageSizeList)
            {
                case "10":
                    pageSize = 10;
                    break;
                case "50":
                    pageSize = 50;
                    break;
                default:
                    pageSize = tbCompany_Company.Count();
                    break;
            }
            // Set page number
            int pageNumber = (page ?? 1);

            // Return View
            return View(tbCompany_Company.ToPagedList(pageNumber, pageSize));
        


        
        }

        //
        // GET: Admin/CompanyDetails/5
        //
        [Authorize(Roles = "canViewMyCompany, canViewAllCompanies")]
        public ActionResult CompanyDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCompany_Company tbCompany_Company = dbCompany.tbCompany_Company.Find(id);
            if (tbCompany_Company == null)
            {
                return HttpNotFound();
            }
            return View(tbCompany_Company);
        }

        //
        // GET: Admin/CompanyCreate
        //
        [Authorize(Roles = "canEditAllCompanies")]
        public ActionResult CompanyCreate()
        {
            ViewBag.PwdComplexityID = new SelectList(dbCompany.tbSysReference_PwdComplexity, "ID", "PwdComplexityName");
            ViewBag.PwdExpiryID = new SelectList(dbCompany.tbSysReference_PwdExpiry, "ID", "PwdExpiryName");
            //ViewBag.AdminName = "NewAdministrator";
            // Set Defaults
            ext_tbCompany_Company tbCompany_Company = new ext_tbCompany_Company();
            tbCompany_Company.AllowSelfRegistration = true;
            tbCompany_Company.AllowLive = true;
            tbCompany_Company.AllowTraining = true;
            tbCompany_Company.DefaultLive = true;
            tbCompany_Company.MinPwdLength = 4;
            tbCompany_Company.IsActive = true;
            return View(tbCompany_Company);
        }

        //
        // POST: Admin/CompanyCreate
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CompanyCreate([Bind(Include = "NewAdministratorEMail,NewAdministratorName,NewAdministratorPassword,ID,CompanyName,AllowSelfRegistration,DefaultGroupID,AllowTraining,AllowLive,DefaultLive,MinPwdLength,PwdComplexityID,PwdExpiryID,IsActive")] ext_tbCompany_Company ext_tbCompany_Company)
        {
            if (ModelState.IsValid)
            {
                // See if Admin User will work
                // Create default Admin User
                var user = new ApplicationUser { UserName = ext_tbCompany_Company.NewAdministratorName, Email = ext_tbCompany_Company.NewAdministratorEMail, LastName = "Temp", FirstName = "Temp", CompanyID = -1 };
                ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                IdentityResult OKUsername = await userManager.UserValidator.ValidateAsync(user);

                if (OKUsername.Succeeded != true) { 
                    // Admin User won't work
                    //Generate Error, and return To view 
                    StringBuilder sb = new StringBuilder();
                    foreach (string ErrorMessage in OKUsername.Errors) {
                        if (sb.Length > 0) { sb.Append(", "); }
                        sb.Append(ErrorMessage);                   
                    }
                    ModelState.AddModelError("", "Validation of initial Admin User failed.  Error [" + sb.ToString() + "]");
                    ViewBag.PwdComplexityID = new SelectList(dbCompany.tbSysReference_PwdComplexity, "ID", "PwdComplexityName", ext_tbCompany_Company.PwdComplexityID);
                    ViewBag.PwdExpiryID = new SelectList(dbCompany.tbSysReference_PwdExpiry, "ID", "PwdExpiryName", ext_tbCompany_Company.PwdExpiryID);
                    return View(ext_tbCompany_Company);
                }

                // Create Company
                tbCompany_Company tbCompany_Company = new tbCompany_Company();
                tbCompany_Company.AllowLive = ext_tbCompany_Company.AllowLive;
                tbCompany_Company.AllowSelfRegistration = ext_tbCompany_Company.AllowSelfRegistration;
                tbCompany_Company.AllowTraining = ext_tbCompany_Company.AllowTraining;
                tbCompany_Company.CompanyName = ext_tbCompany_Company.CompanyName;
                //tbCompany_Company.DefaultGroupID = ext_tbCompany_Company.DefaultGroupID;
                tbCompany_Company.DefaultLive = ext_tbCompany_Company.DefaultLive;
                tbCompany_Company.IsActive = ext_tbCompany_Company.IsActive;
                tbCompany_Company.MinPwdLength = ext_tbCompany_Company.MinPwdLength;
                tbCompany_Company.PwdComplexityID = ext_tbCompany_Company.PwdComplexityID;
                tbCompany_Company.PwdExpiryID = ext_tbCompany_Company.PwdExpiryID;
                dbCompany.tbCompany_Company.Add(tbCompany_Company);
                dbCompany.SaveChanges();

                // Write ID to debug window
                System.Diagnostics.Debug.WriteLine("New ID: " + ext_tbCompany_Company.ID);

                // Get new Company ID
                int CompanyID = tbCompany_Company.ID;

                // Assign Company to Module 0
                tbCompany_CompanyModule tbCompany_CompanyModule = new tbCompany_CompanyModule();
                tbCompany_CompanyModule.CompanyID = CompanyID;
                tbCompany_CompanyModule.ModuleID = 0;
                dbCompany.tbCompany_CompanyModule.Add(tbCompany_CompanyModule);
                dbCompany.SaveChanges();

                // Create default Administrator Group & Get ID
                tbAccess_Group tbAccess_Group = new tbAccess_Group();
                tbAccess_Group.CompanyID = CompanyID;
                tbAccess_Group.DisplayOrder = 0;
                tbAccess_Group.GroupName = "Administrators";
                tbAccess_Group.IsActive = true;
                tbAccess_Group.GroupDescription = "Administrators";
                dbAccess.tbAccess_Group.Add(tbAccess_Group);
                dbAccess.SaveChanges();

                int GroupID = tbAccess_Group.ID;

                // Add company ID to user, and actually create user
                user.CompanyID = CompanyID;
                IdentityResult result = await userManager.CreateAsync(user, ext_tbCompany_Company.NewAdministratorPassword);

                if (result.Succeeded == true)
                {
                    // Add default user to group
                    tbAccess_GroupMembership tbAccess_GroupMembership = new tbAccess_GroupMembership();
                    tbAccess_GroupMembership.GroupID = GroupID;
                    tbAccess_GroupMembership.UserID = user.Id;
                    dbAccess.tbAccess_GroupMembership.Add(tbAccess_GroupMembership);
                    dbAccess.SaveChanges();

                    // Add group and user administration Roles to new admin User & Group
                    userManager.AddToRoles(user.Id, "canViewMyCompany", "canEditMyCompany", "canViewUserList", "canAddUsers", "canDeleteUsers", "canEditUsers", "canResetPasswords", "canViewGroupList", "canAddGroups", "canDeleteGroups", "canEditGroups", "canEditGroupMembership");
                    dbAccess.spAccess_GroupRole_CopyRolesFromUser(GroupID, user.Id);

                    // Return to main company page
                    return RedirectToAction("Company");
                }
                else { 
                    // Still Didn't succeed - return to wizard page, display failure message
                    StringBuilder sb = new StringBuilder();
                    foreach (string ErrorMessage in OKUsername.Errors)
                    {
                        if (sb.Length > 0) { sb.Append(", "); }
                        sb.Append(ErrorMessage);
                    }
                    ModelState.AddModelError("", "Unable to create initial Admin User.  Error [" + sb.ToString() + "]");
                    ViewBag.PwdComplexityID = new SelectList(dbCompany.tbSysReference_PwdComplexity, "ID", "PwdComplexityName", ext_tbCompany_Company.PwdComplexityID);
                    ViewBag.PwdExpiryID = new SelectList(dbCompany.tbSysReference_PwdExpiry, "ID", "PwdExpiryName", ext_tbCompany_Company.PwdExpiryID);
                    return View(ext_tbCompany_Company);
                }
            }

            ViewBag.PwdComplexityID = new SelectList(dbCompany.tbSysReference_PwdComplexity, "ID", "PwdComplexityName", ext_tbCompany_Company.PwdComplexityID);
            ViewBag.PwdExpiryID = new SelectList(dbCompany.tbSysReference_PwdExpiry, "ID", "PwdExpiryName", ext_tbCompany_Company.PwdExpiryID);
            return View(ext_tbCompany_Company);
        }

        //
        // GET: Admin/CompanyEdit/5
        //
        [Authorize(Roles = "canEditAllCompanies, canEditMyCompany")]
        public ActionResult CompanyEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCompany_Company tbCompany_Company = dbCompany.tbCompany_Company.Find(id);
            if (tbCompany_Company == null)
            {
                return HttpNotFound();
            }
            ViewBag.PwdComplexityID = new SelectList(dbCompany.tbSysReference_PwdComplexity, "ID", "PwdComplexityName", tbCompany_Company.PwdComplexityID);
            ViewBag.PwdExpiryID = new SelectList(dbCompany.tbSysReference_PwdExpiry, "ID", "PwdExpiryName", tbCompany_Company.PwdExpiryID);
            // Get list of active groups by company
            var DefaultGroupID = from s in dbAccess.spAccess_Group_ListGroupsByCompanyID(id)
                                 select s;
            DefaultGroupID = DefaultGroupID.Where(s => s.IsActive.Equals(true));
            ViewBag.VBDefaultGroupID = new SelectList(DefaultGroupID, "ID", "GroupName", tbCompany_Company.DefaultGroupID);
            
            return View(tbCompany_Company);
        }
        //
        // POST: Admin/CompanyEdit/5
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyEdit([Bind(Include = "ID,CompanyName,AllowSelfRegistration,DefaultGroupID,AllowTraining,AllowLive,DefaultLive,MinPwdLength,PwdComplexityID,PwdExpiryID,IsActive")] tbCompany_Company tbCompany_Company)
        {
            if (ModelState.IsValid)
            {
                dbCompany.Entry(tbCompany_Company).State = EntityState.Modified;
                dbCompany.SaveChanges();
                return RedirectToAction("Company");
            }
            ViewBag.PwdComplexityID = new SelectList(dbCompany.tbSysReference_PwdComplexity, "ID", "PwdComplexityName", tbCompany_Company.PwdComplexityID);
            ViewBag.PwdExpiryID = new SelectList(dbCompany.tbSysReference_PwdExpiry, "ID", "PwdExpiryName", tbCompany_Company.PwdExpiryID);
            // Get list of active groups by company
            var DefaultGroupID = from s in dbAccess.spAccess_Group_ListGroupsByCompanyID(tbCompany_Company.ID)
                                 select s;
            DefaultGroupID = DefaultGroupID.Where(s => s.IsActive.Equals(true));
            ViewBag.DefaultGroupID = new SelectList(DefaultGroupID, "ID", "GroupName", tbCompany_Company.DefaultGroupID);
            return View(tbCompany_Company);
        }

        //
        // GET: Admin/CompanyDelete/5
        //
        [Authorize(Roles = "canEditAllCompanies")]
        public ActionResult CompanyDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbCompany_Company tbCompany_Company = dbCompany.tbCompany_Company.Find(id);
            if (tbCompany_Company == null)
            {
                return HttpNotFound();
            }
            return View(tbCompany_Company);
        }

        //
        // POST: Admin/CompanyDelete/5
        //
        [HttpPost, ActionName("CompanyDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbCompany_Company tbCompany_Company = dbCompany.tbCompany_Company.Find(id);
            dbCompany.tbCompany_Company.Remove(tbCompany_Company);
            dbCompany.SaveChanges();
            return RedirectToAction("Company");
        }


        //
        // GET: Admin/Group
        //
        [Authorize(Roles = "canViewMyCompany, canEditMyCompany, canViewAllCompanies, canEditAllCompanies")]
        public ActionResult Group()
        {
            return View(dbAccess.tbAccess_Group.ToList());
        }






        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbCompany.Dispose();
                dbAccess.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
