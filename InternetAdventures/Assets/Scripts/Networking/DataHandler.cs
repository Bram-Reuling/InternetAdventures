namespace Networking
{
    public static class DataHandler
    {
        public static ushort Port;
        public static bool IsMainServerClientAlreadySpawned = false;
        public static MainServerClient MainServerClientInstance;
        public static string MenuState = "LoginPanel";

        public static int NoOfPlayers = 1;

        public static bool PlayersAreSpawned = false;

        public static string PlayerName = "Bram";
    }
}