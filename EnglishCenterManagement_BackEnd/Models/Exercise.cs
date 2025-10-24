using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class Exercise
{
    public int ExerciseId { get; set; }

    public int TeacherId { get; set; }

    public string Topic { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Suggest { get; set; } = null!;

    public string ImageLink { get; set; } = null!;

    public string Note { get; set; } = null!;

    public virtual ICollection<StudentExercise> StudentExercises { get; set; } = new List<StudentExercise>();

    public virtual Teacher Teacher { get; set; } = null!;
}
