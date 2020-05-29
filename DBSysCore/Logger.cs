using System;
using System.IO;

namespace DBSysCore
{
    public class Logger
    {
        private static string filename => $"{Paths.logDirectory}\\{DateTime.Now:yyyy_MM_dd_HH}.txt";

        private static string GetMeta()
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string dump = Session.GetCurrentWorkingDumpFileName();
            string userName = Session.GetUserName();

            return $"[{time}|{dump}|{userName}] ";
        }

        /**
         * Записывает переданную строку в текущий файл логирования.
         * 
         * Дополняет переданную строку мета данными:
         *      [время|файл дампа|пользователь]
         */
        public static void Log(string data)
        {
            using (StreamWriter file = File.AppendText(filename))
            {
                if (data.EndsWith("\n"))
                    data = data.Substring(0, data.Length - 1);

                file.WriteLine(GetMeta() + data);
            }
        }

        /**
         * Записывает переданные строки в текущий файл логгирования.
         * 
         * Дополняет переданную строку мета данными:
         *      [время|файл дампа|пользователь] Error in {где}: {что}
         */
        public static void Error(string where, string what)
        {
            using (StreamWriter file = File.AppendText(filename))
            {
                file.WriteLine($"{GetMeta()} Error in {where}: {what}");
            }
        }

        public static void Func(string funcName)
        {
#if DEBUG
            using (StreamWriter file = File.AppendText(filename))
            {
                file.WriteLine($"  -> call {funcName}");
            }
#endif
        }
    }
}
