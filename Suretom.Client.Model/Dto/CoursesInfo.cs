using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.Entity
{
    /// <summary>
    ///
    /// </summary>
    public class CoursesInfo
    {
        /// <summary>
        ///
        /// </summary>
        public Guid guid { get; set; } = Guid.NewGuid();

        /// <summary>
        ///学生信息
        /// </summary>
        public Student student { get; set; } = new Student();

        /// <summary>
        ///课程信息
        /// </summary>
        public CourseDto course = new CourseDto();
    }
}