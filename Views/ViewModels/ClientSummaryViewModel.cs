namespace Loan_Processing_Inzamam.ViewModels
{
    public class ClientSummaryViewModel
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string NIDNumber { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalLoanTaken { get; set; }
        public decimal TotalInstallmentsMade { get; set; }
    }
}
