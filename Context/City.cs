using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Desktop.Context;

public partial class City
{
    public ushort CityId { get; set; }

    public string City1 { get; set; } = null!;

    [Reference(typeof(Country), nameof(Country.CountryId))]
    public ushort CountryId { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Country Country { get; set; } = null!;
}
