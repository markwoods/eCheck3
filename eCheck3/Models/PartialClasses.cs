using System;
using System.ComponentModel.DataAnnotations;

//
// Link Metadata class which includes validation and labeling to database integration class
//
namespace eCheck3.Models
{
    [MetadataType(typeof(ext_tbCompany_CompanyMetaData))]
    public partial class ext_tbCompany_Company
    {
    }

    [MetadataType(typeof(tbCompany_CompanyMetaData))]
    public partial class tbCompany_Company
    {
    }

    [MetadataType(typeof(tbSysReference_PwdComplexityMetaData))]
    public partial class tbSysReference_PwdComplexity
    {
    }

    [MetadataType(typeof(tbSysReference_PwdExpiryMetaData))]
    public partial class tbSysReference_PwdExpiry
    {
    }

    [MetadataType(typeof(spCompany_ListCompanyModuleAccessMetaData))]
    public partial class spCompany_ListCompanyModuleAccess_Result
    {
    }
}