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
            BusinessTypeTb.Text = currentPartner.TypeOfBusiness?.NameBusiness ?? "Не указан";
            DirectorTb.Text = $"{currentPartner.SurnameDirector} {currentPartner.NameDirector} {currentPartner.PatronumicDirector}";
            RatingTb.Text = currentPartner.Raiting?.ToString() ?? "Нет";
        }

        private void LoadSalesHistory()
        {
            try
            {
                // Получаем все точки продаж этого партнера
                var salePoints = Connection.comfort.SalePoint
                    .Where(sp => sp.Id_partner == currentPartner.Id_partner)
                    .Select(sp => sp.Id_point)
                    .ToList();

                // Получаем историю продаж по этим точкам
                var sales = Connection.comfort.SaleHistory
                    .Where(sh => salePoints.Contains(sh.Id_point ?? 0))
                    .ToList()
                    .Select(sh => new
                    {
                        sh.Id_sale,
                        sh.Quantity,
                        sh.Amount,
                        sh.Id_product,
                        sh.Id_point,
                        SalePointName = sh.SalePoint != null ? sh.SalePoint.NamePoint : "Неизвестно",
                        ProductName = sh.Products != null ? sh.Products.NameProduct : "Неизвестно",
                        SaleDate = GetSaleDateForPartner(sh)
                    })
                    .ToList();

                // Если нет продаж в SaleHistory, пробуем через заявки
                if (!sales.Any())
                {
                    // Получаем заявки партнера
                    var partnerRequests = Connection.comfort.Request
                        .Where(r => r.Id_partner == currentPartner.Id_partner)
                        .ToList();

                    var salesFromRequests = new List<object>();

                    foreach (var request in partnerRequests)
                    {
                        // Получаем детали заявки
                        var requestDetails = Connection.comfort.RequestDetails
                            .Where(rd => rd.Id_request == request.Id_request)
                            .ToList();

                        foreach (var detail in requestDetails)
                        {
                            var product = Connection.comfort.Products
                                .FirstOrDefault(p => p.Id_product == detail.Id_product);

                            if (product != null)
                            {
                                salesFromRequests.Add(new
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

                    if (salesFromRequests.Any())
                    {
                        SalesListView.ItemsSource = salesFromRequests;
                        EmptyMessage.Visibility = Visibility.Collapsed;
                        CalculateStatsFromList(salesFromRequests);
                        return;
                    }
                }

                if (sales.Any())
                {
                    SalesListView.ItemsSource = sales;
                    EmptyMessage.Visibility = Visibility.Collapsed;
                    CalculateStats(sales);
                }
                else
                {
                    SalesListView.Visibility = Visibility.Collapsed;
                    EmptyMessage.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории продаж: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DateTime? GetSaleDateForPartner(SaleHistory sale)
        {
            try
            {
                // Получаем заявки партнера
                var requests = Connection.comfort.Request
                    .Where(r => r.Id_partner == currentPartner.Id_partner)
                    .OrderByDescending(r => r.RequestDate)
                    .ToList();

                if (requests.Any())
                {
                    return requests.First().RequestDate;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private void CalculateStats(dynamic sales)
        {
            int totalCount = 0;
            decimal totalAmount = 0;

            foreach (var sale in sales)
            {
                if (sale.Quantity != null)
                    totalCount += sale.Quantity;
                if (sale.Amount != null)
                    totalAmount += sale.Amount;
            }

            TotalSalesCountTb.Text = totalCount.ToString();
            TotalAmountTb.Text = $"{totalAmount:N0} ₽";
            DiscountTb.Text = currentPartner.SizeDiscount;
        }

        private void CalculateStatsFromList(List<object> sales)
        {
            int totalCount = 0;
            decimal totalAmount = 0;

            foreach (var sale in sales)
            {
                var properties = sale.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.Name == "Quantity" && prop.GetValue(sale) != null)
                        totalCount += (int)prop.GetValue(sale);
                    if (prop.Name == "Amount" && prop.GetValue(sale) != null)
                        totalAmount += (decimal)prop.GetValue(sale);
                }
            }

            TotalSalesCountTb.Text = totalCount.ToString();
            TotalAmountTb.Text = $"{totalAmount:N0} ₽";
            DiscountTb.Text = currentPartner.SizeDiscount;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}