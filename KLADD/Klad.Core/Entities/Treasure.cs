namespace Klad.Core.Entities
{
    public class Treasure : Prize
    {
        public override void Apply(IPlayer player)
        {
            player.Score++;
        }
    }
}
