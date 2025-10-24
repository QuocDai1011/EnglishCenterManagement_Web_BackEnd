using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class ExamType
{
    public int ExamTypeId { get; set; }

    public string ExamResultName { get; set; } = null!;

    public string ExamResultCode { get; set; } = null!;

    public virtual ICollection<ResultExam> ResultExams { get; set; } = new List<ResultExam>();
}
