namespace Klad.Core.Map
{
    public class TempEffect
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float TimeLeft { get; set; }
        public bool Visible => TimeLeft > 1.0f || (int)(TimeLeft * 10) % 2 == 0;
    }
}
