using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;


namespace eCheck3.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAdministration" in both code and config file together.
    [ServiceContract]
    public interface IAdministration
    {
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "Company/{strCompanyID}/SelfRegistration?strEmailSuffix={strEmailSuffix}")]
        void CompanySelfRegistrationDelete(string strCompanyID, string strEmailSuffix);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Company/{strCompanyID}/SelfRegistration?strEmailSuffix={strEmailSuffix}")]
        void CompanySelfRegistrationPost(string strCompanyID, string strEmailSuffix);
    }
}
