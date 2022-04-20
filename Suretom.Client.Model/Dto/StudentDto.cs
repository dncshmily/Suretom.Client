using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.Entity
{
    public class StudentDto
    {
        public Info info { get; set; } = new Info();
    }

    public class Info
    {
        public string CampusZoneName { get; set; }
        public string ClassName { get; set; }
        public string DisplayName { get; set; }
        public string EnterBatch { get; set; }
        public string LearnType { get; set; }
        public string MajorName { get; set; }
        public string SchoolName { get; set; }
        public string Sex { get; set; }
        public string StudentNumber { get; set; }
    }
}