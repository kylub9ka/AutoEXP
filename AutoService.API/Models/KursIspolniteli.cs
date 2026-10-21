using System;
using System.Collections.Generic;

namespace AutoService.API.Models;

public partial class KursIspolniteli
{
    public int KodIspolnitelya { get; set; }

    public string Fio { get; set; } = null!;

    public string Specializaciya { get; set; } = null!;

    public virtual ICollection<KursOtzvy> KursOtzvies { get; set; } = new List<KursOtzvy>();

    public virtual ICollection<KursZakazy> KursZakazies { get; set; } = new List<KursZakazy>();
}
