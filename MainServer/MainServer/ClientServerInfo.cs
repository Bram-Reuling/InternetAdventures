using System;
using System.Net.Sockets;
using Shared.model;

namespace MainServer
{
    public class ClientServerInfo
    {
        public TcpClient TcpClient { get; private set; } = new TcpClient();
        public Client Client { get; private set; } = new Client();
        public DateTime LastIsAliveTime { get; private set; } = new DateTime();

        public ClientServerInfo()
        {
            
        }

        /// <summary>
        /// Set the TcpClient
        /// </summary>
        /// <param name="pClient"></param>
        /// <returns></returns>
        public bool SetTcpClient(TcpClient pClient)
        {
            try
            {
                TcpClient = pClient;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Set the Client
        /// </summary>
        /// <param name="pClient"></param>
        /// <returns></returns>
        public bool SetClient(Client pClient)
        {
            try
            {
                Client = pClient;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Set the last is alive time
        /// </summary>
        /// <param name="pTime"></param>
        /// <returns></returns>
        public bool SetLastIsAliveTime(DateTime pTime)
        {
            try
            {
                LastIsAliveTime = pTime;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}