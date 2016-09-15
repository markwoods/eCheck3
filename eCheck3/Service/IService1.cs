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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "DoSomething")]
        void DoWork();
 
        //            <WebInvoke(method:="GET", UriTemplate:="CompletionReportBasic?dt_StartDate={dt_StartDate}&dt_EndDate={dt_EndDate}&int_PrimaryFilter={int_PrimaryFilter}&bln_PriSec={bln_PriSec}&str_PrimaryFilterData={str_PrimaryFilterData}&str_CheckSheets={str_CheckSheets}")> _

        //      <WebInvoke(Method:="GET")>        
    }
}
