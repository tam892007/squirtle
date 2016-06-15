namespace BSE365.Model.Enum
{
    public enum TransactionState
    {
        Begin = 0,
        Transfered = 1,
        Confirmed = 2,

        NotTransfer = 21,
        NotConfirm = 22,
        ReportedNotTransfer = 23,

        Abandoned = 31,

        Failed = 51,
    }
}