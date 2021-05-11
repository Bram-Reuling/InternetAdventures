using System;
using BLog;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new Logger();
            
            logger.Log(LogType.Error, "Yeet");
        }
    }
}