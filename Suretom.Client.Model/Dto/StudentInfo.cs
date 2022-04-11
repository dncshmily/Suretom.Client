using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.Entity
{
    public class StudentInfo
    {
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 学生集合
        /// </summary>
        public List<Student> List { get; set; } = new List<Student>();
    }
}