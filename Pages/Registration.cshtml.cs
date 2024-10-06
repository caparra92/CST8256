using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AcademicManagement;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab2.Pages
{
    public class RegistrationModel : PageModel
    {
        [BindProperty]
        public string? Sl_Student { get; set; }

        [BindProperty]
        public List<string>? SelectedCourses { get; set; } = new List<string>();

        public List<Student>? Students { get;set; }
        public List<Course>? Courses { get;set; }
        public List<Course>? StudentCourses { get;set; }
        public List<AcademicRecord>? AcademicRecords { get; set; }
        public SelectList? StudentsList { get; set; }
        public string? SelectedStudent { get; set; }
        public string? NoCourseSelectedError { get; set; }
        public string? NoStudentSelectedError { get; set; }
        public int NumberOfRecords { get; set; }
        public bool SelectStudentSubmitted { get; set; } = false;
        public bool RegisterStudentSubmitted { get; set; } = false;

        public RegistrationModel()
        {
            Students = new List<Student>();
            Courses = new List<Course>();
            Students = DataAccess.GetAllStudents();
            Courses = DataAccess.GetAllCourses();
            StudentCourses = new List<Course>();
            StudentsList = new SelectList(Students, nameof(Student.StudentId), nameof(Student.StudentName));
        }
        
        public void OnGet()
        {
            SelectedStudent = Sl_Student;
            NoCourseSelectedError = "";
            NoStudentSelectedError = "";
        }

        public Course GetCourseById(string courseId)
        {
            Courses = DataAccess.GetAllCourses();

            return (from course in Courses
                   where course.CourseCode == courseId
                   select course).First();
        }

        public List<Course> GetStudentCourseRegistrations(string studentId)
        {
            AcademicRecords = new List<AcademicRecord>();
            StudentCourses = new List<Course>();
            if (!string.IsNullOrEmpty(studentId))
            {
                AcademicRecords = DataAccess.GetAcademicRecordsByStudentId(studentId);
            }
            foreach (var record in AcademicRecords)
            {
                StudentCourses.Add(GetCourseById(record.CourseCode));
            }
            return StudentCourses;
        }

        public void OnPostStudentSelected()
        {
            SelectStudentSubmitted = true;

            if (string.IsNullOrEmpty(Sl_Student))
            {
                NoStudentSelectedError = "You must select a Student";
            } else
            {
                var studentId = Sl_Student;
                StudentCourses = GetStudentCourseRegistrations(studentId);
                NumberOfRecords = StudentCourses.Count;
            }
        }

        public void OnPostRegister()
        {
            RegisterStudentSubmitted = true;
            var studentId = Sl_Student;
            if(SelectedCourses.Count == 0)
            {
                NoCourseSelectedError = "You must select at least one course";
            } 
            else
            {
                foreach (string selectedCourse in SelectedCourses)
                {
                    AcademicRecord record = new AcademicRecord(studentId, selectedCourse);
                    DataAccess.AddAcademicRecord(record);
                }
                StudentCourses = GetStudentCourseRegistrations(studentId);
                NumberOfRecords = StudentCourses.Count;
            }
        }
    }
}
