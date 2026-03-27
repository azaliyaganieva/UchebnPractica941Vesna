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
            LoadRoles();
        }

        private void LoadRoles()
        {
            try
            {
                var roles = Connection.comfort.Role.Where(r => r.NameRole != "Admin").ToList();
                RoleCmb.ItemsSource = roles;
                RoleCmb.DisplayMemberPath = "NameRole";
                RoleCmb.SelectedValuePath = "Id_role";

                if (roles.Any())
                    RoleCmb.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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
                    ErrorText.Text = "Пароль должен быть не менее 3 символов!";
                    return;
                }

               
                var existingLogin = Connection.comfort.Logins
                    .FirstOrDefault(x => x.Login == LoginBox.Text.Trim());

                if (existingLogin != null)
                {
                    ErrorText.Text = "Логин уже существует!";
                    return;
                }

               
                var newEmployee = new Employee
                {
                    Surname = SurnameBox.Text.Trim(),
                    Name = NameBox.Text.Trim(),
                    Patronumic = PatronumicBox.Text.Trim(),
                    Birthday = BirthdayPicker.SelectedDate,
                    Id_role = (int)RoleCmb.SelectedValue
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

                MessageBox.Show("Регистрация успешна!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new LoginPage());
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}