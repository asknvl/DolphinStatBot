using DolphinStatBot.Dolphin;
using DolphinStatBot.pdf;
using DolphinStatBot.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace DolphinStatBot.TG
{
    public class Bot
    {
        #region const
#if DEBUG1
        const string Token = "5166876147:AAHqU1jssTleiNMz52BfEo5qkPaLeUnXa-w";
#else
        const string Token = "5166876147:AAHqU1jssTleiNMz52BfEo5qkPaLeUnXa-w";
#endif
        #endregion

        #region vars
        TelegramBotClient botClient;
        CancellationTokenSource cts;
        UserManager userManager;
        DolphinApi dolphin;
        PdfCreator pdf;
        System.Timers.Timer timer;

        int Hours = 0;
        int Minutes = 0;

        DateTime run, now;
        TimeSpan span;

        #endregion

        public Bot()
        {

            dolphin = new DolphinApi("http://188.225.43.87", "1-578000f643ac0dd4f72579dd758ebd8e");
            dolphin.FilteredIDs = new List<uint> { 2, 6, 7, 8, 14 };

            pdf = new PdfCreator();           

            userManager = new UserManager();
            userManager.Init();

            botClient = new TelegramBotClient(Token);

            timer = new System.Timers.Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;

            initTime(14, 13);

        }

        void initTime(int hours, int minutes)
        {
            timer.Enabled = false;
            Hours = hours;
            Minutes = minutes;
            now = DateTime.Now;
            run = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Hours, Minutes, 0);
            span = run - now;
            if (span < TimeSpan.Zero)
                run = run.AddDays(1);
            timer.Enabled = true;
        }
        
        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {            
            now = DateTime.Now;
            span = run - now;
            if (span < TimeSpan.Zero) {
                Console.WriteLine(now);
                run = run.AddMinutes(5);
                await sendStat();
            }                        
        }

        #region private
        async void send(string msg, Update upd, CancellationToken ct)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                           chatId: upd.Message.Chat.Id,
                           text: msg,
                           cancellationToken: ct);
        }

        async Task sendStat(long id)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH mm ss");

            var users = await dolphin.GetUsers();
            var ids = users.Select(user => user.id).ToArray();
            var statistics = await dolphin.GetStatistics(ids, date, date);

            var stream = pdf.GetPdf(users, statistics);

            InputOnlineFile inputOnlineFile = new InputOnlineFile(stream, $"{date} {time}.pdf");
            await botClient.SendDocumentAsync(id, inputOnlineFile);
        }

        async Task sendStat()
        {
            await Task.Run(async () =>
            {
                foreach (var id in userManager.GetIDs())
                {
                   await sendStat(id);
                }
            });
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Message == null)
                return;

            var id = update.Message.Chat.Id;
            string name = $"{update.Message.Chat.FirstName} {update.Message.Chat.LastName}";
            var msg = update.Message.Text.ToLowerInvariant();

            switch (msg)
            {
                case "5555":
                    userManager.Add(id, name);
                    break;
                case "/getstat":
                    if (userManager.Check(id))
                    {
                        await sendStat(id);
                        
                    } else
                    {
                        send("Нет доступа", update, cancellationToken);
                    }
                    break;
                case "/getinfo":
                    send(userManager.GetInfo(), update, cancellationToken);
                    break;

                case "/gettime":
                    send($"Следующее время сбора данных: {run}", update, cancellationToken);
                    break;

                default:
                    break;
            }

            if (msg.Contains("/settime"))
            {
                string[] spl = msg.Split(":");
                try
                {
                    int hh = int.Parse(spl[1]);
                    int mm = int.Parse(spl[2]);

                    initTime(hh, mm);

                    send($"Следующее время сбора данных: {run}", update, cancellationToken);

                } catch {
                    send("Неверный формат ввода времени", update, cancellationToken);
                }
            }

            string upd = $"id={id}\n" +
                         $"name={name}\n" +
                         $"msg={msg}\n" +
                         $"-----------";

            Console.WriteLine(upd);
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        #endregion

        #region public
        public void Start()
        {
            cts = new CancellationTokenSource();
            userManager.Init();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message }
            };
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

        }        
        #endregion
    }
}
