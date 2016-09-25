using eCheck3.Helpers;
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
        private EMSDataAccessEntities dbAccess = new EMSDataAccessEntities();

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

        public void GroupMembershipPost(string strGroupID, string strUserIDList){
           //
            // Accept a comma delimited string of usernames, parse it and
            // add it to group membership for the group ID sent
            //

            // split comma delimited list into array of strings
            string[] userIDs = strUserIDList.Split(',');

            // Convert GroupID to Integer
            int intGroupID = int.Parse(strGroupID);

            tbAccess_GroupMembership tbAccess_GroupMembership;

            // loop through each user id
            foreach (string userID in userIDs) {
                //add user to group membership table
                tbAccess_GroupMembership = new tbAccess_GroupMembership();
                tbAccess_GroupMembership.GroupID = intGroupID;
                tbAccess_GroupMembership.UserID = userID;
                dbAccess.tbAccess_GroupMembership.Add(tbAccess_GroupMembership);
                dbAccess.SaveChanges();
            }
            // Call function to update roles for users impacted by this change
            AccessHelper AccessHelper = new AccessHelper();
            AccessHelper.UpdateRolesForCommaDelimitedGroupOfUsers(strUserIDList);

        }

        public void GroupMembershipDelete(string strGroupID, string strUserIDList) {
            //
            // Accept a comma delimited string of usernames, parse it and
            // delete it from group membership for the group ID sent
            //

            // split comma delimited list into array of strings
            string[] userIDs = strUserIDList.Split(',');

            // Convert GroupID to Integer
            int intGroupID = int.Parse(strGroupID);

            // loop through each user id
            foreach (string userID in userIDs)
            {
                // find the group membership record
                var x = (from y in dbAccess.tbAccess_GroupMembership
                         where y.GroupID == intGroupID
                         && y.UserID == userID
                         select y).FirstOrDefault();
                // if record found, delete it
                if (x != null)
                {
                    dbAccess.tbAccess_GroupMembership.Remove(x);
                    dbAccess.SaveChanges();
                }        
            }
            // Call function to update roles for users impacted by this change
            AccessHelper AccessHelper = new AccessHelper();
            AccessHelper.UpdateRolesForCommaDelimitedGroupOfUsers(strUserIDList);

        }
    
    }
}
