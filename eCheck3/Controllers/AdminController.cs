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
using eCheck3.ViewModels;
using System.Data.Entity.Core.Objects;

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
            // Get Default Group Name
            if (tbCompany_Company.DefaultGroupID.HasValue)
            {
                tbAccess_Group tbAccessGroup = dbAccess.tbAccess_Group.Find(tbCompany_Company.DefaultGroupID);
                if (tbAccessGroup != null) {
                    ViewBag.DefaultGroupName = tbAccessGroup.GroupName;
                }
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

            // build view model
            CompanyDetailsVM vm = new CompanyDetailsVM();
            vm.tbCompany_Company = dbCompany.tbCompany_Company.Find(id);
            ObjectResult<spCompany_ListCompanyModuleAccess_Result> ModuleAccessResults = dbCompany.spCompany_ListCompanyModuleAccess(id);
            vm.CompanyModuleAccess = ModuleAccessResults.ToList();

            // Get company email list
            var tbCompany_CompanyEmail = from s in dbCompany.tbCompany_CompanyEmail where s.CompanyID == id
                                         select s;
            vm.CompanyEmail = tbCompany_CompanyEmail.ToList();

            // Verify company exists
            if (vm.tbCompany_Company == null)
            {
                return HttpNotFound();
            }

            // build list information for drop down lists
            ViewBag.PwdComplexityID = new SelectList(dbCompany.tbSysReference_PwdComplexity, "ID", "PwdComplexityName", vm.tbCompany_Company.PwdComplexityID);
            ViewBag.PwdExpiryID = new SelectList(dbCompany.tbSysReference_PwdExpiry, "ID", "PwdExpiryName", vm.tbCompany_Company.PwdExpiryID);
            // Get list of active groups by company
            var DefaultGroupID = from s in dbAccess.spAccess_Group_ListGroupsByCompanyID(id) 
                                 where s.IsActive == true
                                 select s;
            ViewBag.VBDefaultGroupID = new SelectList(DefaultGroupID, "ID", "GroupName", vm.tbCompany_Company.DefaultGroupID);
            return View(vm);
        }
        //
        // POST: Admin/CompanyEdit/5
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyEdit(CompanyDetailsVM CompanyDetailsVM)
        {
            if (ModelState.IsValid)
            {
                dbCompany.Entry(CompanyDetailsVM.tbCompany_Company).State = EntityState.Modified;
                //save changes to company information
                dbCompany.SaveChanges();
                // save any changes to module access
                foreach (var item in CompanyDetailsVM.CompanyModuleAccess) { 
                    if (item.HasAccess==true){
                    // Access granted, add or update access and pricing in DB
                        dbCompany.spCompany_CompanyModule_UpdateOrInsert(CompanyDetailsVM.tbCompany_Company.ID, item.ID, item.ModulePrice);
                    }else{
                    // Access denied, delete any access from db
                        dbCompany.spCompany_CompanyModule_Delete(CompanyDetailsVM.tbCompany_Company.ID, item.ID);
                    }
                }
                return RedirectToAction("Company");
            }

            ViewBag.PwdComplexityID = new SelectList(dbCompany.tbSysReference_PwdComplexity, "ID", "PwdComplexityName", CompanyDetailsVM.tbCompany_Company.PwdComplexityID);
            ViewBag.PwdExpiryID = new SelectList(dbCompany.tbSysReference_PwdExpiry, "ID", "PwdExpiryName", CompanyDetailsVM.tbCompany_Company.PwdExpiryID);
            // Get list of active groups by company
            var DefaultGroupID = from s in dbAccess.spAccess_Group_ListGroupsByCompanyID(CompanyDetailsVM.tbCompany_Company.ID)
                                 where s.IsActive == true
                                 select s;
            DefaultGroupID = DefaultGroupID.Where(s => s.IsActive.Equals(true));
            ViewBag.VBDefaultGroupID = new SelectList(DefaultGroupID, "ID", "GroupName", CompanyDetailsVM.tbCompany_Company.DefaultGroupID);
            return View(CompanyDetailsVM);
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
        [Authorize(Roles = "canAddGroups, canDeleteGroups, canEditGroups,canEditGroupMembership")]
        public ActionResult Group(string sortOrder, string hfSortOrder, string currentFilter, string searchString, int? page, string pageSizeList)
        {

            // Build page size select list (Default = 10)
            if (String.IsNullOrEmpty(pageSizeList))
            {
                pageSizeList = "10";
            }
            List<SelectListItem> items = ViewHelper.PageSizeList(pageSizeList);
            ViewBag.PageSizeList = items;
            ViewBag.PageSizeListValue = pageSizeList;

            // Get actual user's company ID
            ApplicationUser AppUser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.User.Identity.GetUserId());
            int intCompanyID = AppUser.CompanyID;

            // Get list of groups from model
            var tbAccess_Group = from s in dbAccess.tbAccess_Group where s.CompanyID == intCompanyID
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
                tbAccess_Group = tbAccess_Group.Where(s => s.GroupName.Contains(searchString));
            }

            // Handle sort item / order
            if (string.IsNullOrEmpty(sortOrder)) { sortOrder = hfSortOrder; }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ActiveSortParm = sortOrder == "active" ? "active_desc" : "active";
            switch (sortOrder)
            {
                case "name_desc":
                    tbAccess_Group = tbAccess_Group.OrderByDescending(s => s.GroupName);
                    break;
                case "active_desc":
                    tbAccess_Group = tbAccess_Group.OrderByDescending(s => s.IsActive);
                    break;
                case "active":
                    tbAccess_Group = tbAccess_Group.OrderBy(s => s.IsActive);
                    break;
                default:
                    tbAccess_Group = tbAccess_Group.OrderBy(s => s.GroupName);
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
                    pageSize = tbAccess_Group.Count();
                    break;
            }
            // Set page number
            int pageNumber = (page ?? 1);

            // Return View
            return View(tbAccess_Group.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: Admin/GroupCreate
        //
        [Authorize(Roles = "canAddGroups")]
        public ActionResult GroupCreate()
        {
            tbAccess_Group tbAccessGroup = new tbAccess_Group();
            // Get logged on user's company ID
            ApplicationUser AppUser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.User.Identity.GetUserId());
            tbAccessGroup.CompanyID = AppUser.CompanyID;
            tbAccessGroup.DisplayOrder = 0;
            tbAccessGroup.IsActive = true;
            return View(tbAccessGroup);
        }

        //
        // POST: Admin/GroupCreate
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupCreate(tbAccess_Group tbAccessGroup)
        {
            if (ModelState.IsValid)
            {
                dbAccess.tbAccess_Group.Add(tbAccessGroup);
                dbAccess.SaveChanges();
                return RedirectToAction("Group");
            }
            return View(tbAccessGroup);
        }
        
        //
        // GET: Admin/GroupDelete/5
        //
        [Authorize(Roles = "canDeleteGroups")]
        public ActionResult GroupDelete(int? id)
        {
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tbAccess_Group tbAccessGroup = dbAccess.tbAccess_Group.Find(id);
                if (tbAccessGroup == null)
                {
                    return HttpNotFound();
                }
                return View(tbAccessGroup);
            }
        }

        //
        // POST: Admin/GroupDelete/5
        //
        [HttpPost, ActionName("GroupDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult GroupDeleteConfirmed(int id)
        {
            tbAccess_Group tbAccessGroup = dbAccess.tbAccess_Group.Find(id);
            dbAccess.tbAccess_Group.Remove(tbAccessGroup);
            dbAccess.SaveChanges();
            return RedirectToAction("Group");
        }

        //
        // GET: Admin/GroupEdit/5
        //
        [Authorize(Roles = "canEditGroups")]
        public ActionResult GroupEdit(int? id)
        {
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Get logged on user's company ID
                ApplicationUser AppUser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.User.Identity.GetUserId());
                // build view model
                GroupDetailsVM vm = new GroupDetailsVM();
                vm.tbAccess_Group = dbAccess.tbAccess_Group.Find(id);
                ObjectResult<spAccess_GroupRoleByCompanyModule_Result> GroupRolesByCompanyModule = dbAccess.spAccess_GroupRoleByCompanyModule(id, AppUser.CompanyID);
                vm.GroupRoleCompanyModuleAccess = GroupRolesByCompanyModule.ToList();

                if (vm.tbAccess_Group == null)
                {
                    return HttpNotFound();
                }
                return View(vm);
            }
        }

        //
        // POST: Admin/GroupEdit/5
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupEdit(GroupDetailsVM vm)
        {
            if (ModelState.IsValid)
            {
                // Save changes to Group details
                dbAccess.Entry(vm.tbAccess_Group).State = EntityState.Modified;
                dbAccess.SaveChanges();

                // Look for changes to group roles, and save if changes made
                // Get logged on user's company ID
                int id = vm.tbAccess_Group.ID;
                ApplicationUser AppUser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.User.Identity.GetUserId());
                // Get original role/access, prior to any edits for comparison
                ObjectResult<spAccess_GroupRoleByCompanyModule_Result> originalGroupRolesByCompanyModule = dbAccess.spAccess_GroupRoleByCompanyModule(id, AppUser.CompanyID);
                List<spAccess_GroupRoleByCompanyModule_Result> originalItem = originalGroupRolesByCompanyModule.ToList();
                bool hasGroupRoleAccessChanged = false;

               // Originals[0].
                int ItemCount = 0;
                foreach (var item in vm.GroupRoleCompanyModuleAccess) {
                    if (originalItem[ItemCount].InRole != item.InRole) { 
                    // Access is changed - save changes
                        hasGroupRoleAccessChanged = true;
                        if (item.InRole == false)
                        {
                            // Remove Group from Role
                            var x = (from y in dbAccess.tbAccess_GroupRole
                                         where y.GroupID == vm.tbAccess_Group.ID
                                         && y.RoleID == item.RoleID
                                         select y).FirstOrDefault();
                            if (x != null)
                            {
                                dbAccess.tbAccess_GroupRole.Remove(x);
                                dbAccess.SaveChanges();
                            }
                        }
                        else { 
                            // Add Group to Role
                            tbAccess_GroupRole tbAccessGroupRole = new tbAccess_GroupRole();
                            tbAccessGroupRole.GroupID = vm.tbAccess_Group.ID;
                            tbAccessGroupRole.RoleID = item.RoleID;
                            dbAccess.tbAccess_GroupRole.Add(tbAccessGroupRole);
                            dbAccess.SaveChanges();                                
                        }
                    }
                    ItemCount++;
                }
                if (hasGroupRoleAccessChanged)
                {
                    // Update all group members to role change
                    AccessHelper AccessHelper = new AccessHelper();
                    AccessHelper.UpdateRolesForAllMembersOfGroup(vm.tbAccess_Group.ID);
                }
                return RedirectToAction("Group");
            }
            return View(vm.tbAccess_Group);
        }

        //
        // GET: Admin/GroupEditMembership/5
        //
        [Authorize(Roles = "canEditGroupMembership")]
        public ActionResult GroupEditMembership(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Get and confirm group exists
            tbAccess_Group tbAccessGroup = dbAccess.tbAccess_Group.Find(id);
            if (tbAccessGroup == null)
            {
                return HttpNotFound();
            }

            // Get logged on user's company ID
            ApplicationUser AppUser = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(HttpContext.User.Identity.GetUserId());
            // build view model
            GroupMembershipVM vm = new GroupMembershipVM();
            ObjectResult<spAccess_GroupMembershipByGroupID_Result> GroupMembershipResult = dbAccess.spAccess_GroupMembershipByGroupID(id);
            vm.MemberList = GroupMembershipResult.ToList();
            ObjectResult<spAccess_UserListByCompanyExclusiveOfGroup_Result> UserListResult = dbAccess.spAccess_UserListByCompanyExclusiveOfGroup(id, AppUser.CompanyID);
            vm.UserList = UserListResult.ToList();
            ViewBag.GroupID = id;
            ViewBag.GroupName = tbAccessGroup.GroupName;
            return View(vm);
        }


        //
        // Class Disposing - get rid of everything
        //
        protected override void Dispose
            (bool disposing)
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
