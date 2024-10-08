using System.Windows;

namespace EntityFramework
{
    /// <summary>
    /// Логика взаимодействия для MenuForm.xaml
    /// </summary>
    public partial class MenuForm : Window
    {
        public MenuForm()
        {
            InitializeComponent();
            DataContext = new MenuFormVM();
        }
    }
}
