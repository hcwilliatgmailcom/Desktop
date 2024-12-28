using System;
using System.Collections.Generic;

namespace Desktop.Context;

public partial class SalesByFilmCategory
{
    public string Category { get; set; } = null!;

    public decimal? TotalSales { get; set; }
}
