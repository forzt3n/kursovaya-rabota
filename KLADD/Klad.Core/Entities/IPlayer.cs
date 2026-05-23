using System.Numerics;

namespace Klad.Core.Entities
{
    public interface IPlayer
    {
        int Id { get; }
        Vector2 Position { get; set; }
        Vector2 Direction { get; set; }
        float SpeedMultiplier { get; set; }
        float CurrentSpeed { get; }
        int Score { get; set; }
        TextureId TextureId { get; set; }
        
        void Move(IMap map, Vector2 moveDir);
        void CheckCollisions(List<Prize> prizes);
        void ActionPlaceWall(IMap map);
        void ActionBreakWall(IMap map);
    }
}
