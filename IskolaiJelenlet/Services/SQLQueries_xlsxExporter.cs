using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskolaiJelenlet.Services
{
    public static class SQLQueries_xlsxExporter
    {
        // === ATTENDANCE QUERIES ===
        public const string AttendanceBase = @"
            SELECT 
                a.student_id, s.first_name, s.last_name, 
                a.lesson_id, l.title AS lesson_title, 
                a.status 
            FROM Attendance a
            LEFT JOIN Student s ON a.student_id = s.student_id
            LEFT JOIN Lesson l ON a.lesson_id = l.lesson_id";

        public const string AttendanceAll = AttendanceBase;
        public const string AttendanceByStudent = AttendanceBase + " WHERE a.student_id = @student_id";
        public const string AttendanceByLesson = AttendanceBase + " WHERE a.lesson_id = @lesson_id";

        // === COURSE QUERIES ===
        public const string CoursesAll = @"
            SELECT course_id, course_name, description 
            FROM Course";

        // === GRADE QUERIES ===
        public const string GradesBase = @"
            SELECT 
                g.student_id, s.first_name, s.last_name, 
                g.course_id, c.course_name, 
                g.grade_value, g.grade_date 
            FROM Grade g
            LEFT JOIN Student s ON g.student_id = s.student_id
            LEFT JOIN Course c ON g.course_id = c.course_id";

        public const string GradesAll = GradesBase;
        public const string GradesByStudent = GradesBase + " WHERE g.student_id = @student_id";
        public const string GradesByCourse = GradesBase + " WHERE g.course_id = @course_id";
        public const string GradesByCourseAndStudent = GradesBase + " WHERE g.course_id = @course_id AND g.student_id = @student_id";

        // === LESSON QUERIES ===
        public const string LessonsAll = @"
            SELECT 
                l.lesson_id, l.lesson_date, l.title, 
                l.course_id, c.course_name 
            FROM Lesson l
            LEFT JOIN Course c ON l.course_id = c.course_id";

        // === NOTE QUERIES ===
        public const string NotesBase = @"
            SELECT 
                n.student_id, s.first_name, s.last_name, 
                n.lesson_id, l.title AS lesson_title, 
                n.content, n.created_at 
            FROM Note n
            LEFT JOIN Student s ON n.student_id = s.student_id
            LEFT JOIN Lesson l ON n.lesson_id = l.lesson_id";

        public const string NotesAll = NotesBase;
        public const string NotesByStudent = NotesBase + " WHERE n.student_id = @student_id";

        // === STUDENT QUERIES ===
        public const string StudentsAll = @"
            SELECT student_id, first_name, last_name, email, enrollment_date 
            FROM Student";
    }
}
