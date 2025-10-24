using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool Gender { get; set; }

    public string Address { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public decimal Salary { get; set; }

    public DateOnly? CreateAt { get; set; }

    public DateOnly? UpdateAt { get; set; }

    public bool IsActive { get; set; }

    public bool IsSuper { get; set; }

    public bool IsStudentManager { get; set; }

    public bool IsTeacherManager { get; set; }
}
