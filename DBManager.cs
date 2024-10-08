using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EntityFramework
{
    public static class DBManager
    {
        #region Connection

        public static string ConnectionString { get; private set; } = String.Empty;
        public static string ConnectionStatus { get; private set; } = String.Empty;

        public static string GetConnectionString()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                ConnectionString = $"{db.Database.GetConnectionString()}";
            }

            return ConnectionString;
        }

        public static string GetConnectionStatus()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                ConnectionStatus = $"{db.Database.GetDbConnection().State}";
            }           

            return ConnectionStatus;
        }

        #endregion

        #region Customers

        public static List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();            

            using (ApplicationContext db = new ApplicationContext())
            {
                var customersDB = db.Customers.ToList();                

                foreach (var customer in customersDB)
                {
                    customers.Add(customer);
                }
            }            

            return customers;
        }
        public static Customer GetCustomer(int id)
        {
            Customer customer = new Customer();

            using (ApplicationContext db = new ApplicationContext())
            {
                var customersDB = db.Customers.Where(p => p.Id == id).ToList();

                customer = customersDB.First();
            }

            return customer;
        }

        public static void AddCustomer(Customer customer)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Customers.Add(customer);
                db.SaveChanges();
            }            
        }

        public static void DeleteCustomer(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var customersDB = db.Customers.Where(p => p.Id == id).ToList();

                db.Customers.Remove(customersDB.First());
                db.SaveChanges();
            }            
        }        

        public static void EditCustomer(Customer customer)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var customersDB = db.Customers.Where(p => p.Id == customer.Id).ToList();

                bool propertyChanged = false;

                if (customersDB[0].Surname != customer.Surname)
                {
                    customersDB[0].Surname = customer.Surname;
                    propertyChanged = true;
                }

                if (customersDB[0].Name != customer.Name)
                {
                    customersDB[0].Name = customer.Name;
                    propertyChanged = true;
                }

                if (customersDB[0].Patronymic != customer.Patronymic)
                {
                    customersDB[0].Patronymic = customer.Patronymic;
                    propertyChanged = true;
                }

                if (customersDB[0].PhoneNumber != customer.PhoneNumber)
                {
                    customersDB[0].PhoneNumber = customer.PhoneNumber;
                    propertyChanged = true;
                }

                if (customersDB[0].Email != customer.Email)
                {
                    customersDB[0].Email = customer.Email;
                    propertyChanged = true;
                }

                if (propertyChanged == true)
                {
                    db.Customers.Update(customersDB[0]);
                    db.SaveChanges();
                }
            }            
        }

        #endregion

        #region Purchases

        public static List<Purchase> GetPurchases()
        {
            List<Purchase> purchases = new List<Purchase>();

            using (ApplicationContext db = new ApplicationContext())
            {
                var purchasesDB = db.Purchases.ToList();

                foreach (var purchase in purchasesDB)
                {
                    purchases.Add(purchase);
                }
            }

            return purchases;
        }

        public static List<Purchase> GetPurchasesByEmail(string email)
        {
            List<Purchase> purchases = new List<Purchase>();

            using (ApplicationContext db = new ApplicationContext())
            {
                var purchasesDB = db.Purchases.Where(p => p.Email == email).ToList();

                foreach (var purchase in purchasesDB)
                {
                    purchases.Add(purchase);
                }
            }

            return purchases;
        }

        public static Purchase GetPurchase(int id)
        {
            Purchase purchase = new Purchase();

            using (ApplicationContext db = new ApplicationContext())
            {
                var purchasesDB = db.Purchases.Where(p => p.Id == id).ToList();

                purchase = purchasesDB.First();
            }                        

            return purchase;
        }

        public static void AddPurchase(Purchase purchase)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Purchases.Add(purchase);
                db.SaveChanges();
            }
        }

        public static void DeletePurchase(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var purchasesDB = db.Purchases.Where(p => p.Id == id).ToList();

                db.Purchases.Remove(purchasesDB.First());
                db.SaveChanges();
            }            
        }        

        public static void EditPurchase(Purchase purchase)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var purchasesDB = db.Purchases.Where(p => p.Id == purchase.Id).ToList();

                bool propertyChanged = false;

                if (purchasesDB[0].Email != purchase.Email)
                {
                    purchasesDB[0].Email = purchase.Email;
                    propertyChanged = true;
                }

                if (purchasesDB[0].ProductCode != purchase.ProductCode)
                {
                    purchasesDB[0].ProductCode = purchase.ProductCode;
                    propertyChanged = true;
                }

                if (purchasesDB[0].ProductName != purchase.ProductName)
                {
                    purchasesDB[0].ProductName = purchase.ProductName;
                    propertyChanged = true;
                }

                if (propertyChanged)
                {
                    db.Purchases.Update(purchasesDB[0]);
                    db.SaveChanges();
                }
            }            
        }       

        #endregion
    }
}
