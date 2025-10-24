using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class Expertise
{
    public int ExpertiseId { get; set; }

    public string ExpertiseName { get; set; } = null!;

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
