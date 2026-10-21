using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json;

namespace AutoService.WPF
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbFio.Text) || string.IsNullOrWhiteSpace(TbPhone.Text) ||
                string.IsNullOrWhiteSpace(TbLogin.Text) || string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (!Regex.IsMatch(TbFio.Text, @"^[А-Яа-яЁёA-Za-z\s\-]+$"))
            {
                MessageBox.Show("ФИО должно содержать только буквы (без цифр и спецсимволов)!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TbPhone.Text.StartsWith("+7"))
            {
                MessageBox.Show("Номер телефона должен начинаться с +7!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var regData = new
                {
                    Login = TbLogin.Text,
                    Password = PbPassword.Password,
                    Fio = TbFio.Text,
                    Phone = TbPhone.Text
                };

                var json = JsonConvert.SerializeObject(regData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (s, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(handler);

                var response = await client.PostAsync("https://localhost:7223/api/Auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    var errJson = await response.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic errObj = JsonConvert.DeserializeObject(errJson);
                        MessageBox.Show(errObj.message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка регистрации. Проверьте данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сети: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}