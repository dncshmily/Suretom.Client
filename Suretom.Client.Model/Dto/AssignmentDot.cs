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
    public class AssignmentDot
    {
        /// <summary>
        ///
        /// </summary>
        public int code { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public List<DataItem> data { get; set; } = new List<DataItem>();

        /// <summary>
        ///
        /// </summary>
        public string canModify { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string state { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string firstAnaswer { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string _paperId { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string prev_paperId { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string isSubjective { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public int status { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public float totalScore { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public double endDate { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public string isPublishScore { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string isAllHasSubjective { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public int userType { get; set; } = 0;
    }

    /// <summary>
    ///
    /// </summary>
    public class OptionsItem
    {
        /// <summary>
        ///
        /// </summary>
        public string content { get; set; } = string.Empty;
    }

    /// <summary>
    ///
    /// </summary>
    public class ListItem
    {
        /// <summary>
        ///
        /// </summary>
        public string id { get; set; } = string.Empty;

        /// <summary>
        /// 下列关于仲裁的各项表述中，正确的是（  ）。

        /// </summary>
        public string content { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public List<OptionsItem> options { get; set; } = new List<OptionsItem>();

        /// <summary>
        ///
        /// </summary>
        public int type { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public float totalScore { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public List<string> subQuesList { get; set; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        public string studentAnswer { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string analysis { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string answers { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string correctStatus { get; set; } = string.Empty;
    }

    /// <summary>
    ///
    /// </summary>
    public class DataItem
    {
        /// <summary>
        ///
        /// </summary>
        public int type { get; set; } = 0;

        /// <summary>
        /// 单选题
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public List<ListItem> list { get; set; } = new List<ListItem>();
    }
}