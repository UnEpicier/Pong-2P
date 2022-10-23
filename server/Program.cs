using LiteNetLib;
using LiteNetLib.Utils;
using shared;

namespace Server
{
    public class Program
    {
        static EventBasedNetListener listener = new();
        static NetManager? server;
        static NetPacketProcessor processor = new();
        static NetSerializer serializer = new();

        static string? port;
        static string? pswd;

        static string gameState = "Idle"; // Idle || Playing || Paused || Ended
        static List<NetPeer> players;

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

            serializer.Register<Assignation>();


            listener.ConnectionRequestEvent += request =>
            {
                Console.WriteLine($"New connection request from \"{request.RemoteEndPoint}\"");
                request.AcceptIfKey(pswd);
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("Connection accepted for: {0}", peer.EndPoint);
                if (peer != null)
                {
                    players.Add(peer);
                }
            };

            listener.PeerDisconnectedEvent += (peer, infos) =>
            {
                Console.WriteLine($"{peer.EndPoint} has left.\nError code: {infos.SocketErrorCode}\nReason: {infos.Reason}");
                players.Remove(peer);
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                if (server.GetPeersCount(ConnectionState.Connected) >= 2)
                {
                    // Differents game states
                    if (gameState == "Idle")
                    {
                        Assignation packet = new() { controller = 2, ballX = 0, ballY = 0 };
                        processor.Send(players[0], packet, DeliveryMethod.ReliableOrdered);
                        processor.Send(players[1], packet, DeliveryMethod.ReliableOrdered);
                        Console.WriteLine("Assignation sended");
                        gameState = "Playing";
                    }
                }
                Thread.Sleep(15);
            }

            server.Stop();
        }
    }
}