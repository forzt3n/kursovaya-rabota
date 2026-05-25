namespace Klad.Core
{
    public enum GameKey
    {
        Up, Down, Left, Right,
        Action1, Action2,
        P2_Up, P2_Down, P2_Left, P2_Right,
        P2_Action1, P2_Action2,
        Enter, Escape, Space
    }

    public interface IInputService
    {
        bool IsKeyPressed(GameKey key);
        bool IsKeyDown(GameKey key);
    }
}
