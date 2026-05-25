using Klad.Core.Entities;

namespace Klad.Core.Factories
{



    public abstract class PrizeFactory
    {





        public abstract Prize CreatePrize(PrizeType type);
    }
}
