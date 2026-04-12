namespace klad.Models
{
    public interface IMazeElement
    {
        bool IsWalkable { get; }
        bool CanPassage { get; }
        bool IsDestructible { get; }
        int TextureId { get; }
    }

    public class Floor : IMazeElement
    {
        public bool IsWalkable => true;
        public bool CanPassage => false;
        public bool IsDestructible => false;
        public int TextureId => 0;
    }

    public class Wall : IMazeElement
    {
        public bool IsWalkable => false;
        public bool CanPassage { get; }
        public bool IsDestructible => false;
        public int TextureId => 1;

        public Wall(bool canPassage)
        {
            CanPassage = canPassage;
        }
    }

    public class DestructibleWall : IMazeElement
    {
        public bool IsWalkable => false;
        public bool CanPassage => false;
        public bool IsDestructible => true;
        public int TextureId => 2;
    }
}
