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
    
    public partial class tbSysReference_Module
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbSysReference_Module()
        {
            this.tbCompany_CompanyModule = new HashSet<tbCompany_CompanyModule>();
        }
    
        public int ID { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbCompany_CompanyModule> tbCompany_CompanyModule { get; set; }
    }
}
