using Data;
using Domain;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
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
        Dictionary<string, int> orderProductSteps = new Dictionary<string, int>();
        Dictionary<string, Product> productsToOrder = new Dictionary<string, Product>();
        Dictionary<string, int> addUserSteps = new Dictionary<string, int>();
        Dictionary<string, Domain.User> usersToAdd = new Dictionary<string, Domain.User>();

        int offset = 0;

        while (true)
        {
            Telegram.Bot.Types.Update[] updates = bot.GetUpdatesAsync(offset).Result;

            foreach (var update in updates)
            {
                offset = update.Id + 1;

                if (update.Message == null)
                    continue;
                //if (update.Message.Text == null)
                //    continue;
                if (update.Message.From == null)
                    continue;
                var text = update.Message.Text == null ? "" : update.Message.Text;
                var from = update.Message.From;
                var chatId = update.Message.Chat.Id;
                if (from.Username != "arashaf199")
                {
                    if (usersToAdd.ContainsKey(from.Username))
                    {
                        if (addUserSteps[from.Username] == 1)
                        {
                            usersToAdd[from.Username].FullName = text;
                            addUserSteps[from.Username] = 2;
                            bot.SendTextMessageAsync(chatId: chatId, text: "لطفا شماره تماس خود را ارسال کنید.");
                            break;
                        }
                        else if (addUserSteps[from.Username] == 2)
                        {
                            usersToAdd[from.Username].PhoneNumber = text;
                            addUserSteps[from.Username] = 3;
                            bot.SendTextMessageAsync(chatId: chatId, text: "لطفا آدرس خود را ارسال کنید.");
                            break;
                        }
                        else if (addUserSteps[from.Username] == 3)
                        {
                            usersToAdd[from.Username].Address = text;
                            addUserSteps[from.Username] = 3;
                            bot.SendTextMessageAsync(chatId: chatId, text: $"{usersToAdd[from.Username].FullName} عزیز ،" + "ثبت نام شما با موفقیت انجام شد.");
                            usersToAdd[from.Username].InsertTime = DateTime.Now;
                            context.Users.Add(usersToAdd[from.Username]);
                            context.SaveChanges();
                            usersToAdd.Remove(from.Username);
                            addUserSteps.Remove(from.Username);
                        }
                    }

                    if (productsToOrder.ContainsKey(from.Username))
                    {
                        if (orderProductSteps[from.Username] == 1)
                        {
                            var productId = productsToOrder[from.Username].Id;
                            var productImage = context.ProductImages.Where(p => p.ProductId.Equals(productId)).FirstOrDefault();
                            bot.SendTextMessageAsync(chatId: chatId, text: "شما این محصول را جهت سفارش انتخاب کرده اید :");
                            bot.SendPhotoAsync(chatId: chatId,
                                photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(productImage.ImageId),
                                caption:
                                $"نام محصول : {productsToOrder[from.Username].Name} \n" +
                                $"سایزبندی : {productsToOrder[from.Username].Size}\n" +
                                $"رنگ های موجود : {productsToOrder[from.Username].Colors}\n" +
                                $"توضیحات : {productsToOrder[from.Username].Description}\n" +
                                $"قیمت : {productsToOrder[from.Username].Price} تومان");
                        }
                    }

                    if (text.ToLower().Contains("/start"))
                    {
                        var stringProductId = text.Substring(7);
                        if (stringProductId != null)
                        {
                            productsToOrder.Add(from.Username, new Product() { Id = Convert.ToInt32(stringProductId) });
                            orderProductSteps[from.Username] = 1;

                            if (!context.Users.Any(u => u.UserName.Equals(from.Username)))
                            {
                                usersToAdd.Add(from.Username , new Domain.User() { UserName = from.Username}) ;
                                addUserSteps[from.Username] = 1;
                                bot.SendTextMessageAsync(chatId: chatId, text: "جهت ثبت سفارش لطفا با گذراندن سه مرحله در ربات پامانو ثبت نام کنید." +
                                    "لطفا نام و نام خانوادگی خود را ارسال کنید.");
                                break;
                            }

                        }
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
                    //        else if (text.ToLower().Contains("/aboutus") || text.ToLower().Contains("درباره ما"))
                    //        {
                    //            StringBuilder sb = new StringBuilder();
                    //            sb.AppendLine("ما خیلی خوبیم");
                    //            bot.SendTextMessageAsync(chatId, sb.ToString());
                    //        }
                    //        else if (text.ToLower().Contains("/contactus") || text.ToLower().Contains("تماس با ما"))
                    //        {
                    //            StringBuilder sb = new StringBuilder();
                    //            sb.AppendLine("آدرس ایمیل : Arash.Af1997@Gmail.Com");
                    //            sb.AppendLine("شماره تماس : 09308545585");

                    //            KeyboardButton[] row1 =
                    //  {
                    //     new KeyboardButton ("تماس با مدیریت " + "\U00002714"), new KeyboardButton("تماس با پشتیبانی " + "\U00002714"),new KeyboardButton("تماس با بخش فروش " + "\U00002714")
                    //};
                    //            KeyboardButton[] row2 =
                    //  {
                    //     new KeyboardButton ("بازگشت به منوی اصلی " + "\U00002716")
                    //};
                    //            KeyboardButton[][] keyboardButtons =
                    //            {
                    //    row1,row2
                    //};
                    //            mainKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);

                    //            bot.SendTextMessageAsync(chatId: chatId, text: sb.ToString(), replyMarkup: mainKeyboardMarkup);
                    //        }
                    //        else if (text.ToLower().Contains("/address") || text.ToLower().Contains("آدرس ما"))
                    //        {
                    //            StringBuilder sb = new StringBuilder();
                    //            sb.AppendLine("یک جا که غم نباشه :)");
                    //            bot.SendTextMessageAsync(chatId, sb.ToString());
                    //        }
                    //        else if (text.ToLower().Contains("بازگشت به منوی اصلی"))
                    //        {
                    //            KeyboardButton[] row1 =
                    //    {
                    //     new KeyboardButton ("درباره ما " + "\U00002764"), new KeyboardButton("تماس با ما " + "\U00002709")
                    //};
                    //            KeyboardButton[] row2 =
                    //                {
                    //     new KeyboardButton ("آدرس ما " + "\U0001F680"), new KeyboardButton("نظر سنجی " + "\U0000270F")
                    //};
                    //            KeyboardButton[][] keyboardButtons =
                    //            {
                    //    row1,row2
                    //};
                    //            mainKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);

                    //            bot.SendTextMessageAsync(chatId: chatId, text: "منوی اصلی", replyMarkup: mainKeyboardMarkup);
                    //        }
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
                        bot.SendTextMessageAsync(chatId: chatId, text: "نام محصول را وارد کنید", replyMarkup: default);
                    }
                    else if (addProductSteps.ContainsKey(from.Username))
                    {
                        var step = addProductSteps.FirstOrDefault(a => a.Key == from.Username).Value;
                        if (step == 0)
                        {
                            productsToAdd.Add(from.Username, new Product { Name = text });
                            bot.SendTextMessageAsync(chatId: chatId, text: "قیمت محصول را به تومان وارد کنید");
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
                                bot.SendTextMessageAsync(chatId: chatId, text: "قیمت محصول باید با اعداد انگلیسی ارسال شود");
                                continue;
                            }
                            bot.SendTextMessageAsync(chatId: chatId, text: "سایزبندی محصول را وارد کنید");
                            addProductSteps[from.Username] = 2;
                        }
                        else if (step == 2)
                        {
                            productsToAdd[from.Username].Size = text;
                            bot.SendTextMessageAsync(chatId: chatId, text: "رنگ های محصول را وارد کنید");
                            addProductSteps[from.Username] = 3;
                        }
                        else if (step == 3)
                        {
                            if (text.Contains("مرحله بعد"))
                            {
                                bot.SendTextMessageAsync(chatId: chatId, text: "توضیحات محصول را وارد کنید");
                                addProductSteps[from.Username] = 4;
                            }
                            else
                            {
                                if (productsToAdd[from.Username].Colors == null)
                                    productsToAdd[from.Username].Colors = text;
                                else
                                    productsToAdd[from.Username].Colors = productsToAdd[from.Username].Colors + ',' + text;


                                List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
                                List<KeyboardButton> row1 = new List<KeyboardButton>();
                                row1.Add(new KeyboardButton("مرحله بعد " + "\U00002714"));
                                buttons.Add(row1);
                                ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);
                                bot.SendTextMessageAsync(chatId: chatId, text: "رنگ را وارد کنید در غیر اینصورت مرحله بعد را بزنید", replyMarkup: keyboard);
                            }
                        }
                        else if (step == 4)
                        {

                            Product product = productsToAdd[from.Username];
                            product.Description = text;

                            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
                            List<KeyboardButton> row1 = new List<KeyboardButton>();
                            row1.Add(new KeyboardButton("مرحله بعد " + "\U00002714"));
                            buttons.Add(row1);
                            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);

                            //                      KeyboardButton[] row1 =
                            //{
                            //           new KeyboardButton ("مرحله بعد " + "\U00002714")
                            //      };
                            //                      KeyboardButton[][] keyboardButtons =
                            //                      {
                            //          row1
                            //      };
                            //mainKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);
                            bot.SendTextMessageAsync(chatId: chatId, text: "عکس محصول را ارسال کنید");
                            addProductSteps[from.Username] = 5;
                            product.InsertTime = DateTime.Now;
                            product.UserId = 1;
                            context.Products.Add(product);
                            productsToAdd[from.Username] = product;
                            context.SaveChanges();
                        }
                        else if (step == 5)
                        {
                            var productId = productsToAdd[from.Username].Id;

                            ProductImage productImage = new ProductImage();
                            productImage.ImageId = update.Message.Photo[0].FileId;
                            productImage.ProductId = productId;
                            productImage.InsertTime = DateTime.Now;
                            context.ProductImages.Add(productImage);
                            context.SaveChanges();
                            //bot.SendTextMessageAsync(chatId: chatId, text: "عکس بعدی محصول را ارسال کنید در غیر اینصورت دکمه مرحله بعد را لمس کنید.", replyMarkup: mainKeyboardMarkup);
                            //addProductSteps[from.Username] = 3;

                            bot.SendTextMessageAsync(chatId: chatId, text: "محصول با موفقیت به کانال اضافه شد");

                            //var insertedProductImage = context.ProductImages.Where(p => p.ProductId.Equals(productId)).FirstOrDefault();

                            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
                            List<InlineKeyboardButton> row1 = new List<InlineKeyboardButton>();
                            row1.Add(InlineKeyboardButton.WithUrl("\U00002934" + " سفارش این محصول", $"https://telegram.me/PamanoBot?start={productId}"));
                            buttons.Add(row1);
                            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

                            bot.SendPhotoAsync(chatId: "@PamanoShoes",
                                photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(productImage.ImageId),
                                caption:
                                $"نام محصول : {productsToAdd[from.Username].Name} \n" +
                                $"سایزبندی : {productsToAdd[from.Username].Size}\n" +
                                $"رنگ های موجود : {productsToAdd[from.Username].Colors}\n" +
                                $"توضیحات : {productsToAdd[from.Username].Description}\n" +
                                $"قیمت : {productsToAdd[from.Username].Price} تومان"
                                , replyMarkup: keyboard);

                            addProductSteps.Remove(from.Username);
                            productsToAdd.Remove(from.Username);

                            //IAlbumInputMedia[] streamArray = new IAlbumInputMedia[productImages.Count()];
                            //for (int i = 0; i < productImages.Count(); i++)
                            //{
                            //    streamArray[i] = new InputMediaPhoto(new InputMedia(productImages[i].ImageId.ToString()));
                            //}
                            //bot.SendMediaGroupAsync(chatId, streamArray);
                        }
                    }
                }
            }
        }
    }
}

