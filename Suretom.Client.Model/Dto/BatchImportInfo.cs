using System.ComponentModel;

namespace Suretom.Client.Entity
{
    /// <summary>
    /// 批量导入的信息
    /// </summary>
    public class BatchImportInfo : INotifyPropertyChanged
    {
        private string wordFileFullPath = string.Empty;
        private string wordFileName = string.Empty;
        private string resourcePaperGuid = string.Empty;

        private BatchImportStatus _batchImportStatus = BatchImportStatus.未开始;

        public int Id { get; set; }

        public string ShowName
        {
            get
            {
                return $"Word {Id}";
            }
        }

        public string WordFileFullPath
        {
            get
            {
                return wordFileFullPath;
            }
            set
            {
                wordFileFullPath = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WordFileFullPath"));
            }
        }

        public string ResourcePaperGuid
        {
            get
            {
                return resourcePaperGuid;
            }
            set
            {
                resourcePaperGuid = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResourcePaperGuid"));
            }
        }

        public string WordFileName
        {
            get
            {
                return wordFileName;
            }
            set
            {
                wordFileName = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WordFileName"));
            }
        }

        /// <summary>
        /// 引用的处理控件
        /// </summary>
        public object Ctl { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public BatchImportStatus Status
        {
            get
            {
                return _batchImportStatus;
            }
            set
            {
                _batchImportStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}