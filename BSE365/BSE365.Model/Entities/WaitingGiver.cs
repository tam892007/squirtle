using BSE365.Common.Constants;

namespace BSE365.Model.Entities
{
    public class WaitingGiver : WaitingBase
    {
        public WaitingGiver()
        {
            Amount = TransactionConfig.GiveAmountDefault;
        }
    }
}