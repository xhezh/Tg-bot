using Telegram.Bot;
using Telegram.Bot.Types;
using static kdz3_3.Program;

namespace kdz3_3;
internal static class Filter {
    internal static bool filterIsActivated = false; // Флаг работы фильтра.
    internal static string? filterValue1 = null; // Значение для фильтрации.
    internal static string? filterValue2 = null; // Значение для фильтрации.

    internal static FilterAndSortFunc filterDelegate = null; // Делегат для удобства.

    /// <summary>
    /// Фильтрация по AdmArea.
    /// </summary>
    /// <returns></returns>
    internal static async Task AdmAreaFilter() {
        filterIsActivated = true;

        Message sentMessage = await Program.botClient.SendTextMessageAsync(
            chatId: Program.chatId,
            text: "Введите значение для выборки.",
            cancellationToken: Program.cts.Token);

        string value = null;
        while ((value = filterValue1) == null) { // Ждем пока пользователь введет значение.
            Thread.Sleep(100); // Чтобы не грузить проц.
        }
        var selected = from p in Program.electrocarPowers // Фильруем через LINQ.
                       where p.AdmArea == value 
                       select p;
        if (selected == null || selected.Count() == 0) {
            sentMessage = await botClient.SendTextMessageAsync(
                chatId: Program.chatId,
                text: "Ничего не найдено.",
                cancellationToken: Program.cts.Token);
            Program.StartAndFlush();
        }
        Program.electrocarPowers = selected.ToArray();

        filterIsActivated = false;
    }
    /// <summary>
    /// Фильтрация по District.
    /// </summary>
    /// <returns></returns>
    internal static async Task DistrictFilter() {
        filterIsActivated = true;

        Message sentMessage = await Program.botClient.SendTextMessageAsync(
            chatId: Program.chatId,
            text: "Введите значение для выборки.",
            cancellationToken: Program.cts.Token);

        string value = null;
        while ((value = filterValue1) == null) { // Ждем пока пользователь введет значение.
            Thread.Sleep(100); // Чтобы не грузить проц.
        }
        var selected = from p in Program.electrocarPowers // Фильруем через LINQ.
                       where p.District == value
                       select p;
        if (selected == null || selected.Count() == 0) {
            sentMessage = await botClient.SendTextMessageAsync(
                chatId: Program.chatId,
                text: "Ничего не найдено.",
                cancellationToken: Program.cts.Token);
            Program.StartAndFlush();
        }
        Program.electrocarPowers = selected.ToArray();
        filterIsActivated = false;
    }
    /// <summary>
    /// Фильтрация по AdmArea и Longitude_WGS84.
    /// </summary>
    /// <returns></returns>
    internal static async Task AdmAreaAndLongitude_WGS84() {
        filterIsActivated = true;

        Message sentMessage = await Program.botClient.SendTextMessageAsync(
            chatId: Program.chatId,
            text: "Введите 2 значения для выборки через запятую.",
            cancellationToken: Program.cts.Token);

        string value1 = null;
        string value2 = null;
        while ((value1 = filterValue1) == null) { // Ждем пока пользователь введет значения.
            Thread.Sleep(100); // Чтобы не грузить проц.
        }
        value2 = filterValue2;
        if (value2 != null) { // Убираем пробел после запятой.
            value2 = value2.Replace(" ", "");
        }
        else { // Если пользователь забил на второе значение.
            value2 = "";
        }

        var selected = from p in Program.electrocarPowers // Фильруем через LINQ.
                       where p.AdmArea == value1 && p.Longitude_WGS84 == value2
                       select p;
        if (selected == null || selected.Count() == 0) {
            sentMessage = await botClient.SendTextMessageAsync(
                chatId: Program.chatId,
                text: "Ничего не найдено.",
                cancellationToken: Program.cts.Token);
            Program.StartAndFlush();
        }
        Program.electrocarPowers = selected.ToArray();

        filterIsActivated = false;
    }
}

