using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suretom.Client.Entity.Dto
{
    public class AssignmentDot
    {
        public string code { get; set; }
        public data data { get; set; }
        public int status { get; set; }

        public int state { get; set; }
        public int userType { get; set; }

        public bool firstAnaswer { get; set; }
        public bool isAllHasSubjective { get; set; }
        public bool isPublishScore { get; set; }
        public bool isSubjective { get; set; }
        public int totalScore { get; set; }
    }

    public class data
    {
        /// <summary>
        ///
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 单选题
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<ItemList> list { get; set; }
    }

    public class ItemList
    {
        /// <summary>
        ///
        /// </summary>
        public string id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string content { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<OptionsItem> options { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int totalScore { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<string> subQuesList { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string studentAnswer { get; set; }

        public string analysis { get; set; }
        public string answers { get; set; }
        public string correctStatus { get; set; }
    }

    public class OptionsItem
    {
        public string content { get; set; }
    }
}