namespace Core.Board
{
    public struct ObstacleModel
    {
        public ObstacleKind Kind;
        public int Health;
        
        public bool IsEmpty => Kind == ObstacleKind.None;
        
        public static ObstacleModel Empty => new() { Kind = ObstacleKind.None, Health = 0 };
    }
}