using Klad.Core.Entities;

namespace Klad.Core.Decorators
{
    public class DestructibleDecorator : ElementDecorator
    {
        public DestructibleDecorator(IMazeElement inner) : base(inner) { }

        public override TextureId TextureId => TextureId.BreakableWall;

        public override void OnInteract(IMap map, int x, int y)
        {
            // Logic to break the wall
            map.SetCell(x, y, new Floor());
        }
    }
}
