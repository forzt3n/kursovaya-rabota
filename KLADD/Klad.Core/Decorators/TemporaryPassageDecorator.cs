using Klad.Core.Entities;

namespace Klad.Core.Decorators
{
    public class TemporaryPassageDecorator : ElementDecorator
    {
        public TemporaryPassageDecorator(IMazeElement inner) : base(inner) { }

        public override bool IsWalkable => true;

        public override TextureId TextureId => TextureId.Empty;
    }
}
