using LiteNetLib;
using LiteNetLib.Utils;

namespace Server
{
    public class Program
    {
        static EventBasedNetListener listener = new();
        static NetManager? server;

        static string? port;
        static string? pswd;

        public static void Main(string[] args)
        {
            server = new NetManager(listener);
            port = Lib.ReadSetting("PORT");
            pswd = Lib.ReadSetting("PSWD");

            if (port != null && pswd != null)
            {
                Console.WriteLine($"Server started on port {port}");
                server.Start(int.Parse(port));
            }
            else
            {
                Console.Error.WriteLine("Missing port or password in config file !\nClosing server...");
                return;
            }


            listener.ConnectionRequestEvent += request =>
            {
                Console.WriteLine($"New connection request from \"{request.RemoteEndPoint}\"");
                request.AcceptIfKey(pswd);
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("Connection accepted for: {0}", peer.EndPoint);
                /* NetDataWriter writer = new();
                writer.Put("Hello client!");
                peer.Send(writer, DeliveryMethod.ReliableOrdered); */
            };

            listener.PeerDisconnectedEvent += (peer, infos) =>
            {
                Console.WriteLine($"{peer.EndPoint} has left.\nError code: {infos.SocketErrorCode}\nReason: {infos.Reason}");
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }

            server.Stop();
        }
    }
}