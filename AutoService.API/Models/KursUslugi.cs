using System;
using System.Collections.Generic;

namespace AutoService.API.Models;

public partial class KursUslugi
{
    public int KodUslugi { get; set; }

    public string Nazvanie { get; set; } = null!;

    public decimal Cena { get; set; }

    public virtual ICollection<KursZakazy> KursZakazies { get; set; } = new List<KursZakazy>();
}
