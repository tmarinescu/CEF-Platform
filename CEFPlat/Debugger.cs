using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEFPlat
{
    public static class Debugger
    {
        public static bool DebugFunction(Action act, string str)
        {
            Console.Write(str + "... ");
            try
            {
                act.Invoke();
                Console.WriteLine("Done!");
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Failed! ({e.Message})");
                return false;
            }
        }
    }
}
