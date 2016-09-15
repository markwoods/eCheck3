using eCheck3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace eCheck3.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Administration" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Administration.svc or Administration.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Administration : IAdministration
    {
        private EMSDataCompanyEntities dbCompany = new EMSDataCompanyEntities();

        public void CompanySelfRegistrationDelete(string strCompanyID, string strEmailSuffix)
        {
            //
            // Delete email suffix from self-registration list for company
            //
            int intCompanyID = int.Parse(strCompanyID);
            var x = (from y in dbCompany.tbCompany_CompanyEmail
                     where y.CompanyID == intCompanyID
                     && y.EmailSuffix == strEmailSuffix
                      select y).FirstOrDefault();

            if (x != null) {
                dbCompany.tbCompany_CompanyEmail.Remove(x);
                dbCompany.SaveChanges();
            }
        
        }

        public void CompanySelfRegistrationPost(string strCompanyID, string strEmailSuffix)
        {
            //
            // Add email suffix from self-registration list for company
            //
            tbCompany_CompanyEmail tbCompanyEmail = new tbCompany_CompanyEmail();
            tbCompanyEmail.CompanyID = int.Parse(strCompanyID);
            tbCompanyEmail.EmailSuffix = strEmailSuffix;
            dbCompany.tbCompany_CompanyEmail.Add(tbCompanyEmail);
            dbCompany.SaveChanges();
        }
    }
}
