using System;
using System.Runtime.CompilerServices;

namespace BasicTCPServer {
    public static class Log {
        public static void Write(object output, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
            file = file.Remove(0, file.LastIndexOf('\\') + 1);
            string where = file + ":" + line;

            Console.WriteLine("[" + where + "] " + output.ToString());
        }

        public static void WriteError(object output, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
            file = file.Remove(0, file.LastIndexOf('\\') + 1);
            string where = file + ":" + line;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[" + where + "] " + output.ToString());
            Console.ResetColor();
        }
    }
}
