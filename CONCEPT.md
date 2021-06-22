## CONCEPT

### ASP.NET Core configuration (JSON format)
```json
{
  "TelegramBots": [
    {
      "BotName": "Bot1",
      "MarkerName": "Marker1",
      "Token": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11"
    },
    {
      "BotName": "Bot2",
      "MarkerName": "Marker1",
      "Token": "123457:ABC-DEF1234ghIkl-adwqfr55tyg4rweqd"
    }
  ]
}
```

### Database context
```c#
public class MyContext : TelegramBotEngineContext
{
}
```

### Startup
```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddTelegramBotEngine()
        .AddBots<Marker1, Marker2, ..., Marker16>()
        .AddBotEngineStore<MyContext>();
}
```

### Bot marker
```c#
public class Marker1 : ITelegramBotMarker
{
    public override string Name => "Marker1";
}
```

### Message handlers
```c#
[Text("/start")]
[ChatTypes(ChatType.Private, ChatType.Supergroup, ChatType.Group)] // enabled by default for private chats only
[Text("/start {Payload}")]
public class StartMessage : BotMessage
{
    [Parameter]
    public string Payload { get; set; }

    public override async Task HandleAsync()
    {
        await Client.SendTextMessageAsync(Chat, "Hello world!", cancellationToken: CancellationToken);
    }
}
```

Supported parameter types: byte, sbyte, short, ushort, int, uint, long, ulong, decimal, double, float, string, TimeSpan, DateTime.

```c#
[Status("book-message")]
[Text("{Time}")]
public class BookMessage : BotMessage
{
    // set CultureName as null to use invariant culture
    [Parameter(CultureNames = new[] {"ru-RU", "en-US"}, Formats = new[] {"h:mm"})]
    public TimeSpan Time { get; set; }
    
    public override async Task HandleAsync()
    {
        await SetStatusAsync("new-status");
        await Client.SendTextMessageAsync(Chat, "Thank you for sending booking time!", cancellationToken: CancellationToken);
    }
}
```