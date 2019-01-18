using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonRPC;
using Newtonsoft.Json.Linq;

namespace ConsoleApp4
{
    class Program
    {
        private object td;

        //Runs through for the client to use the API headers
        public static Client ApiPass()
        {
            String apiURL = "https://cloud.gravityzone.bitdefender.com/api/v1.0/jsonrpc/network";
            Client rpcClient = new Client(apiURL);
            String auth = AuthMethod();
            rpcClient.Headers.Add("Authorization", "Basic " + auth);
            return rpcClient;
        }

        //Uses an API Key to pass to ApiPass
        public static String AuthMethod()
        {
            String apiKey = "0/Bw0vslK+JdScZgt0LRWhIhkB8Yr8LR";
            String userPassString = apiKey + ":";
            String authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(userPassString));
            return authHeader;
        }

        //Gets the custom group lists using a GroupID.
        public static String GetCustomGroupList(string groupID)
        {
            if (String.IsNullOrEmpty(groupID))
            {
                JToken parameters = new JObject();   
                Client test = ApiPass();
                Request request = test.NewRequest("getCustomGroupsList", parameters);
                GenericResponse testResponse = test.Rpc(request);
                Console.WriteLine("fuck we are here");
                Console.WriteLine(testResponse.ToString());
                JToken results = testResponse.Result;
                
                return testResponse.Id.ToString();
            }
            else
            {
                JToken parameters = new JObject();
                parameters["parentId"] = JToken.Parse(groupID);

                Client test = ApiPass();
                Request request = test.NewRequest("getCustomGroupsList", parameters);
                Response testResponse = test.Rpc(request);
                Console.WriteLine(testResponse.ToString());
                return testResponse.Id.ToString();
            }
        }

        //Recursive boi RIP
        public static List<String> GetSubGroups(String groupID)
        {

            List<String> results = new List<String>();
            List<String> groups = new List<String>();
            groups.Add(GetCustomGroupList(groupID));
            results.Add(groupID);

            if(groups.Count >= 1)
            {
                foreach(var group in groups)
                {
                    var td = GetSubGroups(group);
                    foreach(var t in td)
                    {
                        results.Add(t);
                    }
                }
            }    
            return results;

        }

        //Only void for testing
        public static String GetEndpointList(String parentID)
        {
            JToken parameters = new JObject();
            Client test = ApiPass();
            Request requester = test.NewRequest("getEndpointsList", parameters);
            Response testResponse = test.Rpc(requester);
            return testResponse.ToString();

        }

        //Get the details for a specific Endpoint ID.
        public static String GetManagedEndpointDetails(String EndpointId)
        {
            JToken parameters = new JObject();
            parameters["endpointId"] = EndpointId;
            Client test = ApiPass();
            Request requester = test.NewRequest("getManagedEndpointDetails", parameters);
            Response testResponse = test.Rpc(requester);
            return testResponse.ToString();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            List<String> computers = new List<String>();
            List<String> groups = new List<String>();
            //Example ID: 5c3c937429631119cfd4ff93
            List<String> topLevelGroups = new List<String>();
            topLevelGroups.Add(GetCustomGroupList(null));
            Console.WriteLine("Top Level Groups");
            foreach(String group in topLevelGroups)
            {
                Console.WriteLine("We are inside the ForEach");
                List<string> td = GetSubGroups(group);
                foreach(var t in td)
                {
                    groups.Add(t);
                }
            }
            
            Console.WriteLine("Get Endpoints List Finished");

            foreach(var group in groups)
            {
                List<String> endpoints = new List<String>();
                endpoints.Add(GetEndpointList(group));
                foreach(var c in endpoints)
                {
                    Console.WriteLine("Working on Computer: " + c);
                    computers.Add(GetManagedEndpointDetails(c));
                }
            }
            Console.WriteLine("Overall Program Done!");
            Console.ReadKey();
        }

    }
}
