using System;
using System.IO;
using System.Text;

namespace DBSysCore
{
    public static class Logger
    {
        /**
         * Текущий файл для логирования.
         */
        private static StreamWriter file = null;

        /**
         * Создаёт название для текущего файла логгирования.
         * 
         * Изначально все файлы логгирования создаются в log/ директории.
         */
        static public string GenerateFileName()
        {
            return "log/" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
        }

        /**
         * Открывает текущий файл логирования.
         * 
         * Создаёт директорию log/ если она не существует.
         */
        public static void Open()
        {
            string filename = GenerateFileName();

            if (!Directory.Exists("log"))
                Directory.CreateDirectory("log");

            file = new StreamWriter(filename, true);
        }

        private static void EnsureOpen()
        {
            if (file == null)
            {
                try
                {
                    Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: unable to open logging file!");
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /**
         * Записывает переданную строку в текущий файл логирования.
         * 
         * Дополняет переданную строку мета данными:
         *      [время|файл дампа|пользователь]
         */
        public static void Log(string data)
        {
            // unsure that logging file opened successfully
            EnsureOpen();

            string userName = Session.GetUserName();

            string time = DateTime.Now.ToString("HH:mm:ss");
            string meta = $"[{time}|{Session.sessionData.filename}|{userName}] ";

            if (data.EndsWith("\n"))
                data = data.Substring(0, data.Length - 1);

            file.WriteLine(meta + data);

            // WARNING: bad solution. Find out more efficient one
            Logger.Close();
        }

        /**
         * Записывает переданные строки в текущий файл логгирования.
         * 
         * Дополняет переданную строку мета данными:
         *      [время|файл дампа|пользователь] Error in {где}: {что}
         */
        public static void Error(string where, string what)
        {
            // unsure that logging file opened successfully
            EnsureOpen();

            string userName = Session.GetUserName();

            string time = DateTime.Now.ToString("HH:mm:ss");
            string meta = $"[{time}|{Session.sessionData.filename}|{userName}] ";

            file.WriteLine($"{meta} Error in {where}: {what}");

            // WARNING: bad solution. Find out more efficient one
            Logger.Close();
        }

        /**
         * Закрывает текущий файл логирования.
         */
        public static void Close()
        {
            try
            {
                file.Dispose();
                file.Close();
            }
            catch
            {
                Console.WriteLine("Error happened while closing logger!");
            }
        }
    }
}
