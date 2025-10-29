using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseCode { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public decimal TutitionFee { get; set; }

    public int NumberSessions { get; set; }

    public string Description { get; set; } = null!;

    public string Level { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string AvatarLink { get; set; } = null!;

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    [JsonIgnore]
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
