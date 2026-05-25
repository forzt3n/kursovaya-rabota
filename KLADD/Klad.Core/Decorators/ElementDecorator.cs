using Klad.Core.Entities;

namespace Klad.Core.Decorators
{
    public abstract class ElementDecorator : IMazeElement
    {
        protected IMazeElement _inner;

        public ElementDecorator(IMazeElement inner) { _inner = inner; }

        public virtual bool IsWalkable => _inner.IsWalkable;

        public virtual TextureId TextureId => _inner.TextureId;

        public virtual void OnInteract(IMap map, int x, int y) => _inner.OnInteract(map, x, y);

        public IMazeElement GetInner() => _inner;
    }
}
