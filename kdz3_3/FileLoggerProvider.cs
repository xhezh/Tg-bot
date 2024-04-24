using Microsoft.Extensions.Logging;

namespace kdz3_3 {
    internal class CustomFileLoggerProvider : ILoggerProvider {
        private readonly StreamWriter _logFileWriter;
        public CustomFileLoggerProvider(StreamWriter logFileWriter) {
            _logFileWriter = logFileWriter ?? throw new ArgumentNullException(nameof(logFileWriter));
        }
        public ILogger CreateLogger(string categoryName) {
            return new CustomFileLogger(categoryName, _logFileWriter);
        }
        public void Dispose() {
            _logFileWriter.Dispose();
        }
    }
}

