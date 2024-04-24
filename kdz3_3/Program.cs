using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ElectrocarPowers;

namespace kdz3_3;
class Program {
    internal static TelegramBotClient botClient = new TelegramBotClient("6880198631:AAHMO7M8mDeDhS1YbjmMl-fhS7JJs5JEwoY");
    internal static CancellationTokenSource cts = new CancellationTokenSource();
    internal static Message? message;
    internal static Document? messageDocument;
    internal static string? messageText;
    internal static long chatId;

    internal static ElectrocarPower[]? electrocarPowers;

    private static bool processingCompleted = false;

    private static string? destinationFilePath;
    private static CSVProcessing? csvProcessing;
    private static JSONProcessing? jsonProcessing;

    internal delegate Task FilterAndSortFunc();
    
    private static ILogger<Program> logger;

    static async Task Main(string[] args) {
        try {
            logger = FileLoggerInit.CreateLogger(); // Логи, сохраняющиеся в файл через Microsoft.Extensions.Logging

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new() {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            Console.ReadKey();

            if (FileLoggerInit.logFileWriter != null) {
                FileLoggerInit.logFileWriter.Dispose(); // Закрываем поток логгера.
            }

        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
        cts.Cancel(); // Send cancellation request to stop bot
    }

    /// <summary>
    /// Метод откатывает программу до "заводских настроек" (приводит поля в исходное положение и чистит директорию, в которой хранятся json и csv файлы).
    /// Нужен чтобы корркетно обрабатывать последующие файлы, не нарушая логику программы.
    /// </summary>
    internal static async void StartAndFlush() {
        await botClient.SendTextMessageAsync(message.Chat, "Отправь мне csv файл, а я его обработаю и верну в .csv и .json форматах.");

        processingCompleted = false;
        Menu.filterMenuValue = null; Menu.sortMenuValue = null; Menu.filterMenuIsActive = false; Menu.SortMenuIsActivated = false;
        Filter.filterIsActivated = false; Filter.filterValue1 = null; Filter.filterValue2 = null; Filter.filterDelegate = null;
        Sort.sortDelegate = null;

        try {
            var dir = new DirectoryInfo("../");
            if (dir.Exists) {
                foreach (var file in dir.GetFiles("*.json")) {
                    file.Delete();
                }
                foreach (var file in dir.GetFiles("*.csv")) {
                    file.Delete();
                }
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Ошибка очистки директории с файлами: {ex.Message}");
        }
    }
    /// <summary>
    /// Метод для обработки текстовых сообщений.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    static async Task TextMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        message = update.Message;
        messageText = update.Message.Text;

        if (messageText.ToLower() == "/start") {
            StartAndFlush();
            return;
        }
        
        if (Menu.filterMenuIsActive) { // Если параллельно работает FilterMenu.
            Menu.filterMenuValue = messageText; // Передаем туда значение от пользователя.
            while (Menu.filterMenuIsActive) { // Ждем пока FilterMenu не закончит работать.
                Thread.Sleep(50); // Чтобы не грузить проц.
            }
            if (Filter.filterDelegate != null) {
                Filter.filterDelegate(); // Запускаем выбранную фильтрацию параллельно, чтобы считать значение от пользователя.
            }
            else { // Если пользователь отказался от фильтрации, то сразу переходим к сортировке.
                Menu.SortMenu(); // Запускаем параллельно, чтобы считать значение от пользователя.
            }
        }
        else if (Filter.filterIsActivated) { // Если параллельно работает какой-либо фильтр.
            if (Filter.filterDelegate == Filter.AdmAreaAndLongitude_WGS84) { // Если фильтр по 2 элементам.
                if (messageText.Split(',').Length == 2) {
                    Filter.filterValue1 = messageText.Split(',')[0]; // Передаем 2 значения от пользователя.
                    Filter.filterValue2 = messageText.Split(',')[1];
                }
                else {
                    await botClient.SendTextMessageAsync(message.Chat, "Неверный формат введенных данных, обработка остановлена.");
                    StartAndFlush(); 
                }
            }
            else { // Если фильтр по 1 элементу.
                Filter.filterValue1 = messageText; // Передаем 1 значение от пользователя.
            }
            while (Filter.filterIsActivated) { // Ждем пока фильтр не закончит работать.
                Thread.Sleep(50); // Чтобы не грузить проц.
            }
            if (electrocarPowers.Length == 0) { // Если фильтр ничего не нашел.
                return;
            }
            Menu.SortMenu(); // Запускаем параллельно, чтобы считать значение от пользователя.
        }
        else if (Menu.SortMenuIsActivated) {  // Если параллельно работает SortMenu.
            Menu.sortMenuValue = messageText; // Передаем туда значение от пользователя.
            while (Menu.SortMenuIsActivated) {  // Ждем пока SortMenu не закончит работать.
                Thread.Sleep(50); // Чтобы не грузить проц.
            }
            if (Sort.sortDelegate != null) { // Если пользователь не отказался от сортировки.
                await Sort.sortDelegate(); // Тк никаких значений от пользователя принимать не нужно, просто ждем метод.
            }
            processingCompleted = true; // Файл обработан!
        }
        else { // Если пользователь написал что-то не то.
            StartAndFlush(); // Откатываемся.
        }
        if (processingCompleted) { // Если файл обработан.
            try {
                await UploadFile.UploadF(botClient, destinationFilePath, csvProcessing, jsonProcessing); // Записываем данные в новые файлы csv и json, скидываем пользователю в чат.
            }
            catch (Exception e) {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: Program.chatId,
                    text: $"Ошибка: {e.Message}",
                    cancellationToken: Program.cts.Token);
            }
            StartAndFlush(); // Откатываемся, ждем новый файл.
        }
        Console.WriteLine($"{DateTime.Now} ::: {message.From.Username} sent '{messageText}' message in chat {chatId}."); // Логи в консоль.
        try {
            using (logger.BeginScope("[scope is enabled]")) {
                logger.LogInformation($"{DateTime.Now} ::: {message.From.Username} sent '{messageText}' message in chat {chatId}.");
            }
        }
        catch {
            Console.WriteLine("Ошибка записи логов.");
        }
    }
    /// <summary>
    /// Метод для работы с документами, скинутыми в чат.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    static async Task DocumentMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        message = update.Message;
        messageDocument = message.Document;
        destinationFilePath = "../" + messageDocument.FileName; // Путь, где складывается скачанный файл, и создаются новые.

        try {
            // Объекты для работы с файлом.
            csvProcessing = new(destinationFilePath);
            jsonProcessing = new(destinationFilePath);
            await DownloadFile.DownloadF(botClient, destinationFilePath, csvProcessing, jsonProcessing); // Качаем файл из чата и читаем его.
        }
        catch (Exception e) {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: Program.chatId,
                text: $"Ошибка: {e.Message}",
                cancellationToken: Program.cts.Token);
            StartAndFlush();
            return;
        }
        Menu.FilterMenu(); // Запускаем параллельно, чтобы считать значение от пользователя.
        Console.WriteLine($"{DateTime.Now} ::: {message.From.Username} sent '{message.Document.FileName}' message in chat {chatId}."); // Логи в консоль.
        try {
            using (logger.BeginScope("[scope is enabled]")) {
                logger.LogInformation($"{DateTime.Now} ::: {message.From.Username} sent '{message.Document.FileName}' message in chat {chatId}.");
            }
        }
        catch {
            Console.WriteLine("Ошибка логгера.");
        }
    }
    /// <summary>
    /// Обработчик происходящего в чате.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        if (update.Message is not { } message) { // Обрабатываем только сообщения.
            return;
        }

        chatId = message.Chat.Id;
        
        if (message.Text != null) { // Если пользователь прислал текст.
            TextMessageAsync(botClient, update, cancellationToken);
        }
        if (message.Document != null) {  // Если пользователь прислал документ.
            DocumentMessageAsync(botClient, update, cancellationToken);
        }
        if (message.Text == null && message.Document == null) { // Если пользователь приколист или ломает бота.
            StartAndFlush(); // Откатываемся.
        }
    }

    /// <summary>
    /// Обработчик ошибок tg api.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) {
        var ErrorMessage = exception switch {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        try {
            using (logger.BeginScope("")) {
                logger.LogError($"{DateTime.Now} ::: {ErrorMessage}.");
            }
        }
        catch {
            Console.WriteLine("Ошибка записи логов.");
        }
        return Task.CompletedTask;
    }

}

