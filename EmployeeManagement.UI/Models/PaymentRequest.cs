using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.UI.Models
{
    public class PaymentRequest
    {
        [Required(ErrorMessage = "Employee ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid employee")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 1000000, ErrorMessage = "Amount must be between ₹1 and ₹10,00,000")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public string PaymentMethod { get; set; }

        // Card Payment Fields
        [StringLength(19)]
        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Invalid card number")]
        public string CardNumber { get; set; }

        [StringLength(5)]
        [RegularExpression(@"^\d{2}/\d{2}$", ErrorMessage = "Format: MM/YY")]
        public string CardExpiry { get; set; }

        [StringLength(4)]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVV")]
        public string CardCVV { get; set; }

        // Net Banking Fields
        [StringLength(50)]
        public string BankName { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code format")]
        public string IFSCCode { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^\d{9,18}$", ErrorMessage = "Invalid account number")]
        public string AccountNumber { get; set; }

        // UPI Payment Fields
        [StringLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z]{3,}$", ErrorMessage = "Invalid UPI ID format")]
        public string UpiId { get; set; }

        // Wallet Payment Fields
        [StringLength(50)]
        public string WalletName { get; set; }

        [StringLength(20)]
        public string WalletPhoneNumber { get; set; }

        [StringLength(50)]
        public string WalletEmail { get; set; }
    }
}