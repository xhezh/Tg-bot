using ElectrocarPowers;
using Telegram.Bot;


namespace kdz3_3;
internal class DownloadFile {
    /// <summary>
    /// Метод для скачивания файла из чата.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="destinationFilePath"></param>
    /// <param name="csvProcessing"></param>
    /// <param name="jsonProcessing"></param>
    /// <returns></returns>
    internal static async Task DownloadF(ITelegramBotClient botClient, string destinationFilePath, CSVProcessing csvProcessing, JSONProcessing jsonProcessing) {
        var fileId = Program.messageDocument.FileId; 
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath; // Путь для скачивания.
        var messageDocument = Program.messageDocument;

        await using (FileStream fileStream = new FileStream(destinationFilePath, FileMode.OpenOrCreate)) { // Открываем поток для сохранения файла по пути destinationFilePath.
            await botClient.DownloadFileAsync( // Качаем файл из чата.
            filePath: filePath,
                destination: fileStream,
            cancellationToken: Program.cts.Token);

            // Тк поток не закрываем, а передаем в библиотеку классов для считывания скачанного файла (так требует условие), то: 
            fileStream.Flush(); // Чистим буфер потока.
            fileStream.Seek(0, System.IO.SeekOrigin.Begin); // Перемещаем указатель на начало, чтобы потоком можно было пользоваться.

            if (messageDocument.FileName.EndsWith(".json")) { // Если пользователь скинул json файл.
                Program.electrocarPowers = jsonProcessing.Read(fileStream);
            }
            else if (messageDocument.FileName.EndsWith(".csv")) { // Если пользователь скинул csv файл.
                Program.electrocarPowers = csvProcessing.Read(fileStream);
            }
            else { // Если пользователь скинул что-то не то.
                throw new Exception("Неверный формат файла");
            }
        }
    }
}

