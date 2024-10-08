using System;

namespace EntityFramework
{
    public class Purchase
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public Purchase(string email, string productCode, string productName)
        {
            Email = email;
            ProductCode = productCode;
            ProductName = productName;
        }

        public Purchase()
        {
            Email = String.Empty;
            ProductCode = String.Empty;
            ProductName = String.Empty;
        }
    }
}
