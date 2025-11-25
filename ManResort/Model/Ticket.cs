namespace ManResort.Model
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime RedeemDate { get; set; }
        public int Adults { get; set; }
        public int Kids { get; set; }
        public int HolidayAdults { get; set; }
        public int HolidayKids { get; set; }
        public int CoolerBoxes { get; set; }
        public int BusinessEntrance { get; set; }
        public int BusinessElectricEntrance { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string QRCodePath { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsRedeemed { get; set; }
        public string PaymentReference { get; set; }
    }
}
