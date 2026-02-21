namespace EmployeeManagement.API.Models
{
    public class PaymentDetails
    {
        public string PaymentMethod { get; set; }
        public CardDetails Card { get; set; }
        public NetBankingDetails NetBanking { get; set; }
        public UpiDetails Upi { get; set; }
        public WalletDetails Wallet { get; set; }
    }

    public class CardDetails
    {
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string CardCVV { get; set; }
        public string MaskedCardNumber => 
            $"****-****-****-{CardNumber?.Substring(CardNumber.Length - 4)}";
    }

    public class NetBankingDetails
    {
        public string BankName { get; set; }
        public string IFSCCode { get; set; }
        public string AccountNumber { get; set; }
        public string MaskedAccountNumber => 
            $"****{AccountNumber?.Substring(Math.Max(0, AccountNumber.Length - 4))}";
    }

    public class UpiDetails
    {
        public string UpiId { get; set; }
        public string Provider => UpiId?.Split('@')[1];
    }

    public class WalletDetails
    {
        public string WalletName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string MaskedPhoneNumber => 
            $"****{PhoneNumber?.Substring(Math.Max(0, PhoneNumber.Length - 4))}";
    }
}