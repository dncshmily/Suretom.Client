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
        }

        /// <summary>
        ///添加单个学生
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public HttpResult AddStudent(NameValueCollection paramValue)
        {
            if (string.IsNullOrEmpty(paramValue["schoolName"]))
                throw new ArgumentException("schoolName");
            if (string.IsNullOrEmpty(paramValue["idCard"]))
                throw new ArgumentException("idCard");
            if (string.IsNullOrEmpty(paramValue["moviePwd"]))
                throw new ArgumentException("moviePwd");
            if (string.IsNullOrEmpty(paramValue["studyType"]))
                throw new ArgumentException("studyType");
            if (string.IsNullOrEmpty(paramValue["className"]))
                throw new ArgumentException("className");
            if (string.IsNullOrEmpty(paramValue["studyCode"]))
                throw new ArgumentException("studyCode");
            if (string.IsNullOrEmpty(paramValue["studentName"]))
                throw new ArgumentException("studentName");

            var result = PostForm(Urls["Add"], paramValue);

            return result;
        }

        /// <summary>
        ///获取学生信息
        /// </summary>
        /// <returns></returns>
        public StudentInfo GetStudentList()
        {
            var param = new { GlobalContext.Token, };

            var result = Get(JsonConvert.SerializeObject(param), Urls["List"], this.GetType());

            if (result.Success)
            {
                return JsonConvert.DeserializeObject<StudentInfo>(result.Data.ToString());
            }

            return null;
        }
    }
}