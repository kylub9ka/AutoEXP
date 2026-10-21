using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace AutoService.WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginData = new { Login = TbLogin.Text, Password = PbPassword.Password };
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (s, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(handler);

                var response = await client.PostAsync("https://localhost:7223/api/Auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var resBody = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<LoginResponse>(resBody);

                    if (user.Role == "Admin")
                    {
                        new MainWindow().Show();
                    }
                    else if (user.Role == "Master")
                    {
                        new MasterWindow(user.MasterId ?? 1).Show();
                    }
                    else if (user.Role == "Client")
                    {
                        new ClientWindow(user.ClientId ?? 1).Show();
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения: " + ex.Message);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegisterWindow();
            regWindow.Owner = this;
            regWindow.ShowDialog();
        }
    }

    public class LoginResponse
    {
        public string Role { get; set; }
        public string Login { get; set; }
        public int? ClientId { get; set; }
        public int? MasterId { get; set; }
    }
}