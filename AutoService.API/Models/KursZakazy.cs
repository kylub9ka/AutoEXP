using System;
using System.Collections.Generic;

namespace AutoService.API.Models;

public partial class KursZakazy
{
    public int KodZakaza { get; set; }

    public int? KodKlienta { get; set; }

    public int? KodAvto { get; set; }

    public int? KodUslugi { get; set; }

    public int? KodIspolnitelya { get; set; }

    public DateTime? DataZakaza { get; set; }

    public decimal ItogovayaStoimost { get; set; }

    public string? Status { get; set; }

    public virtual KursAvtomobili? KodAvtoNavigation { get; set; }

    public virtual KursIspolniteli? KodIspolnitelyaNavigation { get; set; }

    public virtual KursKlienty? KodKlientaNavigation { get; set; }

    public virtual KursUslugi? KodUslugiNavigation { get; set; }
    public string SpisokUslug { get; set; }
}
