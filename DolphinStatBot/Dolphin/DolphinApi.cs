using DolphinStatBot.Accounts;
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
                                //if (!FilteredIDs.Contains(user.id))
                                if (user.status != -1 && !FilteredIDs.Contains(user.id))
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
        public async Task<Dictionary<uint, Statistics>> GetStatistics(uint[] userids, string startDate, string endDate)
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
                    foreach (var id in userids)
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
                  
                    foreach (var item in userids) {
                        double spend = 0;
                        uint leads = 0;
                        double cpa = 0;
                        dynamic jspend = json["data"][$"{item}"];
                        spend = jspend.spend;
                        JToken actions = json["data"][$"{item}"]["actions"];
                        dynamic action_lead = actions.SelectToken("action_lead");
                        
                        if (action_lead != null)
                        {
                            leads = action_lead.quantity;
                            cpa = action_lead.cpa;
                        }
                        res.Add(item, new Statistics() { 
                            spend = spend,
                            results = leads,
                            cpa = cpa
                        });                     
                    }

                    dynamic? ttl = json["data"]["total"];                    
                    if (ttl != null)
                    {
                        JToken actions = json["data"]["total"]["actions"];
                        dynamic action_lead = actions.SelectToken("action_lead");
                        double spend = 0;
                        uint leads = 0;
                        double cpa = 0;
                        if (action_lead != null)
                        {
                            leads = action_lead.quantity;
                            cpa = action_lead.cpa;
                        }
                        res.Add(0xFF, new Statistics()
                        {
                            spend = ttl.spend,
                            results = leads,
                            cpa = cpa
                        });
                    }
                });

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Get statistics exception (ids number = {userids.Length})");
            }
            res = res.OrderByDescending(o => o.Value.spend).ToDictionary(x => x.Key, x => x.Value);
            return res;
        }
        public async Task<List<Account>> GetAccounts(int[] userids, string[] includetags, string[] excludetags)
        {
            List<Account> accounts = new List<Account>();
            try
            {
                await Task.Run(() => {
                    var client = new RestClient($"{url}/new/accounts");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Authorization", $"{token}");
                    request.AddHeader("Content-Type", "application/json");
                    dynamic p = new JObject();
                    p.user_ids = new JArray();
                    foreach (var item in userids)
                    {
                        p.user_ids.Add(item);
                    }
                    request.AddParameter("application/json", p.ToString(), ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception("GetAccounts request fail");
                    JObject json = JObject.Parse(response.Content);
                    bool success = json["success"].ToObject<bool>();
                    if (!success)
                        throw new Exception($"GetStatistics success={success}");
                    JToken data = json["data"];
                    foreach (var item in data.Children()) {
                        JToken jid = item["id"];
                        JToken jtags = item["tags"];
                        JToken jarch = item["archived"];
                        var acc = new Account()
                        {
                            id = jid.ToObject<uint>(),
                            tags = jtags.ToObject<string[]>(),     
                            archived = jarch.ToObject<int>()
                        };
                        bool incl = (includetags.Length > 0) ? acc.tags.Any(x => includetags.Any(y => x.Equals(y))) : true;                        
                        bool excl = (excludetags.Length > 0) ? acc.tags.Any(x => excludetags.Any(y => y.Equals(x))) : false;
                        if (incl && !excl && acc.archived == 0)
                        {
                            accounts.Add(acc);
                        }                        
                    }
                });
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"GetAccounts exception");
            }
            return accounts;
        }

        public async Task<Dictionary<uint, Statistics>> GetStatisticsByAccounts(uint[] accids, string startDate, string endDate)
        {
            Dictionary<uint, Statistics> res = new Dictionary<uint, Statistics>();
            try
            {
                await Task.Run(() =>
                {
                    var client = new RestClient($"{url}/new/stat/by_account?currency=USD");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", $"{token}");
                    request.AddHeader("Content-Type", "application/json");
                    dynamic p = new JObject();
                    p.ids = new JArray();
                    foreach (var id in accids)
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

                    foreach (var item in accids)
                    {
                        double spend = 0;
                        uint leads = 0;
                        double cpa = 0;
                        dynamic jspend = json["data"][$"{item}"];
                        spend = jspend.spend;
                        JToken actions = json["data"][$"{item}"]["actions"];
                        dynamic action_lead = actions.SelectToken("action_lead");

                        if (action_lead != null)
                        {
                            leads = action_lead.quantity;
                            cpa = action_lead.cpa;
                        }
                        res.Add(item, new Statistics()
                        {
                            spend = spend,
                            results = leads,
                            cpa = cpa
                        });
                    }

                    dynamic? ttl = json["data"]["total"];

                    if (ttl != null)
                    {
                        JToken actions = json["data"]["total"]["actions"];
                        dynamic action_lead = actions.SelectToken("action_lead");
                        double spend = 0;
                        uint leads = 0;
                        double cpa = 0;
                        if (action_lead != null)
                        {
                            leads = action_lead.quantity;
                            cpa = action_lead.cpa;
                        }
                        res.Add(0xFF, new Statistics()
                        {
                            spend = ttl.spend,
                            results = leads,
                            cpa = cpa
                        });
                    }

                });

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Get statistics exception (ids number = {accids.Length})");
            }
            res = res.OrderByDescending(o => o.Value.spend).ToDictionary(x => x.Key, x => x.Value);
            return res;
        }

    }
}
