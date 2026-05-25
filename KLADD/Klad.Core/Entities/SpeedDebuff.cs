namespace Klad.Core.Entities
{
    public class SpeedDebuff : Prize
    {
        public override void Apply(IPlayer player)
        {
            player.SpeedMultiplier -= 0.15f;
            if (player.SpeedMultiplier < 0.5f) player.SpeedMultiplier = 0.5f;
        }
    }
}
