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
    public class CourseInfoDto
    {
        /// <summary>
        ///
        /// </summary>
        public AddCourseDto course { get; set; } = new AddCourseDto();

        /// <summary>
        ///
        /// </summary>
        public AddSectionDto section { get; set; } = new AddSectionDto();
    }

    /// <summary>
    ///添加课程
    /// </summary>
    public class AddCourseDto
    {
        /// <summary>
        ///
        /// </summary>
        public List<CourseInfo> list { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string idCard { get; set; }

        /// <summary>
        /// 学校编码
        /// </summary>
        public string schoolCode { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class CourseInfo
    {
        /// <summary>
        /// 春季
        /// </summary>
        public string studyTerm { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int studyYear { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string schedule { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string courseName { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// 课程id
        /// </summary>
        public string courseId { get; set; }
    }

    /// <summary>
    ///添加章节
    /// </summary>
    public class AddSectionDto
    {
        /// <summary>
        /// 课程id
        /// </summary>
        public string courseId { get; set; }

        /// <summary>
        /// 学校编码
        /// </summary>
        public string schoolCode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<SectionsItem> sections { get; set; } = new List<SectionsItem>();
    }

    /// <summary>
    ///
    /// </summary>
    public class SectionsItem
    {
        /// <summary>
        ///
        /// </summary>
        public List<CellsItem> cells { get; set; } = new List<CellsItem>();

        /// <summary>
        /// 章id
        /// </summary>
        public string sectionId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int sortOrder { get; set; }

        /// <summary>
        /// 章标题
        /// </summary>
        public string title { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class CellsItem
    {
        /// <summary>
        /// 节id
        /// </summary>
        public string cellId { get; set; }

        /// <summary>
        /// 节标题
        /// </summary>
        public string cellTitle { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int sortOrder { get; set; }
    }
}