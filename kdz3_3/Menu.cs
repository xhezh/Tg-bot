using Telegram.Bot;
using Telegram.Bot.Types;

namespace kdz3_3;
internal static class Menu {
    internal static string? filterMenuValue = null; // Значение для фильтр меню.
    internal static string? sortMenuValue = null; // Значение для сорт меню.
    internal static bool filterMenuIsActive = false; // Флаг работы фильтр меню.
    internal static bool SortMenuIsActivated = false; // Флаг работы сорт меню.

    /// <summary>
    /// Меню для выбора фильтра.
    /// </summary>
    /// <returns></returns>
    public static async Task FilterMenu() {
        filterMenuIsActive = true;

        Message sentMessage = await Program.botClient.SendTextMessageAsync(
        chatId: Program.chatId,
        text: "1. Фильтрация по AdmArea \n" +
              "2. Фильтрация по District \n" +
              "3. Фильтрация по AdmArea и Longitude_WGS84 \n" +
              "Не фильтровать - любое другое сообщение",
        cancellationToken: Program.cts.Token);

        string value = null;
        while ((value = filterMenuValue) == null) { // Ждем пока пользователь введет значения.
            Thread.Sleep(100); // Чтобы не грузить проц.
        }

        if (value == "1") {
            Filter.filterDelegate = Filter.AdmAreaFilter;
        }
        if (value == "2") {
            Filter.filterDelegate = Filter.DistrictFilter;
        }
        if (value == "3") {
            Filter.filterDelegate = Filter.AdmAreaAndLongitude_WGS84;
        }

        filterMenuIsActive = false;
    }
    /// <summary>
    /// Меню для выбора сортировки.
    /// </summary>
    /// <returns></returns>
    public static async Task SortMenu() {
        SortMenuIsActivated = true;
        
        Message sentMessage = await Program.botClient.SendTextMessageAsync(
        chatId: Program.chatId,
        text: "1. Сортировка AdmArea по алфавиту в прямом порядке  \n" +
              "2. Сортировка AdmArea по алфавиту в обратном порядке \n" +
              "Не сортировать - любое другое сообщение",
        cancellationToken: Program.cts.Token);

        string value = null;
        while ((value = sortMenuValue) == null) { // Ждем пока пользователь введет значения.
            Thread.Sleep(100); // Чтобы не грузить проц.
        }

        if (value == "1") {
            Sort.sortDelegate = Sort.AdmAreaSortAscending;
        }
        if (value == "2") {
            Sort.sortDelegate = Sort.AdmAreaSortDescending;
        }

        SortMenuIsActivated = false;
    }
}

