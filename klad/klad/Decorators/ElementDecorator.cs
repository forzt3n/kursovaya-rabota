using klad.Models;

namespace klad.Decorators
{
    public abstract class ElementDecorator : IMazeElement
    {
        public IMazeElement DecoratedElement { get; }

        public ElementDecorator(IMazeElement element)
        {
            DecoratedElement = element;
        }

        public virtual bool IsWalkable => DecoratedElement.IsWalkable;
        public virtual bool CanPassage => DecoratedElement.CanPassage;
        public virtual bool IsDestructible => DecoratedElement.IsDestructible;
        public virtual int TextureId => DecoratedElement.TextureId;
    }

    public class TemporaryPassageDecorator : ElementDecorator
    {
        public TemporaryPassageDecorator(IMazeElement element) : base(element) { }

        public override bool IsWalkable => true; 
        public override int TextureId => 3; 
    }

    public class TemporaryWallDecorator : ElementDecorator
    {
        public TemporaryWallDecorator(IMazeElement element) : base(element) { }

        public override bool IsWalkable => false; 
        public override int TextureId => 4; 
    }
}
