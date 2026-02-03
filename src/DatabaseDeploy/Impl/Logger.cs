using System;
using System.Globalization;
using System.IO;

namespace DatabaseDeploy.Impl
{
    public static class Logger
    {
        private const string LogFileName = "Log.txt";

        public static void LogStartApplication()
        {
            LogInfo("\n--------------------------------------------------------------------------------------\n");
            LogInfo(string.Format("Executing On: {0} \n", DateTime.Now.ToString(CultureInfo.InvariantCulture)));
        }

        public static void LogInfo(string text)
        {
            WritetoFile(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }

        public static void LogError(string text, Exception ex)
        {
            WritetoFile(string.Format("{0} Message: {1} \n Stack Trace: {2}", text, ex.Message, ex.StackTrace));

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
        }

        private static void WritetoFile(string text)
        {
            var fs = new FileStream(LogFileName, File.Exists(LogFileName) ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write);
            var streamWriter = new StreamWriter(fs);

            try
            {
                streamWriter.Write(text + "\n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                streamWriter.Close();
                streamWriter.Dispose();
                fs.Close();
                fs.Dispose();
            }
        }
    }
}
