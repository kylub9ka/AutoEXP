using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Linq;
using Newtonsoft.Json;

namespace AutoService.WPF
{
    public partial class MasterWindow : Window
    {
        public ObservableCollection<OrderDto> FreeOrders { get; set; } = new ObservableCollection<OrderDto>();
        public ObservableCollection<OrderDto> MyTasks { get; set; } = new ObservableCollection<OrderDto>();

        private int _masterId;
        private readonly string _apiUrl = "https://localhost:7223/api";

        public MasterWindow(int masterId)
        {
            InitializeComponent();
            _masterId = masterId;
            DataContext = this;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (s, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(handler);

                var res = await client.GetStringAsync($"{_apiUrl}/Orders");
                var allOrders = JsonConvert.DeserializeObject<List<OrderDto>>(res);

                FreeOrders.Clear();
                MyTasks.Clear();

                if (allOrders != null)
                {
                    var free = allOrders.Where(o => o.status == "Ожидает").ToList();
                    foreach (var o in free) FreeOrders.Add(o);

                    var my = allOrders.Where(o => o.status == "В работе" && o.masterId == _masterId).ToList();
                    foreach (var o in my) MyTasks.Add(o);
                }

                FreeOrdersGrid.ItemsSource = FreeOrders;
                MyTasksGrid.ItemsSource = MyTasks;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void BtnTakeOrder_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            var order = btn.DataContext as OrderDto;
            if (order == null) return;

            var client = new HttpClient();
            var response = await client.PutAsync($"{_apiUrl}/Orders/{order.kod_zakaza}/take?masterId={_masterId}", null);

            if (response.IsSuccessStatusCode) LoadData();
            else MessageBox.Show("Не удалось взять заказ. Возможно, его уже взял кто-то другой.");
        }

        private async void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            var order = btn.DataContext as OrderDto;
            if (order == null) return;

            var client = new HttpClient();
            await client.PutAsync($"{_apiUrl}/Orders/{order.kod_zakaza}/complete", null);
            LoadData();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}