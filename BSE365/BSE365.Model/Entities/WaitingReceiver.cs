using BSE365.Common.Constants;

namespace BSE365.Model.Entities
{
    public class WaitingReceiver : WaitingBase
    {
        public WaitingReceiver()
        {
            Amount = TransactionConfig.ReceiveAmountDefault;
        }
    }
}