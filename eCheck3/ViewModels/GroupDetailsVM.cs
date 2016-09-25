using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eCheck3.Models;

namespace eCheck3.ViewModels
{
    public class GroupDetailsVM
    {
        public tbAccess_Group tbAccess_Group { get; set; }
        public List<spAccess_GroupRoleByCompanyModule_Result> GroupRoleCompanyModuleAccess { get; set; }
    }

    public class GroupMembershipVM
    {
        public List<spAccess_GroupMembershipByGroupID_Result> MemberList { get; set; }
        public List<spAccess_UserListByCompanyExclusiveOfGroup_Result> UserList { get; set; }
    }
}