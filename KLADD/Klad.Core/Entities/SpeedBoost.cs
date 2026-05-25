namespace Klad.Core.Entities
{
    public class SpeedBoost : Prize
    {
        public override void Apply(IPlayer player)
        {
            player.SpeedMultiplier += 0.2f;
        }
    }
}
