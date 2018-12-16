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
## Other docs coming soon
