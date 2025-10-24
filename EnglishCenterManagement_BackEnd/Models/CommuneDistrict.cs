using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class CommuneDistrict
{
    public int CommuneDistrictId { get; set; }

    public string CommuneDistrict1 { get; set; } = null!;

    public int ProvinceCityId { get; set; }

    public virtual ProvinceCity ProvinceCity { get; set; } = null!;
}
