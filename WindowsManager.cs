using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace EntityFramework
{
    public static class WindowsManager
    {
        public static CustomersListVM CurrentCustomersListVM { get; set; }
        public static PurchasesListVM CurrentPurchasesListVM { get; set; }        

        public static List<CustomerFormVM> CurrentCustomerFormVMs { get; set; }
            = new List<CustomerFormVM>();

        #region Windows settings

        public static void CloseWindow(string windowName)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window.Name == windowName)
                {
                    window.Close();
                    break;
                }
            }
        }

        public static bool AlreadyOpenCheck(string windowName)
        {
            bool alreadyOpen = false;

            foreach (Window window in App.Current.Windows)
            {
                if (window.Name == windowName)
                {
                    alreadyOpen = true;
                    break;
                }
            }

            return alreadyOpen;
        }

        public static void SetOwner(string windowName, string ownerWindowName)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window.Name == windowName)
                {
                    foreach (Window ownerWindow in App.Current.Windows)
                    {
                        if (ownerWindow.Name == ownerWindowName)
                        {
                            window.Owner = ownerWindow;                            
                            break;
                        }
                    }
                }
            }
        }

        public static void SetMenuFormOwner(string windowName)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window.Name == windowName)
                {
                    foreach (Window ownerWindow in App.Current.Windows)
                    {
                        if (ownerWindow is MenuForm)
                        {
                            window.Owner = ownerWindow;
                            window.Closed += SubordinateWindow_Closed;
                            break;
                        }
                    }                    
                }
            }
        }

        public static void SubordinateWindow_Closed(object sender, EventArgs e)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is MenuForm)
                {
                    window.Activate();
                    break;
                }
            }
        }

        public static void ShowErrorMessageBox(string text)
        {
            MessageBox.Show(text,
                        "Операция не выполнена",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error,
                        MessageBoxResult.OK,
                        MessageBoxOptions.DefaultDesktopOnly);
        }

        #endregion

        #region Connection

        public static void ShowConnectionForm()
        {
            string connectionString = DBManager.GetConnectionString();
            string connectionStatus = DBManager.GetConnectionStatus();                      

            ConnectionForm connectionForm = new ConnectionForm
            {
                DataContext = new ConnectionFormVM(connectionString, connectionStatus)
            };

            connectionForm.Show();

            connectionForm.Name = $"{nameof(connectionForm)}";
            SetMenuFormOwner(connectionForm.Name);            
        }

        #endregion

        #region Customers

        public static void ShowCustomersList()
        {
            CustomersList customersList;
            string customersListName = $"{nameof(customersList)}";
            bool alreadyOpen = AlreadyOpenCheck(customersListName);

            if (alreadyOpen == false)
            {
                customersList = new CustomersList
                {
                    DataContext = new CustomersListVM(customersListName)
                };

                customersList.Show();

                customersList.Name = customersListName;
                SetMenuFormOwner(customersList.Name);                
            }
        }

        public static void ShowCustomerForm(int id, string ownerName)
        {
            CustomerForm customerForm;
            string customerFormName = $"{nameof(customerFormName)}{id}";
            bool alreadyOpen = AlreadyOpenCheck(customerFormName);

            if (alreadyOpen == false)
            {
                Customer customer = DBManager.GetCustomer(id);

                customerForm = new CustomerForm()
                {
                    DataContext = new CustomerFormVM(customer, customerFormName)
                };
                
                customerForm.Show();

                customerForm.Name = customerFormName;
                SetOwner(customerForm.Name, ownerName);
                customerForm.Closed += CustomerForm_Closed;
            }
        }

        public static void ShowNewCustomerForm(string ownerName)
        {
            CustomerForm customerForm;
            string customerFormName = $"{nameof(customerFormName)}";
            bool alreadyOpen = AlreadyOpenCheck(customerFormName);

            if (alreadyOpen == false)
            {
                customerForm = new CustomerForm
                {
                    DataContext = new CustomerFormVM(customerFormName)
                };                
                
                customerForm.Show();

                customerForm.Name = customerFormName;
                SetOwner(customerForm.Name, ownerName);
                customerForm.Closed += CustomerForm_Closed;
            }
        }        

        public static void DeleteCustomer(int id)
        {
            DBManager.DeleteCustomer(id);

            CurrentCustomersListVM.UpdateCustomers();
        }

        public static void ClearCustomerFormVMs()
        {
            CustomerFormVM removeVM = null;

            if (CurrentCustomerFormVMs.Count > 0)
            {
                foreach (CustomerFormVM customerFormVM in CurrentCustomerFormVMs)
                {
                    removeVM = customerFormVM;

                    foreach (Window window in App.Current.Windows)
                    {
                        if (window.Name == customerFormVM.Name)
                        {
                            removeVM = null;
                        }
                    }

                    if (removeVM != null)
                    {
                        break;
                    }
                }
            }

            if (removeVM != null)
            {
                CurrentCustomerFormVMs.Remove(removeVM);
            }
        }

        public static void CustomerForm_Closed(object sender, EventArgs e)
        {
            CurrentCustomersListVM.UpdateCustomers();
            ClearCustomerFormVMs();
        }

        #endregion

        #region Purchases

        public static void ShowPurchasesList()
        {
            PurchasesList purchasesList;
            string purchasesListName = $"{nameof(purchasesList)}";
            bool alreadyOpen = AlreadyOpenCheck(purchasesListName);

            if (alreadyOpen == false)
            {
                purchasesList = new PurchasesList
                {
                    DataContext = new PurchasesListVM(purchasesListName)
                };

                purchasesList.Show();

                purchasesList.Name = purchasesListName;               
                SetMenuFormOwner(purchasesList.Name);                
            }
        }

        public static void ShowPurchaseForm(int id, string ownerName, bool emailIsEnabled)
        {
            PurchaseForm purchaseForm;
            string purchaseFormName = $"{nameof(purchaseFormName)}{id}";
            bool alreadyOpen = AlreadyOpenCheck(purchaseFormName);

            if (alreadyOpen == false)
            {
                Purchase purchase = DBManager.GetPurchase(id);

                purchaseForm = new PurchaseForm
                {
                    DataContext = new PurchaseFormVM(purchase, purchaseFormName, emailIsEnabled)
                };

                purchaseForm.Show();

                purchaseForm.Name = purchaseFormName;
                SetOwner(purchaseForm.Name, ownerName);
                purchaseForm.Closed += PurchaseForm_Closed;
            }
        }

        public static void ShowNewPurchaseForm(string email, string ownerName)
        {
            PurchaseForm purchaseForm;
            string purchaseFormName = $"{nameof(purchaseFormName)}";
            bool alreadyOpen = AlreadyOpenCheck(purchaseFormName);

            if (alreadyOpen == false)
            {
                purchaseForm = new PurchaseForm
                {
                    DataContext = new PurchaseFormVM(email, purchaseFormName)
                };

                purchaseForm.Show();

                purchaseForm.Name = purchaseFormName;
                SetOwner(purchaseForm.Name, ownerName);
                purchaseForm.Closed += PurchaseForm_Closed;
            }            
        }        

        public static void DeletePurchase(int id, string windowName)
        {
            DBManager.DeletePurchase(id);

            if (CurrentPurchasesListVM != null)
            {
                CurrentPurchasesListVM.UpdatePurchases();
            }

            if (CurrentCustomerFormVMs.Count > 0)
            {
                foreach (CustomerFormVM customerFormVM in CurrentCustomerFormVMs)
                {
                    if (customerFormVM.WindowName == windowName)
                    {
                        customerFormVM.UpdatePurchases();
                    }
                }
            }            
        }

        public static void PurchaseForm_Closed(object sender, EventArgs e)
        {
            if (CurrentPurchasesListVM != null)
            {
                CurrentPurchasesListVM.UpdatePurchases();
            }

            if (CurrentCustomerFormVMs.Count > 0)
            {
                foreach (CustomerFormVM customerFormVM in CurrentCustomerFormVMs)
                {
                    customerFormVM.UpdatePurchases();
                }
            }
        }

        #endregion        
    }
}
