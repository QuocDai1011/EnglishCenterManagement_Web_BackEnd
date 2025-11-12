using EnglishCenterManagement_BackEnd.Models;

namespace EnglishCenterManagement_BackEnd.Service
{
    public class ClassService
    {
        public static Class Mapper(Class _class, Class updatedClass) { 
            _class.ClassCode = updatedClass.ClassCode;
            _class.ClassName = updatedClass.ClassName;
            _class.MaxStudent = updatedClass.MaxStudent;
            _class.CurrentStudent = updatedClass.CurrentStudent;
            _class.StartDate = updatedClass.StartDate;
            _class.EndDate = updatedClass.EndDate;
            _class.Shift = updatedClass.Shift;
            _class.Status = updatedClass.Status;
            _class.Note = updatedClass.Note;
            _class.CreateAt = updatedClass.CreateAt;
            _class.UpdateAt = DateOnly.FromDateTime(DateTime.Now);
            _class.OnlineMeetingLink = updatedClass.OnlineMeetingLink;

            return _class;
        }

        public static List<Class> AutoSetStatus(List<Class> classes) { 
            foreach(var item in classes)
            {
                if(item.EndDate <= DateOnly.FromDateTime(DateTime.Now))
                {
                    item.Status = false;
                }else
                {
                    item.Status = true;
                }
            }
            return classes;
        }
    }

    
}
