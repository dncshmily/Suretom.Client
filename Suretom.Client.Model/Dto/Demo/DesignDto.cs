using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suretom.Client.Entity
{
    public class DesignDto
    {
        public string Id { get; set; }
        public int SortOrder { get; set; }
        public string Title { get; set; }
        public List<Lessons> Lessons { get; set; } = new List<Lessons>();
    }

    public class Lessons
    {
        public string Id { get; set; }
        public int SortOrder { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public List<Cells> Cells { get; set; } = new List<Cells>();
    }

    public class Cells
    {
        public string CourseDocId { get; set; }
        public string Icon { get; set; }
        public string Id { get; set; }
        public bool LastLearned { get; set; }
        public string LectureId { get; set; }
        public int SortOrder { get; set; }
        public bool Status { get; set; }
        public string Title { get; set; }
    }
}