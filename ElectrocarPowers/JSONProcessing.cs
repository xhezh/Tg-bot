using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectrocarPowers {
    public class JSONProcessing {
        // Путь к исходному csv файлу
        private string _path;

        /// <summary>
        /// Конструтор без параметров.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public JSONProcessing() {
            throw new ArgumentException();
        }
        /// <summary>
        /// Конструктор с параметрами.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JSONProcessing(string path) {
            _path = path ?? throw new ArgumentNullException();
        }
        /// <summary>
        /// Метод для создания нового json файла.
        /// Принимает на вход коллекцию объектов типа ElectrocarPower и возвращает объект типа Stream, который будет использован для отправки json документа Telegram-ботом.
        /// </summary>
        /// <param name="electrocarPowers"></param>
        /// <returns></returns>
        public Stream Write(ElectrocarPower[] electrocarPowers) {
            if (electrocarPowers == null) {
                throw new ArgumentNullException();
            }
            // 2 реплейса для удобства (.csv -> .json -> _new.json)
            string newJsonPath = _path.Replace(".csv", ".json").Replace(".json", "_new.json");
            Stream fileStream = new FileStream(newJsonPath, FileMode.Create);

            // Сериализация json данных.
            JsonSerializerOptions options = new JsonSerializerOptions() {
                WriteIndented = true // Красивые форматированные строки
            };
            JsonSerializer.Serialize<ElectrocarPower[]>(fileStream, electrocarPowers, options);

            return fileStream;
        }
        /// <summary>
        /// Метод для считывания скачанного из бота json файла.
        /// Принимает на вход Stream с json файлом из Telegram-бота и возвращает коллекцию объектов типа ElectrocarPower.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ElectrocarPower[] Read(Stream fileStream) {
            if (fileStream == null) {
                throw new ArgumentNullException();
            }
            // Десериализация json данных.
            ElectrocarPower[] electrocarPowers = JsonSerializer.Deserialize<ElectrocarPower[]>(fileStream);
            if (electrocarPowers == null) {
                throw new Exception("Не удалось прочитать .json файл, либо он пуст.");
            }
            return electrocarPowers;
        }
    }
}
