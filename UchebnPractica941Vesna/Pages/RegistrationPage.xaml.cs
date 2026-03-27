using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UchebnPractica941Vesna.DBCon;

namespace UchebnPractica941Vesna.Pages
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            try
            {
            
                FamilyStatusCmb.ItemsSource = Connection.comfort.FamilyStatus.ToList();
                FamilyStatusCmb.DisplayMemberPath = "NmaeStatus";
                FamilyStatusCmb.SelectedValuePath = "Id_status";

           
                HealthCmb.ItemsSource = Connection.comfort.Health.ToList();
                HealthCmb.DisplayMemberPath = "NameHealth";
                HealthCmb.SelectedValuePath = "Id_health";

             
                var roles = Connection.comfort.Role.Where(r => r.NameRole != "Admin").ToList();
                RoleCmb.ItemsSource = roles;
                RoleCmb.DisplayMemberPath = "NameRole";
                RoleCmb.SelectedValuePath = "Id_role";

              
                if (roles.Any())
                    RoleCmb.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                if (string.IsNullOrWhiteSpace(SurnameBox.Text) ||
                    string.IsNullOrWhiteSpace(NameBox.Text) ||
                    string.IsNullOrWhiteSpace(LoginBox.Text) ||
                    string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    ErrorText.Text = "Заполните все обязательные поля!";
                    return;
                }

          
                if (PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    ErrorText.Text = "Пароли не совпадают!";
                    return;
                }

                if (PasswordBox.Password.Length < 3)
                {
                    ErrorText.Text = "Пароль должен содержать минимум 3 символа!";
                    return;
                }

              
                var existingLogin = Connection.comfort.Logins
                    .FirstOrDefault(x => x.Login == LoginBox.Text.Trim());

                if (existingLogin != null)
                {
                    ErrorText.Text = "Пользователь с таким логином уже существует!";
                    return;
                }

                var newEmployee = new Employee
                {
                    Surname = SurnameBox.Text.Trim(),
                    Name = NameBox.Text.Trim(),
                    Patronumic = string.IsNullOrWhiteSpace(PatronumicBox.Text) ? null : PatronumicBox.Text.Trim(),
                    Birthday = BirthdayPicker.SelectedDate,
                    PassportSeria = string.IsNullOrWhiteSpace(PassportSeriaBox.Text) ? null : PassportSeriaBox.Text.Trim(),
                    PassportNumber = string.IsNullOrWhiteSpace(PassportNumberBox.Text) ? null : PassportNumberBox.Text.Trim(),
                    BankDetails = string.IsNullOrWhiteSpace(BankDetailsBox.Text) ? null : BankDetailsBox.Text.Trim(),
                    Id_family = FamilyStatusCmb.SelectedItem != null ? (int?)FamilyStatusCmb.SelectedValue : null,
                    Id_health = HealthCmb.SelectedItem != null ? (int?)HealthCmb.SelectedValue : null,
                    Id_role = RoleCmb.SelectedItem != null ? (int?)RoleCmb.SelectedValue : null
                };

             
                Connection.comfort.Employee.Add(newEmployee);
                Connection.comfort.SaveChanges();

               
                var newLogin = new Logins
                {
                    Login = LoginBox.Text.Trim(),
                    Password = PasswordBox.Password.Trim(),
                    Id_user = newEmployee.Id_employee
                };

                Connection.comfort.Logins.Add(newLogin);
                Connection.comfort.SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new LoginPage());
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка регистрации: {ex.Message}";
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
         
            NavigationService.Navigate(new LoginPage());
        }
    }
}