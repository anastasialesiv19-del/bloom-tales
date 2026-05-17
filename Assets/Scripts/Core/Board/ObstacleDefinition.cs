using Core.Matching;

namespace Core.Board
{
    public readonly struct ObstacleDefinition
    {
        public Pos Position { get; }
        public ObstacleKind Kind { get; }
        public int Health { get; }

        public ObstacleDefinition(Pos position, ObstacleKind kind, int health)
        {
            Position = position;
            Kind = kind;
            Health = health;
        }
    }
}