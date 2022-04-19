using Newtonsoft.Json;
using Suretom.Client.Common;
using Suretom.Client.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Suretom.Client.Data
{
    public class CoursesData
    {
        private string apiUrl = string.Empty;
        private string idCard = string.Empty;
        private string passWord = string.Empty;
        private string schoolcode = string.Empty;
        private string key = string.Empty;
        private string info = string.Empty;
        private string cookie = string.Empty;
        private Dictionary<string, string> header;
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private Student student = new Student();

        public CoursesData()
        { }

        /// <summary>
        ///
        /// </summary>
        public CoursesData(Student student)
        {
            apiUrl = "https://main.ahjxjy.cn/";
            idCard = student.IdCard;
            passWord =student.MoviePwd;
            key = UtilityHelper.MD5_Encrypt($"{passWord}zmzrazilo7no32zysvn0ug").ToUpper();
            info = CourseHelper.AesEncrypt(idCard, key);
            cookie = CourseHelper.FromPost($"{apiUrl}api/login/newLogin", $"userName={Uri.EscapeDataString(info) }&passWord={key}&userType=1", Encoding.UTF8);
            header = new Dictionary<string, string>() { { "Cookie", cookie } };
            schoolcode = CourseHelper.ReplaceOssUrl(CourseHelper.HttpGet($"{apiUrl}/studentstudio/", header));
            this.student=student;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public StudentDto GetStudentInfo()
        {
            try
            {
                return JsonConvert.DeserializeObject<StudentDto>(CourseHelper.FromPost($"{apiUrl}/studentstudio/ajax-config-getInfo", header, "getschoolcode=" + schoolcode));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///课程详情数据列表
        /// </summary>
        /// <param name="undocourselist"></param>
        public List<CourseDto> GetCourseList()
        {
            var courseList = new List<CourseDto>();

            var courseResults = JsonConvert.DeserializeObject<ResultDto<CourseDto>>(CourseHelper.FromPost($"{apiUrl}/studentstudio/ajax-course-list", header, $"type=studying&courseType=0&getschoolcode={schoolcode}&studyYear=&studyTerm=&courseName="));

            if (courseResults.Code != 1) return courseList;

            foreach (var course in courseResults.List)
            {
                //章
                var designResult = JsonConvert.DeserializeObject<ResultDto<DesignDto>>(CourseHelper.FromPost($"{apiUrl}/study/design/design", header, $"courseOpenId={course.CourseOpenId}&schoolCode={schoolcode}&icon=video"));

                if (designResult.Code == 1)
                {
                    continue;
                }

                course.CourseList=designResult.List;
                course.Student=student;
            }

            return courseList;
        }

        /// <summary>
        ///课程详情数据列表
        /// </summary>
        /// <param name="undocourselist"></param>
        public ObservableCollection<CourseDto> GetCourseInfoList()
        {
            var courseList = new ObservableCollection<CourseDto>();

            try
            {
                var resultJson = CourseHelper.FromPost($"{apiUrl}/studentstudio/ajax-course-list", header, $"type=studying&courseType=0&getschoolcode={schoolcode}&studyYear=&studyTerm=&courseName=");
                var courseResults = JsonConvert.DeserializeObject<ResultDto<CourseDto>>(resultJson);

                if (courseResults.Code != 1) return courseList;

                foreach (var course in courseResults.List)
                {
                    //章
                    var designResult = JsonConvert.DeserializeObject<ResultDto<DesignDto>>(CourseHelper.FromPost($"{apiUrl}/study/design/design", header, $"courseOpenId={course.CourseOpenId}&schoolCode={schoolcode}&icon=video"));

                    if (designResult.Code != 1)
                    {
                        continue;
                    }

                    course.CourseList=designResult.List;
                    course.Student=student;

                    courseList.Add(course);
                }
            }
            catch (Exception ex)
            {
                log.Error($"GetCourseInfoList：{ex.Message}");
                log.Error($"GetCourseInfoList：{ex}");
            }
            return courseList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CourseDto> GetStudentCourseList()
        {
            var courses= new List<CourseDto>();

            try
            {
                var resultJson = CourseHelper.FromPost($"{apiUrl}/studentstudio/ajax-course-list", header, $"type=studying&courseType=0&getschoolcode={schoolcode}&studyYear=&studyTerm=&courseName=");
                var courseResults = JsonConvert.DeserializeObject<ResultDto<CourseDto>>(resultJson);

                if (courseResults.Code != 1) return courses;

                foreach (var course in courseResults.List)
                {
                    //章
                    var designResult = JsonConvert.DeserializeObject<ResultDto<DesignDto>>(CourseHelper.FromPost($"{apiUrl}/study/design/design", header, $"courseOpenId={course.CourseOpenId}&schoolCode={schoolcode}&icon=video"));

                    if (designResult.Code != 1)
                    {
                        continue;
                    }

                    course.CourseList=designResult.List;
                    course.Student=student;

                    courses.Add(course);
                }
            }
            catch (Exception ex)
            {
                log.Error($"GetStudentCourseList：{ex.Message}");
                log.Error($"GetStudentCourseList：{ex}");
            }
            return courses;
        }


        /// <summary>
        ///单课程学校
        /// </summary>
        /// <param name="undocourselist"></param>
        public void SingeSyudentStart(CourseDto course)
        {
            //章
            var designresult = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<DesignDto>>(CourseHelper.FromPost($"{apiUrl}/study/design/design", header, $"courseOpenId={course.CourseOpenId}&schoolCode={schoolcode}&icon=video"));
            if (designresult.Code != 1) return;

            foreach (var l in designresult.List)
            {
                var undodesignlist = l.Lessons.Where(p => p.Status == 0); //未完成的章
                if (undodesignlist.Count() == 0) continue;

                foreach (var design in undodesignlist)
                {
                    //节
                    var undocells = design.Cells.Where(p => p.Status == false&&p.Icon== "video"); //未完成的节
                    foreach (var cells in undocells)
                    {
                        var doingcellsjson = CourseHelper.FromPost($"{apiUrl}/study/studying/studying", header, $"courseOpenId={course.CourseOpenId}&cellId={cells.Id}&schoolCode={schoolcode}");
                        var doingcells = Newtonsoft.Json.JsonConvert.DeserializeObject<DoingCellsDto>(doingcellsjson);
                        if (doingcells.Code != 1) continue;
                        if (doingcells.Cell.Status) continue;

                        for (var i = true; ;)
                        {
                            if (!i) break;
                            System.Threading.Thread.Sleep(1000 * 60);
                            doingcells.Cell.LastTime += 60;
                            var recordjson = string.Empty;
                            bool isResult = false;

                            try
                            {
                                //重试操作
                                OperationHelper.RetryExceptionAction(() =>
                                {
                                    recordjson = CourseHelper.FromPost($"{apiUrl}/study/studying/recordVideoPosition", header, $"courseOpenId={course.CourseOpenId}&cellId={cells.Id}&schoolCode={schoolcode}&position={doingcells.Cell.LastTime}");

                                    var record = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<DoingCellsDto>>(recordjson);

                                    if (record.Passed) //完成
                                    {
                                        var posturl = $"{apiUrl}/study/studying/studied";
                                        var paramData = $"courseOpenId={course.CourseOpenId}&cellId={cells.Id}&schoolCode={schoolcode}";

                                        var studiedjson = CourseHelper.FromPost(posturl, header, paramData);

                                        var studied = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<DoingCellsDto>>(studiedjson);

                                        if (studied.Code==1)
                                        {
                                            log.Info($"{studied.ToJson()}");
                                        }
                                        else
                                        {
                                            studied = JsonConvert.DeserializeObject<ResultDto<DoingCellsDto>>(CourseHelper.FromPost(posturl, header, paramData));

                                            if (studied.Code==1)
                                            {
                                                log.Info($"{studied.ToJson()}");
                                            }
                                            else
                                            {
                                                log.Error($"{posturl}:{paramData}:{studied.Msg}");
                                            }
                                        }

                                        i = false;
                                    }
                                }, 3, 1000, ref isResult);

                                if (isResult)
                                {
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                i =true;
                                log.Error($"{ex.Message}");
                                log.Error($"{ex}");
                                log.Error($"{recordjson}");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 课程详情
        /// </summary>
        /// <returns></returns>
        public void GetCourseInfo()
        {
            var cookie = CourseHelper.FromPost($"{apiUrl}api/login/newLogin", $"userName={Uri.EscapeDataString(CourseHelper.AesEncrypt(idCard, key)) }&passWord={UtilityHelper.MD5_Encrypt($"{passWord}zmzrazilo7no32zysvn0ug").ToUpper()}&userType=1", Encoding.UTF8);

            var header = new Dictionary<string, string>()
            {
                { "Cookie", cookie}
            };

            var html = CourseHelper.HttpGet($"{apiUrl}/studentstudio/", header);

            var schoolcode = CourseHelper.ReplaceOssUrl(html);

            var coursejson = CourseHelper.FromPost($"{apiUrl}/studentstudio/ajax-course-list", header, "type=studying&courseType=0&getschoolcode=" + schoolcode + "&studyYear=&studyTerm=&courseName=");

            var courseresult = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<CourseDto>>(coursejson);
            if (courseresult.Code != 1) return;

            var undocourselist = courseresult.List.Where(p => p.Schedule < 100); //未完成的课程

            foreach (var course in undocourselist)
            {
                //章
                var designresult = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<DesignDto>>(CourseHelper.FromPost($"{apiUrl}/study/design/design", header, $"courseOpenId={course.CourseOpenId}&schoolCode={schoolcode}"));
                if (designresult.Code != 1) return;

                foreach (var l in designresult.List)
                {
                    var undodesignlist = l.Lessons.Where(p => p.Status == 0); //未完成的章
                    if (undodesignlist.Count() == 0) continue;

                    foreach (var design in undodesignlist)
                    {
                        //节
                        var undocells = design.Cells.Where(p => p.Status == false); //未完成的
                        foreach (var cells in undocells)
                        {
                            var doingcellsjson = CourseHelper.FromPost($"{apiUrl}/study/studying/studying", header, $"courseOpenId={course.CourseOpenId}&cellId={cells.Id}&schoolCode={schoolcode}");
                            var doingcells = Newtonsoft.Json.JsonConvert.DeserializeObject<DoingCellsDto>(doingcellsjson);
                            if (doingcells.Code != 1) continue;
                            if (doingcells.Cell.Status) continue;

                            for (var i = true; ;)
                            {
                                if (!i) break;
                                //System.Threading.Thread.Sleep(1000 * 60);

                                doingcells.Cell.LastTime += 60;
                                var recordjson = CourseHelper.FromPost($"{apiUrl}/study/studying/recordVideoPosition", header, $"courseOpenId={course.CourseOpenId}&cellId={cells.Id}&schoolCode={schoolcode}&position={doingcells.Cell.LastTime}");
                                if (recordjson.Contains("基础连接已经关闭")) continue;
                                var record = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<DoingCellsDto>>(recordjson);

                                if (record.Passed) //完成
                                {
                                    var studiedjson = CourseHelper.FromPost($"{apiUrl}/study/studying/studied", header, $"courseOpenId={course.CourseOpenId}&cellId={cells.Id}&schoolCode={schoolcode}");

                                    var studied = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<DoingCellsDto>>(studiedjson);
                                    i = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}