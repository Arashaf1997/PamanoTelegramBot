using Data;
using Domain;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    static ProductContext context = new ProductContext();

    static void Main(string[] args)
    {
        Thread botThread = new Thread(runBot);
        botThread.Start();
    }

    private static void runBot()
    {
        string Token = "5553588032:AAF4RZHMhr-UMvwlmvZxZ7vrELARfR8_Ero";
        var bot = new Telegram.Bot.TelegramBotClient(Token);
        ReplyKeyboardMarkup mainKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton(""));
        Dictionary<string, int> addProductSteps = new Dictionary<string, int>();
        Dictionary<string, Product> productsToAdd = new Dictionary<string, Product>();

        int offset = 0;

        while (true)
        {
            Telegram.Bot.Types.Update[] updates = bot.GetUpdatesAsync(offset).Result;

            foreach (var update in updates)
            {
                offset = update.Id + 1;

                if (update.Message == null)
                    continue;
                if (update.Message.Text == null)
                    continue;
                if (update.Message.From == null)
                    continue;
                var text = update.Message.Text;
                var from = update.Message.From;
                var chatId = update.Message.Chat.Id;
                if (from.Username != "arashaf1997")
                {

                    if (text.ToLower().Contains("/start"))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(from.Username + "به ربات پامانو خوش آمدید");
                        sb.AppendLine("میتوانید از امکانات ما استفاده کنید");
                        sb.AppendLine("درباره ما : /AboutUs");
                        sb.AppendLine("تماس با ما : /ContactUs");
                        sb.AppendLine("آدرس ما : /Address");
                        KeyboardButton[] row1 =
                            {
                                 new KeyboardButton ("درباره ما " + "\U00002764"), new KeyboardButton("تماس با ما " + "\U00002709")
                             };
                        KeyboardButton[] row2 =
                            {
                                  new KeyboardButton ("آدرس ما " + "\U0001F680"), new KeyboardButton("نظر سنجی " + "\U0000270F")
                              };
                        KeyboardButton[][] keyboardButtons =
                            {
                                   row1,row2
                             };
                        mainKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);
                        bot.SendTextMessageAsync(chatId: chatId, text: sb.ToString(), replyMarkup: mainKeyboardMarkup);
                    }
                    else if (text.ToLower().Contains("/aboutus") || text.ToLower().Contains("درباره ما"))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("ما خیلی خوبیم");
                        bot.SendTextMessageAsync(chatId, sb.ToString());
                    }
                    else if (text.ToLower().Contains("/contactus") || text.ToLower().Contains("تماس با ما"))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("آدرس ایمیل : Arash.Af1997@Gmail.Com");
                        sb.AppendLine("شماره تماس : 09308545585");

                        KeyboardButton[] row1 =
              {
                 new KeyboardButton ("تماس با مدیریت " + "\U00002714"), new KeyboardButton("تماس با پشتیبانی " + "\U00002714"),new KeyboardButton("تماس با بخش فروش " + "\U00002714")
            };
                        KeyboardButton[] row2 =
              {
                 new KeyboardButton ("بازگشت به منوی اصلی " + "\U00002716")
            };
                        KeyboardButton[][] keyboardButtons =
                        {
                row1,row2
            };
                        mainKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);

                        bot.SendTextMessageAsync(chatId: chatId, text: sb.ToString(), replyMarkup: mainKeyboardMarkup);
                    }
                    else if (text.ToLower().Contains("/address") || text.ToLower().Contains("آدرس ما"))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("یک جا که غم نباشه :)");
                        bot.SendTextMessageAsync(chatId, sb.ToString());
                    }
                    else if (text.ToLower().Contains("بازگشت به منوی اصلی"))
                    {
                        KeyboardButton[] row1 =
                {
                 new KeyboardButton ("درباره ما " + "\U00002764"), new KeyboardButton("تماس با ما " + "\U00002709")
            };
                        KeyboardButton[] row2 =
                            {
                 new KeyboardButton ("آدرس ما " + "\U0001F680"), new KeyboardButton("نظر سنجی " + "\U0000270F")
            };
                        KeyboardButton[][] keyboardButtons =
                        {
                row1,row2
            };
                        mainKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);

                        bot.SendTextMessageAsync(chatId: chatId, text: "منوی اصلی", replyMarkup: mainKeyboardMarkup);
                    }
                }
                else
                {
                    if (text.ToLower().Contains("/start"))
                    {
                        KeyboardButton[] row1 =
          {
                 new KeyboardButton ("ثبت محصول جدید " + "\U00002714"), new KeyboardButton("دریافت لیست محصولات " + "\U00002714"),new KeyboardButton("درخواست های خرید " + "\U00002714")
            };
                        KeyboardButton[][] keyboardButtons =
                        {
                row1
            };
                        mainKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);

                        bot.SendTextMessageAsync(chatId: chatId, text: "مدیر عزیز خوش آمدید", replyMarkup: mainKeyboardMarkup);
                    }
                    else if (text.ToLower().Contains("ثبت محصول جدید"))
                    {
                        addProductSteps.Add(from.Username, 0);
                        bot.SendTextMessageAsync(chatId: chatId, text: "نام محصول را وارد کنید", replyMarkup: mainKeyboardMarkup);
                    }
                    else if (addProductSteps.ContainsKey(from.Username))
                    {
                        var step = addProductSteps.FirstOrDefault(a => a.Key == from.Username).Value;
                        if (step == 0)
                        {
                            productsToAdd.Add(from.Username, new Product { Name = text });
                            bot.SendTextMessageAsync(chatId: chatId, text: "قیمت محصول را به تومان وارد کنید", replyMarkup: mainKeyboardMarkup);
                            addProductSteps[from.Username] = 1;
                        }
                        else if (step == 1)
                        {
                            var product = productsToAdd[from.Username];
                            try
                            {
                                product.Price = Int32.Parse(text);
                            }
                            catch
                            {
                                bot.SendTextMessageAsync(chatId: chatId, text: "قیمت محصول باید با اعداد انگلیسی ارسال شود", replyMarkup: mainKeyboardMarkup);
                                continue;
                            }
                            bot.SendTextMessageAsync(chatId: chatId, text: "توضیحات محصول را وارد کنید", replyMarkup: mainKeyboardMarkup);
                            addProductSteps[from.Username] = 2;
                        }
                        else if (step == 2)
                        {
                            Product product = productsToAdd[from.Username];
                            product.Description = text;
                            bot.SendTextMessageAsync(chatId: chatId, text: "محصول با موفقیت به کانال اضافه شد", replyMarkup: mainKeyboardMarkup);
                            //addProductSteps[from.Username] = 3;
                            addProductSteps.Remove(from.Username);
                            productsToAdd.Remove(from.Username);
                            product.InsertTime = DateTime.Now;
                            product.UserId = 1;
                            context.Products.Add(product);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }
    }

}