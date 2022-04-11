using SQLite;
using System;

namespace Suretom.Client.UI.LocalDbModel
{
    /// <summary>
    /// 知识点
    /// </summary>
    public class Knowledge
    {
        [PrimaryKey]
        public long Id { get; set; }

        [Indexed(Unique = true)]
        public Guid Guid { get; set; }

        public string ParentGuid { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public int Level { get; set; }
        public string Period { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastChangeTime { get; set; }
        public int Version { get; set; }

        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime SyncTime { get; set; }
    }
}