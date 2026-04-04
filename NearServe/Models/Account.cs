namespace NearServe.Models
{
    public class Account
    {
            public int CID { get; set; }
            public string Account_No { get; set; } = string.Empty;
            public string Customer_Name { get; set; } = string.Empty;
            public string Branch { get; set; } = string.Empty;
            public string Bank_Name { get; set; } = string.Empty;
            public decimal Balance { get; set; }
        
    }
}

