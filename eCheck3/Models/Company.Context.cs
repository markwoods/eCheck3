﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EMSDataCompanyEntities : DbContext
    {
        public EMSDataCompanyEntities()
            : base("name=EMSDataCompanyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbCompany_Company> tbCompany_Company { get; set; }
        public virtual DbSet<tbSysReference_PwdComplexity> tbSysReference_PwdComplexity { get; set; }
        public virtual DbSet<tbSysReference_PwdExpiry> tbSysReference_PwdExpiry { get; set; }
        public virtual DbSet<tbCompany_CompanyModule> tbCompany_CompanyModule { get; set; }
        public virtual DbSet<tbSysReference_Module> tbSysReference_Module { get; set; }
    
        public virtual ObjectResult<Nullable<decimal>> spCompany_InsertCompanyReturnID(string companyName, Nullable<bool> allowSelfRegistration, Nullable<bool> allowTraining, Nullable<bool> allowLive, Nullable<bool> defaultLive, Nullable<int> minPwdLength, Nullable<int> pwdComplexityID, Nullable<int> pwdExpiryID)
        {
            var companyNameParameter = companyName != null ?
                new ObjectParameter("CompanyName", companyName) :
                new ObjectParameter("CompanyName", typeof(string));
    
            var allowSelfRegistrationParameter = allowSelfRegistration.HasValue ?
                new ObjectParameter("AllowSelfRegistration", allowSelfRegistration) :
                new ObjectParameter("AllowSelfRegistration", typeof(bool));
    
            var allowTrainingParameter = allowTraining.HasValue ?
                new ObjectParameter("AllowTraining", allowTraining) :
                new ObjectParameter("AllowTraining", typeof(bool));
    
            var allowLiveParameter = allowLive.HasValue ?
                new ObjectParameter("AllowLive", allowLive) :
                new ObjectParameter("AllowLive", typeof(bool));
    
            var defaultLiveParameter = defaultLive.HasValue ?
                new ObjectParameter("DefaultLive", defaultLive) :
                new ObjectParameter("DefaultLive", typeof(bool));
    
            var minPwdLengthParameter = minPwdLength.HasValue ?
                new ObjectParameter("MinPwdLength", minPwdLength) :
                new ObjectParameter("MinPwdLength", typeof(int));
    
            var pwdComplexityIDParameter = pwdComplexityID.HasValue ?
                new ObjectParameter("PwdComplexityID", pwdComplexityID) :
                new ObjectParameter("PwdComplexityID", typeof(int));
    
            var pwdExpiryIDParameter = pwdExpiryID.HasValue ?
                new ObjectParameter("PwdExpiryID", pwdExpiryID) :
                new ObjectParameter("PwdExpiryID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("spCompany_InsertCompanyReturnID", companyNameParameter, allowSelfRegistrationParameter, allowTrainingParameter, allowLiveParameter, defaultLiveParameter, minPwdLengthParameter, pwdComplexityIDParameter, pwdExpiryIDParameter);
        }
    
        public virtual ObjectResult<spCompany_ListCompanyModuleAccess_Result> spCompany_ListCompanyModuleAccess(Nullable<int> companyID)
        {
            var companyIDParameter = companyID.HasValue ?
                new ObjectParameter("CompanyID", companyID) :
                new ObjectParameter("CompanyID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spCompany_ListCompanyModuleAccess_Result>("spCompany_ListCompanyModuleAccess", companyIDParameter);
        }
    }
}
