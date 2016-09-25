using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using eCheck3.Models;

namespace eCheck3.Helpers
{
    public class AccessHelper
    {
        private EMSDataAccessEntities dbAccess = new EMSDataAccessEntities();

        public void UpdateRolesForAllMembersOfGroup(int GroupID)
        {
            //
            // get all members of group, and update roles for each member by all groups they are a member of.
            // Intended for use any time group-role is changed
            //

            //
            // Get list of user-members of group sent
            // 
            List<string> UserNameList;
            UserNameList = (from y in dbAccess.tbAccess_GroupMembership
                         where y.GroupID == GroupID
                         select y.UserID).ToList();
            //
            // Loop through user list, adjust roles for each
            //
            foreach (string UserName in UserNameList) { 
                // Call SPROC to 
                // delete all roles for user
                // and to add distinct roles for user
                // based on the roles of each group user is a member of
                dbAccess.spAccess_UpdateUserRoleByAllGroupRoles(UserName);            
            }
        }

        
        public void UpdateRolesForCommaDelimitedGroupOfUsers(string strUserIDList)
        {
            //
            // Accept a list of userids, and update roles for each user based on group memebership
            // Intended for changes to group membership, or individual user membership/role changes
            //

            // split comma delimited list into array of strings
            string[] userIDs = strUserIDList.Split(',');

            //
            // Loop through user list, adjust roles for each
            //
            foreach (string userID in userIDs)
            {
                // Call SPROC to 
                // delete all roles for user
                // and to add distinct roles for user
                // based on the roles of each group user is a member of
                dbAccess.spAccess_UpdateUserRoleByAllGroupRoles(userID);
            }
        }
    }
}
