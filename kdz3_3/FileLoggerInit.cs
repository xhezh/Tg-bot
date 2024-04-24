using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz3_3 {
    internal static class FileLoggerInit {
        internal static StreamWriter logFileWriter = null;
        /// <summary>
        /// Создает логгер кастомный логгер, пишущий в файл .
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ILogger<Program> CreateLogger() {
            try {
            string logFilePath = DirCreate() + "/logs.txt";
                logFileWriter = new StreamWriter(logFilePath, append: true); // Поток для записи логов в директорию var.
                ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { //Create an ILoggerFactory
                    builder.AddProvider(new CustomFileLoggerProvider(logFileWriter)); //Add a custom log provider to write logs to text files
                });
                return loggerFactory.CreateLogger<Program>(); //Create an ILogger
            }
            catch (Exception ex) {
                throw new Exception($"Не удалось создать экземпляр логгера. Ошибка: {ex.Message}"); // Проталкиваем в вызвавший метод.
            }
        }

        /// <summary>
        /// Создает директорию var.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string DirCreate() {
            // Путь к директории (папке) var.
            var dir = new DirectoryInfo("../").Parent.Parent;
            string logFileDir = dir.FullName + "/var";
            ILogger<Program> logger;
            if (!Directory.Exists(logFileDir)) { // Создаем /var, если ее нет.
                try {
                    Directory.CreateDirectory(logFileDir);
                }
                catch (Exception ex) {
                    throw new Exception($"Ошибка при создании директории 'var': {ex.Message}"); // Проталкиваем в CreateLogger().
                }
            }
            return logFileDir;
        }
    }
}
