using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using DarkMultiPlayerCommon;
using MessageStream;

namespace DarkMultiPlayerServer
{
    public class RCON
    {
        public static TcpListener rconListener;

        public static void StartRCONServer()
        {
            if (Settings.settingsStore.rconPort > 0)
            {
                DarkLog.Normal("Starting RCON server...");
                try
                {
                    IPAddress bindAddress = IPAddress.Parse(Settings.settingsStore.address);
                    rconListener = new TcpListener(bindAddress, Settings.settingsStore.rconPort);
                    rconListener.Start(4);
                    rconListener.BeginAcceptTcpClient(new AsyncCallback(ClientCallback), null);
                }
                catch (Exception e)
                {
                    DarkLog.Error("Error while starting RCON Server!, Exception: " + e);
                }
            }
        }

        private static void ClientCallback(IAsyncResult result)
        {
            try
            {
                TcpListener listener = (TcpListener)result.AsyncState;
                TcpClient client = listener.EndAcceptTcpClient(result);

                byte[] bytes = new byte[256];
                string data = null;

                NetworkStream stream = client.GetStream();
                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                 //   data = Encoding.ASCII.GetString(bytes, 0, i);
                 //   Console.WriteLine("[RCON] Received from " + client.Client.RemoteEndPoint + ": " + data);
                    using (MessageReader mr = new MessageReader(bytes, false))
                    {
                        HandleMessage(client, mr.Read<RCONMessage>());
                    }

                    data = data.ToUpper();

                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("[RCON] Sent '" + data + "' to " + client.Client.RemoteEndPoint);
                }

                client.Close();

                Console.WriteLine("Client connected and disconnected.");
            }
            catch (Exception e)
            {
                DarkLog.Error("Error in RCON Callback!, Exception: " + e);
            }
        }

        public static void StopRCONServer()
        {
            if (Settings.settingsStore.rconPort > 0)
            {
                DarkLog.Normal("Stopping RCON server...");
                rconListener.Stop();
            }
        }

        public static void ForceStopRCONServer()
        {
            if (Settings.settingsStore.rconPort > 0)
            {
                DarkLog.Normal("Force stopping RCON server...");
                if (rconListener != null)
                {
                    try
                    {
                        rconListener.Stop();
                    }
                    catch (Exception e)
                    {
                        DarkLog.Fatal("Error trying to shutdown RCON server: " + e);
                        throw;
                    }
                }
            }
        }

        public static void HandleMessage(TcpClient client,  RCONMessage message)
        {
            try
            {
                switch (message.type)
                {
                    case RCONMessageType.HEARTBEAT_REQUEST:
                        Heartbeat(client);
                        break;
                    case RCONMessageType.SAY:
                        HandleSayMessage(client, message.data);
                        break;
                    case RCONMessageType.DISCONNECT:
                        HandleDisconnect(client, message.data);
                        break;
                    default:
                        DarkLog.Debug("[RCON] Unhandled message type " + message.type);
                        break;
                        
                }
            }
            catch (Exception e)
            {
                DarkLog.Debug("Error handling " + message.type + " from " + client.Client.RemoteEndPoint + ", exception: " + e);
            }
        }

        private static void SendToClient(TcpClient client, RCONMessage message)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                stream.Write(message.data, 0, message.data.Length);
            }
            catch (Exception e)
            {
                DarkLog.Error("[RCON] Error while trying to send message to client!, Exception: " + e);
            }
        }

        private static void Heartbeat(TcpClient client)
        {
            RCONMessage newMessage = new RCONMessage();
            newMessage.type = RCONMessageType.HEARTBEAT_REPLY;
            using (MessageWriter mw = new MessageWriter())
            {
                mw.Write<string>("PONG : " + (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
                newMessage.data = mw.GetMessageBytes();
            }
            SendToClient(client, newMessage);
        }

        private static void HandleSayMessage(TcpClient client, byte[] messageData)
        {

        }

        private static void HandleDisconnect(TcpClient client, byte[] messageData)
        {
            using (MessageReader mr = new MessageReader(messageData, false))
            {
                string reason = mr.Read<string>();
                DarkLog.Debug("[RCON] Client " + client.Client.RemoteEndPoint + " disconnected: " + reason);
            }
        }
    }
}
