using System.Globalization;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    //static ProductContext context = new ProductContext();

    static void Main(string[] args)
    {
        Thread botThread = new Thread(runBot);
        botThread.Start();
    }

    private static void runBot()
    {
        try
        {
            string Token = "6113640221:AAHBXfIc7IpV1vcFexh32HY8ctabNaUeB3w";
            var bot = new Telegram.Bot.TelegramBotClient(Token);
            int offset = 0;

            while (true)
            {
                try
                {
                    Console.WriteLine("Lets getUpdates!");
                    Telegram.Bot.Types.Update[] updates = bot.GetUpdatesAsync(offset).Result;
                    Console.WriteLine("getUpdates Done!");
                    foreach (var update in updates)
                    {

                        Console.WriteLine($"offset : {offset} Done!");
                        offset = update.Id + 1;
                        if (update.Message == null)
                            continue;
                        if (update.Message.From == null)
                            continue;
                        var text = update.Message.Text == null ? "" : update.Message.Text;
                        var from = update.Message.From;
                        var chatId = update.Message.Chat.Id;
                        if (update.Message.LeftChatMember != null)
                        {
                            bot.DeleteMessageAsync(chatId, update.Message.MessageId);
                            Console.WriteLine($"One message Deleted, {update.Message.From.Username} Removed {update.Message.LeftChatMember.Username}");
                        }
                        else if (update.Message.NewChatMembers != null)
                        {
                            bot.DeleteMessageAsync(chatId, update.Message.MessageId);
                            Console.WriteLine($"One message Deleted, {update.Message.From.Username} Added {update.Message.NewChatMembers.ToString()}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    public static string ToPersianDateTime(DateTime datetime)
    {
        return datetime.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")) + " " + datetime.Hour + ":" + datetime.Minute + ":" + datetime.Second;
    }
}

