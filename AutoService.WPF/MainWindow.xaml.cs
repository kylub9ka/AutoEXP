using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace AutoService.WPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<OrderDto> Orders { get; set; } = new ObservableCollection<OrderDto>();
        public ObservableCollection<ClientDto> Clients { get; set; } = new ObservableCollection<ClientDto>();

        private int _totalOrders;
        public int TotalOrders { get { return _totalOrders; } set { _totalOrders = value; OnPropertyChanged(); } }

        private decimal _totalRevenue;
        public decimal TotalRevenue { get { return _totalRevenue; } set { _totalRevenue = value; OnPropertyChanged(); } }

        private int _completedOrders;
        public int CompletedOrders { get { return _completedOrders; } set { _completedOrders = value; OnPropertyChanged(); } }

        private readonly string _apiUrl = "https://localhost:7223/api";
        private bool _isClientsLoaded = false;
        private bool _isMastersLoaded = false;
        private bool _isPromosLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
        }

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridDashboard == null) return;

            GridDashboard.Visibility = Visibility.Collapsed;
            GridOrders.Visibility = Visibility.Collapsed;
            GridClients.Visibility = Visibility.Collapsed;
            GridMasters.Visibility = Visibility.Collapsed;
            GridPromotions.Visibility = Visibility.Collapsed;
            GridSettings.Visibility = Visibility.Collapsed;

            int index = MenuList.SelectedIndex;
            if (index == 0) GridDashboard.Visibility = Visibility.Visible;
            else if (index == 1) GridOrders.Visibility = Visibility.Visible;
            else if (index == 2)
            {
                GridClients.Visibility = Visibility.Visible;
                if (!_isClientsLoaded) { LoadClients(); _isClientsLoaded = true; }
            }
            else if (index == 3)
            {
                GridMasters.Visibility = Visibility.Visible;
                if (!_isMastersLoaded) { LoadMastersAdmin(); _isMastersLoaded = true; }
            }
            else if (index == 4)
            {
                GridPromotions.Visibility = Visibility.Visible;
                if (!_isPromosLoaded) { LoadPromosAdmin(); _isPromosLoaded = true; }
            }
            else if (index == 5) GridSettings.Visibility = Visibility.Visible;
        }

        private async void LoadData()
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(handler);

                var response = await client.GetStringAsync($"{_apiUrl}/Orders");
                var data = JsonConvert.DeserializeObject<List<OrderDto>>(response);

                Orders.Clear();
                if (data != null)
                {
                    foreach (var item in data) Orders.Add(item);
                    TotalOrders = data.Count;
                    TotalRevenue = data.Sum(x => x.cena);
                    CompletedOrders = data.Count(x => x.status == "Выполнен");
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки заказов: " + ex.Message); }
        }

        private async void LoadClients()
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync($"{_apiUrl}/Dictionary/clients-full");
                var data = JsonConvert.DeserializeObject<List<ClientDto>>(response);
                Clients.Clear();
                if (data != null) foreach (var item in data) Clients.Add(item);
                ClientsGrid.ItemsSource = Clients;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки клиентов: " + ex.Message); }
        }

        private async void LoadMastersAdmin()
        {
            try
            {
                var client = new HttpClient();
                var res = await client.GetStringAsync($"{_apiUrl}/Masters");
                AdminMastersGrid.ItemsSource = JsonConvert.DeserializeObject<List<dynamic>>(res);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void LoadPromosAdmin()
        {
            try
            {
                var client = new HttpClient();
                var res = await client.GetStringAsync($"{_apiUrl}/Content/promotions");
                AdminPromosGrid.ItemsSource = JsonConvert.DeserializeObject<List<dynamic>>(res);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e) { LoadData(); }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddOrderWindow();
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true) LoadData();
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbSearch.Text)) { System.Windows.Data.CollectionViewSource.GetDefaultView(Orders).Filter = null; }
            else
            {
                System.Windows.Data.CollectionViewSource.GetDefaultView(Orders).Filter = item =>
                {
                    var order = item as OrderDto;
                    return order != null && order.klient.ToLower().Contains(TbSearch.Text.ToLower());
                };
            }
        }

        private async void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var order = btn.DataContext as OrderDto;
            if (order == null) return;
            var client = new HttpClient();
            await client.PutAsync($"{_apiUrl}/Orders/{order.kod_zakaza}/complete", null);
            LoadData();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var order = btn.DataContext as OrderDto;
            if (order == null) return;
            if (MessageBox.Show("Удалить этот заказ?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var client = new HttpClient();
                await client.DeleteAsync($"{_apiUrl}/Orders/{order.kod_zakaza}");
                LoadData();
            }
        }

        private async void BtnAddMaster_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbMasterFio.Text)) { MessageBox.Show("Введите ФИО!"); return; }
            var json = JsonConvert.SerializeObject(new { Fio = TbMasterFio.Text, Spec = TbMasterSpec.Text });
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var client = new HttpClient();
            await client.PostAsync($"{_apiUrl}/Masters", content);
            TbMasterFio.Text = ""; TbMasterSpec.Text = "";
            LoadMastersAdmin();
        }

        private async void BtnDeleteMaster_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            dynamic master = btn.DataContext;
            if (master == null) return;
            int id = master.kod;
            var client = new HttpClient();
            await client.DeleteAsync($"{_apiUrl}/Masters/{id}");
            LoadMastersAdmin();
        }

        private async void BtnAddPromo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbPromoTitle.Text)) return;
            int disc = int.TryParse(TbPromoDiscount.Text, out int d) ? d : 0;

            var json = JsonConvert.SerializeObject(new { Title = TbPromoTitle.Text, Desc = TbPromoDesc.Text, Discount = disc });
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var client = new HttpClient();
            await client.PostAsync($"{_apiUrl}/AdminContent/promotions", content);
            LoadPromosAdmin();
        }

        private async void BtnDeletePromo_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            dynamic promo = btn.DataContext;
            if (promo == null) return;
            int id = (int)promo.id;
            var client = new HttpClient();
            await client.DeleteAsync($"{_apiUrl}/AdminContent/promotions/{id}");
            LoadPromosAdmin();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}