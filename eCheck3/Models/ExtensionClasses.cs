using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace eCheck3.Models
{
    public partial class ext_tbCompany_Company : tbCompany_Company {

        public virtual string NewAdministratorName { get; set; }
        public virtual string NewAdministratorEMail { get; set; }
        public virtual string NewAdministratorPassword { get; set; }    
    }

    //public class ext_CompanyModuleAccess {

    //    public virtual Int32 ID { get; set; }
    //    public virtual bool HasAccess { get; set; }
    //    public virtual decimal ModulePrice { get; set; }
    //    public virtual string ModuleName { get; set; }
    //    public virtual string ModuleDescription { get; set; }
    //    public virtual string ModulePriceQualifier { get; set; }
    //}

    public static class GenericPrincipalExtensions
    {
        public static string FullName(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {

                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.Type == "FullName")
                            return claim.Value;
                    }
                }
                return "";
            }
            else
                return "";
        }
    }

    public class ExtensionClasses
    {
    }
}