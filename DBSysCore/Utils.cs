using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace DBSysCore
{
    /**
     * Класс для работы с хэш ключами и SQLite соединениями.
     */
    public class Utils
    {
        /**
         * Объект для создания хэш сумм.
         */
        private static readonly SHA256 sha256 = SHA256.Create();

        /**
         * Создает числовой хэш код для переданной строки.
         * 
         * Числовой хэш код всегда неотрицателен.
         */
        public static int Hash(string data)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(data));
            int hash = BitConverter.ToInt32(hashed, 0);
            return hash > 0 ? hash : -hash;
        }

        /**
         * Возвращает строковый хэш код для переданной строки.
         * 
         * Строковый хэш код всегда имеет длину 64 символа.
         */
        public static string GetHash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            foreach (byte chr in data)
                sBuilder.Append(chr.ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString().Substring(0, 64);
        }

        /**
         * Производит проверку переданной строки и её хэш кода.
         * 
         * Возвращает true, если проверка пройдена успешно.
         */
        public static bool VerifyHash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetHash(input);
            // Create a StringComparer an compare the hashes.
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }

        public static string StdEncToUTF8(string source)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] utf8Bytes = win1251.GetBytes(source);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
            source = win1251.GetString(win1251Bytes);
            return source;
        }

        /**
         * Метод для исполнения переданного запроса для данного соединения.
         * 
         * Соединение должно быть открыто.
         * 
         * Возвращает количество модифицированных записей.
         */
        public static int ExecuteNonQuery(string query, SQLiteConnection connection)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return result;
        }

        /**
         * Метод для исполнения переданного запроса для данного соединения.
         * 
         * Соединение должно быть открыто.
         * 
         * Возвращает объект SQLiteDataReader для дальнейшего чтения данных.
         */
        public static SQLiteDataReader ExecuteReader(string query, SQLiteConnection connection)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            cmd.Dispose();
            return reader;
        }
    }
}
