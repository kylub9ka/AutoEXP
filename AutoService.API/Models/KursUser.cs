using System;
using System.Collections.Generic;

namespace AutoService.API.Models;

public partial class KursUser
{
    public int KodUser { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? KodKlienta { get; set; }

    public int? KodIspolnitelya { get; set; }
}
