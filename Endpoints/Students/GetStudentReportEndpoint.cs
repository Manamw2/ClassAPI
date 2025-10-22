using ClassAPI.Data;
using ClassAPI.Dtos.Responses;
using ClassAPI.Common;
using FastEndpoints;

namespace ClassAPI.Endpoints.Students
{
    public class GetStudentReportEndpoint : EndpointWithoutRequest<GlobalResponse<StudentReportResponse>>
    {
        public override void Configure()
        {
            Get("/api/students/{studentId}/report");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var studentId = Route<int>("studentId");

            // Check if student exists
            if (!InMemoryDatabase.Students.TryGetValue(studentId, out var student))
            {
                await SendAsync(new GlobalResponse<StudentReportResponse>
                {
                    IsSuccess = false,
                    Message = "Student not found"
                }, 404, ct);
                return;
            }

            // Get all enrollments for this student
            var enrollments = InMemoryDatabase.Enrollments.Values
                .Where(e => e.StudentId == studentId)
                .ToList();

            var classReportItems = new List<ClassReportItem>();

            foreach (var enrollment in enrollments)
            {
                if (InMemoryDatabase.Classes.TryGetValue(enrollment.ClassId, out var classEntity))
                {
                    // Get marks for this student in this class
                    var lastTwoMarks = InMemoryDatabase.Marks.Values
                        .Where(m => m.StudentId == studentId && m.ClassId == enrollment.ClassId && !m.IsArchived)
                        .OrderByDescending(m => m.AddedAt).Take(2);

                    var currentMark = lastTwoMarks.FirstOrDefault();
                    var previousMark = lastTwoMarks.Skip(1).FirstOrDefault();

                    // Initialize all fields as null - they will only be set if there's a current mark
                    string? gradeCategory = null;
                    int? ranking = null;
                    PreformanceTrendStatus? performanceTrend = null;

                    // Only calculate these fields if there's a non-archived mark for the student in this class
                    if (currentMark != null)
                    {
                        // Calculate GradeCategory based on TotalMark (max 200)
                        var totalMark = currentMark.TotalMark;
                        gradeCategory = totalMark switch
                        {
                            >= 180 => "A",
                            >= 160 => "B",
                            >= 140 => "C",
                            >= 120 => "D",
                            _ => "F"
                        };

                        // Calculate Ranking - compare with all students in the same class
                        var allClassMarks = InMemoryDatabase.Marks.Values
                            .Where(m => m.ClassId == enrollment.ClassId && !m.IsArchived)
                            .GroupBy(m => m.StudentId)
                            .Select(g => g.OrderByDescending(m => m.AddedAt).First())
                            .OrderByDescending(m => m.TotalMark)
                            .ToList();

                        var studentPosition = allClassMarks.FindIndex(m => m.StudentId == studentId);
                        ranking = studentPosition >= 0 ? studentPosition + 1 : null;

                        // Calculate PerformanceTrend by comparing current mark with previous mark
                        if (previousMark != null)
                        {
                            var currentTotal = currentMark.TotalMark;
                            var previousTotal = previousMark.TotalMark;
                            var difference = currentTotal - previousTotal;

                            performanceTrend = difference switch
                            {
                                > 5 => PreformanceTrendStatus.Improving,
                                < -5 => PreformanceTrendStatus.Declining,
                                _ => PreformanceTrendStatus.Stable
                            };
                        }
                        else
                        {
                            // If this is the first mark, consider it stable
                            performanceTrend = PreformanceTrendStatus.Stable;
                        }
                    }

                    var reportItem = new ClassReportItem
                    {
                        ClassId = classEntity.Id,
                        ClassName = classEntity.Name,
                        Teacher = classEntity.Teacher,
                        EnrollmentDate = enrollment.EnrollmentDate,
                        ExamMark = currentMark?.ExamMark,
                        AssignmentMark = currentMark?.AssignmentMark,
                        TotalMark = currentMark?.TotalMark,
                        GradeCategory = gradeCategory,
                        Ranking = ranking,
                        PreformanceTrend = performanceTrend
                    };

                    classReportItems.Add(reportItem);
                }
            }

            // Calculate overall average mark across all classes where marks exist
            var marksWithValues = classReportItems.Where(c => c.TotalMark.HasValue).ToList();
            var overallAverageMark = marksWithValues.Any() 
                ? marksWithValues.Average(c => c.TotalMark!.Value) 
                : 0;

            var data = new StudentReportResponse
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                Classes = classReportItems,
                OverallAverageMark = overallAverageMark
            };

            await SendAsync(new GlobalResponse<StudentReportResponse>
            {
                IsSuccess = true,
                Message = "Student report retrieved successfully",
                Data = data
            }, 200, ct);
        }
    }
}

