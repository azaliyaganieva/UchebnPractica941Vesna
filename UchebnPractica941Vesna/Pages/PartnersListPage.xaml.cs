using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UchebnPractica941Vesna.DBCon;

namespace UchebnPractica941Vesna.Pages
{
    public partial class PartnersListPage : Page
    {
        private List<Partners> allPartners;

        public PartnersListPage()
        {
            InitializeComponent();
            LoadPartners();
        }

        private void LoadPartners()
        {
            try
            {
                allPartners = Connection.comfort.Partners.ToList();
                UpdateDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDisplay()
        {
            if (allPartners == null) return;

            var filteredPartners = allPartners.AsEnumerable();

            
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                string searchText = SearchBox.Text.ToLower();
                filteredPartners = filteredPartners.Where(p =>
                    (p.NamePartner != null && p.NamePartner.ToLower().Contains(searchText)) ||
                    (p.SurnameDirector != null && p.SurnameDirector.ToLower().Contains(searchText)) ||
                    (p.NameDirector != null && p.NameDirector.ToLower().Contains(searchText)) ||
                    (p.INN != null && p.INN.Contains(searchText)) ||
                    (p.Phone != null && p.Phone.Contains(searchText))
                );
            }

           
            if (SortCombo.SelectedIndex == 0)
                filteredPartners = filteredPartners.OrderBy(p => p.NamePartner);
            else if (SortCombo.SelectedIndex == 1)
                filteredPartners = filteredPartners.OrderByDescending(p => p.NamePartner);
            else if (SortCombo.SelectedIndex == 2)
                filteredPartners = filteredPartners.OrderBy(p => p.Raiting);
            else if (SortCombo.SelectedIndex == 3)
                filteredPartners = filteredPartners.OrderByDescending(p => p.Raiting);
            else if (SortCombo.SelectedIndex == 4)
                filteredPartners = filteredPartners.OrderBy(p => p.SizeDiscount);
            else if (SortCombo.SelectedIndex == 5)
                filteredPartners = filteredPartners.OrderByDescending(p => p.SizeDiscount);

            PatnersLW.ItemsSource = filteredPartners.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDisplay();
        }

        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDisplay();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPartners(new Partners()));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PatnersLW.SelectedItem as Partners;
            if (selPartner != null)
            {
                NavigationService.Navigate(new AddEditPartners(selPartner));
            }
            else
            {
                MessageBox.Show("Выберите партнера для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

 
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PatnersLW.SelectedItem as Partners;
            if (selPartner == null)
            {
                MessageBox.Show("Выберите партнера для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить партнера \"{selPartner.NamePartner}\"?\n\n",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Connection.comfort.Partners.Remove(selPartner);
                    Connection.comfort.SaveChanges();
                    MessageBox.Show("Партнер успешно удален!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPartners(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void HistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PatnersLW.SelectedItem as Partners;
            if (selPartner == null)
            {
                MessageBox.Show("Выберите партнера для просмотра истории продаж!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NavigationService.Navigate(new PartnerHistoryPage(selPartner));
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPartners();
            SearchBox.Text = string.Empty;
            SortCombo.SelectedIndex = -1;
        }
    }
}