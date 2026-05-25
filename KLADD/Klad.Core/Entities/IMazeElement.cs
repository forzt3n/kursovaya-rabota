namespace Klad.Core.Entities
{
    public interface IMazeElement
    {
        bool IsWalkable { get; }

        TextureId TextureId { get; }

        void OnInteract(IMap map, int x, int y);
    }
}
