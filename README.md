# Telegram.Bot.Helper
.NET library to reduce development time of telegram bots.
Created for developers that know how [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) library works.

## How to install
`Install-Package Telegram.Bot.Helper`

## How to use
Let's start with simple console app. Create new class file named "BotClient.cs":
```c#
public class BotClient
{
    private readonly TelegramBotHelper _helper;
    private readonly TelegramBotClient _c;

    public BotClient(string token)
    {
        _helper = new TelegramBotHelper(() => new TelegramBotClient(token));
        _helper.Client.OnUpdate += async (o, e) =>
        {
            await _helper.UpdateReceived(e.Update);
        };
        _c = _helper.Client;

        Initialize();
    }

    public void Start()
    {
        _helper.Client.StartReceiving(allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery });
    }
    public void Stop()
    {
        _helper.Client.StopReceiving();
    }

    public void Initialize()
    {
        // Initialize actions
    }
}
```
And don't forget about using:
```c#
using Telegram.Bot.Helper;
```
Initialize() method should add some logic for bot update handling.
#### Handle text messages
You can explicitly match message text:
```c#
public void Initialize()
{
	_helper.Messages(_m =>
	{
		_m["/start"] = async (m, _) =>
		{
			await _c.SendTextMessageAsync(m.Chat.Id, "Hello in my test bot!",
				replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton("About")));
		};
		_m["About"] = async (m, _) =>
		{
			await _c.SendTextMessageAsync(m.Chat.Id, "This bot was created by Panda.");
		};
	});
}
```
**_m** is a builder for text messages only.  
If user sends `/start`, bot will answer with text `Hello in my test bot` and single keyboard button with text `About`. Then when user presses `About`, Telegram sends `About` text to bot, so bot will answer with text `This bot was created by Panda`.  
Pretty easy, isn't it? :)  
**m** is original `Telegram.Bot.Types.Message` object which contains incoming message.
#### Handle callback queries
Callback query is result of inline button. So if you use inline keyboard to communicate with user, follow next code snippets:
```c#
public void Initialize()
{
	_helper.Messages(_m =>
	{
		_m["/start"] = async (m, _) =>
		{
			await _c.SendTextMessageAsync(m.Chat.Id, "Hello in my test bot!",
				replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
				{
					Text = "About",
					CallbackData = "about~text"
				}));
		};
	});
	_helper.Inlines(i =>
	{
		i["about~text"] = async (q, c, _) =>
		{
			// c[0] == "about"
			// c[1] == "text"
			await _c.SendTextMessageAsync(q.ChatId, "Bot received callback query!");
			await _c.AnswerCallbackQueryAsync(q.Query.Id);
		};
	});
}
```
**i** is a builder for callback queries only.  
**q** is an instance of `Telegram.Bot.Helper.CallbackQueryInfo` class. It contains original `Telegram.Bot.Types.CallbackQuery` object and some properties of popular items (ChatId, MessageId, MessageText, etc.).  
**c** is an array of separated strings with separator from constructor or static property `TelegramBotHelper.Separator`. Default is `~`. Usually there is no need to change separator.
#### Handle callback queries with unknown parameters
Sometimes we need to send callback data with non-permanent text like `delete~order~{id}` where {id} is order identifier, so bot knows that it must delete order with this id. It can be `delete~order~25` or `delete~order~84`. To accomplish this, you need to leave an empty space. Look at next example:
```c#
i["delete~order~ "] = async (q, c, _) =>
{
	// c[0] == "delete"
	// c[1] == "order"
	// c[2] == "your id will be here"
	if (!int.TryParse(c[2], out int orderId)
		return;
	
	await _context.Orders.DeleteOneAsync(o => o.Id == orderId);
	await _c.SendTextMessageAsync(q.ChatId, $"Order with id {c[2]} was successfully deleted.");
};
```
#### Handle message expressions
```c#
public void Initialize()
{
	_helper.Expressions(e =>
	{
		e[m => m.Type == MessageType.Text && m.Text.StartsWith("/start")] = async (m, _) =>
		{
			string[] parameters = m.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (parameters.Length == 1)
				return;
			
			for (int i = 1; i < parameters.Length; i++)
			{
				await _c.SendTextMessageAsync(m.Chat.Id, $"Parameter {i} = {parameters[i]}")
			}
		};
		_m[m => m.Type == MessageType.Location] = async (m, _) =>
		{
			await _c.SendTextMessageAsync(m.Chat.Id, $"Location received.\nLongitude: {m.Location.Longitude}\n"
				+ $"Latitude: {m.Location.Latitude}");
		};
	});
}
```
**e** is expression builder.  
**m** is original `Telegram.Bot.Types.Message` object.
#### Start our bot
In Program.cs file add next lines inside Main method:
```c#
BotClient client = new BotClient("your bot's token here, write to @BotFather to generate it");
client.Start();
Console.WriteLine("Bot is running...");
Console.ReadKey();
client.Stop();
```
Now start your console application.
## Verification
Access restrictions can be made using the following functionality:
```c#
public void Initialize()
{
	_helper.Verifying = async u =>
	{
		var userInfo = await _context.UserInfo.Find(info => info.UserId == u.Id).SingleOrDefaultAsync();
		if (userInfo == null)
			return Verify.Unknown;
		
		return userInfo.Authorized ? Verify.Verified : Verify.Unverified;
	};
	_helper.Messages(_m =>
	{
		_m["/start", Verify.Unknown] = async (m, v) =>
		{
			// Code here will run for unknown users only
		};
		_m["Restricted", Verify.Unverified] = async (m, v) =>
		{
			// Code here will run for unverified users only
		};
		_m["About us", Verify.Verified | Verify.Unverified] = async (m, v) =>
		{
			// Code here will run for verified and unverified users only
			
			if (v == Verify.Verified)
				Console.WriteLine("User is verified");
			else Console.WriteLine("User is unverified");
		};
		_m["Contact us"] = async (m, v) =>
		{
			// Code here will run for all users
		};
	});
	_helper.Expressions(e =>
	{
		_m[m => m.Type == MessageType.Location, Verify.Verified] = async (m, v) =>
		{
			// Code here will run for verified users only
		};
	});
}
```
Verifying delegate is used for verification of user. For example, you can check user's access level in database and return Verify enum value. In your code you can be sure that user has access only to allowed areas so additional checks are redundant.  
In my opinion `Verify.Unknown` represents users who have never written to bot before, `Verify.Unverified` - have already written to bot, but they haven't logged in or sent phone number/location, `Verify.Verified` - users who have full access to bot.
## Keyboards
```c#
var b = new Builder<KeyboardButton>();
for (int i = 0; i < 20; i++)
{
	b.B("Button").B("Button with location", requestLocation: true)
  		.B("Button with contact", requestContact: true)
		.B(new KeyboardButton("Test"));
	if (i != 19) b.L();
}
await _c.SendTextMessageAsync(m.Chat, "Keyboard test", replyMarkup: b.M().RK().OTK());
```
```c#
var b = new Builder<InlineKeyboardButton>();
for (int i = 0; i < 20; i++)
{
	b.B("Text", "callback~data")
		.B("Text", "callback~data")
  		.B("Button with url", null, url: "https://www.google.com/")
		.B(new InlineKeyboardButton { Text = "Button text", ... });
	if (i != 19) b.L();
}
await _c.SendTextMessageAsync(m.Chat, "Keyboard test", replyMarkup: b.M());
```
**b** is markup builder for KeyboardButtons or InlineKeyboardButtons.  
**b.B()** adds new button to current row  
**b.L()** moves builder to new row  
**b.M()** builds markup  
**markup.RK()** sets ResizeKeyboard property to specified value (default value is true; to set false - `markup.RK(false)`). This method exists only for KeyboardButton's builder  
**markup.OTK()** sets OneTimeKeyboard property to specified value (default value is true; to set false - `markup.OTK(false)`). This method exists only for KeyboardButton's builder
## Other docs coming soon
