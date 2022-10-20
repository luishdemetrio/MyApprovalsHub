using MyApprovalsHub.Common;
using MyApprovalsHub.Interfaces;
using MyApprovalsHub.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Concurrent;

namespace MyApprovalsHub.Services;

public class ServiceNowService : IPendingApprovalService
{
    private string _serviceNowInstanceUrl;
    private string _serviceNowUsername;
    private string _serviceNowPassword;
    private string _serviceNowClientId;
    private string _serviceNowClientSecret;

    private static Dictionary<int, string> _impacts ;

    public string InstanceUrl {
        get {
            return _serviceNowInstanceUrl;
        }
        private set { 
        } 
    }

    public ServiceNowService(ApprovalsHubOptions config)
    {

        _serviceNowInstanceUrl = config.ServiceNowBaseUrl;
        _serviceNowUsername = config.ServiceNowUsername;
        _serviceNowPassword = config.ServiceNowPassword;

        _serviceNowClientId = config.ServiceNowClientId;
        _serviceNowClientSecret = config.ServiceNowClientSecret;

        if (string.IsNullOrWhiteSpace(_serviceNowInstanceUrl))
        {
            throw new Exception($"{nameof(_serviceNowInstanceUrl)} must be set.");
        }

        if (string.IsNullOrWhiteSpace(_serviceNowUsername))
        {
            throw new Exception($"{nameof(_serviceNowUsername)} must be set.");
        }

        if (string.IsNullOrWhiteSpace(_serviceNowPassword))
        {
            throw new Exception($"{nameof(_serviceNowPassword)} must be set.");
        }

        if (string.IsNullOrWhiteSpace(_serviceNowClientId))
        {
            throw new Exception($"{nameof(_serviceNowClientId)} must be set.");
        }

        if (string.IsNullOrWhiteSpace(_serviceNowClientSecret))
        {
            throw new Exception($"{nameof(_serviceNowClientSecret)} must be set.");
        }

        PopulateImpacts();
    }

    private void PopulateImpacts()
    {
        //for now it is hardcoded

        _impacts = new();

        _impacts.Add(1, "High");
        _impacts.Add(2, "Medium");
        _impacts.Add(3, "Low");

    }

    private string GetToken()
    {
        string token = string.Empty;

        var client = new RestClient($"{_serviceNowInstanceUrl}/oauth_token.do");
        var request = new RestRequest();
        request.Method = Method.Post;

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("client_id", _serviceNowClientId);
        request.AddParameter("client_secret", _serviceNowClientSecret);
        request.AddParameter("grant_type", "password");
        request.AddParameter("username", _serviceNowUsername);
        request.AddParameter("password", _serviceNowPassword);

        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            if (dic != null && dic.ContainsKey("access_token") == true)
            {
                token = dic["access_token"];
            }
        }

        return token;
    }

    //API
    //https://dev52648.service-now.com/api/now/table/sys_user?sysparm_query=email%3Dluke.wilson%40example.com&sysparm_fields=sys_id&sysparm_limit=1
    //RETURN
    //"{\"result\":[{\"sys_id\":\"46d96f57a9fe198101947a9620895886\"}]}"
    private string GetServiceNowUser(string email)
    {
        string sys_id = string.Empty;

        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sys_user?sysparm_query=email={email}&sysparm_fields=sys_id&sysparm_limit=1");

        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(response.Content)!;

            if (dynamicObject.result.Count > 0)
            {
                var item = dynamicObject.result[0].sys_id;
                sys_id = item;
            }
            // else
            // {
            //user not found
            //}
        }

        return sys_id;
    }

    //API
    //https://dev52648.service-now.com/api/now/table/sysapproval_approver?approver=46d96f57a9fe198101947a9620895886&state=requested&sysparm_exclude_reference_link=true&sysparm_fields=state%2Csys_created_by%2Csysapproval%2Csys_updated_by
    //RETURN
    /*     
    {
    "result": [
        {
            "sysapproval": "31cd7552db252200a6a2b31be0b8f55c",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "41cdb152db252200a6a2b31be0b8f527",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "02cd7552db252200a6a2b31be0b8f5e0",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "66cdf552db252200a6a2b31be0b8f559",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "29cdf152db252200a6a2b31be0b8f58b",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "9acdf552db252200a6a2b31be0b8f507",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "99cdb152db252200a6a2b31be0b8f5e7",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "06cdb552db252200a6a2b31be0b8f53e",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "5dcdf152db252200a6a2b31be0b8f546",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "55cdf152db252200a6a2b31be0b8f540",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "8ecd7552db252200a6a2b31be0b8f581",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "92cdf552db252200a6a2b31be0b8f501",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "cacd7552db252200a6a2b31be0b8f5ad",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "0ecdb552db252200a6a2b31be0b8f537",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "25cdf152db252200a6a2b31be0b8f5ca",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "c6cdb552db252200a6a2b31be0b8f505",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "1acdb552db252200a6a2b31be0b8f583",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "9acdb552db252200a6a2b31be0b8f55d",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "6dcdf152db252200a6a2b31be0b8f5fc",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "9dcdf152db252200a6a2b31be0b8f526",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "91cdf152db252200a6a2b31be0b8f50e",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "35cd3552db252200a6a2b31be0b8f5de",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "5acdb552db252200a6a2b31be0b8f5bc",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "39cd7552db252200a6a2b31be0b8f555",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "21cdf152db252200a6a2b31be0b8f592",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "b5cd7552db252200a6a2b31be0b8f57b",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "b1cd7552db252200a6a2b31be0b8f536",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "c9cdb152db252200a6a2b31be0b8f5bb",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "8dcdb152db252200a6a2b31be0b8f562",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "5ecdb552db252200a6a2b31be0b8f5b5",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "f5cd3552db252200a6a2b31be0b8f5b2",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "a9cd3552db252200a6a2b31be0b8f502",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "b1cd3552db252200a6a2b31be0b8f580",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "a9cd3552db252200a6a2b31be0b8f579",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "49cdb152db252200a6a2b31be0b8f569",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "81cdb152db252200a6a2b31be0b8f589",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "fdcd3552db252200a6a2b31be0b8f5ab",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "ddcdf152db252200a6a2b31be0b8f513",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "3dcd3552db252200a6a2b31be0b8f5e4",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "0dcdb152db252200a6a2b31be0b8f58f",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "f1cd7552db252200a6a2b31be0b8f523",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "0dcdb152db252200a6a2b31be0b8f54f",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "21cd3552db252200a6a2b31be0b8f522",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "29cd3552db252200a6a2b31be0b8f528",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "c2cd7552db252200a6a2b31be0b8f5b4",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "d2cdb552db252200a6a2b31be0b8f5ee",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "dacdb552db252200a6a2b31be0b8f5e7",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "92cdb552db252200a6a2b31be0b8f564",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "e2cdf552db252200a6a2b31be0b8f57f",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "35cd7552db252200a6a2b31be0b8f51d",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "eecdf552db252200a6a2b31be0b8f539",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "0ecd7552db252200a6a2b31be0b8f5e6",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "e6cdf552db252200a6a2b31be0b8f533",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "12cdb552db252200a6a2b31be0b8f58a",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "cecdb552db252200a6a2b31be0b8f50b",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "2dcdf152db252200a6a2b31be0b8f5d0",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "55cdb152db252200a6a2b31be0b8f5ee",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "95cdf152db252200a6a2b31be0b8f52d",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "eacdf552db252200a6a2b31be0b8f578",
            "state": "requested",
            "sys_created_by": "admin"
        },
        {
            "sysapproval": "49cdb152db252200a6a2b31be0b8f548",
            "state": "requested",
            "sys_created_by": "admin"
        }
    ]
}
     */
    private ServiceNowApprovals GetPendingApprovalsRestAPI(string sysId)
    {

        ServiceNowApprovals approvals = new();

        //var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sysapproval_approver?approver={sysId}&state=requested&sysparm_exclude_reference_link=true&sysparm_fields=state%2Csys_created_by%2Csysapproval");
        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sysapproval_approver?sysparm_query=approver%3D{sysId}%5EstateNOT%20INapproved%2Crejected%2Ccancelled%2Cnot_required&sysparm_exclude_reference_link=true&sysparm_fields=sysapproval,due_date,state,sys_id");

        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var r = JsonConvert.DeserializeObject<ServiceNowApprovals>(response.Content)!;

            approvals = r;
        }

        return approvals;
    }

    private PendingApprovalDetails GetPendingApprovalDetailsRestAPI(string approvalId)
    {
        PendingApprovalDetails details = new();

        //var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/task?sysparm_query=sys_idIN{approvalId}&sysparm_exclude_reference_link=true&sysparm_fields=number%2Cdescription%2Cshort_description%2Cstate%2Cpriority%2Curgency%2Cassigned_to");
        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/task?sysparm_query=sys_idIN{approvalId}&sysparm_exclude_reference_link=true&sysparm_fields=number%2Cshort_description%2Cassigned_to%2Csys_id%2Cimpact");
        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var r = JsonConvert.DeserializeObject<PendingApprovalDetails>(response.Content)!;

            if (r != null && r.result.Count() > 0)
            {
                details.result = r.result;
                
            }

        }

        return details;
    }

    private User GetUserDetail(string userSysIds)
    {
        User user = new();

        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sys_user?sysparm_query=sys_idIN{userSysIds}&sysparm_fields=sys_id,name,email");

        var request = new RestRequest();

        request.Method = Method.Get;

        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        RestResponse response = client.Execute(request);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            user = JsonConvert.DeserializeObject<User>(response.Content)!;

        }

        return user;
    }


    /*
    * PUT https://dev52648.service-now.com/api/now/table/sysapproval_approver/00e9c07adba52200a6a2b31be0b8f5ae
    * Request Body
    * {"state":"approved","comments":"Approved via Teams","approval_source":"MyApprovalsHub"}
    */
    private bool ChangeApprovalStatus(string SysId, string comments, string status)
    {

        var client = new RestClient($"{_serviceNowInstanceUrl}/api/now/table/sysapproval_approver/{SysId}");
                
        var request = new RestRequest();

        request.Method = Method.Put;

        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", $"Bearer {GetToken()}");

        
        var body = @"{""state"":""" + status + @""",""comments"":""" + comments + @""",""approval_source"":""MyApprovalsHub""}";

        request.AddParameter("application/json", body, ParameterType.RequestBody);

        RestResponse response = client.Execute(request);

        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    public bool Approve(string id, string comments)
    {
        return ChangeApprovalStatus(id, comments, "approved");
    }
    public bool Reject(string id, string comments)
    {
        return ChangeApprovalStatus(id, comments, "rejected");
    }

    public IEnumerable<PendingApproval> GetPendingApprovals(string approverEmail)
    {

        var sys_id = GetServiceNowUser(approverEmail);

        var approvals = GetPendingApprovalsRestAPI(sys_id);

        PendingApprovalDetails approvalDetails = new();

        //we need to chunck the pending approvals to avoid getting an exception due the request is too long
        var chunck = approvals.result.Select(p => p.sysapproval).ToList().Chunk(10);

        Parallel.ForEach(chunck, p =>
        {
            foreach (var item in GetPendingApprovalDetailsRestAPI(string.Join(",", p)).result)
            {
                approvalDetails.result.Add(item);

            }
            
            
        });

        
        //we need to get the name and email of the user
        var users = GetUserDetail(string.Join(",", approvalDetails.result.Select(u => u.assigned_to).Distinct()));


        var result = from approval in approvals.result
                     join approvalDetail in approvalDetails.result
                        on approval.sysapproval equals approvalDetail.sys_id
                     join user in users.result
                        on approvalDetail.assigned_to equals user.sys_id
                     
                     select new PendingApproval
                     {
                         Number = approvalDetail.number,
                         Description = approvalDetail.short_description,
                         Requestor = user.name,
                         Email = user.email,
                         Date = approval.due_date.Date,
                         Source = PendingApprovalSource.ServiceNow.ToString(),
                         SourcePhoto = "servicenow.png",
                         State = approval.state,
                         SysId = approvalDetail.sys_id,
                         Impact = _impacts.ContainsKey(approvalDetail.impact) ? _impacts[approvalDetail.impact] : string.Empty,
                         SysApproval = approval.sys_id
                     };

        return result;
             
    }



 

    private class ServiceNowApprovals
    {
        public Pending[] result { get; set; }

        public class Pending
        {
            public string sysapproval { get; set; }
            public DateTime due_date { get; set; }
            public string state { get; set; }
            public string sys_id { get; set; }
            
        }
    }


    public class PendingApprovalDetails
    {
        public ConcurrentBag<ApprovalDetail> result { get; set; } = new();

        public class ApprovalDetail
        {
            public string sys_id { get; set; }
            public string number { get; set; }
            public string short_description { get; set; }
            //public string urgency { get; set; }
            public string assigned_to { get; set; }
            public int impact { get; set; }
            //public string description { get; set; }
            //public string state { get; set; }
            //public string priority { get; set; }
        }
    }


    public class User
    {
        public List<UserDetail> result { get; set; } = new();
    }

    public class UserDetail
    {
        public string sys_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }



}



