using DolphinStatBot.Users;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinStatBot.DolphinApi
{
    public class DolphinApi
    {
        #region vars
        string url;
        #endregion
        public DolphinApi(string url)
        {
            this.url = url;
        }

        public async Task<List<User>> GetUsers()
        {
            List<User> users = new List<User>();
            try
            {
                await Task.Run(() => { 

                    var client = new RestClient($"{url}/new/users");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Authorization", "1-578000f643ac0dd4f72579dd758ebd8e");
                    request.AddHeader("Content-Type", "application/json");
                    IRestResponse response = client.Execute(request);                    
                    JObject json = JObject.Parse(response.Content);
                    IList<JToken> results = json["data"].Children().ToList();
                    foreach (JToken result in results)
                    {
                        User user = result.ToObject<User>();                        
                        users.Add(user);
                    }
                });
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return users;
        }
        
    }
}
