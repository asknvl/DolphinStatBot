

using DolphinStatBot.DolphinApi;

using DolphinStatBot.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

class Program
{
    public static async Task Main(string[] args)
    {
               
        DolphinApi dolphin = new DolphinApi("http://188.225.43.87");        
        List<User> users = await dolphin.GetUsers();


        var client = new RestClient("http://188.225.43.87/new/stat/by_user?currency=USD");
        client.Timeout = -1;
        var request = new RestRequest(Method.POST);
        request.AddHeader("Authorization", "1-578000f643ac0dd4f72579dd758ebd8e");
        request.AddHeader("Content-Type", "application/json");

        dynamic p = new JObject();
        p.ids = new JArray();
        p.ids.Add("0");

        request.AddParameter("application/json", p.ToString(), ParameterType.RequestBody);
        //request.AddParameter("ids[]", "{1}");
        request.AddParameter("dates[startDate]", "2022-04-08");
        request.AddParameter("dates[endDate]", "2022-04-08");
        IRestResponse response = client.Execute(request);
        Console.WriteLine(response.Content);

        Console.ReadLine();
    }
}