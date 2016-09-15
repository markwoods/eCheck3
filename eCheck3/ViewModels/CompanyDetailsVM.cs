using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eCheck3.Models;

namespace eCheck3.ViewModels
{
    public class CompanyDetailsVM
    {
        public tbCompany_Company tbCompany_Company { get; set; }
        public List<spCompany_ListCompanyModuleAccess_Result> CompanyModuleAccess { get; set; }
        public List<tbCompany_CompanyEmail> CompanyEmail { get; set; }
    }
}