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
    
    public partial class tbAccess_Group
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbAccess_Group()
        {
            this.tbAccess_GroupMembership = new HashSet<tbAccess_GroupMembership>();
            this.tbAccess_GroupRole = new HashSet<tbAccess_GroupRole>();
        }
    
        public int ID { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int CompanyID { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbAccess_GroupMembership> tbAccess_GroupMembership { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbAccess_GroupRole> tbAccess_GroupRole { get; set; }
    }
}
