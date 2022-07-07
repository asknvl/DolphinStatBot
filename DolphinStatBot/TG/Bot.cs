﻿using DolphinStatBot.Accounts;
using DolphinStatBot.Dolphin;
using DolphinStatBot.pdf;
using DolphinStatBot.Stat;
using DolphinStatBot.Store;
using DolphinStatBot.Users;
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
        string version = "dailyreporter_bot v1.12";
#if DEBUG
        const string Token = "5136456760:AAGIgTrNI7sTdf8xxLPNRFeI38uXdAbOS0o";
        //const string Token = "5166876147:AAHqU1jssTleiNMz52BfEo5qkPaLeUnXa-w";
#else
        #region const
        const string Token = "5166876147:AAHqU1jssTleiNMz52BfEo5qkPaLeUnXa-w";        
        #endregion
#endif

        #region vars
        TelegramBotClient botClient;
        CancellationTokenSource cts;
        UserManager userManager;
        DolphinApi dolphin;
        PdfCreator pdf;

        System.Timers.Timer sendTimer;
        DateTime run, now;
        TimeSpan span;
        int minuteInteral;

#endregion

        public Bot()
        {
            dolphin = new DolphinApi("http://188.225.43.87", "1-578000f643ac0dd4f72579dd758ebd8e");
            dolphin.FilteredIDs = new List<uint> { 1, 5 };

            pdf = new PdfCreator(version);
            userManager = new UserManager();

            userManager.Init();

            botClient = new TelegramBotClient(Token);
            sendTimer = new System.Timers.Timer(2000);
            sendTimer.Elapsed += Timer_Elapsed;
            sendTimer.AutoReset = true;
            initTime(0, 5, 60);
        }

        void initTime(int hours, int minutes, int interval)
        {
            sendTimer.Enabled = false;
            minuteInteral = interval;
            now = DateTime.Now;
            run = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0);
            span = run - now;
            if (span < TimeSpan.Zero)
                run = run.AddDays(1);
            sendTimer.Enabled = true;
        }
        
        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            now = DateTime.Now;
            span = run - now;
            if (span < TimeSpan.Zero)
            {
                Console.WriteLine(now);
                run = run.AddMinutes(minuteInteral);
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

        async Task<MemoryStream> getStat(string date)
        {
            MemoryStream stream = new MemoryStream();
            await Task.Run(async () => { 

                var users = await dolphin.GetUsers();
                var ids = users.Select(user => user.id).ToArray();
                var userstat = await dolphin.GetStatistics(ids, date, date);

                var intids = users.Select(user => (int)user.id).ToArray();
                var tagstat = new Dictionary<string, Dictionary<uint, Statistics>>();
                string[] tags = new string[] { "IND", "PER", "LAM1", "LAM2", "CHL" };
                foreach (var tag in tags)
                {
                    var accs = await dolphin.GetAccounts(intids, new string[] { tag }, new string[] { "не-обновлять", "без-комментов" });
                    var intIds = accs.Select(acc => acc.id).ToArray();
                    var stat = await dolphin.GetStatisticsByAccounts(intIds, date, date);
                    tagstat.Add(tag, stat);
                }

                stream = pdf.GetPdf(date, users, userstat, tagstat);
            });
            return stream;
        }

        async Task<string> getAndSaveStat(string date)
        {
            string path = "";
            await Task.Run(async () => {

                var users = await dolphin.GetUsers();
                var ids = users.Select(user => user.id).ToArray();
                var userstat = await dolphin.GetStatistics(ids, date, date);

                var intids = users.Select(user => (int)user.id).ToArray();
                var tagstat = new Dictionary<string, Dictionary<uint, Statistics>>();
                //string[] tags = new string[] { "IND", "PER", "AUS" };
                string[] tags = new string[] { "IND", "PER", "LAM1", "LAM2", "CHL" };
                foreach (var tag in tags)
                {
                    var accs = await dolphin.GetAccounts(intids, new string[] { tag }, new string[] { "не-обновлять", "без-комментов" });
                    var intIds = accs.Select(acc => acc.id).ToArray();
                    var stat = await dolphin.GetStatisticsByAccounts(intIds, date, date);
                    tagstat.Add(tag, stat);
                }

                path = pdf.GetAndSavePdf(date, users, userstat, tagstat);
            });
            return path;
        }

        async Task sendStat(long id, Stream stream, string date, string time)
        {
            try
            {
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream, $"{date} {time}.pdf");
                await botClient.SendDocumentAsync(id, inputOnlineFile);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task sendStat()
        {
            await Task.Run(async () =>
            {
                string reqdate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH mm ss");
                string path = await getAndSaveStat(reqdate);
                foreach (var id in userManager.GetIDs())
                {
                    using (var stream = System.IO.File.Open(path, FileMode.Open))
                    {
                        await sendStat(id, stream, date, time);
                    }
                }
            });
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null)
                return;

            var id = update.Message.Chat.Id;

            string name = $"{update.Message.Chat.FirstName} {update.Message.Chat.LastName}";
            string msg = update.Message.Text?.ToLowerInvariant();
            if (msg == null)
                return;


            switch (msg)
            {
                case "5555":
                    userManager.Add(id, name);
                    break;
                case "/getstat":
                    if (userManager.Check(id))
                    {
                        //await sendStat(id);                        
                        string date = DateTime.Now.ToString("yyyy-MM-dd");
                        string time = DateTime.Now.ToString("HH mm ss");
                        MemoryStream stream = await getStat(date);
                        await sendStat(id, stream, date, time);

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
                    int interval = int.Parse(spl[3]);

                    initTime(hh, mm,interval);

                    send($"Следующее время сбора данных: {run}, интервал:{interval} минут", update, cancellationToken);

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
