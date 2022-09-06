using Data;
using Domain;
using System.Globalization;
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
            Dictionary<long, int> addProductSteps = new Dictionary<long, int>();
            Dictionary<long, Product> productsToAdd = new Dictionary<long, Product>();
            Dictionary<long, int> orderProductSteps = new Dictionary<long, int>();
            Dictionary<long, Product> productsToOrder = new Dictionary<long, Product>();
            Dictionary<long, Order> orders = new Dictionary<long, Order>();
            Dictionary<long, int> addUserSteps = new Dictionary<long, int>();
            Dictionary<long, Domain.User> usersToAdd = new Dictionary<long, Domain.User>();
            ReplyKeyboardMarkup cancelButton = new ReplyKeyboardMarkup(new List<List<KeyboardButton>> { new List<KeyboardButton> { new KeyboardButton("لغو عملیات") } });
            cancelButton.ResizeKeyboard = true;

            int offset = 0;

            while (true)
            {
                try
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
                        if (from.Username != null && from.Username == "arashaf1997")
                        {
                            if (text.ToLower().Contains("لغو عملیات"))
                            {
                                if (addProductSteps.ContainsKey(chatId))
                                {
                                    if (addProductSteps[chatId] == 6)
                                    {
                                        var productId = productsToAdd[chatId].Id;
                                        if (context.Products.Any(p => p.Id.Equals(productId)))
                                        {
                                            var productImagesToDelete = context.ProductImages.Where(pi => pi.ProductId == productId).ToList();
                                            if (productImagesToDelete != null)
                                                foreach (var item in productImagesToDelete)
                                                    context.ProductImages.Remove(item);
                                            context.Products.Remove(productsToAdd[chatId]);
                                            context.SaveChanges();
                                        }
                                    }
                                    addProductSteps.Remove(chatId);
                                    if (productsToAdd.ContainsKey(chatId))
                                        productsToAdd.Remove(chatId);
                                }
                                if (orderProductSteps.ContainsKey((chatId)))
                                {
                                    orderProductSteps.Remove(chatId);
                                    if (productsToOrder.ContainsKey(chatId))
                                        productsToOrder.Remove(chatId);
                                }
                                List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
                                List<KeyboardButton> row = new List<KeyboardButton>();
                                row.Add(new KeyboardButton("ثبت محصول جدید " + "\U00002714"));
                                row.Add(new KeyboardButton("دریافت لیست محصولات " + "\U00002714"));
                                row.Add(new KeyboardButton("درخواست های خرید " + "\U00002714"));
                                buttons.Add(row);
                                ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);
                                keyboard.ResizeKeyboard = true;
                                bot.SendTextMessageAsync(chatId: chatId, text: "عملیات لغو شد.", replyMarkup: keyboard);
                            }
                            else if (text.ToLower().Contains("/start"))
                            {
                                List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
                                List<KeyboardButton> row = new List<KeyboardButton>();
                                row.Add(new KeyboardButton("ثبت محصول جدید " + "\U00002714"));
                                row.Add(new KeyboardButton("دریافت لیست محصولات " + "\U00002714"));
                                row.Add(new KeyboardButton("درخواست های خرید " + "\U00002714"));
                                buttons.Add(row);
                                ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);
                                keyboard.ResizeKeyboard = true;
                                bot.SendTextMessageAsync(chatId: chatId, text: "مدیر عزیز خوش آمدید", replyMarkup: keyboard);
                                if (addProductSteps.ContainsKey(chatId))
                                {
                                    addProductSteps.Remove(chatId);
                                    if (productsToAdd.ContainsKey(chatId))
                                        productsToAdd.Remove(chatId);
                                }
                            }
                            else if (text.ToLower().Contains("ثبت محصول جدید"))
                            {
                                addProductSteps.Add(chatId, 0);
                                bot.SendTextMessageAsync(chatId: chatId, text: "نام محصول جدید را وارد کنید", replyMarkup: cancelButton);
                            }
                            else if (addProductSteps.ContainsKey(chatId))
                            {
                                var step = addProductSteps.FirstOrDefault(a => a.Key == chatId).Value;
                                if (step == 0)
                                {
                                    productsToAdd.Add(chatId, new Product { Name = text });
                                    bot.SendTextMessageAsync(chatId: chatId, text: "قیمت محصول را به تومان وارد کنید", replyMarkup: cancelButton);
                                    addProductSteps[chatId] = 1;
                                }
                                else if (step == 1)
                                {
                                    try
                                    {
                                        productsToAdd[chatId].Price = Int32.Parse(text);
                                    }
                                    catch
                                    {
                                        bot.SendTextMessageAsync(chatId: chatId, text: "قیمت محصول باید با اعداد انگلیسی ارسال شود", replyMarkup: cancelButton);
                                        continue;
                                    }
                                    bot.SendTextMessageAsync(chatId: chatId, text: "سایزبندی محصول را وارد کنید", replyMarkup: cancelButton);
                                    addProductSteps[chatId] = 2;
                                }
                                else if (step == 2)
                                {
                                    productsToAdd[chatId].Size = text;
                                    bot.SendTextMessageAsync(chatId: chatId, text: "تعداد محصول در هر سری را وارد کنید", replyMarkup: cancelButton);
                                    addProductSteps[chatId] = 3;
                                }
                                else if (step == 3)
                                {
                                    try
                                    {
                                        productsToAdd[chatId].SeriesCount = Int32.Parse(text);
                                    }
                                    catch
                                    {
                                        bot.SendTextMessageAsync(chatId: chatId, text: "تعداد در هر سری باید با اعداد انگلیسی ارسال شود", replyMarkup: cancelButton);
                                        continue;
                                    }
                                    bot.SendTextMessageAsync(chatId: chatId, text: "رنگ های محصول را به ترتیب وارد کنید", replyMarkup: cancelButton);
                                    addProductSteps[chatId] = 4;
                                }
                                else if (step == 4)
                                {
                                    if (text.Contains("مرحله بعد"))
                                    {
                                        bot.SendTextMessageAsync(chatId: chatId, text: "توضیحات محصول را وارد کنید", replyMarkup: cancelButton);
                                        addProductSteps[chatId] = 5;
                                    }
                                    else
                                    {
                                        if (productsToAdd[chatId].Colors == null)
                                            productsToAdd[chatId].Colors = text;
                                        else
                                            productsToAdd[chatId].Colors = productsToAdd[chatId].Colors + ',' + text;


                                        List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
                                        List<KeyboardButton> row1 = new List<KeyboardButton>();
                                        row1.Add(new KeyboardButton("مرحله بعد " + "\U00002714"));
                                        buttons.Add(row1);
                                        buttons.Add(new List<KeyboardButton> { new KeyboardButton("لغو عملیات") });
                                        ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);
                                        keyboard.ResizeKeyboard = true;
                                        bot.SendTextMessageAsync(chatId: chatId, text: "رنگ بعدی محصول را وارد کنید در غیر اینصورت مرحله بعد را بزنید", replyMarkup: keyboard);
                                    }
                                }
                                else if (step == 5)
                                {

                                    Product product = productsToAdd[chatId];
                                    product.Description = text;

                                    bot.SendTextMessageAsync(chatId: chatId, text: "عکس اول محصول را ارسال کنید", replyMarkup: cancelButton);
                                    product.InsertTime = DateTime.Now;
                                    product.UserId = 1;
                                    context.Products.Add(product);
                                    productsToAdd[chatId] = product;
                                    addProductSteps[chatId] = 6;
                                    context.SaveChanges();
                                }
                                else if (step == 6)
                                {
                                    if (text.Contains("ثبت محصول"))
                                    {
                                        var pId = productsToAdd[chatId].Id;
                                        bot.SendTextMessageAsync(chatId: chatId, text: "محصول با موفقیت به کانال اضافه شد.");
                                        List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
                                        List<InlineKeyboardButton> row1 = new List<InlineKeyboardButton>();
                                        row1.Add(InlineKeyboardButton.WithUrl("\U00002934" + " سفارش این محصول", $"https://telegram.me/PamanoBot?start={pId}"));
                                        buttons.Add(row1);

                                        var productImages = context.ProductImages.Where(p => p.ProductId.Equals(pId)).ToList();

                                        if (productImages.Count > 1)
                                        {
                                            IAlbumInputMedia[] streamArray = new IAlbumInputMedia[productImages.Count()];
                                            for (int i = 0; i < productImages.Count(); i++)
                                                streamArray[i] = new InputMediaPhoto(new InputMedia(productImages[i].ImageId.ToString()));
                                            var sended = bot.SendMediaGroupAsync("@PamanoShoes", streamArray);
                                            bot.EditMessageCaptionAsync(chatId: "@PamanoShoes", messageId: sended.Result[0].MessageId
                                                , caption: $"نام محصول : {productsToAdd[chatId].Name} \n" +
                                                    $"سایزبندی : {productsToAdd[chatId].Size}\n" +
                                                    $"رنگ های موجود : {productsToAdd[chatId].Colors}\n" +
                                                    $"تعداد جفت در هر سری : {productsToAdd[chatId].SeriesCount} جفت\n" +
                                                    $"توضیحات : {productsToAdd[chatId].Description}\n" +
                                                    $"قیمت : {productsToAdd[chatId].Price} تومان\n" +
                                                    $"<a href='https://telegram.me/PamanoBot?start={pId}'>سفارش این محصول</a>"
                                                    , parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                                        }
                                        else
                                        {
                                            bot.SendPhotoAsync(chatId: "@PamanoShoes",
                                                photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(productImages[0].ImageId),
                                                caption:
                                                $"نام محصول : {productsToAdd[chatId].Name} \n" +
                                                $"سایزبندی : {productsToAdd[chatId].Size}\n" +
                                                $"رنگ های موجود : {productsToAdd[chatId].Colors}\n" +
                                                $"تعداد جفت در هر سری : {productsToAdd[chatId].SeriesCount} جفت \n" +
                                                $"توضیحات : {productsToAdd[chatId].Description}\n" +
                                                $"قیمت : {productsToAdd[chatId].Price} تومان\n" +
                                                $"<a href='https://telegram.me/PamanoBot?start={pId}'>سفارش این محصول</a>",
                                                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                                        }
                                        addProductSteps.Remove(chatId);
                                        productsToAdd.Remove(chatId);
                                    }
                                    else
                                    {
                                        var productId = productsToAdd[chatId].Id;
                                        ProductImage productImage = new ProductImage();
                                        productImage.ImageId = update.Message.Photo[0].FileId;
                                        productImage.ProductId = productId;
                                        productImage.InsertTime = DateTime.Now;
                                        context.ProductImages.Add(productImage);
                                        context.SaveChanges();

                                        List<List<KeyboardButton>> buttons6 = new List<List<KeyboardButton>>();
                                        List<KeyboardButton> row6 = new List<KeyboardButton>();
                                        row6.Add(new KeyboardButton("ثبت محصول" + "\U00002714"));
                                        buttons6.Add(row6);
                                        buttons6.Add(new List<KeyboardButton> { new KeyboardButton("لغو سفارش") });
                                        ReplyKeyboardMarkup keyboard6 = new ReplyKeyboardMarkup(buttons6);
                                        keyboard6.ResizeKeyboard = true;
                                        bot.SendTextMessageAsync(chatId: chatId, text: "عکس بعدی محصول را ارسال کنید در غیر اینصورت دکمه ثبت سفارش را لمس کنید.", replyMarkup: keyboard6);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (usersToAdd.ContainsKey(chatId))
                            {
                                if (text.ToLower().Contains("لغو عملیات"))
                                {
                                    usersToAdd.Remove(chatId);
                                    addUserSteps.Remove(chatId);
                                    List<List<KeyboardButton>> button = new List<List<KeyboardButton>>();
                                    List<KeyboardButton> row = new List<KeyboardButton>();
                                    row.Add(new KeyboardButton("ثبت نام" + "\U00002714"));
                                    button.Add(row);
                                    ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(button);
                                    keyboard.ResizeKeyboard = true;
                                    bot.SendTextMessageAsync(chatId: chatId, text: "عملیات ثبت نام لغو شد. جهت شروع مجدد دکمه ثبت نام را بزنید.\n" +
                                        "<a href='https://telegram.me/PamanoShoes'>کانال فروشگاه پامانو</a>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                                    break;
                                }
                                switch (addUserSteps[chatId])
                                {
                                    case 1:
                                        usersToAdd[chatId].FullName = text;
                                        addUserSteps[chatId] = 2;
                                        bot.SendTextMessageAsync(chatId: chatId, text: "لطفا نام فروشگاه خود را ارسال کنید.", replyMarkup: cancelButton);
                                        break;
                                    case 2:
                                        usersToAdd[chatId].StoreName = text;
                                        addUserSteps[chatId] = 3;
                                        bot.SendTextMessageAsync(chatId: chatId, text: "لطفا شماره تماس خود را ارسال کنید.", replyMarkup: cancelButton);
                                        break;
                                    case 3:
                                        usersToAdd[chatId].PhoneNumber = text;
                                        addUserSteps[chatId] = 4;
                                        bot.SendTextMessageAsync(chatId: chatId, text: "لطفا آدرس خود را ارسال کنید.", replyMarkup: cancelButton);
                                        break;
                                    case 4:
                                        usersToAdd[chatId].Address = text;
                                        bot.SendTextMessageAsync(chatId: chatId, text: $"{usersToAdd[chatId].FullName} عزیز ،" + "ثبت نام شما با موفقیت انجام شد.");
                                        usersToAdd[chatId].InsertTime = DateTime.Now;
                                        context.Users.Add(usersToAdd[chatId]);
                                        context.SaveChanges();
                                        usersToAdd.Remove(chatId);
                                        addUserSteps.Remove(chatId);
                                        break;
                                }
                                if (addUserSteps.ContainsKey(chatId))
                                    break;
                            }

                            if (text.ToLower().Contains("/start"))
                            {
                                if (text.Length > 6)
                                {
                                    var stringProductId = text.Substring(7);
                                    var productId = Convert.ToInt32(stringProductId);
                                    var product = context.Products.FirstOrDefault(p => p.Id.Equals(productId));
                                    productsToOrder.Add(chatId, product);
                                    orderProductSteps[chatId] = 1;
                                    if (!context.Users.Any(u => u.ChatId.Equals(chatId)))
                                    {
                                        usersToAdd.Add(chatId, new Domain.User() { UserName = from.Username, ChatId = chatId });
                                        addUserSteps[chatId] = 1;
                                        bot.SendTextMessageAsync(chatId: chatId, text: "جهت انجام سفارش در ربات پامانو مشخصات خود را ثبت کنید. \n" +
                                            "لطفا نام و نام خانوادگی خود را ارسال کنید.", replyMarkup: cancelButton);
                                        break;
                                    }
                                }
                                else
                                {
                                    bot.SendTextMessageAsync(chatId: chatId, text: "خوش آمدید. ابتدا از کانال پامانو یک محصول را برای سفارش انتخاب کنید.");
                                    break;
                                }
                            }
                            else if (text.ToLower().Contains("تماس با ما"))
                            {
                                bot.SendTextMessageAsync(chatId: chatId, text: "جهت ارسال پیام به پشتیبانی پامانو روی لینک زیر کلیک کنید. \n" +
                                            "<a href='https://telegram.me/SabouriSaeed'>پشتیبانی</a>.",parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                                break;
                            }
                            if (productsToOrder.ContainsKey(chatId))
                            {
                                if (text.ToLower().Contains("لغو سفارش"))
                                {
                                    productsToOrder.Remove(chatId);
                                    orderProductSteps.Remove(chatId);
                                    orders.Remove(chatId);
                                    bot.SendTextMessageAsync(chatId: chatId, text: "عملیات ثبت سفارش لغو شد. جهت شروع مجدد یک محصول را جهت سفارش از کانال پامانو انتخاب فرمایید.\n" +
                                        "<a href='https://telegram.me/PamanoShoes'>کانال فروشگاه پامانو</a>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                                    break;
                                }
                                switch (orderProductSteps[chatId])
                                {

                                    case 1:
                                        var productId = productsToOrder[chatId].Id;
                                        var product = context.Products.FirstOrDefault(p => p.Id.Equals(productId));
                                        var productImages = context.ProductImages.Where(p => p.ProductId.Equals(productId)).ToList();
                                        var colors1 = productsToOrder[chatId].Colors.Split(',');

                                        List<List<KeyboardButton>> buttons1 = new List<List<KeyboardButton>>();
                                        for (int i = 0; i < colors1.Count(); i++)
                                        {
                                            List<KeyboardButton> row = new List<KeyboardButton>();
                                            row.Add(new KeyboardButton(colors1[i]));
                                            buttons1.Add(row);
                                        }
                                        buttons1.Add(new List<KeyboardButton> { new KeyboardButton("لغو سفارش") });
                                        ReplyKeyboardMarkup keyboard1 = new ReplyKeyboardMarkup(buttons1);
                                        keyboard1.ResizeKeyboard = true;
                                        bot.SendTextMessageAsync(chatId: chatId, text: "شما این محصول را جهت سفارش انتخاب کرده اید :");
                                        if (productImages.Count > 1)
                                        {
                                            IAlbumInputMedia[] streamArray = new IAlbumInputMedia[productImages.Count()];
                                            for (int i = 0; i < productImages.Count(); i++)
                                                streamArray[i] = new InputMediaPhoto(new InputMedia(productImages[i].ImageId.ToString()));
                                            var sended = bot.SendMediaGroupAsync(chatId, streamArray);
                                            bot.EditMessageCaptionAsync(chatId: chatId, messageId: sended.Result[0].MessageId
                                                , caption: $"نام محصول : {product.Name} \n" +
                                                    $"سایزبندی : {product.Size}\n" +
                                                    $"رنگ های موجود : {product.Colors}\n" +
                                                    $"تعداد جفت در هر سری : {product.SeriesCount} جفت\n" +
                                                    $"توضیحات : {product.Description}\n" +
                                                    $"قیمت : {product.Price} تومان\n");
                                        }
                                        else
                                        {
                                            bot.SendPhotoAsync(chatId: chatId,
                                                photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(productImages[0].ImageId),
                                                caption:
                                                $"نام محصول : {product.Name} \n" +
                                                $"سایزبندی : {product.Size}\n" +
                                                $"رنگ های موجود : {product.Colors}\n" +
                                                $"تعداد جفت در هر سری : {product.SeriesCount} جفت \n" +
                                                $"توضیحات : {product.Description}\n" +
                                                $"قیمت : {product.Price} تومان\n");
                                        }
                                        bot.SendTextMessageAsync(chatId: chatId, text: "رنگ مورد نظر برای سفارش را انتخاب کنید :", replyMarkup: keyboard1);
                                        orderProductSteps[chatId] = 2;
                                        orders.Add(chatId, new Order { ProductId = productId, TotalCount = 0 });
                                        break;

                                    case 2:
                                        if (productsToOrder[chatId].Colors.Contains(text))
                                        {
                                            orders[chatId].Details = orders[chatId].Details + $"رنگ {text} ";
                                            bot.SendTextMessageAsync(chatId: chatId, text: $"تعداد سری از رنگ {text} را وارد کنید :", replyMarkup: cancelButton);
                                            orderProductSteps[chatId] = 3;
                                        }
                                        else
                                        {
                                            bot.SendTextMessageAsync(chatId: chatId, text: "رنگ مورد نظر برای سفارش را از بین گزینه ها انتخاب کنید");
                                        }
                                        break;

                                    case 3:
                                        List<List<KeyboardButton>> buttons3 = new List<List<KeyboardButton>>();
                                        List<KeyboardButton> row3 = new List<KeyboardButton>();
                                        row3.Add(new KeyboardButton("بله " + "\U00002714"));
                                        row3.Add(new KeyboardButton("خیر " + "\U00002714"));
                                        buttons3.Add(row3);
                                        buttons3.Add(new List<KeyboardButton> { new KeyboardButton("لغو سفارش") });
                                        ReplyKeyboardMarkup keyboard3 = new ReplyKeyboardMarkup(buttons3);
                                        try
                                        {
                                            orders[chatId].TotalCount = orders[chatId].TotalCount + Convert.ToInt32(text);
                                        }
                                        catch
                                        {
                                            bot.SendTextMessageAsync(chatId: chatId, text: "تعداد هر سری باید با اعداد انگلیسی ارسال شود", replyMarkup: keyboard3);
                                            break ;
                                        }
                                        if (orders[chatId].Details == null)
                                            orders[chatId].Details = orders[chatId].Details + $"تعداد {text} سری";
                                        else
                                            orders[chatId].Details = orders[chatId].Details + $"\nتعداد {text} سری";
                                        bot.SendTextMessageAsync(chatId: chatId, text: $"{orders[chatId].Details} ثبت شد. \n" +
                                            $"آیا میخواهید رنگ دیگری از محصول را سفارش دهید؟", replyMarkup: keyboard3);
                                        orderProductSteps[chatId] = 4;
                                        break;
                                    case 4:
                                        if (text.Contains("بله"))
                                        {
                                            var colors4 = productsToOrder[chatId].Colors.Split(',');
                                            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();
                                            for (int i = 0; i < colors4.Count(); i++)
                                            {
                                                List<KeyboardButton> row = new List<KeyboardButton>();
                                                row.Add(new KeyboardButton(colors4[i]));
                                                buttons.Add(row);
                                            }
                                            buttons.Add(new List<KeyboardButton> { new KeyboardButton("لغو سفارش") });
                                            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);
                                            keyboard.ResizeKeyboard = true;
                                            bot.SendTextMessageAsync(chatId: chatId, text: "رنگ مورد نظر برای سفارش را انتخاب کنید :", replyMarkup: keyboard);
                                            orderProductSteps[chatId] = 2;
                                        }
                                        else if (text.Contains("خیر"))
                                        {
                                            List<List<KeyboardButton>> buttons4 = new List<List<KeyboardButton>>();
                                            List<KeyboardButton> row4 = new List<KeyboardButton>();
                                            row4.Add(new KeyboardButton("تایید سفارش " + "\U00002714"));
                                            row4.Add(new KeyboardButton("لغو سفارش " + "\U00002714"));
                                            buttons4.Add(row4);
                                            ReplyKeyboardMarkup keyboard4 = new ReplyKeyboardMarkup(buttons4);
                                            var x = bot.SendTextMessageAsync(chatId: chatId, text: $"سفارش شما : {productsToOrder[chatId].Name} \n" +
                                                $"جزئیات سفارش : {orders[chatId].Details}\n" +
                                                $"قیمت هر جفت : {productsToOrder[chatId].Price} تومان \n" +
                                                $"تعداد : {productsToOrder[chatId].SeriesCount * orders[chatId].TotalCount} جفت \n" +
                                                $"قیمت کلی : {productsToOrder[chatId].Price * productsToOrder[chatId].SeriesCount * orders[chatId].TotalCount} تومان \n" +
                                                $"در صورت لزوم میتوانید برای ما توضیحاتی در مورد سفارش خود ارسال کنید در غیر اینصورت یک گزینه را انتخاب کنید."
                                                , replyMarkup: keyboard4);
                                            orderProductSteps[chatId] = 5;
                                        }
                                        break;
                                    case 5:
                                        if (text.Contains("تایید سفارش"))
                                        {
                                            var user = context.Users.FirstOrDefault(u => u.ChatId.Equals(chatId));
                                            orders[chatId].TotalPrice = productsToOrder[chatId].Price * productsToOrder[chatId].SeriesCount * orders[chatId].TotalCount;
                                            orders[chatId].InsertTime = DateTime.Now;
                                            orders[chatId].UserId = user.Id;
                                            context.Orders.Add(orders[chatId]);
                                            context.SaveChanges();
                                            bot.SendTextMessageAsync(chatId: chatId, text: "سفارش شما با موفقیت ثبت شد. تیم پشتیبانی با شما در ارتباط خواهد بود. با تشکر", replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton("تماس با ما")));
                                            bot.SendTextMessageAsync(chatId: "@PamanoShoes", text: $"یک سفارش جدید توسط {user.FullName} ثبت شد.\n" +
                                                $"محصول : {productsToOrder[chatId].Name}\n" +
                                                $"جزئیات : {orders[chatId].Details}\n" +
                                                $"تاریخ ثبت : {ToPersianDateTime(orders[chatId].InsertTime)}\n" +
                                                $"توضیحات سفارش : {orders[chatId].CustomerDescription}\n" +
                                                $"مبلغ هر جفت : {productsToOrder[chatId].Price}\n" +
                                                $"تعداد : {productsToOrder[chatId].SeriesCount * orders[chatId].TotalCount} جفت \n" +
                                                $"مبلغ اعلام شده سفارش : {productsToOrder[chatId].Price * productsToOrder[chatId].SeriesCount * orders[chatId].TotalCount} تومان\n" +
                                                $"نام فروشگاه : {user.StoreName}\n" +
                                                $"شماره تماس : {user.PhoneNumber}\n" +
                                                $"آدرس : {user.Address}");

                                            orders.Remove(chatId);
                                            orderProductSteps.Remove(chatId);
                                            productsToOrder.Remove(chatId);
                                        }
                                        else if (text.Contains("لغو سفارش"))
                                        {
                                            bot.SendTextMessageAsync(chatId: chatId, text: "سفارش شما لغو شد. جهت سفارش محصول جدید به کانال پامانو مراجعه کنید.",
                                                replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton("ارتباط با ما")));
                                            orders.Remove(chatId);
                                            orderProductSteps.Remove(chatId);
                                            productsToOrder.Remove(chatId);
                                        }
                                        else
                                        {
                                            var user = context.Users.FirstOrDefault(u => u.ChatId.Equals(chatId));
                                            orders[chatId].CustomerDescription = text;
                                            orders[chatId].TotalPrice = productsToOrder[chatId].Price * productsToOrder[chatId].SeriesCount * orders[chatId].TotalCount;
                                            orders[chatId].InsertTime = DateTime.Now;
                                            orders[chatId].UserId = user.Id;
                                            context.Orders.Add(orders[chatId]);
                                            context.SaveChanges();
                                            bot.SendTextMessageAsync(chatId: chatId, text: "سفارش شما با موفقیت ثبت شد. تیم پشتیبانی با شما در ارتباط خواهد بود. با تشکر",
                                                replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton("ارتباط با ما")));
                                            bot.SendTextMessageAsync(chatId: "@PamanoShoes", text: $"یک سفارش جدید توسط {user.FullName} ثبت شد.\n" +
                                                $"محصول : {productsToOrder[chatId].Name}\n" +
                                                $"جزئیات : {orders[chatId].Details}\n" +
                                                $"تاریخ ثبت : {ToPersianDateTime(orders[chatId].InsertTime)}\n" +
                                                $"توضیحات سفارش : {orders[chatId].CustomerDescription}\n" +
                                                $"نام فروشگاه : {user.StoreName}\n" +
                                                $"شماره تماس : {user.PhoneNumber}\n" +
                                                $"آدرس : {user.Address}");
                                            orders.Remove(chatId);
                                            orderProductSteps.Remove(chatId);
                                            productsToOrder.Remove(chatId);
                                        }
                                        break;
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
        catch (Exception ex)
        {

        }
    }

    public static string ToPersianDateTime(DateTime datetime)
    {
        return datetime.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")) + " " + datetime.Hour + ":" + datetime.Minute + ":" + datetime.Second;
    }
}

