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

            // ПРОВЕРКА: Если уже какая-то другая стена сейчас восстанавливается (является проходом)
            if (map.IsAnyEffectActive<TemporaryPassageDecorator>()) return;

            // Если всё чисто, превращаем в проход на 4 секунды
            map.ApplyTemporaryEffect(x, y, 4.0f, inner => new TemporaryPassageDecorator(inner));
        }
    }
}
