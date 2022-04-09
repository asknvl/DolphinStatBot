

using DolphinStatBot.DolphinApi;
using DolphinStatBot.pdf;
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
        
        var ids = users.Select(user => user.id).ToArray();
        var statistics = await dolphin.GetStatistics(ids, "2022-04-09", "2022-04-09");

        var filter = new List<uint>();

        PdfCreator pdf = new PdfCreator();
        pdf.GetPdf(users, statistics);
         


        Console.ReadLine();
    }
}