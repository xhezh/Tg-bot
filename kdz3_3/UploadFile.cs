using ElectrocarPowers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace kdz3_3;
internal class UploadFile {
    /// <summary>
    /// Метод для загрузки файла в чат.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="destinationFilePath"></param>
    /// <param name="csvProcessing"></param>
    /// <param name="jsonProcessing"></param>
    /// <returns></returns>
    internal static async Task UploadF(ITelegramBotClient botClient, string destinationFilePath, CSVProcessing csvProcessing, JSONProcessing jsonProcessing) {
        await using (Stream fileStream = csvProcessing.Write(Program.electrocarPowers)) { // Получаем уже открытый и использованный поток из csvProcessing.Write (так требует условие).
            fileStream.Flush(); // Чистим буфер потока.
            fileStream.Seek(0, System.IO.SeekOrigin.Begin); // Перемещаем указатель на начало, чтобы потоком можно было пользоваться.

            Program.message = await botClient.SendDocumentAsync( // Загружаем файл в чат.
                chatId: Program.chatId,
                document: InputFile.FromStream(stream: fileStream, fileName: Program.messageDocument.FileName.Replace(".json", ".csv").Replace(".csv", "_new.csv")),
                caption: "Обработанный файл .csv");
        }

        await using (Stream fileStream = jsonProcessing.Write(Program.electrocarPowers)) { // Получаем уже открытый и использованный поток из jsonProcessing.Write (так требует условие).
            fileStream.Flush(); // Чистим буфер потока.
            fileStream.Seek(0, System.IO.SeekOrigin.Begin); // Перемещаем указатель на начало, чтобы потоком можно было пользоваться.

            Program.message = await botClient.SendDocumentAsync( // Загружаем файл в чат.
                chatId: Program.chatId,
                document: InputFile.FromStream(stream: fileStream, fileName: Program.messageDocument.FileName.Replace(".csv", ".json").Replace(".json", "_new.json")),
                caption: "Обработанный файл .json");
        }
    }
}
