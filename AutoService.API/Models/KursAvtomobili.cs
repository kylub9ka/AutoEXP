using System;
using System.Collections.Generic;

namespace AutoService.API.Models;

public partial class KursAvtomobili
{
    public int KodAvto { get; set; }

    public int? KodKlienta { get; set; }

    public string Marka { get; set; } = null!;

    public string Gosnomer { get; set; } = null!;

    public virtual KursKlienty? KodKlientaNavigation { get; set; }

    public virtual ICollection<KursZakazy> KursZakazies { get; set; } = new List<KursZakazy>();
}
