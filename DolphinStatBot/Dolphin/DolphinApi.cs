using DolphinStatBot.Stat;
using DolphinStatBot.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinStatBot.Dolphin
{
    public class DolphinApi
    {
        #region vars
        string url;
        string token;
        #endregion

        #region properties
        public List<uint> FilteredIDs { get; set; } = new List<uint>();
        #endregion

        public DolphinApi(string url, string token)
        {
            this.url = url;
            this.token = token;
        }

        /// <summary>
        /// Returns all users 
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetUsers()
        {
            List<User> users = new List<User>();
            try
            {
                await Task.Run(() =>
                {

                    var client = new RestClient($"{url}/new/users");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Authorization", $"{token}");
                    request.AddHeader("Content-Type", "application/json");
                    IRestResponse response = client.Execute(request);
                    JObject json = JObject.Parse(response.Content);

                    var data = json["data"];
                    if (data != null)
                    {
                        IList<JToken> results = data.Children().ToList();
                        foreach (JToken result in results)
                        {
                            User? user = result.ToObject<User>();
                            if (user != null)
                                if (!FilteredIDs.Contains(user.id))
                                users.Add(user);
                        }
                    }

                });
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"GetUser exception");
            }
            return users;
        }
        public async Task<Dictionary<uint, Statistics>> GetStatistics(uint[] ids, string startDate, string endDate)
        {
            Dictionary<uint, Statistics> res = new Dictionary<uint, Statistics>();
            try
            {
                await Task.Run(() =>
                {
                    var client = new RestClient($"{url}/new/stat/by_user?currency=USD");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", $"{token}");
                    request.AddHeader("Content-Type", "application/json");
                    dynamic p = new JObject();
                    p.ids = new JArray();
                    foreach (var id in ids)
                    {
                        p.ids.Add(id);
                    }
                    p.dates = new JObject();
                    p.dates.startDate = startDate;
                    p.dates.endDate = endDate;
                    request.AddParameter("application/json", p.ToString(), ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception("GetStatistics request fail");

                    JObject json = JObject.Parse(response.Content);                    
                    bool success = json["success"].ToObject<bool>();
                    if (!success)
                        throw new Exception($"GetStatistics success={success}");
                  
                    JToken data = json["data"];
                    foreach (var item in ids)
                    {
                        string sid = $"{item}";
                        dynamic stat = data[sid];
                        res.Add(/*sid*/item, new Statistics()
                        {
                            spend = stat.spend,
                            results = stat.results,
                            cpa = stat.cpa
                        });
                    }

                    dynamic? ttl = data["total"];
                    if (ttl != null)
                    {
                        res.Add(0xFF, new Statistics()
                        {
                            spend = ttl.spend,
                            results = ttl.results,
                            cpa = ttl.cpa
                        });
                    }

                });

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Get statistics exception (ids number = {ids.Length})");
            }
            res = res.OrderByDescending(o => o.Value.spend).ToDictionary(x => x.Key, x => x.Value);
            return res;
        }


    }
}
