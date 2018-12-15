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
		_m["/start"] = async (m, v) =>
		{
			await _c.SendTextMessageAsync(m.Chat.Id, "Hello in my test bot!",
				replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton("About")));
		};
		_m["About"] = async (m, v) =>
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
**v** is `Telegram.Bot.Helper.Verify` enum. If you don't create bot with restricted areas, just ignore this parameter.
## Other docs coming soon
