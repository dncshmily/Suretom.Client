using SQLite;
using System;

namespace Suretom.Client.UI.LocalDbModel
{
    /// <summary>
    /// 题型数据
    /// </summary>
    public class ItemPattern
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Indexed]
        public Guid Guid { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 题型编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 题型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据的同步时间
        /// </summary>
        public DateTime SyncTime { get; set; }
    }
}