namespace Klad.Core.Entities
{
    public class Wall : IMazeElement
    {
        public virtual bool IsWalkable => false;
        public virtual TextureId TextureId => TextureId.SolidWall;
        public virtual void OnInteract(IMap map, int x, int y) { }
        public Wall(bool walkable = false) { }
    }
}
