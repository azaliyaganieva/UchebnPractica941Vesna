using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UchebnPractica941Vesna.DBCon;

namespace UchebnPractica941Vesna.Pages
{
    public partial class PartnerHistoryPage : Page
    {
        private Partners currentPartner;

        public PartnerHistoryPage(Partners partner)
        {
            InitializeComponent();
            currentPartner = partner;
            LoadPartnerInfo();
            LoadSalesHistory();
        }

        private void LoadPartnerInfo()
        {
            PartnerNameTb.Text = currentPartner.NamePartner;
            DirectorTb.Text = $"{currentPartner.SurnameDirector} {currentPartner.NameDirector}";
            DiscountTb.Text = currentPartner.SizeDiscount;
        }

        private void LoadSalesHistory()
        {
            try
            {
         
                var requests = Connection.comfort.Request
                    .Where(r => r.Id_partner == currentPartner.Id_partner)
                    .ToList();

                var salesList = new List<object>();

                foreach (var request in requests)
                {
                    var details = Connection.comfort.RequestDetails
                        .Where(rd => rd.Id_request == request.Id_request)
                        .ToList();

                    foreach (var detail in details)
                    {
                        var product = Connection.comfort.Products
                            .FirstOrDefault(p => p.Id_product == detail.Id_product);

                        if (product != null)
                        {
                            salesList.Add(new
                            {
                                SaleDate = request.RequestDate,
                                ProductName = product.NameProduct,
                                Quantity = detail.Quantity,
                                Amount = product.MinPrice * detail.Quantity,
                                SalePointName = "По заявке"
                            });
                        }
                    }
                }

                if (salesList.Any())
                {
                    SalesListView.ItemsSource = salesList;
                }
                else
                {
                    SalesListView.Visibility = Visibility.Collapsed;
                    EmptyMessage.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}