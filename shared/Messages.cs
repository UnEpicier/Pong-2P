namespace shared
{
    public class Assignation
    {
        public int controller { get; set; } // 0 || 1
        public float ballX { get; set; }
        public float ballY { get; set; }
    }

    public class Position
    {
        public int controller { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }

    public class GameStateChange
    {
        public string gameState { get; set; }
    }
}