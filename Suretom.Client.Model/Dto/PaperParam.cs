using System.ComponentModel;

namespace Suretom.Client.Entity
{
    /// <summary>
    /// 试卷参数
    /// </summary>
    public class PaperParam : INotifyPropertyChanged
    {
        private string paperGuid = string.Empty;
        private string paperName = string.Empty;
        private string fileName = string.Empty;
        private string examGuid = string.Empty;
        private string subject = string.Empty;
        private string useYear = string.Empty;
        private int artScience = 0;
        private int type = 0;
        private int source = 0;
        private string period = string.Empty;
        private string grade = string.Empty;
        private string documentGuid = string.Empty;

        /// <summary>
        /// Word文件的完全路径
        /// </summary>
        public string wordFilePath { get; set; }

        public string WordFilePath
        {
            get
            {
                return wordFilePath;
            }
            set
            {
                wordFilePath = value;
                OnPropertyChanged("WordFilePath");
            }
        }

        public string DocumentGuid
        {
            get
            {
                return documentGuid;
            }
            set
            {
                documentGuid = value;
                OnPropertyChanged("DocumentGuid");
            }
        }

        public string PaperGuid
        {
            get
            {
                return paperGuid;
            }
            set
            {
                paperGuid = value;
                OnPropertyChanged("PaperGuid");
            }
        }

        public string PaperName
        {
            get
            {
                return paperName;
            }
            set
            {
                paperName = value;
                OnPropertyChanged("PaperName");
            }
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public string ExamGuid
        {
            get
            {
                return examGuid;
            }
            set
            {
                examGuid = value;
                OnPropertyChanged("ExamGuid");
            }
        }

        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
                OnPropertyChanged("Subject");
            }
        }

        public string UseYear
        {
            get
            {
                return useYear;
            }
            set
            {
                useYear = value;
                OnPropertyChanged("UseYear");
            }
        }

        public int ArtScience
        {
            get
            {
                return artScience;
            }
            set
            {
                artScience = value;
                OnPropertyChanged("ArtScience");
            }
        }

        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        public int Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                OnPropertyChanged("Source");
            }
        }

        public string Period
        {
            get
            {
                return period;
            }
            set
            {
                period = value;
                OnPropertyChanged("Period");
            }
        }

        /// <summary>
        /// 年级，一张卷子的题目年级是一样的，
        /// item表中的GradeString：G3,G2,G1,C3,C2,C1
        /// </summary>
        public string Grade
        {
            get
            {
                return grade;
            }
            set
            {
                grade = value;
                OnPropertyChanged("Grade");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 同一知识点的试卷参数
    /// </summary>
    public class SameKnowledgePaperParam : PaperParam
    {
        private string knowledgeName = string.Empty;
        private string knowledgeGuid = string.Empty;
        private string knowledgeCode = string.Empty;

        public string KnowledgeName
        {
            get
            {
                return knowledgeName;
            }
            set
            {
                knowledgeName = value;
                OnPropertyChanged("KnowledgeName");
            }
        }

        public string KnowledgeGuid
        {
            get
            {
                return knowledgeGuid;
            }
            set
            {
                knowledgeGuid = value;
                OnPropertyChanged("KnowledgeGuid");
            }
        }

        public string KnowledgeCode
        {
            get
            {
                return knowledgeCode;
            }
            set
            {
                knowledgeCode = value;
                OnPropertyChanged("KnowledgeCode");
            }
        }
    }
}