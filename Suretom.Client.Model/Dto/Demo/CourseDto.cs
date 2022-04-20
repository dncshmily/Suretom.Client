using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Suretom.Client.Entity
{
    /// <summary>
    ///
    /// </summary>
    public class CourseDto : IComparable<CourseDto>
    {
        /// <summary>
        /// 声明CancellationTokenSource对象
        /// </summary>
        public CancellationTokenSource TokenSource = new CancellationTokenSource();

        /// <summary>
        ///
        /// </summary>
        public CancellationToken Token = new CancellationToken();

        public CourseDto()
        {
            TokenSource = new CancellationTokenSource();
            Token =TokenSource.Token;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(CourseDto courseDto)
        {
            return this.Completed.CompareTo(courseDto.Id);
        }

        /// <summary>
        ///
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Completed { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string CourseOpenId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ExpiredTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Hoplinks { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsPracticeCourse { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string NoStudyNoExam { get; set; }

        /// <summary>
        ///
        /// </summary>
        public float Schedule { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string StudyTerm { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int StudyYear { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ScheduleTxt { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string imgStr { get; set; }

        /// <summary>
        /// -1：未开始 0：待开始 1：学习中 2：已完成
        /// </summary>
        public int Status { get; set; } = -1;

        /// <summary>
        ///是否开始
        /// </summary>
        public bool IsStartSuccess = false;

        /// <summary>
        ///是否暂停
        /// </summary>
        public bool IsPauseSuccess = false;

        /// <summary>
        ///
        /// </summary>
        public Student Student { get; set; } = new Student();

        /// <summary>
        ///章节
        /// </summary>
        public List<DesignDto> CourseList { get; set; } = new List<DesignDto>();
    }
}