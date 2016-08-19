//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eCheck3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbCompany_Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbCompany_Company()
        {
            this.tbCompany_CompanyModule = new HashSet<tbCompany_CompanyModule>();
        }
    
        public int ID { get; set; }
        public string CompanyName { get; set; }
        public bool AllowSelfRegistration { get; set; }
        public Nullable<int> DefaultGroupID { get; set; }
        public bool AllowTraining { get; set; }
        public bool AllowLive { get; set; }
        public bool DefaultLive { get; set; }
        public int MinPwdLength { get; set; }
        public int PwdComplexityID { get; set; }
        public int PwdExpiryID { get; set; }
        public bool IsActive { get; set; }
    
        public virtual tbSysReference_PwdComplexity tbSysReference_PwdComplexity { get; set; }
        public virtual tbSysReference_PwdExpiry tbSysReference_PwdExpiry { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbCompany_CompanyModule> tbCompany_CompanyModule { get; set; }
    }
}