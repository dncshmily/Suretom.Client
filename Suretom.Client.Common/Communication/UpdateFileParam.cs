using System.IO;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 上传文件的参数
    /// </summary>
    public class UpdateFileParam
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值，若是文件，则为 fileName.Ext
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否为文件，若为True，则Value为文件的路径
        /// </summary>
        public bool IsFile { get; set; } = false;

        /// <summary>
        /// 文件内容,当IsFile=true时，可以使用,
        /// 备注：使用流，可以兼容各种情况（比如以流的形式压缩后，再上传）
        /// </summary>
        public Stream FileContent { get; set; }
    }
}