using System;
using System.Collections.Generic;

namespace AutoService.API.Models;

public partial class KursOtzvy
{
    public int KodOtzyva { get; set; }

    public int? KodKlienta { get; set; }

    public int? KodIspolnitelya { get; set; }

    public int? Ocenka { get; set; }

    public string? TextOtzyva { get; set; }

    public DateTime? DataOtzyva { get; set; }

    public virtual KursIspolniteli? KodIspolnitelyaNavigation { get; set; }

    public virtual KursKlienty? KodKlientaNavigation { get; set; }
}
