using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class ProvinceCity
{
    public int ProvinceCityId { get; set; }

    public string ProvinceCityName { get; set; } = null!;

    public virtual ICollection<CommuneDistrict> CommuneDistricts { get; set; } = new List<CommuneDistrict>();
}
