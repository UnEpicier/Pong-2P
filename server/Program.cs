using LiteNetLib;
using LiteNetLib.Utils;
using shared;

namespace Server
{
    public class Program
    {
        static EventBasedNetListener listener;
        static NetManager server;
        static NetPacketProcessor processor;

        static string? port;
        static string? pswd;
        /*
         None => Waiting for minimum 2 clients
         Idle => Preparaing the game, waiting for client 1 to start the game
         Playing => Game actually playing
         Paused => Game paused by a player
         Ended => Game Ended, waiting for client 1 decision (exiting or restart)
         Disconnected => One of the two players has been disconnected
         */
        static string gameState = "None";
        static List<NetPeer> activeConnections;
        static List<NetPeer> players;

        // Game

        public static void Main(string[] args)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            // Application
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            listener = new EventBasedNetListener();
            server = new NetManager(listener);
            processor = new NetPacketProcessor();

            activeConnections = new List<NetPeer>();
            players = new List<NetPeer>();

            processor.SubscribeReusable<GameStateChange>(GameStateHandler);
            processor.SubscribeReusable<Position>(PositionHandler);


            port = Lib.ReadSetting("PORT");
            pswd = Lib.ReadSetting("PSWD");

            if (port != null && pswd != null)
            {
                server.Start(int.Parse(port));
                Console.WriteLine($"Server started on port {port}");
            }
            else
            {
                Console.Error.WriteLine("Missing port or password in config file !\nClosing server...");
                return;
            }
            listener.NetworkReceiveEvent += (client, reader, deliveryMethod) =>
            {
                processor.ReadAllPackets(reader, client);
            };

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
                    activeConnections.Add(peer);
                }
            };

            listener.PeerDisconnectedEvent += (peer, infos) =>
            {
                Console.WriteLine($"{peer.EndPoint} has left.\nError code: {infos.SocketErrorCode}\nReason: {infos.Reason}");
                activeConnections.Remove(peer);
                if (players.Contains(peer))
                {
                    players.Remove(peer);
                    gameState = "Disconnected";
                    GameStateChange packet = new() { gameState = "Disconnected" };
                    server.SendToAll(processor.Write(packet), DeliveryMethod.ReliableOrdered);
                }

                if (activeConnections.Count < 2)
                {
                    gameState = "None";
                }
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                if (activeConnections.Count >= 2)
                {
                    // Differents game states
                    if (gameState == "None")
                    {
                        float MoveX = new float[2] { -2f, 2f }[random.Next(2)];
                        float MoveY = new float[2] { -2f, 2f }[random.Next(2)];
                        Assignation packet = new() { controller = 0, ballX = MoveX, ballY = MoveY };
                        processor.Send(activeConnections[0], packet, DeliveryMethod.ReliableOrdered);
                        players.Add(activeConnections[0]);

                        packet.controller = 1;

                        processor.Send(activeConnections[1], packet, DeliveryMethod.ReliableOrdered);
                        players.Add(activeConnections[1]);

                        gameState = "Idle";
                        Console.WriteLine($"Change GameState to {gameState}");
                        server.SendToAll(processor.Write(new GameStateChange() { gameState = "Idle" }), DeliveryMethod.ReliableOrdered);
                    }
                }
                Thread.Sleep(15);
            }

            server.Stop();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            server.DisconnectAll();
        }

        private static void PositionHandler(Position position)
        {
            server.SendToAll(processor.Write(position), DeliveryMethod.ReliableOrdered);
        }

        private static void GameStateHandler(GameStateChange change)
        {
            Console.WriteLine($"Change GameState to {change.gameState}");
            gameState = change.gameState;
            server.SendToAll(processor.Write(change), DeliveryMethod.ReliableOrdered);
        }
    }
}