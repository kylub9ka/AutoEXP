using System;

namespace AutoService.WPF
{
    public class OrderDto
    {
        public int kod_zakaza { get; set; }
        public int kod_klienta { get; set; }
        public string klient { get; set; }
        public string telefon { get; set; }
        public string avto { get; set; }
        public string usluga { get; set; }
        public string master { get; set; }
        public int? masterId { get; set; }
        public decimal cena { get; set; }
        public string status { get; set; }
        public DateTime data { get; set; }
    }

    public class ClientDto
    {
        public int kod { get; set; }
        public string fio { get; set; }
        public string telefon { get; set; }
    }

    public class ComboItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}