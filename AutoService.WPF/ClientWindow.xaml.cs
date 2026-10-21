using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using Newtonsoft.Json;

namespace AutoService.WPF
{
    public partial class ClientWindow : Window
    {
        public ObservableCollection<OrderDto> MyOrders { get; set; } = new ObservableCollection<OrderDto>();
        private int _clientId;
        private readonly string _apiUrl = "https://localhost:7223/api";
        private int _activeDiscount = 0;

        public ClientWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
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

                var resOrders = await client.GetStringAsync($"{_apiUrl}/Orders");
                var allOrders = JsonConvert.DeserializeObject<List<OrderDto>>(resOrders);

                MyOrders.Clear();
                if (allOrders != null)
                {
                    var my = allOrders.Where(o => o.kod_klienta == _clientId).ToList();
                    foreach (var o in my) MyOrders.Add(o);
                }
                MyOrdersGrid.ItemsSource = MyOrders;

                var resServices = await client.GetStringAsync($"{_apiUrl}/Dictionary/services");
                LbService.ItemsSource = JsonConvert.DeserializeObject<List<ComboItem>>(resServices);

                var resMasters = await client.GetStringAsync($"{_apiUrl}/Masters");
                var mastersList = JsonConvert.DeserializeObject<List<dynamic>>(resMasters);
                MastersGrid.ItemsSource = mastersList;

                var masterItems = new List<ComboItem>();
                foreach (var m in mastersList) { masterItems.Add(new ComboItem { Id = (int)m.kod, Name = (string)m.fio }); }
                CbReviewMaster.ItemsSource = masterItems;

                var resReviews = await client.GetStringAsync($"{_apiUrl}/Content/reviews");
                ReviewsList.ItemsSource = JsonConvert.DeserializeObject<List<dynamic>>(resReviews);

                LoadPromotions(client);
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private async void LoadPromotions(HttpClient client)
        {
            var resPromos = await client.GetStringAsync($"{_apiUrl}/Content/promotions");
            var promos = JsonConvert.DeserializeObject<List<dynamic>>(resPromos);

            PromotionsPanel.Children.Clear();
            foreach (var p in promos)
            {
                Border card = new Border
                {
                    Background = (Brush)FindResource("CardBackground"),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(20),
                    Margin = new Thickness(0, 0, 0, 15)
                };

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel sp = new StackPanel();
                sp.Children.Add(new TextBlock { Text = (string)p.title, FontSize = 20, FontWeight = FontWeights.Bold, Foreground = (Brush)FindResource("NeonCyan") });
                sp.Children.Add(new TextBlock { Text = (string)p.desc, FontSize = 14, Foreground = (Brush)FindResource("TextMain"), Margin = new Thickness(0, 5, 0, 5) });
                if ((int)p.discount > 0)
                    sp.Children.Add(new TextBlock { Text = $"Скидка: {(int)p.discount}%", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = (Brush)FindResource("NeonPink") });

                Button btnApply = new Button { Content = "Применить", Width = 120, Height = 40, Style = (Style)FindResource("NeonButton"), VerticalAlignment = VerticalAlignment.Center };
                int discountVal = (int)p.discount;
                btnApply.Click += (s, e) => { _activeDiscount = discountVal; MessageBox.Show($"Акция применена! Скидка {_activeDiscount}% будет учтена при записи."); };

                Grid.SetColumn(sp, 0);
                Grid.SetColumn(btnApply, 1);
                grid.Children.Add(sp);
                grid.Children.Add(btnApply);
                card.Child = grid;
                PromotionsPanel.Children.Add(card);
            }
        }

        private async void BtnBook_Click(object sender, RoutedEventArgs e)
        {
            if (LbService.SelectedItems.Count == 0) { MessageBox.Show("Выберите хотя бы одну услугу!"); return; }
            if (string.IsNullOrWhiteSpace(TbMarka.Text) || string.IsNullOrWhiteSpace(TbGosnomer.Text)) { MessageBox.Show("Введите марку и госномер авто!"); return; }

            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (s, cert, chain, sslPolicyErrors) => true;
                var client = new HttpClient(handler);

                var selectedIds = LbService.SelectedItems.Cast<ComboItem>().Select(x => x.Id).ToList();

                var newOrder = new
                {
                    KlientId = _clientId,
                    Marka = TbMarka.Text,
                    Gosnomer = TbGosnomer.Text,
                    UslugaIds = selectedIds,
                    Cost = 2500,
                    Discount = _activeDiscount
                };

                var json = JsonConvert.SerializeObject(newOrder);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_apiUrl}/Orders/create-with-car", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Успешно записаны на все выбранные услуги в одном заказе!");
                    _activeDiscount = 0;
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Ошибка сервера: " + await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void BtnSendReview_Click(object sender, RoutedEventArgs e)
        {
            if (CbReviewMaster.SelectedValue == null || string.IsNullOrWhiteSpace(TbReviewText.Text)) { MessageBox.Show("Выберите мастера и напишите текст!"); return; }
            if (!int.TryParse(TbOcenka.Text, out int ocenka) || ocenka < 1 || ocenka > 5) { MessageBox.Show("Оценка от 1 до 5!"); return; }

            try
            {
                var client = new HttpClient();
                var review = new { KlientId = _clientId, MasterId = (int)CbReviewMaster.SelectedValue, Ocenka = ocenka, Text = TbReviewText.Text };
                var json = JsonConvert.SerializeObject(review);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_apiUrl}/Content/reviews", content);
                if (response.IsSuccessStatusCode) { MessageBox.Show("Спасибо за отзыв!"); LoadData(); }
                else { MessageBox.Show("Ошибка отправки."); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e) { new LoginWindow().Show(); this.Close(); }
    }
}