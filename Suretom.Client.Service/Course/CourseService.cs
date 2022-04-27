using Suretom.Client.Common;
using Suretom.Client.Entity;
using Suretom.Client.IService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.Service
{
    /// <summary>
    /// 課程
    /// </summary>
    public class CourseService : ServiceBase, ICourseService
    {
        /// <summary>
        ///
        /// </summary>
        public CourseService()
        {
            Urls.Add("AddCourse", "Grab/AddCourse");
            Urls.Add("AddSection", "Grab/AddSection");
        }

        /// <summary>
        ///添加课程
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public HttpResult AddCourse(AddCourseDto dto)
        {
            if (string.IsNullOrEmpty(dto.idCard))
                throw new ArgumentException("idCard");
            if (string.IsNullOrEmpty(dto.schoolCode))
                throw new ArgumentException("schoolCode");
            if (dto.list.Count==0)
                throw new ArgumentException("list");

            var paramValue = new NameValueCollection() {
                       { "idCard",dto.idCard},
                       { "schoolCode",dto.schoolCode},
                       { "list",dto.list.ToJson()},
                       { "token",GlobalContext.Token}
                };

            var result = PostForm(Urls["AddCourse"], paramValue);

            return result;
        }

        /// <summary>
        ///添加章节
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public HttpResult AddSection(AddSectionDto dto)
        {
            if (string.IsNullOrEmpty(dto.courseId))
                throw new ArgumentException("courseId");
            if (string.IsNullOrEmpty(dto.schoolCode))
                throw new ArgumentException("schoolCode");
            if (dto.sections.Count==0)
                throw new ArgumentException("sections");

            var paramValue = new NameValueCollection() {
                       { "idCard",dto.courseId},
                       { "schoolCode",dto.schoolCode},
                       { "list",dto.sections.ToJson()},
                       { "token",GlobalContext.Token}
                };

            var result = PostForm(Urls["AddSection"], paramValue);

            return result;
        }
    }
}