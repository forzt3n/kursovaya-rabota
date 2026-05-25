using Klad.Core.Entities;

namespace Klad.Core.Decorators
{
    public class TemporaryWallDecorator : ElementDecorator
    {
        public TemporaryWallDecorator(IMazeElement inner) : base(inner) { }

        public override bool IsWalkable => false;

        public override TextureId TextureId => TextureId.TemporaryWallPlaceholder;
    }
}
