using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonRPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/*
 * This program uses the BitDefender API and JSON parsing to recursively search through the folders that are setup within BitDefender and find all the computers in that folder.
 * After grabbing each computer it then adds them to a custom object that holds each parameter and can be called on it's own.
 * 
 * BitDefender API Guide: https://download.bitdefender.com/business/API/Bitdefender_GravityZone_Cloud_APIGuide_forCustomers_enUS.pdf
 * JSONRpc: https://github.com/adamashton/json-rpc-csharp
 */

namespace ConsoleApp4
{
    class BitGrab
    {
        //This is the default response of the API.
        public class BitDefenderApiResponse<T>
        {
            [JsonProperty("result")]
            public T Result { get; set; }

            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }

            [JsonProperty("id")]
            public long Id { get; set; }
        }

        public class GetCustomGroupsResult
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class GetEndpointListResult
        {
            [JsonProperty("total")]
            public long Total { get; set; }

            [JsonProperty("page")]
            public long Page { get; set; }

            [JsonProperty("perPage")]
            public long PerPage { get; set; }

            [JsonProperty("pagesCount")]
            public long PagesCount { get; set; }

            [JsonProperty("items")]
            public List<Item> Items { get; set; }
        }

        //The API returns the following parameters when GetManagedEndpointDetails is used
        public class GetManagedEndpointDetailsResult
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("operatingSystem")]
            public string OperatingSystem { get; set; }

            [JsonProperty("policy")]
            public Policy Policy { get; set; }

            [JsonProperty("ip")]
            public string Ip { get; set; }

            [JsonProperty("machineType")]
            public long MachineType { get; set; }

            [JsonProperty("malwareStatus")]
            public MalwareStatus MalwareStatus { get; set; }

            [JsonProperty("agent")]
            public Agent Agent { get; set; }

            [JsonProperty("lastSeen")]
            public DateTime LastSeen { get; set; }

            [JsonProperty("state")]
            public string State { get; set; }

            [JsonProperty("group")]
            public Group Group { get; set; }

            [JsonProperty("modules")]
            public Modules Modules { get; set; }

            [JsonProperty("label")]
            public string Label { get; set; }

            [JsonProperty("managedWithBest")]
            public string ManagedWithBest { get; set; }

        }

        //This holds the parameters for individual items and holding them.
        public class Item
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("label")]
            public string Label { get; set; }

            [JsonProperty("fqdn")]
            public string Fqdn { get; set; }

            [JsonProperty("groupId")]
            public string GroupId { get; set; }

            [JsonProperty("isManaged")]
            public string IsManaged { get; set; }

            [JsonProperty("machineType")]
            public string MachineType { get; set; }

            [JsonProperty("operatingSystemVersion")]
            public string OperatingSystemVersion { get; set; }

            [JsonProperty("ip")]
            public string Ip { get; set; }

            [JsonProperty("macs")]
            public List<string> Macs { get; set; }

            [JsonProperty("ssid")]
            public string Ssid { get; set; }

            [JsonProperty("managedWithBest", NullValueHandling = NullValueHandling.Ignore)]
            public string ManagedWithBest { get; set; }
        }

        //This is an internal list to the GetManagedEndpointDetails
        public class Agent
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("productVersion")]
            public string ProductVersion { get; set; }

            [JsonProperty("engineVersion")]
            public string EngineVersion { get; set; }

            [JsonProperty("lastUpdate")]
            public string LastUpdate { get; set; }

            [JsonProperty("signatureOutdated")]
            public string SignatureOutdated { get; set; }

            [JsonProperty("signatureUpdateDisabled")]
            public string SignatureUpdateDisabled { get; set; }

            [JsonProperty("productUpdateDisabled")]
            public string ProductUpdateDisabled { get; set; }

            [JsonProperty("productOutdated")]
            public string ProductOutdated { get; set; }

            [JsonProperty("primaryEngine")]
            public string PrimaryEngine { get; set; }

            [JsonProperty("fallbackEngine")]
            public string FallbackEngine { get; set; }

            [JsonProperty("licensed")]
            public string Licensed { get; set; }
        }

        //The groups return the ID and the Name when asked.
        public class Group
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class MalwareStatus
        {
            [JsonProperty("detection")]
            public bool Detection { get; set; }

            [JsonProperty("infected")]
            public bool Infected { get; set; }
        }

        public class Modules
        {
            [JsonProperty("antimalware")]
            public string Antimalware { get; set; }

            [JsonProperty("firewall")]
            public string Firewall { get; set; }

            [JsonProperty("contentControl")]
            public string ContentControl { get; set; }

            [JsonProperty("powerUser")]
            public string PowerUser { get; set; }

            [JsonProperty("deviceControl")]
            public string DeviceControl { get; set; }

            [JsonProperty("advancedThreatControl")]
            public string AdvancedThreatControl { get; set; }

            [JsonProperty("applicationControl")]
            public string ApplicationControl { get; set; }
        }

        //Tells what policy is applied to the endpoint
        public class Policy
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("applied")]
            public string Applied { get; set; }
        }


        //Uses the default authorization passing of Basic to the BitDefender API
        public static Client ApiPass()
        {
            String apiURL = "https://cloud.gravityzone.bitdefender.com/api/v1.0/jsonrpc/network";
            Client rpcClient = new Client(apiURL);
            String auth = AuthMethod();
            rpcClient.Headers.Add("Authorization", "Basic " + auth);
            return rpcClient;
        }

        //Uses an API Key to pass to ApiPass to connect properly
        public static String AuthMethod()
        {
            String apiKey = "XXXXXXXXX"; //Insert API Key here
            String userPassString = apiKey + ":";
            String authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(userPassString));
            return authHeader;
        }

        //This retrieves a list of groups under a specified group using the paramter ParentID
        //Returns ID of group and the Name of the group.
        public static List<GetCustomGroupsResult> GetCustomGroupList(string parentID)
        {
            JToken parameters = new JObject();   
            if (!String.IsNullOrWhiteSpace(parentID))
            {
                parameters["parentId"] = parentID;
            }
            Client test = ApiPass();
            Request request = test.NewRequest("getCustomGroupsList", parameters);
            GenericResponse testResponse = test.Rpc(request);
            var model = JsonConvert.DeserializeObject<BitDefenderApiResponse<List<GetCustomGroupsResult>>>(testResponse.ToString());
            return model.Result;
        }

        //This method does not call the API at all, but instead is recursively calling to find all folders/groups.
        public static List<GetCustomGroupsResult> GetSubGroups(String groupID)
        {

            List<GetCustomGroupsResult> topLevelGroups = new List<GetCustomGroupsResult>();
            List<GetCustomGroupsResult> childGroups = new List<GetCustomGroupsResult>();
            topLevelGroups = GetCustomGroupList(groupID);                  
            if(topLevelGroups.Count > 0)
            {
                foreach (var customGroup in topLevelGroups)
                {
                    childGroups.AddRange(GetSubGroups(customGroup.Id));
                }
            }
            topLevelGroups.AddRange(childGroups);
            return topLevelGroups;
        }

        //Using the parentID paramter, this returns a list of endpoints that are under a specific folder.
        //parentID would be the target group.
        public static GetEndpointListResult GetEndpoints(String parentID)
        {
            JToken parameters = new JObject
            {
                ["parentId"] = parentID
            };
            Client test = ApiPass();
            Request requester = test.NewRequest("getEndpointsList", parameters);
            GenericResponse testResponse = test.Rpc(requester);
            var model = JsonConvert.DeserializeObject<BitDefenderApiResponse<GetEndpointListResult>>(testResponse.ToString());
            return model.Result;
        }

        //Using the endpointID (Required), this will return all information about the endpoint.
        //This can include the name, IP, malware status, etc.
        public static GetManagedEndpointDetailsResult GetManagedEndpointDetails(Item item)
        {
            JToken parameters = new JObject
            {
                ["endpointId"] = item.Id
            };
            Client test = ApiPass();
            Request requester = test.NewRequest("getManagedEndpointDetails", parameters);
            GenericResponse testResponse = test.Rpc(requester);
            var model = JsonConvert.DeserializeObject<BitDefenderApiResponse<GetManagedEndpointDetailsResult>>(testResponse.ToString());
            return model.Result;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            List<GetManagedEndpointDetailsResult> computers = new List<GetManagedEndpointDetailsResult>();
            List<GetManagedEndpointDetailsResult> infected = new List<GetManagedEndpointDetailsResult>();
            Console.WriteLine("Grabbing Endpoint Details");
            List<GetCustomGroupsResult> groups = GetSubGroups(null);
            Console.WriteLine("Get Endpoints List Finished");

            //This area searches the groups and adds them to the list of Computers if there is a computer there.
            foreach(var group in groups)
            {
                foreach(Item i in GetEndpoints(group.Id).Items)
                {
                    Console.WriteLine("Working on Computer: " + i.Name);
                    if (!(i == null) && !(GetManagedEndpointDetails(i) == null))
                    {
                        computers.Add(GetManagedEndpointDetails(i));
                        if(GetManagedEndpointDetails(i).MalwareStatus.Detection == true || GetManagedEndpointDetails(i).MalwareStatus.Infected == true)
                        {
                            infected.Add(GetManagedEndpointDetails(i));
                        }
                        
                    }
                }
            }
            Console.WriteLine();
            if(infected.Count > 0)
            {
                Console.WriteLine("There were devices with Malware detected:");
                foreach(var inf in infected)
                {
                    Console.WriteLine("Computer: " + inf.Name);
                }
            }
            Console.WriteLine();
            Console.WriteLine("Overall Program Done!");
            Console.ReadKey();
        }

    }
}
