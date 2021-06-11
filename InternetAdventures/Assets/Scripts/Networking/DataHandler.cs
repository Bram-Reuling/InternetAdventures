namespace Networking
{
    public static class DataHandler
    {
        public static ushort Port;
        public static bool IsMainServerClientAlreadySpawned = false;
        public static MainServerClient MainServerClientInstance;
        public static string MenuState = "LoginPanel";
    }
}