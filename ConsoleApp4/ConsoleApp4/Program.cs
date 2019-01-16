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
        public static Client ApiPass()
        {
            String apiURL = "https://cloud.gravityzone.bitdefender.com/api/v1.0/jsonrpc/network";
            Client rpcClient = new Client(apiURL);
            String auth = AuthMethod();
            rpcClient.Headers.Add("Authorization", "Basic " + auth);
            return rpcClient;
        }

        public static String AuthMethod()
        {
            String apiKey = "2sBqp0JghAhJFLrcPjK5P+OjGP+pkWIP";
            String userPassString = apiKey + ":";
            String authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(userPassString));
            return authHeader;
        }

        //GetCustomGroupLists needed
        public static int GetCustomGroupList(string groupID)
        {
            if (String.IsNullOrEmpty(groupID))
            {
                JToken parameters = new JObject();

                Client test = ApiPass();
                Request request = test.NewRequest("getEndpointsList", parameters);
                Response testResponse = test.Rpc(request);
                Console.WriteLine(testResponse.ToString());
                return testResponse.Id;
            }
            else
            {
                JToken parameters = new JObject();
                parameters["parentId"] = JToken.Parse(groupID);

                Client test = ApiPass();
                Request request = test.NewRequest("getEndpointsList", parameters);
                Response testResponse = test.Rpc(request);
                Console.WriteLine(testResponse.ToString());
                return testResponse.Id;
            }
        }

        //This is the recursion boi
        public void GetSubGroups(String groupID)
        {

            List<String> results = new List<String>();
            List<String> groups = new List<String>(GetCustomGroupList(groupID));
            List<String> group = new List<string>();

            if (groups.Count >= 1)
            {
                for (var i = 0; i < groups.Count; i++)
                {
                    List<String> td = new List<String>();
                    results.Add(td.ToString());
                }
            }

        }

        //Only void for testing
        public String GetEndpointList(String parentID)
        {
            JToken parameters = new JObject();
            Client test = ApiPass();
            Request requester = test.NewRequest("getEndpointsList", parameters);
            Response testResponse = test.Rpc(requester);
            Console.WriteLine(testResponse.ToString());
            return testResponse.ToString();

        }

        //Only void for testing
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

            JToken parameters = new JObject();
            parameters["page"] = 1;
            parameters["perPage"] = 2;

            Client passer = ApiPass();

            Request request = passer.NewRequest("getEndpointsList", parameters);

            Response response = passer.Rpc(request);
            Console.WriteLine(response.ToString());

            Console.Write("Testing Line");

            List<String> computers = new List<String>();
            List<String> groups = new List<String>();


            String groupID = null;
            String endpointID = "1";
            int topLevelGroups = GetCustomGroupList(groupID);
            Console.WriteLine("Top Level Groups");
            Console.WriteLine(GetManagedEndpointDetails(endpointID));
            
            Console.WriteLine("Hello");



            Console.ReadKey();
        }

    }
}
