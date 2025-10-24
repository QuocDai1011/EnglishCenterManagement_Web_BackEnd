using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool Gender { get; set; }

    public string Address { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string PhoneNumberOfParents { get; set; } = null!;

    public DateOnly? CreatAt { get; set; }

    public DateOnly? UpdateAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ResultExam> ResultExams { get; set; } = new List<ResultExam>();

    public virtual ICollection<StudentAttendance> StudentAttendances { get; set; } = new List<StudentAttendance>();

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    public virtual ICollection<StudentExercise> StudentExercises { get; set; } = new List<StudentExercise>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
