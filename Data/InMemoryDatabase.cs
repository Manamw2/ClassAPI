using ClassAPI.Models.Entities;
using System.Collections.Concurrent;

namespace ClassAPI.Data
{
    public static class InMemoryDatabase
    {
        public static ConcurrentDictionary<int, Student> Students = new();
        public static ConcurrentDictionary<int, Class> Classes = new();
        public static ConcurrentDictionary<int, Enrollment> Enrollments = new();
        public static ConcurrentDictionary<int, Mark> Marks = new();

        public static int StudentId = 1;
        public static int ClassId = 1;
        public static int EnrollmentId = 1;
        public static int MarkId = 1;
    }
}
