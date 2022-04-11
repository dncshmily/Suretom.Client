using System;

namespace Suretom.Client.Entity
{
    /// <summary>
    /// 批量导入过程中产生的信息
    /// </summary>
    public class BatchImportProcessInfo
    {
        public int Id { get; set; }
        public string Info { get; set; }

        /// <summary>
        /// 信息类型
        /// </summary>
        public BatchImportProcessInfoType Type { get; set; } = BatchImportProcessInfoType.错误;

        public string CreateTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public enum BatchImportProcessInfoType
    {
        信息 = 0,
        警告 = 1,
        错误 = 2
    }
}