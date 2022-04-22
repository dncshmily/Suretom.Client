using Newtonsoft.Json;
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
    ///
    /// </summary>
    public class StudentService : ServiceBase, IStudentService
    {
        public StudentService()
        {
            Urls.Add("Add", "User/AddStudent");
            Urls.Add("List", "User/StudentList");
            Urls.Add("QuestionLog", "Grab/QuestionLog");
        }

        /// <summary>
        ///添加单个学生
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public HttpResult AddStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.SchoolName))
                throw new ArgumentException("schoolName");
            if (string.IsNullOrEmpty(student.IdCard))
                throw new ArgumentException("idCard");
            if (string.IsNullOrEmpty(student.MoviePwd))
                throw new ArgumentException("moviePwd");
            if (string.IsNullOrEmpty(student.ClassName))
                throw new ArgumentException("className");
            if (string.IsNullOrEmpty(student.StudyCode))
                throw new ArgumentException("studyCode");
            if (string.IsNullOrEmpty(student.StudentName))
                throw new ArgumentException("studentName");

            var paramValue = new NameValueCollection() {
                       { "schoolName",student.SchoolName},
                       { "idCard",student.IdCard},
                       { "moviePwd",student.MoviePwd},
                       { "studyType",student.StudyType.ToString()},
                       { "className",student.ClassName},
                       { "studyCode",student.StudyCode},
                       { "studentName",student.StudentName},
                       { "token",GlobalContext.Token}
                };

            var result = PostForm(Urls["Add"], paramValue);

            return result;
        }

        /// <summary>
        ///获取学生信息
        /// </summary>
        /// <returns></returns>
        public List<StudentInfo> GetStudentList()
        {
            var param = new { GlobalContext.Token, };

            var result = Get(JsonConvert.SerializeObject(param), Urls["List"], this.GetType());

            if (result.Success)
            {
                var data = result.Data.ToString();

                try
                {
                    //data= data.Remove(0, 1);
                    //data= data.Remove(data.Length-1, 1);
                    // var ss = JsonConvert.DeserializeObject<List<StudentInfo>>(data);

                    return JsonConvert.DeserializeObject<List<StudentInfo>>(data);
                }
                catch (Exception)
                {
                }
            }
            return new List<StudentInfo>();
        }

        /// <summary>
        /// 记录试题
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public HttpResult QuestionLog(string idCard, string passWord, string courseId, string schoolCode, string cookieContent, string cellsIds)
        {
            if (string.IsNullOrEmpty(courseId))
                throw new ArgumentException("courseId");
            if (string.IsNullOrEmpty(schoolCode))
                throw new ArgumentException("schoolCode");
            if (string.IsNullOrEmpty(cookieContent))
                throw new ArgumentException("cookieContent");
            if (string.IsNullOrEmpty(cellsIds))
                throw new ArgumentException("cellsIds");
            if (string.IsNullOrEmpty(idCard))
                throw new ArgumentException("idCard");
            if (string.IsNullOrEmpty(passWord))
                throw new ArgumentException("passWord");

            var paramValue = new NameValueCollection() {
                       { "courseId",courseId},
                       { "schoolCode",schoolCode},
                       { "cookieContent",cookieContent},
                       { "cellsIds",cellsIds},
                       { "idCard",idCard},
                       { "passWord",passWord},
                       { "token",GlobalContext.Token}
                };

            var result = PostForm(Urls["QuestionLog"], paramValue);

            return result;
        }
    }
}