using Suretom.Client.Common;
using Suretom.Client.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.IService
{
    public interface ICourseService
    {
        /// <summary>
        ///添加课程
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        HttpResult AddCourse(AddCourseDto dto);

        /// <summary>
        ///添加章节
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        HttpResult AddSection(AddSectionDto dto);
    }
}