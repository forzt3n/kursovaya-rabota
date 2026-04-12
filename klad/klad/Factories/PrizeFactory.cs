using klad.Models;

namespace klad.Factories
{
    public abstract class PrizeFactory
    {
        public abstract Prize CreatePrize(PrizeType type);
    }

    public class GamePrizeFactory : PrizeFactory
    {
        public override Prize CreatePrize(PrizeType type)
        {
            switch (type)
            {
                case PrizeType.Treasure:
                    return new Treasure { TextureId = 5 }; 
                case PrizeType.SpeedBoost:
                    return new SpeedBoost { TextureId = 6 };
                case PrizeType.SpeedDebuff:
                    return new SpeedDebuff { TextureId = 7 };
                default:
                    throw new ArgumentException("Unknown prize type");
            }
        }
    }
}
