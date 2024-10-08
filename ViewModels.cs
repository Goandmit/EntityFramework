using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace EntityFramework
{
    public class MenuFormVM
    {
        public MenuFormVM() { }               

        private RelayCommand showConnectionCommand;
        public RelayCommand ShowConnectionCommand
        {
            get
            {
                return showConnectionCommand ??
                  (showConnectionCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowConnectionForm();
                  }));
            }
        }

        private RelayCommand showCustomersCommand;
        public RelayCommand ShowCustomersCommand
        {
            get
            {
                return showCustomersCommand ??
                  (showCustomersCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowCustomersList();
                  }));
            }
        }

        private RelayCommand showPurchasesCommand;
        public RelayCommand ShowPurchasesCommand
        {
            get
            {
                return showPurchasesCommand ??
                  (showPurchasesCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowPurchasesList();
                  }));
            }
        }
    }

    public class ConnectionFormVM
    {
        public string ConnectionString { get; set; }
        public string ConnectionStatus { get; set; }
        
        public ConnectionFormVM(string connectionString, string connectionStatus)
        {
            ConnectionString = connectionString;
            ConnectionStatus = connectionStatus;
        }
    }

    public class CustomersListVM
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public Customer SelectedCustomer { get; set; }

        public string WindowName { get; set; }

        public CustomersListVM(string windowName)
        {
            Customers = new ObservableCollection<Customer>();
            FillCustomers();

            WindowName = windowName;
            WindowsManager.CurrentCustomersListVM = this;            
        }

        private void FillCustomers()
        {
            List<Customer> customers = DBManager.GetCustomers();

            foreach (Customer customer in customers)
            {
                Customers.Add(customer);
            }
        }

        public void UpdateCustomers()
        {
            Customers.Clear();
            FillCustomers();
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowNewCustomerForm(WindowName);

                      UpdateCustomers();
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedCustomer}"))
                      {
                          WindowsManager.DeleteCustomer(SelectedCustomer.Id);
                      }

                      UpdateCustomers();                      
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedCustomer}"))
                      {
                          WindowsManager.ShowCustomerForm(SelectedCustomer.Id, WindowName);
                      }
                  }));
            }
        }
    }

    public class CustomerFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public ObservableCollection<Purchase> Purchases { get; set; }
        public Purchase SelectedPurchase { get; set; }        

        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged($"{nameof(Id)}");
            }
        }

        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string WindowName { get; set; }

        public CustomerFormVM(Customer customer, string windowName)
        {
            Id = customer.Id.ToString();
            Surname = customer.Surname;
            Name = customer.Name;
            Patronymic = customer.Patronymic;
            PhoneNumber = customer.PhoneNumber;
            Email = customer.Email;

            WindowName = windowName;

            Purchases = new ObservableCollection<Purchase>();
            FillPurchases();

            WindowsManager.CurrentCustomerFormVMs.Add(this);
        }

        public CustomerFormVM(string windowName)
        {
            WindowName = windowName;

            Purchases = new ObservableCollection<Purchase>();

            WindowsManager.CurrentCustomerFormVMs.Add(this);
        }

        private void FillPurchases()
        {
            List<Purchase> purchases = DBManager.GetPurchasesByEmail(Email);

            foreach (Purchase purchase in purchases)
            {
                Purchases.Add(purchase);
            }
        }

        public void UpdatePurchases()
        {
            Purchases.Clear();
            FillPurchases();
        }

        private string EliminateNull(string userInput)
        {
            userInput = (userInput == null) ? String.Empty : userInput.Trim();

            return userInput;
        }

        private void PrepareFields()
        {
            Id = EliminateNull(Id);
            Surname = EliminateNull(Surname);
            Name = EliminateNull(Name);
            Patronymic = EliminateNull(Patronymic);
            PhoneNumber = EliminateNull(PhoneNumber);
            Email = EliminateNull(Email);
        }

        private bool CheckFields()
        {
            bool fieldsAreFilled = true;

            PrepareFields();

            if (Surname.Length == 0 || Name.Length == 0 ||
                        Patronymic.Length == 0 || Email.Length == 0)
            {
                fieldsAreFilled = false;
                WindowsManager.ShowErrorMessageBox("Пустым может быть только поле \"Номер телефона\"");
            }

            return fieldsAreFilled;
        }

        private Customer GetCustomerFromForm()
        {
            Customer customer = new Customer(Surname, Name, Patronymic, PhoneNumber, Email);

            return customer;
        }

        private void EditOrAddCustomer()
        {
            Customer customer = GetCustomerFromForm();

            if (!string.IsNullOrEmpty(Id))
            {
                customer.Id = Convert.ToInt32(Id);
                DBManager.EditCustomer(customer);
            }
            else
            {
                DBManager.AddCustomer(customer);
                Id = customer.Id.ToString();                
            }
        }

        private RelayCommand writeCommand;
        public RelayCommand WriteCommand
        {
            get
            {
                return writeCommand ??
                  (writeCommand = new RelayCommand(obj =>
                  {
                      bool fieldsAreFilled = CheckFields();

                      if (fieldsAreFilled)
                      {
                          EditOrAddCustomer();
                      }
                  }));
            }
        }

        private RelayCommand okCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return okCommand ??
                  (okCommand = new RelayCommand(obj =>
                  {
                      bool fieldsAreFilled = CheckFields();

                      if (fieldsAreFilled)
                      {
                          EditOrAddCustomer();

                          WindowsManager.CloseWindow(WindowName);
                      }
                  }));
            }
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowNewPurchaseForm(Email, WindowName);
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedPurchase}"))
                      {
                          WindowsManager.DeletePurchase(SelectedPurchase.Id, WindowName);
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedPurchase}"))
                      {
                          WindowsManager.ShowPurchaseForm(SelectedPurchase.Id, WindowName, false);
                      }
                  }));
            }
        }
    }

    public class PurchasesListVM
    {
        public ObservableCollection<Purchase> Purchases { get; set; }
        public Purchase SelectedPurchase { get; set; }

        public string WindowName { get; set; }

        public PurchasesListVM(string windowName)
        {
            Purchases = new ObservableCollection<Purchase>();
            FillPurchases();

            WindowName = windowName;

            WindowsManager.CurrentPurchasesListVM = this;
        }

        private void FillPurchases()
        {
            List<Purchase> purchases = DBManager.GetPurchases();

            foreach (Purchase purchase in purchases)
            {
                Purchases.Add(purchase);
            }
        }

        public void UpdatePurchases()
        {
            Purchases.Clear();
            FillPurchases();
        }        

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.ShowNewPurchaseForm(String.Empty, WindowName);
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedPurchase}"))
                      {
                          WindowsManager.DeletePurchase(SelectedPurchase.Id, WindowName);
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedPurchase}"))
                      {
                          WindowsManager.ShowPurchaseForm(SelectedPurchase.Id, WindowName, true);
                      }
                  }));
            }
        }
    }

    public class PurchaseFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged($"{nameof(Id)}");
            }
        }

        public string Email { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public string WindowName { get; set; }
        public bool EmailIsEnabled { get; set; }

        public PurchaseFormVM(Purchase purchase, string windowName, bool emailIsEnabled)
        {
            Id = purchase.Id.ToString();
            Email = purchase.Email;
            ProductCode = purchase.ProductCode;
            ProductName = purchase.ProductName;

            WindowName = windowName;
            EmailIsEnabled = emailIsEnabled;
        }

        public PurchaseFormVM(string email, string windowName)
        {
            WindowName = windowName;

            if (!string.IsNullOrEmpty($"{email}"))
            {
                Email = email;
                EmailIsEnabled = false;
            }
            else
            {
                EmailIsEnabled = true;
            }
        }

        private string EliminateNull(string userInput)
        {
            userInput = (userInput == null) ? String.Empty : userInput.Trim();

            return userInput;
        }

        private void PrepareFields()
        {
            Id = EliminateNull(Id);
            Email = EliminateNull(Email);
            ProductCode = EliminateNull(ProductCode);
            ProductName = EliminateNull(ProductName);
        }

        private bool CheckFields()
        {
            bool fieldsAreFilled = true;

            PrepareFields();

            if (Email.Length == 0 || ProductCode.Length == 0 ||
                        ProductName.Length == 0)
            {
                fieldsAreFilled = false;
                WindowsManager.ShowErrorMessageBox("Все поля должны быть заполнены");
            }

            return fieldsAreFilled;
        }

        private Purchase GetPurchaseFromForm()
        {
            Purchase purchase = new Purchase(Email, ProductCode, ProductName);

            return purchase;
        }

        private void EditOrAddPurchase()
        {
            Purchase purchase = GetPurchaseFromForm();

            if (!string.IsNullOrEmpty(Id))
            {
                purchase.Id = Convert.ToInt32(Id);
                DBManager.EditPurchase(purchase);
            }
            else
            {
                DBManager.AddPurchase(purchase);
                Id = purchase.Id.ToString();
            }
        }

        private RelayCommand writeCommand;
        public RelayCommand WriteCommand
        {
            get
            {
                return writeCommand ??
                  (writeCommand = new RelayCommand(obj =>
                  {
                      bool fieldsAreFilled = CheckFields();

                      if (fieldsAreFilled)
                      {
                          EditOrAddPurchase();
                      }
                  }));
            }
        }

        private RelayCommand okCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return okCommand ??
                  (okCommand = new RelayCommand(obj =>
                  {
                      bool fieldsAreFilled = CheckFields();

                      if (fieldsAreFilled)
                      {
                          EditOrAddPurchase();

                          WindowsManager.CloseWindow(WindowName);
                      }
                  }));
            }
        }
    }
}
