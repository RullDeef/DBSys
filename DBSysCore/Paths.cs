using System;
using System.IO;

namespace DBSysCore
{
    public static class Paths
    {
        private static string GetBaseFolder()
        {
            // Console.WriteLine($"Path.GetBaseFolder: {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}");
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DSPLAB\\DBSys";
        }

        public static string appFolder
        {
            get
            {
                string folder = GetBaseFolder();
                Directory.CreateDirectory(folder);
                return folder;
            }
        }

        /**
         * Название файла сессии.
         */
        public static string sessionFilename => appFolder + "\\.session";

        /**
         * Название файла, используемого для хранения данных
         * о пользователях независимо от обычных файлов дампов.
         * 
         * В дальнейшем - пользовательский файл.
         */
        public static string usersFilename => appFolder + "\\.users";
        public static string dumpModelFilename => appFolder + "\\model.sql";
        public static string usersModelFilename => appFolder + "\\users.sql";
        public static string staticDataTable => appFolder + "\\static.xls";

        public static string dumpsDirectory
        {
            get
            {
                string folder = GetBaseFolder() + "\\dumps";
                Directory.CreateDirectory(folder);
                return folder;
            }
        }

        public static string logDirectory
        {
            get
            {
                string folder = GetBaseFolder() + "\\log";
                Directory.CreateDirectory(folder);
                return folder;
            }
        }

        public static string reportDirectory
        {
            get
            {
                string folder = GetBaseFolder() + "\\reports";
                Directory.CreateDirectory(folder);
                return folder;
            }
        }

        public static string reportTemplateDirectory
        {
            get
            {
                string folder = GetBaseFolder() + "\\report_templates";
                Directory.CreateDirectory(folder);
                return folder;
            }
        }
    }
}
