using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class StudentExercise
{
    public int StudentId { get; set; }

    public int ExerciseId { get; set; }

    public string? AnswerLink { get; set; }

    public string? Description { get; set; }

    public double? Score { get; set; }

    public string? CommentOfTeacher { get; set; }

    public string? CommentPrivateOfStudent { get; set; }

    public string? Note { get; set; }

    public virtual Exercise Exercise { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
