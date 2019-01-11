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

            Console.ReadKey();

        }

        public static Client ApiPass() {            
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
        public int GetCustomGroupList()
        {
            string groupID = null;
            if (String.IsNullOrEmpty(groupID))
            {
                JToken parameters = new JObject();

                Client test = ApiPass();
                Request request = test.NewRequest("getEndpointsList", parameters);
                Response testResponse = test.Rpc(request);

                return testResponse.Id;
            }
            else
            {
                JToken parameters = new JObject();
                parameters["parentId"] = JToken.Parse(groupID);

                Client test = ApiPass();
                Request request = test.NewRequest("getEndpointsList", parameters);
                Response testResponse = test.Rpc(request);

                return testResponse.Id;
            }
        }

        //This is the recursion boi
        public String GetSubGroups(String groupID)
        {

            List<String> results = new List<String>();

            return "test";

        }

        //Only void for testing
        public void GetEndpointList(String parentID)
        {
            JToken parameters = new JObject();


        }

        //Only void for testing
        public void GetManagedEndpointDetails()
        {

        }


    }
}
