using EnglishCenterManagement_BackEnd.Models;

namespace EnglishCenterManagement_BackEnd.Service
{
    public class CourseService
    {
        public static Course Mapper(Course course, Course updatedCourse)
        {
            course.CourseCode = updatedCourse.CourseCode;
            course.CourseName = updatedCourse.CourseName;
            course.TutitionFee = updatedCourse.TutitionFee;
            course.NumberSessions = updatedCourse.NumberSessions;
            course.Description = updatedCourse.Description;
            course.Level = updatedCourse.Level;
            course.CreateAt = course.CreateAt;
            course.UpdateAt = DateTime.Now;
            course.AvatarLink = course.AvatarLink;

            return course;
        }
    }
}
