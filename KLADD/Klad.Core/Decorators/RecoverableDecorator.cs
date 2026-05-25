using Klad.Core.Entities;
using Klad.Core.Map;

namespace Klad.Core.Decorators
{
    public class RecoverableDecorator : ElementDecorator
    {
        public RecoverableDecorator(IMazeElement inner) : base(inner) { }

        public override TextureId TextureId => TextureId.RecoverableWall;

        public override void OnInteract(IMap map, int x, int y)
        {
            if (map.HasEffectAt(x, y)) return;

            if (map.IsAnyEffectActive<TemporaryPassageDecorator>()) return;

            map.ApplyTemporaryEffect(x, y, 4.0f, inner => new TemporaryPassageDecorator(inner));
        }
    }
}
