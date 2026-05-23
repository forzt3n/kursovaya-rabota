using System;

namespace Klad.Core.Entities
{
    public interface IMap
    {
        bool IsPassable(float x, float y);
        void TryPlaceWall(int ownerId, int x, int y);
        void OnInteract(int x, int y);
        void SetCell(int x, int y, IMazeElement element);
        bool HasEffectAt(int x, int y);
        bool IsAnyEffectActive<T>() where T : IMazeElement;
        void ApplyTemporaryEffect(int x, int y, float duration, Func<IMazeElement, IMazeElement> wrap);
        int Width { get; }
        int Height { get; }
    }

    public interface IMazeElement
    {
        bool IsWalkable { get; }
        TextureId TextureId { get; }
        void OnInteract(IMap map, int x, int y);
    }

    public class Floor : IMazeElement
    {
        public bool IsWalkable => true;
        public TextureId TextureId => TextureId.Empty;
        public void OnInteract(IMap map, int x, int y) { }
    }

    public class Wall : IMazeElement
    {
        public virtual bool IsWalkable => false;
        public virtual TextureId TextureId => TextureId.SolidWall;
        public virtual void OnInteract(IMap map, int x, int y) { }
        
        public Wall(bool walkable = false) { }
    }
}


