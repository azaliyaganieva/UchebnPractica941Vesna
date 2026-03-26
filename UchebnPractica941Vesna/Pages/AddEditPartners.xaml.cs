using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UchebnPractica941Vesna.DBCon;

namespace UchebnPractica941Vesna.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditPartners.xaml
    /// </summary>
    public partial class AddEditPartners : Page
    {
        Partners partner;
        public AddEditPartners(Partners _partner)
        {
            InitializeComponent();
            partner= _partner;
            this.DataContext = partner;
            TypeCmb.ItemsSource=Connection.comfort.TypeOfBusiness.ToList();
            TypeCmb.DisplayMemberPath = "NameBusiness";
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                partner.Id_type = (TypeCmb.SelectedItem as TypeOfBusiness).Id_type;
                if (partner.Id_partner == 0)
                {
                    Connection.comfort.Partners.Add(partner);
                }
                Connection.comfort.SaveChanges();
                MessageBox.Show("Успешно");
                NavigationService.Navigate(new PartnersListPage());
            }
            catch(Exception ex) {MessageBox.Show(ex.Message); } 
        }

        private void OtmenaBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
