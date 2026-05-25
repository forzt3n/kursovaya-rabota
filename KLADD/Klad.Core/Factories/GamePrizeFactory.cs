using System;
using Klad.Core.Entities;

namespace Klad.Core.Factories
{
    public class GamePrizeFactory : PrizeFactory
    {
        public override Prize CreatePrize(PrizeType type)
        {
            switch (type)
            {
                case PrizeType.Treasure:
                    return new Treasure { TextureId = TextureId.Treasure };
                case PrizeType.SpeedBoost:
                    return new SpeedBoost { TextureId = TextureId.Boost };
                case PrizeType.SpeedDebuff:
                    return new SpeedDebuff { TextureId = TextureId.Debuff };
                default:
                    throw new ArgumentException("Unknown prize type");
            }
        }
    }
}
