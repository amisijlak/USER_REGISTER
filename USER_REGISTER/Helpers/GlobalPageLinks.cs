using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_REGISTER
{
    public static class GlobalPageLinks
    {
        private static string GetLink(string suffix) => $"~/{suffix}";
        /// <summary>
        /// Homepage
        /// </summary>
        public static string Index => GetLink("Index");
        public static string AfterLoginIndex => GetLink("Profile/Index");

        public static string AccountLogin => GetLink("Account/Login");
        public static string AccountLockout => GetLink("Account/Lockout");
        public static string AccountLogout => GetLink("Account/Logout");
        public static string AccountUserRegister => GetLink("Account/UserRegister");
        public static string AccountCreateUser => GetLink("Account/CreateUser");
        public static string AccountEditUser => GetLink("Account/EditUser");
        public static string AccountLoginConfirmationCode => GetLink("Account/LoginConfirmationCode");
        public static string AccountSystemUsers => GetLink("Account/SystemUsers");


        public static string ProfilesUpdate => GetLink("Profile/Update");
        public static string ProfilesViewProfile => GetLink("Profile/ViewProfile");
        public static string ProfileIndex => GetLink("Profile/Index");
        public static string ProfileExport => GetLink("Profile/PrintView");


        public static string UserRegisterIndex => GetLink("Account/UserRegister");

        public static string ProfileKeyCustomerTemplate => GetLink("Profile/Update?handler=CustomerTemplate");
        public static string ProfileKeySupplierTemplate => GetLink("Profile/Update?handler=SupplierTemplate");
        public static string ProfileKeyEnergySourcesChallengeTemplate => GetLink("Profile/Update?handler=EnergySourcesChallengeTemplate");
        public static string ProfileKeyEntityTemplate => GetLink("Profile/Update?handler=EntityTemplate");
        public static string ProfileKeyEroblemsTemplate => GetLink("Profile/Update?handler=ProblemsTemplate");
        public static string ProfileKeyIssuesTemplate => GetLink("Profile/Update?handler=IssuesTemplate");
        public static string ProfileKeyCostTemplate => GetLink("Profile/Update?handler=CostTemplate");
        public static string ProfileKeyChallengeTemplate => GetLink("Profile/Update?handler=ChallengeTemplate");
        public static string ProfileKeyTrainingTemplate => GetLink("Profile/Update?handler=TrainingTemplate");
        public static string ProfileKeyUiaServiceTemplate => GetLink("Profile/Update?handler=UiaServiceTemplate");
        public static string ProfileKeyEnergyUsedTemplate => GetLink("Profile/Update?handler=UsedEnergyTemplate");
        public static string ProfileApprovalSubmit => GetLink("Profile/ApprovalSubmit");
        public static string ProfileViewProfile => GetLink("Profile/ViewProfile");
        public static string ProfileUpdateSubmit => GetLink("Profile/Update?handler=Submit");

        public static string ProfileUpdateGetItemId => GetLink("Profile/Update?handler=ItemId");
        public static string LookupsUpdate => GetLink("Lookups/Update");
        public static string LookupsIndex => GetLink("Lookups/Index");       
        public static string LookupsList => GetLink("Lookups/Index?handler=Lookups");
        public static string LookupsDelete => GetLink("Lookups/Delete");
        public static string LookupsMapping => GetLink("Lookups/Mapping");
        public static string LookupsMappingTemplate => GetLink("Lookups/Mapping?handler=MappingTemplate");
        public static string LookupsGetItemId => GetLink("Lookups/Update?handler=ItemId");
        public static string LookupsParentIdTemplate => GetLink("Lookups/Update?handler=ParentId");
    }
}
