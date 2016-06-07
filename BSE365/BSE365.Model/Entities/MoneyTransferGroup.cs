namespace BSE365.Model.Entities
{
    public class MoneyTransferGroup : BaseEntity
    {
        public string Giver1Id { get; set; }
        public string Giver2Id { get; set; }
        public string Giver3Id { get; set; }
        public string Receiver1Id { get; set; }
        public string Receiver2Id { get; set; }
    }
}