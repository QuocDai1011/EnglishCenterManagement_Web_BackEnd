using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class EnglishCenterManagementDevContext : DbContext
{
    public EnglishCenterManagementDevContext()
    {
    }

    public EnglishCenterManagementDevContext(DbContextOptions<EnglishCenterManagementDevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<CommuneDistrict> CommuneDistricts { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<ExamType> ExamTypes { get; set; }

    public virtual DbSet<Exercise> Exercises { get; set; }

    public virtual DbSet<Expertise> Expertises { get; set; }

    public virtual DbSet<ProvinceCity> ProvinceCities { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<ResultExam> ResultExams { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentAttendance> StudentAttendances { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }

    public virtual DbSet<StudentExercise> StudentExercises { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeacherAttendance> TeacherAttendances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1444;Database=english_center_management_dev;User Id=english_center_manager;Password=Abc1234@;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("admins");

            entity.HasIndex(e => e.UserName, "IX_admins_user_name").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsStudentManager).HasColumnName("is_student_manager");
            entity.Property(e => e.IsSuper).HasColumnName("is_super");
            entity.Property(e => e.IsTeacherManager).HasColumnName("is_teacher_manager");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Salary)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("salary");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("class");

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("class_code");
            entity.Property(e => e.ClassName)
                .HasMaxLength(255)
                .HasColumnName("class_name");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.CurrentStudent).HasColumnName("current_student");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.MaxStudent).HasColumnName("max_student");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.OnlineMeetingLink)
                .HasColumnType("text")
                .HasColumnName("online_meeting_link");
            entity.Property(e => e.Shift).HasColumnName("shift");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");

            entity.HasMany(d => d.Courses).WithMany(p => p.Classes)
                .UsingEntity<Dictionary<string, object>>(
                    "ClassCourse",
                    r => r.HasOne<Course>().WithMany().HasForeignKey("CourseId"),
                    l => l.HasOne<Class>().WithMany().HasForeignKey("ClassId"),
                    j =>
                    {
                        j.HasKey("ClassId", "CourseId");
                        j.ToTable("class_course");
                        j.HasIndex(new[] { "CourseId" }, "IX_class_course_course_id");
                        j.IndexerProperty<int>("ClassId").HasColumnName("class_id");
                        j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                    });
        });

        modelBuilder.Entity<CommuneDistrict>(entity =>
        {
            entity.ToTable("commune_district");

            entity.HasIndex(e => e.ProvinceCityId, "IX_commune_district_province_city_id");

            entity.Property(e => e.CommuneDistrictId).HasColumnName("commune_district_id");
            entity.Property(e => e.CommuneDistrict1)
                .HasMaxLength(255)
                .HasColumnName("commune_district");
            entity.Property(e => e.ProvinceCityId).HasColumnName("province_city_id");

            entity.HasOne(d => d.ProvinceCity).WithMany(p => p.CommuneDistricts).HasForeignKey(d => d.ProvinceCityId);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("course");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.AvatarLink)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar_link");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(20)
                .HasColumnName("course_code");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .HasColumnName("course_name");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Level)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("level");
            entity.Property(e => e.NumberSessions).HasColumnName("number_sessions");
            entity.Property(e => e.TutitionFee)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("tutition_fee");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");
        });

        modelBuilder.Entity<ExamType>(entity =>
        {
            entity.ToTable("exam_type");

            entity.Property(e => e.ExamResultCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("exam_result_code");
            entity.Property(e => e.ExamResultName)
                .HasMaxLength(255)
                .HasColumnName("exam_result_name");
        });

        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.ToTable("exercise");

            entity.HasIndex(e => e.TeacherId, "IX_exercise_teacher_id");

            entity.Property(e => e.ExerciseId).HasColumnName("exercise_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ImageLink)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_link");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("note");
            entity.Property(e => e.Suggest)
                .HasMaxLength(255)
                .HasColumnName("suggest");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Topic)
                .HasMaxLength(255)
                .HasColumnName("topic");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Exercises).HasForeignKey(d => d.TeacherId);
        });

        modelBuilder.Entity<Expertise>(entity =>
        {
            entity.ToTable("expertise");

            entity.Property(e => e.ExpertiseName)
                .HasMaxLength(255)
                .HasColumnName("expertise_name");
        });

        modelBuilder.Entity<ProvinceCity>(entity =>
        {
            entity.ToTable("province_city");

            entity.Property(e => e.ProvinceCityId).HasColumnName("province_city_id");
            entity.Property(e => e.ProvinceCityName)
                .HasMaxLength(255)
                .HasColumnName("province_city_name");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.ToTable("receipt");

            entity.HasIndex(e => new { e.StudentId, e.CourseId }, "IX_receipt_student_id_course_id").IsUnique();

            entity.Property(e => e.ReceiptId).HasColumnName("receipt_id");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.StudentCourse).WithOne(p => p.Receipt).HasForeignKey<Receipt>(d => new { d.StudentId, d.CourseId });
        });

        modelBuilder.Entity<ResultExam>(entity =>
        {
            entity.ToTable("result_exam");

            entity.HasIndex(e => e.ExamTypeId, "IX_result_exam_ExamTypeId");

            entity.HasIndex(e => e.StudentId, "IX_result_exam_student_id");

            entity.Property(e => e.ResultExamId).HasColumnName("result_exam_id");
            entity.Property(e => e.ResultListening).HasColumnName("result_listening");
            entity.Property(e => e.ResultReading).HasColumnName("result_reading");
            entity.Property(e => e.ResultSpeaking).HasColumnName("result_speaking");
            entity.Property(e => e.ResultWriting).HasColumnName("result_writing");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.ExamType).WithMany(p => p.ResultExams)
                .HasForeignKey(d => d.ExamTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Student).WithMany(p => p.ResultExams).HasForeignKey(d => d.StudentId);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");

            entity.HasIndex(e => e.UserName, "IX_students_user_name").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreatAt).HasColumnName("creat_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.PhoneNumberOfParents)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number_of_parents");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_name");

            entity.HasMany(d => d.Classes).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentClass",
                    r => r.HasOne<Class>().WithMany().HasForeignKey("ClassId"),
                    l => l.HasOne<Student>().WithMany().HasForeignKey("StudentId"),
                    j =>
                    {
                        j.HasKey("StudentId", "ClassId");
                        j.ToTable("student_class");
                        j.HasIndex(new[] { "ClassId" }, "IX_student_class_class_id");
                        j.IndexerProperty<int>("StudentId").HasColumnName("student_id");
                        j.IndexerProperty<int>("ClassId").HasColumnName("class_id");
                    });
        });

        modelBuilder.Entity<StudentAttendance>(entity =>
        {
            entity.ToTable("student_attendance");

            entity.HasIndex(e => e.ClassId, "IX_student_attendance_class_id");

            entity.HasIndex(e => e.StudentId, "IX_student_attendance_student_id");

            entity.Property(e => e.StudentAttendanceId).HasColumnName("student_attendance_id");
            entity.Property(e => e.AttendanceDate).HasColumnName("attendance_date");
            entity.Property(e => e.CheckInTime).HasColumnName("check_in_time");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");

            entity.HasOne(d => d.Class).WithMany(p => p.StudentAttendances).HasForeignKey(d => d.ClassId);

            entity.HasOne(d => d.Student).WithMany(p => p.StudentAttendances).HasForeignKey(d => d.StudentId);
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.CourseId });

            entity.ToTable("student_course");

            entity.HasIndex(e => e.CourseId, "IX_student_course_course_id");

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("discount_value");

            entity.HasOne(d => d.Course).WithMany(p => p.StudentCourses).HasForeignKey(d => d.CourseId);

            entity.HasOne(d => d.Student).WithMany(p => p.StudentCourses).HasForeignKey(d => d.StudentId);
        });

        modelBuilder.Entity<StudentExercise>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ExerciseId });

            entity.ToTable("student_exercise");

            entity.HasIndex(e => e.ExerciseId, "IX_student_exercise_ExerciseId");

            entity.Property(e => e.AnswerLink)
                .HasColumnType("text")
                .HasColumnName("answer_link");
            entity.Property(e => e.CommentOfTeacher)
                .HasColumnType("text")
                .HasColumnName("comment_of_teacher");
            entity.Property(e => e.CommentPrivateOfStudent)
                .HasColumnType("text")
                .HasColumnName("comment_private_of_student");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.Score).HasColumnName("score");

            entity.HasOne(d => d.Exercise).WithMany(p => p.StudentExercises).HasForeignKey(d => d.ExerciseId);

            entity.HasOne(d => d.Student).WithMany(p => p.StudentExercises).HasForeignKey(d => d.StudentId);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.AdminId);

            entity.ToTable("teachers");

            entity.HasIndex(e => e.UserName, "IX_teachers_user_name").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Salary)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("salary");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_name");

            entity.HasMany(d => d.Classes).WithMany(p => p.Teachers)
                .UsingEntity<Dictionary<string, object>>(
                    "TeacherClass",
                    r => r.HasOne<Class>().WithMany().HasForeignKey("ClassId"),
                    l => l.HasOne<Teacher>().WithMany().HasForeignKey("TeacherId"),
                    j =>
                    {
                        j.HasKey("TeacherId", "ClassId");
                        j.ToTable("teacher_class");
                        j.HasIndex(new[] { "ClassId" }, "IX_teacher_class_class_id");
                        j.IndexerProperty<int>("TeacherId").HasColumnName("teacher_id");
                        j.IndexerProperty<int>("ClassId").HasColumnName("class_id");
                    });

            entity.HasMany(d => d.Courses).WithMany(p => p.Teachers)
                .UsingEntity<Dictionary<string, object>>(
                    "TeacherCourse",
                    r => r.HasOne<Course>().WithMany().HasForeignKey("CourseId"),
                    l => l.HasOne<Teacher>().WithMany().HasForeignKey("TeacherId"),
                    j =>
                    {
                        j.HasKey("TeacherId", "CourseId");
                        j.ToTable("teacher_course");
                        j.HasIndex(new[] { "CourseId" }, "IX_teacher_course_course_id");
                        j.IndexerProperty<int>("TeacherId").HasColumnName("teacher_id");
                        j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                    });

            entity.HasMany(d => d.Expertises).WithMany(p => p.Teachers)
                .UsingEntity<Dictionary<string, object>>(
                    "ExpertiseTeacher",
                    r => r.HasOne<Expertise>().WithMany().HasForeignKey("ExpertiseId"),
                    l => l.HasOne<Teacher>().WithMany().HasForeignKey("TeacherId"),
                    j =>
                    {
                        j.HasKey("TeacherId", "ExpertiseId");
                        j.ToTable("expertise_teacher");
                        j.HasIndex(new[] { "ExpertiseId" }, "IX_expertise_teacher_expertise_id");
                        j.IndexerProperty<int>("TeacherId").HasColumnName("teacher_id");
                        j.IndexerProperty<int>("ExpertiseId").HasColumnName("expertise_id");
                    });
        });

        modelBuilder.Entity<TeacherAttendance>(entity =>
        {
            entity.ToTable("teacher_attendance");

            entity.HasIndex(e => e.ClassId, "IX_teacher_attendance_class_id");

            entity.HasIndex(e => e.TeacherId, "IX_teacher_attendance_teacher_id");

            entity.Property(e => e.TeacherAttendanceId).HasColumnName("teacher_attendance_id");
            entity.Property(e => e.AttendanceDate).HasColumnName("attendance_date");
            entity.Property(e => e.CheckInTime).HasColumnName("check_in_time");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.Note)
                .HasColumnType("text")
                .HasColumnName("note");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");

            entity.HasOne(d => d.Class).WithMany(p => p.TeacherAttendances).HasForeignKey(d => d.ClassId);

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeacherAttendances).HasForeignKey(d => d.TeacherId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
