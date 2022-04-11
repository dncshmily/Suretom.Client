using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suretom.Client.Entity
{
    public class CourseDto
    {
        public int Completed { get; set; }
        public string CourseName { get; set; }
        public string CourseOpenId { get; set; }
        public string DisplayName { get; set; }
        public string ExpiredTime { get; set; }
        public string Hoplinks { get; set; }
        public bool IsPracticeCourse { get; set; }
        public string NoStudyNoExam { get; set; }
        public float Schedule { get; set; }
        public string StudyTerm { get; set; }
        public int StudyYear { get; set; }
        public int TotalCount { get; set; }
        public string ScheduleTxt { get; set; }
    }
}