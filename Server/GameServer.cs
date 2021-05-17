using System;
using Shared;

namespace Server
{
    class GameServer
    {
        public static void Main(string[] args)
        {
            GameServer server = new GameServer();
            server.run();
        }

        private void run()
        {
            Log.LogInfo("This is a test", this, ConsoleColor.Red);
        }
    }
}