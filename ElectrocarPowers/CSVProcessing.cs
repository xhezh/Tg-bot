using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ElectrocarPowers {
    public class CSVProcessing {
        // Шапка csv файла (2 первые строки)
        private readonly string[] _titles = {"object_category_Id;ID;Name;AdmArea;District;Address;Longitude_WGS84;Latitude_WGS84;global_id;geodata_center;geoarea",
            "object_category_Id;Код;Наименование;Административный округ;Район;Адрес;Долгота в WGS-84;Широта в WGS-84;global_id;geodata_center;geoarea" };
        // Путь к исходному csv файлу
        private string _path;
        /// <summary>
        /// Конструктор без параметров.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public CSVProcessing() {
            throw new ArgumentException();
        }
        /// <summary>
        /// Конструктор с параметрами.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CSVProcessing(string path) {
            _path = path ?? throw new ArgumentNullException("path");
        }
        /// <summary>
        /// Метод для создания нового csv файла.
        /// Принимает на вход коллекцию объектов типа ElectrocarPower и возвращает объект типа Stream, который будет использован для отправки csv документа Telegram-ботом.
        /// </summary>
        /// <param name="electrocarPowers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Stream Write(ElectrocarPower[] electrocarPowers) {
            if (electrocarPowers == null) {
                throw new ArgumentNullException();
            }
            // 2 реплейса для удобства (.json -> .csv -> _new.csv)
            Stream fileStream = new FileStream(_path.Replace(".json", ".csv").Replace(".csv", "_new.csv"), FileMode.Create);

            var writer = new StreamWriter(fileStream, System.Text.Encoding.Default);
            writer.WriteLine(_titles[0]);
            writer.WriteLine(_titles[1]);
            for (int i = 0; i < electrocarPowers.Length; i++) {
                writer.WriteLine(electrocarPowers[i].ToCSV());
            }
            // Тут поток writer не закрываем,тк закроется поток fileStream (жесть какая-то, часа 2 не мог понять чзнх).
            // Поток writer сам закроется потом, когда закроем поток fileStream в UploadFile.cs.
            return fileStream;
        }
        /// <summary>
        /// Метод для считывания скачанного из бота csv файла.
        /// Принимает на вход Stream с csv файлом из Telegram-бота и возвращает коллекцию объектов типа ElectrocarPower.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public ElectrocarPower[] Read(FileStream fileStream) {
            if (fileStream == null) { 
                throw new ArgumentNullException(); 
            }
            List<ElectrocarPower> electrocarPowersList = new();
            using (var reader = new StreamReader(fileStream)) {
                string? line1;
                string? line2;
                // Проверка корректности файла. Свериваем 2 шапки из _titles и 2 шапки из файла.
                if ((line1 = reader.ReadLine()) != null && (line2 = reader.ReadLine()) != null) {
                    // Файл читается очень странно - все значения полей в кавычках, решил их убрать.
                    line1 = line1.Replace('"'.ToString(), String.Empty);
                    line2 = line2.Replace('"'.ToString(), String.Empty);
                    if (line1 != _titles[0] || line2 != _titles[1]) {
                        throw new Exception("Данные файла не соотвествует заданному формату.");
                    }
                }
                else {
                    throw new Exception("Не удалось прочитать .csv файл, либо он пуст.");
                }
                // Читаем и сохранем данные.
                string? line;
                while ((line = reader.ReadLine()) != null) {
                    // Файл читается очень странно - все значения полей в кавычках, решил их убрать.
                    line = line.Replace('"'.ToString(), String.Empty);
                    string[] sline = line.Split(';');
                    electrocarPowersList.Add(new ElectrocarPower(int.Parse(sline[1]), sline[2], sline[3], sline[4], sline[5], sline[6], sline[7], sline[8]));
                }
            }
            return electrocarPowersList.ToArray();
        }
    }
}
