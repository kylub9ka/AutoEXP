using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace AutoService.WPF
{
    public partial class AddOrderWindow : Window
    {
        public AddOrderWindow()
        {
            InitializeComponent();
            Loaded += AddOrderWindow_Loaded;
        }

        private async void AddOrderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (s, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(handler);

                var resClients = await client.GetStringAsync("https://localhost:7223/api/Dictionary/clients");
                CbKlient.ItemsSource = JsonConvert.DeserializeObject<List<ComboItem>>(resClients);

                var resServices = await client.GetStringAsync("https://localhost:7223/api/Dictionary/services");
                LbUslugi.ItemsSource = JsonConvert.DeserializeObject<List<ComboItem>>(resServices);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки справочников: " + ex.Message);
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CbKlient.SelectedValue == null || LbUslugi.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Выберите клиента и хотя бы одну услугу!");
                    return;
                }

                if (!decimal.TryParse(TbCost.Text, out decimal cost))
                {
                    MessageBox.Show("Введите корректную стоимость!");
                    return;
                }

                var selectedIds = LbUslugi.SelectedItems.Cast<ComboItem>().Select(x => x.Id).ToList();

                var newOrder = new
                {
                    KlientId = (int)CbKlient.SelectedValue,
                    Marka = TbMarka.Text,
                    Gosnomer = TbGosnomer.Text,
                    UslugaIds = selectedIds,
                    Cost = cost,
                    Discount = 0
                };

                var json = JsonConvert.SerializeObject(newOrder);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (s, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(handler);

                var response = await client.PostAsync("https://localhost:7223/api/Orders/create-with-car", content);

                if (response.IsSuccessStatusCode)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка сервера: " + await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сети: " + ex.Message);
            }
        }
    }
}