using System;
using System.Collections.Generic;

namespace AutoService.API.Models;

public partial class KursKlienty
{
    public int KodKlienta { get; set; }

    public string Fio { get; set; } = null!;

    public string Telefon { get; set; } = null!;

    public virtual ICollection<KursAvtomobili> KursAvtomobilis { get; set; } = new List<KursAvtomobili>();

    public virtual ICollection<KursOtzvy> KursOtzvies { get; set; } = new List<KursOtzvy>();

    public virtual ICollection<KursZakazy> KursZakazies { get; set; } = new List<KursZakazy>();
}
