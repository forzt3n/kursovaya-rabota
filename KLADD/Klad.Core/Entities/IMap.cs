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
}
