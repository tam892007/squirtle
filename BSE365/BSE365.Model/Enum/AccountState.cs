namespace BSE365.Model.Enum
{
    public enum AccountState
    {
        /// <summary>
        ///     Must give
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Queued in WaitingGive
        /// </summary>
        WaitGive = 1,

        /// <summary>
        ///     Gave
        ///     Can queue receive
        /// </summary>
        Gave = 2,

        /// <summary>
        ///     Queued in WaitingReceive
        /// </summary>
        WaitReceive = 3,

        InGiveTransaction = 11,
        InReceiveTransaction = 12,

        NotGive = 21,
        NotConfirm = 22,
        ReportedNotTransfer = 23,

        AbandonOne = 31
    }
}