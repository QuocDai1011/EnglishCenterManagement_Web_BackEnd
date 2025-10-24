using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassCode { get; set; } = null!;

    public string ClassName { get; set; } = null!;

    public int MaxStudent { get; set; }

    public int CurrentStudent { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int Shift { get; set; }

    public bool Status { get; set; }

    public string? Note { get; set; }

    public DateOnly CreateAt { get; set; }

    public DateOnly UpdateAt { get; set; }

    public string? OnlineMeetingLink { get; set; }

    public virtual ICollection<StudentAttendance> StudentAttendances { get; set; } = new List<StudentAttendance>();

    public virtual ICollection<TeacherAttendance> TeacherAttendances { get; set; } = new List<TeacherAttendance>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
