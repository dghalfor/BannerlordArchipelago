using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace BannerlordArchipelago.Utils
{
    public static class APLog
    {
        public static void LogToFile(string msg)
        {
            try
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(BasePath.Name, "Modules", "BannerlordArchipelago", "ap_debug.log"),
                    $"{DateTime.Now:HH:mm:ss.fff} {msg}\n");
            }
            catch { /* never let logging itself crash the game */ }
        }
    }
}
