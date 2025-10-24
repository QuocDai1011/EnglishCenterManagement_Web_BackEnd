using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class ResultExam
{
    public int ResultExamId { get; set; }

    public double ResultListening { get; set; }

    public double ResultReading { get; set; }

    public double ResultWriting { get; set; }

    public double ResultSpeaking { get; set; }

    public int StudentId { get; set; }

    public int ExamTypeId { get; set; }

    public virtual ExamType ExamType { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
