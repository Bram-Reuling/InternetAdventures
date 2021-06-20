using System;
using System.Diagnostics.CodeAnalysis;

namespace MainServer.PacketHandlers
{
    public class AlivePacketsHandler
    {
        public AlivePacketsHandler()
        {
        }
        
        public void HandleIsAlivePacket(ClientServerInfo clientServerInfo)
        {
            clientServerInfo.SetLastIsAliveTime(DateTime.Now);
        }
    }
}   