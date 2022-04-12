using DolphinStatBot.Accounts;
using DolphinStatBot.Dolphin;
using DolphinStatBot.TG;
class Program
{
    public static async Task Main(string[] args)
    {

        Bot bot = new Bot();
        bot.Start();
        Console.ReadLine();

        //DolphinApi dolphin = new DolphinApi("http://188.225.43.87", "1-578000f643ac0dd4f72579dd758ebd8e");
        //dolphin.FilteredIDs = new List<uint> { 2, 6, 7, 8, 14 };
        //var users = await dolphin.GetUsers();
        //var ids = users.Select(user => (int)user.id).ToArray();
        //var res = await dolphin.GetAccounts(new int[] { -1 }, new string[] {"Макс"}, new string[] { "не-обновлять", "без-комментов" });        
        //var accids = res.Select(acc => acc.id).ToArray();
        //var stat = await dolphin.GetStatisticsByAccounts(accids, "2022-04-12", "2022-04-12");
        //foreach (var item in res)
        //{
        //    Console.WriteLine(item.id);
        //}
        Console.ReadLine();

    }
}