

using DolphinStatBot.DolphinApi;
using DolphinStatBot.Stat;
using DolphinStatBot.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

class Program
{
    public static async Task Main(string[] args)
    {

        string token = "1-578000f643ac0dd4f72579dd758ebd8e";

        DolphinApi dolphin = new DolphinApi("http://188.225.43.87", token);        
        List<User> users = await dolphin.GetUsers();
        users.ForEach(user => Console.WriteLine($"id={user.id}\tlogin={user.login}\tname={user.display_name}"));

        Console.WriteLine();
        double total = 0;

        //foreach (var user in users)
        //{
        //    Statistics statistics = await dolphin.GetStatistics("2022-02-15", user.id);

        //    string name = !user.display_name.Equals("") ? user.display_name : user.login ;

        //    Console.WriteLine($"id={user.id} name={name}");
        //    Console.WriteLine($"spend={statistics.spend}");
        //    Console.WriteLine($"leads={statistics.results}");
        //    Console.WriteLine($"cpa={statistics.cpa}");
        //    Console.WriteLine();

        //    total += statistics.spend;
        //}

        var ids = users.Select(user => user.id).ToArray();
        Dictionary<string, Statistics> statistics = await dolphin.GetStatistics(ids, "2022-04-08", "2022-04-09");


        Console.ReadLine();
    }
}