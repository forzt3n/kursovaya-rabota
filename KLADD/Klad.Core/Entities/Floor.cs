namespace Klad.Core.Entities
{
    public class Floor : IMazeElement
    {
        public bool IsWalkable => true;

        public TextureId TextureId => TextureId.Empty;

        public void OnInteract(IMap map, int x, int y) { }
    }
}
