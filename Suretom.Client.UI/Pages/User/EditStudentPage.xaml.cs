using Suretom.Client.Common;
using Suretom.Client.Entity;
using Suretom.Client.IService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Suretom.Client.UI.Pages.User
{
    /// <summary>
    /// AddStudentPage.xaml 的交互逻辑
    /// </summary>
    public partial class EditStudentPage : Window
    {
        /// <summary>
        ///
        /// </summary>
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///
        /// </summary>
        private IStudentService studentService;

        /// <summary>
        ///
        /// </summary>
        public bool studentExists = false;

        /// <summary>
        ///
        /// </summary>
        public Student student = null;

        /// <summary>
        ///
        /// </summary>
        public EditStudentPage()
        {
            InitializeComponent();
            //居中显示
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            studentService = GlobalContext.Resolve<IStudentService>();

            CmbStudentType.SelectedValuePath = "Key";
            CmbStudentType.DisplayMemberPath = "Value";
            CmbStudentType.ItemsSource =new Dictionary<int, string>() { { 0, "本科" }, { 1, "函授" }, { 2, "成教" } };
            CmbStudentType.SelectedIndex = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (student!=null)
            {
                txtStudentName.Text=student.StudentName;
                txtidCard.Text=student.IdCard;
                txtmoviePwd.Password=student.MoviePwd;
                txtStudyCode.Text   =student.StudyCode;
                txtSchoollName.Text =   student.SchoolName;
                txtClassName.Text = student.ClassName;
                CmbStudentType.SelectedIndex =student.StudyType;
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Student student = new Student()
            {
                SchoolName =txtSchoollName.Text,
                IdCard=txtidCard.Text,
                MoviePwd=txtmoviePwd.Password.Trim(),
                StudyCode=txtStudyCode.Text,
                StudentName=txtStudentName.Text,
                ClassName=txtClassName.Text,
                StudyType=CmbStudentType.SelectedIndex
            };

            var result = studentService.AddStudent(student);
            if (result.Success)
            {
                studentExists=result.Success;
                MessageBox.Show("保存成功");
            }
            else
            {
                MessageBox.Show($"保存失败:{result.Message}");
            }

            this.Close();
        }
    }
}