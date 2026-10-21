using System;
using System.Collections.Generic;

namespace AutoService.API.Models;

public partial class KursAkcii
{
    public int KodAkcii { get; set; }

    public string Zagolovok { get; set; } = null!;

    public string Opisanie { get; set; } = null!;

    public int Skidka { get; set; }
}
