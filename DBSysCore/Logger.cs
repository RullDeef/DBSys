using System;
using System.IO;

namespace DBSysCore
{
    public static class Logger
    {
        /**
         * Текущий файл для логирования.
         */
        private static StreamWriter file = null;

        /**
         * Открывает текущий файл логирования.
         * 
         * Изначально все файлы логгирования создаются в log/ директории.
         * Создаёт директорию log\ если она не существует.
         */
        private static void EnsureOpen()
        {
            if (file == null)
            {
                try
                {
                    string filename = $"{Paths.logDirectory}\\{DateTime.Now:yyyy_MM_dd}.txt";
                    file = new StreamWriter(filename, true);
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

            string time = DateTime.Now.ToString("HH:mm:ss");
            string dump = Session.GetCurrentWorkingDumpFileName();
            string userName = Session.GetUserName();

            string meta = $"[{time}|{dump}|{userName}] ";

            if (data.EndsWith("\n"))
                data = data.Substring(0, data.Length - 1);

            file.WriteLine(meta + data);

            // WARNING: bad solution. Find out more efficient one
            Close();
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

            string time = DateTime.Now.ToString("HH:mm:ss");
            string dump = Session.GetCurrentWorkingDumpFileName();
            string userName = Session.GetUserName();

            string meta = $"[{time}|{dump}|{userName}] ";

            file.WriteLine($"{meta} Error in {where}: {what}");

            // WARNING: bad solution. Find out more efficient one
            Close();
        }

        /**
         * Закрывает текущий файл логирования.
         */
        private static void Close()
        {
            try
            {
                if (file != null)
                {
                    // WARNING: comented line may cause some issues
                    // file.Dispose();
                    file.Close();
                    file = null;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error happened while closing logger! {e.Message}");
            }
        }
    }
}
