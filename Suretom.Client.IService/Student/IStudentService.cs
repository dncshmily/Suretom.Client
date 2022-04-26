using Suretom.Client.Common;
using Suretom.Client.Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.IService
{
    /// <summary>
    ///
    /// </summary>
    public interface IStudentService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="StudentInfo"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        HttpResult AddStudent(Student student);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResult DeleteStudent(string id);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        List<StudentInfo> GetStudentList();

        /// <summary>
        ///
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="schoolCode"></param>
        /// <param name="cookieContent"></param>
        /// <param name="cellsIds"></param>
        /// <returns></returns>
        HttpResult QuestionLog(string idCard, string passWord, string courseId, string schoolCode, string cookieContent, string cellsIds);
    }
}