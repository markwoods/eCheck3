using System;
using System.ComponentModel.DataAnnotations;

namespace eCheck3.Models
{
    public class ext_tbCompany_CompanyMetaData
    {
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName;
        [Display(Name = "Allow Self-Registration")]
        public bool AllowSelfRegistration;
        [Display(Name = "Default Group for Self-Registration")]
        public int DefaultGroupID;
        [Display(Name = "Allow Training Mode")]
        public bool AllowTraining;
        [Display(Name = "Allow Live Mode")]
        public bool AllowLive;
        [Display(Name = "Default to Live Mode")]
        public bool DefaultLive;
        [Display(Name = "Password Complexity")]
        public int PwdComplexityID;
        [Display(Name = "Password Expiry")]
        public int PwdExpiryID;
        [Display(Name = "Active")]
        public bool IsActive;
        [Range(4, 10, ErrorMessage = "Password Mininum Length must be between 4 and 10")]
        [Display(Name = "Minimum Password Length")]
        public int MinPwdLength;
        [Required]
        [Display(Name = "New Administrator User Name")]
        public string NewAdministratorName;
        [Required]
        [MinLength(6, ErrorMessage = "Minimum password length is 6 characters.")]
        [Display(Name = "New Administrator Temporary Password")]
        public string NewAdministratorPassword;
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "New Administrator eMail")]
        public string NewAdministratorEMail;

    }
    public class tbCompany_CompanyMetaData
    {
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName;
        [Display(Name = "Allow Self-Registration")]
        public bool AllowSelfRegistration;
        [Display(Name = "Default Group for Self-Registration")]
        public int DefaultGroupID;
        [Display(Name = "Allow Training Mode")]
        public bool AllowTraining;
        [Display(Name = "Allow Live Mode")]
        public bool AllowLive;
        [Display(Name = "Default to Live Mode")]
        public bool DefaultLive;
        [Display(Name = "Password Complexity")]
        public int PwdComplexityID;
        [Display(Name = "Password Expiry")]
        public int PwdExpiryID;
        [Display(Name = "Active")]
        public bool IsActive;
        [Range(4, 10, ErrorMessage = "Password Mininum Length must be between 4 and 10")]
        [Display(Name = "Minimum Password Length")]
        public int MinPwdLength;

    }
    public class tbSysReference_PwdComplexityMetaData
    {
        public int ID;
        [Display(Name = "Password Complexity")]
        public string PwdComplexityName;
    }

    public class tbSysReference_PwdExpiryMetaData
    {
        public int ID;
        [Display(Name = "Password Expiry")]
        public string PwdExpiryName;
    }

    public class spCompany_ListCompanyModuleAccessMetaData
    {
        [RegularExpression(@"\d*\.[0-9]{2}", ErrorMessage = "Price must be a valid price [$x.xx]")]
        public decimal ModulePrice;
    }
}