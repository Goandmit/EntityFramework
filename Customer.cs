using System;
using System.Xml.Linq;

namespace EntityFramework
{
    public class Customer
    {
        public int Id { get; set; }       
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Customer(string surname, string name, string patronymic, string phoneNumber, 
            string email)
        {           
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public Customer()
        {
            Surname = String.Empty;
            Name = String.Empty;
            Patronymic = String.Empty;            
            Email = String.Empty;
        }
    }
}
