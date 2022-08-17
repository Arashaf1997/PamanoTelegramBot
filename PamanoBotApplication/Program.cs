﻿using Data;
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
        try
        {
            string Token = "5553588032:AAF4RZHMhr-UMvwlmvZxZ7vrELARfR8_Ero";
            var bot = new Telegram.Bot.TelegramBotClient(Token);
            ReplyKeyboardMarkup mainKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton(""));
            Dictionary<string, int> addProductSteps = new Dictionary<string, int>();
            Dictionary<string, Product> productsToAdd = new Dictionary<string, Product>();
            Dictionary<string, int> orderProductSteps = new Dictionary<string, int>();
            Dictionary<string, Product> productsToOrder = new Dictionary<string, Product>();
            Dictionary<string, Order> orders = new Dictionary<string, Order>();
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
                    if (from.Username != "arashaf1997")
                    {
                        if (usersToAdd.ContainsKey(from.Username))
                        {
                            switch (addUserSteps[from.Username])
                            {
                                case 1:
                                    usersToAdd[from.Username].FullName = text;
                                    addUserSteps[from.Username] = 2;
                                    bot.SendTextMessageAsync(chatId: chatId, text: "لطفا نام فروشگاه خود را ارسال کنید.");
                                    break;
                                case 2:
                                    usersToAdd[from.Username].StoreName = text;
                                    addUserSteps[from.Username] = 3;
                                    bot.SendTextMessageAsync(chatId: chatId, text: "لطفا شماره تماس خود را ارسال کنید.");
                                    break;
                                case 3:
                                    usersToAdd[from.Username].PhoneNumber = text;
                                    addUserSteps[from.Username] = 4;
                                    bot.SendTextMessageAsync(chatId: chatId, text: "لطفا آدرس خود را ارسال کنید.");
                                    break;
                                case 4:
                                    usersToAdd[from.Username].Address = text;
                                    bot.SendTextMessageAsync(chatId: chatId, text: $"{usersToAdd[from.Username].FullName} عزیز ،" + "ثبت نام شما با موفقیت انجام شد.");
                                    usersToAdd[from.Username].InsertTime = DateTime.Now;
                                    context.Users.Add(usersToAdd[from.Username]);
                                    context.SaveChanges();
                                    usersToAdd.Remove(from.Username);
                                    addUserSteps.Remove(from.Username);
                                    break;
                            }
                            if (addUserSteps.ContainsKey(from.Username))
                                break;
                        }

                        if (text.ToLower().Contains("/start"))
                        {
                            if (text.Length > 6)
                            {
                                var stringProductId = text.Substring(7);
                                var productId = Convert.ToInt32(stringProductId);
                                var product = context.Products.FirstOrDefault(p => p.Id.Equals(productId));
                                productsToOrder.Add(from.Username, product);
                                orderProductSteps[from.Username] = 1;
                                if (!context.Users.Any(u => u.UserName.Equals(from.Username)))
                                {
                                    usersToAdd.Add(from.Username, new Domain.User() { UserName = from.Username });
                                    addUserSteps[from.Username] = 1;
                                    bot.SendTextMessageAsync(chatId: chatId, text: "جهت ثبت سفارش لطفا با گذراندن سه مرحله در ربات پامانو ثبت نام کنید. \n" +
                                        "لطفا نام و نام خانوادگی خود را ارسال کنید.");
                                    break;
                                }
                            }
                            else
                                bot.SendTextMessageAsync(chatId: chatId, text: "خوش آمدید. ابتدا از کانال پامانو یک محصول را برای سفارش انتخاب کنید.");
                        }

                        if (productsToOrder.ContainsKey(from.Username))
                        {
                            switch (orderProductSteps[from.Username])
                            {

                                case 1:
                                    var productId = productsToOrder[from.Username].Id;
                                    var productImage = context.ProductImages.Where(p => p.ProductId.Equals(productId)).FirstOrDefault();
                                    var colors1 = productsToOrder[from.Username].Colors.Split(',');

                                    List<List<KeyboardButton>> buttons1 = new List<List<KeyboardButton>>();
                                    for (int i = 0; i < colors1.Count(); i++)
                                    {
                                        List<KeyboardButton> row = new List<KeyboardButton>();
                                        row.Add(new KeyboardButton(colors1[i]));
                                        buttons1.Add(row);
                                    }
                                    ReplyKeyboardMarkup keyboard1 = new ReplyKeyboardMarkup(buttons1);
                                    bot.SendTextMessageAsync(chatId: chatId, text: "شما این محصول را جهت سفارش انتخاب کرده اید :");
                                    bot.SendPhotoAsync(chatId: chatId,
                                        photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(productImage.ImageId),
                                        caption:
                                        $"{productsToOrder[from.Username].Name} \n" +
                                        $"سایزبندی : {productsToOrder[from.Username].Size}\n" +
                                        $"رنگ های موجود : {productsToOrder[from.Username].Colors}\n" +
                                        $"توضیحات : {productsToOrder[from.Username].Description}\n" +
                                        $"قیمت : {productsToOrder[from.Username].Price} تومان");
                                    bot.SendTextMessageAsync(chatId: chatId, text: "رنگ مورد نظر برای سفارش را انتخاب کنید :", replyMarkup: keyboard1);
                                    orderProductSteps[from.Username] = 2;
                                    orders.Add(from.Username, new Order { ProductId = productId, TotalCount = 0 });
                                    break;

                                case 2:
                                    if (productsToOrder[from.Username].Colors.Contains(text))
                                    {
                                        orders[from.Username].Details = orders[from.Username].Details + $"رنگ {text} ";
                                        bot.SendTextMessageAsync(chatId: chatId, text: $"تعداد سری از رنگ {text} را وارد کنید :", replyMarkup: null);
                                        orderProductSteps[from.Username] = 3;
                                    }
                                    break;

                                case 3:
                                    List<List<KeyboardButton>> buttons3 = new List<List<KeyboardButton>>();
                                    List<KeyboardButton> row3 = new List<KeyboardButton>();
                                    row3.Add(new KeyboardButton("بله " + "\U00002714"));
                                    row3.Add(new KeyboardButton("خیر " + "\U00002714"));
                                    buttons3.Add(row3);
                                    ReplyKeyboardMarkup keyboard3 = new ReplyKeyboardMarkup(buttons3);
                                    orders[from.Username].TotalCount = orders[from.Username].TotalCount + Convert.ToInt32(text);
                                    orders[from.Username].Details = orders[from.Username].Details + $"تعداد {text} سری \n";
                                    bot.SendTextMessageAsync(chatId: chatId, text: $"{orders[from.Username].Details} ثبت شد. \n" +
                                        $"آیا میخواهید رنگ دیگری از محصول را سفارش دهید؟", replyMarkup: keyboard3);
                                    orderProductSteps[from.Username] = 4;
                                    break;
                                case 4:
                                    if (text.Contains("بله"))
                                    {
                                        var colors4 = productsToOrder[from.Username].Colors.Split(',');
                                        List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
                                        for (int i = 0; i < colors4.Count(); i++)
                                        {
                                            List<KeyboardButton> row = new List<KeyboardButton>();
                                            row.Add(new KeyboardButton(colors4[i]));
                                            buttons.Add(row);
                                        }
                                        ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);
                                        bot.SendTextMessageAsync(chatId: chatId, text: "رنگ مورد نظر برای سفارش را انتخاب کنید :", replyMarkup: keyboard);
                                        orderProductSteps[from.Username] = 2;
                                    }
                                    else if (text.Contains("خیر"))
                                    {
                                        List<List<KeyboardButton>> buttons4 = new List<List<KeyboardButton>>();
                                        List<KeyboardButton> row4 = new List<KeyboardButton>();
                                        row4.Add(new KeyboardButton("تایید سفارش " + "\U00002714"));
                                        row4.Add(new KeyboardButton("لغو سفارش " + "\U00002714"));
                                        buttons4.Add(row4);
                                        ReplyKeyboardMarkup keyboard4 = new ReplyKeyboardMarkup(buttons4);
                                        var x = bot.SendTextMessageAsync(chatId: chatId, text: $"سفارش شما : {productsToOrder[from.Username].Name} \n" +
                                            $"جزئیات سفارش : {orders[from.Username].Details}\n" +
                                            $"قیمت هر جفت : {productsToOrder[from.Username].Price} تومان \n" +
                                            $"قیمت کلی : {productsToOrder[from.Username].Price * productsToOrder[from.Username].SeriesCount * orders[from.Username].TotalCount} تومان \n" +
                                            $"در صورت لزوم میتوانید برای ما توضیحاتی در مورد سفارش خود ارسال کنید در غیر اینصورت یک گزینه را انتخاب کنید."
                                            , replyMarkup: keyboard4);
                                        orderProductSteps[from.Username] = 5;
                                    }
                                    break;
                                case 5:
                                    if (text.Contains("تایید سفارش"))
                                    {
                                        var userName = from.Username;
                                        var user = context.Users.FirstOrDefault(u => u.UserName.Equals(userName));
                                        orders[from.Username].TotalPrice = productsToOrder[from.Username].Price * productsToOrder[from.Username].SeriesCount * orders[from.Username].TotalCount;
                                        orders[from.Username].InsertTime = DateTime.Now;
                                        orders[from.Username].UserId = user.Id;
                                        context.Orders.Add(orders[from.Username]);
                                        context.SaveChanges();
                                        bot.SendTextMessageAsync(chatId: chatId, text: "سفارش شما با موفقیت ثبت شد. تیم پشتیبانی با شما در ارتباط خواهد بود. با تشکر");
                                        bot.SendTextMessageAsync(chatId: "@PamanoShoes", text: $"یک سفارش جدید توسط {user.FullName} ثبت شد.\n" +
                                            $"محصول : {productsToOrder[from.Username].Name}\n" +
                                            $"جزئیات : {orders[from.Username].Details}\n" +
                                            $"تاریخ ثبت : {orders[from.Username].InsertTime}\n" +
                                            $"توضیحات سفارش : {orders[from.Username].CustomerDescription}\n" +
                                            $"نام فروشگاه : {user.StoreName}\n" +
                                            $"شماره تماس : {user.PhoneNumber}\n" +
                                            $"آدرس : {user.Address}");

                                        orders.Remove(from.Username);
                                        orderProductSteps.Remove(from.Username);
                                        productsToOrder.Remove(from.Username);
                                    }
                                    else if (text.Contains("لغو سفارش"))
                                    {
                                        bot.SendTextMessageAsync(chatId: chatId, text: "سفارش شما لغو شد. جهت سفارش محصول جدید به کانال پامانو مراجعه کنید.");
                                        orders.Remove(from.Username);
                                        orderProductSteps.Remove(from.Username);
                                        productsToOrder.Remove(from.Username);
                                    }
                                    else
                                    {
                                        var userName = from.Username;
                                        var user = context.Users.FirstOrDefault(u => u.UserName.Equals(userName));
                                        orders[from.Username].CustomerDescription = text;
                                        orders[from.Username].TotalPrice = productsToOrder[from.Username].Price * productsToOrder[from.Username].SeriesCount * orders[from.Username].TotalCount;
                                        orders[from.Username].InsertTime = DateTime.Now;
                                        orders[from.Username].UserId = user.Id;
                                        context.Orders.Add(orders[from.Username]);
                                        context.SaveChanges();
                                        bot.SendTextMessageAsync(chatId: chatId, text: "سفارش شما با موفقیت ثبت شد. تیم پشتیبانی با شما در ارتباط خواهد بود. با تشکر");
                                        bot.SendTextMessageAsync(chatId: "@PamanoShoes", text: $"یک سفارش جدید توسط {user.FullName} ثبت شد.\n" +
                                            $"محصول : {productsToOrder[from.Username].Name}\n" +
                                            $"جزئیات : {orders[from.Username].Details}\n" +
                                            $"تاریخ ثبت : {orders[from.Username].InsertTime}\n" +
                                            $"توضیحات سفارش : {orders[from.Username].CustomerDescription}\n" +
                                            $"نام فروشگاه : {user.StoreName}\n" +
                                            $"شماره تماس : {user.PhoneNumber}\n" +
                                            $"آدرس : {user.Address}");
                                        orders.Remove(from.Username);
                                        orderProductSteps.Remove(from.Username);
                                        productsToOrder.Remove(from.Username);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (text.ToLower().Contains("/start"))
                        {
                            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
                            List<KeyboardButton> row = new List<KeyboardButton>();
                            row.Add(new KeyboardButton("ثبت محصول جدید " + "\U00002714"));
                            row.Add(new KeyboardButton("دریافت لیست محصولات " + "\U00002714"));
                            row.Add(new KeyboardButton("درخواست های خرید " + "\U00002714"));
                            buttons.Add(row);
                            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);
                            bot.SendTextMessageAsync(chatId: chatId, text: "مدیر عزیز خوش آمدید", replyMarkup: keyboard);
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
                                try
                                {
                                    productsToAdd[from.Username].Price = Int32.Parse(text);
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
                                bot.SendTextMessageAsync(chatId: chatId, text: "تعداد محصول در هر سری را وارد کنید");
                                addProductSteps[from.Username] = 3;
                            }
                            else if (step == 3)
                            {
                                try
                                {
                                    productsToAdd[from.Username].SeriesCount = Int32.Parse(text);
                                }
                                catch
                                {
                                    bot.SendTextMessageAsync(chatId: chatId, text: "تعداد در هر سری باید با اعداد انگلیسی ارسال شود");
                                    continue;
                                }
                                bot.SendTextMessageAsync(chatId: chatId, text: "رنگ های محصول را وارد کنید");
                                addProductSteps[from.Username] = 4;
                            }
                            else if (step == 4)
                            {
                                if (text.Contains("مرحله بعد"))
                                {
                                    bot.SendTextMessageAsync(chatId: chatId, text: "توضیحات محصول را وارد کنید");
                                    addProductSteps[from.Username] = 5;
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
                            else if (step == 5)
                            {

                                Product product = productsToAdd[from.Username];
                                product.Description = text;

                                bot.SendTextMessageAsync(chatId: chatId, text: "عکس محصول را ارسال کنید");
                                product.InsertTime = DateTime.Now;
                                product.UserId = 1;
                                context.Products.Add(product);
                                productsToAdd[from.Username] = product;
                                addProductSteps[from.Username] = 6;
                                context.SaveChanges();
                            }
                            else if (step == 6)
                            {
                                if (text.Contains("ثبت سفارش"))
                                {
                                    var pId = productsToAdd[from.Username].Id;
                                    bot.SendTextMessageAsync(chatId: chatId, text: "محصول با موفقیت به کانال اضافه شد.");
                                    List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
                                    List<InlineKeyboardButton> row1 = new List<InlineKeyboardButton>();
                                    row1.Add(InlineKeyboardButton.WithUrl("\U00002934" + " سفارش این محصول", $"https://telegram.me/PamanoBot?start={pId}"));
                                    buttons.Add(row1);
                                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

                                    var productImages = context.ProductImages.Where(p => p.ProductId.Equals(pId)).ToList();

                                    if (productImages.Count >= 1)
                                    {
                                        IAlbumInputMedia[] streamArray = new IAlbumInputMedia[productImages.Count()];
                                        for (int i = 0; i < productImages.Count(); i++)
                                        {
                                            streamArray[i] = new InputMediaPhoto(new InputMedia(productImages[i].ImageId.ToString()));
                                        }
                                        var sended = bot.SendMediaGroupAsync("@PamanoShoes", streamArray);
                                        bot.SendTextMessageAsync(chatId: "@PamanoShoes", replyToMessageId: sended.Result[0].MessageId,
                                                text:
                                                $"نام محصول : {productsToAdd[from.Username].Name} \n" +
                                                $"سایزبندی : {productsToAdd[from.Username].Size}\n" +
                                                $"رنگ های موجود : {productsToAdd[from.Username].Colors}\n" +
                                                $"تعداد جفت در هر سری : {productsToAdd[from.Username].SeriesCount} جفت\n" +
                                                $"توضیحات : {productsToAdd[from.Username].Description}\n" +
                                                $"قیمت : {productsToAdd[from.Username].Price} تومان"
                                                , replyMarkup: keyboard);
                                    }
                                    else
                                    {
                                        bot.SendPhotoAsync(chatId: "@PamanoShoes",
                                            photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(productImages[0].ImageId),
                                            caption:
                                            $"نام محصول : {productsToAdd[from.Username].Name} \n" +
                                            $"سایزبندی : {productsToAdd[from.Username].Size}\n" +
                                            $"رنگ های موجود : {productsToAdd[from.Username].Colors}\n" +
                                            $"تعداد جفت در هر سری : {productsToAdd[from.Username].SeriesCount} جفت \n" +
                                            $"توضیحات : {productsToAdd[from.Username].Description}\n" +
                                            $"قیمت : {productsToAdd[from.Username].Price} تومان"
                                            , replyMarkup: keyboard);
                                    }

                                    addProductSteps.Remove(from.Username);
                                    productsToAdd.Remove(from.Username);

                                }
                                else
                                {
                                    var productId = productsToAdd[from.Username].Id;
                                    ProductImage productImage = new ProductImage();
                                    productImage.ImageId = update.Message.Photo[0].FileId;
                                    productImage.ProductId = productId;
                                    productImage.InsertTime = DateTime.Now;
                                    context.ProductImages.Add(productImage);
                                    context.SaveChanges();

                                    List<List<KeyboardButton>> buttons6 = new List<List<KeyboardButton>>();
                                    List<KeyboardButton> row6 = new List<KeyboardButton>();
                                    row6.Add(new KeyboardButton("ثبت سفارش " + "\U00002714"));
                                    buttons6.Add(row6);
                                    ReplyKeyboardMarkup keyboard6 = new ReplyKeyboardMarkup(buttons6);

                                    bot.SendTextMessageAsync(chatId: chatId, text: "عکس بعدی محصول را ارسال کنید در غیر اینصورت دکمه ثبت سفارش را لمس کنید.", replyMarkup: keyboard6);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }
    }
}

