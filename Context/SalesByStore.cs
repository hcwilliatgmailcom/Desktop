using System;
using System.Collections.Generic;

namespace Desktop.Context;

public partial class SalesByStore
{
    public string Store { get; set; } = null!;

    public string Manager { get; set; } = null!;

    public decimal? TotalSales { get; set; }
}
