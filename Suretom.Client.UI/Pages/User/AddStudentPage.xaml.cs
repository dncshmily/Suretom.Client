using Suretom.Client.Common;
using Suretom.Client.Entity;
using Suretom.Client.IService;
using System;
using System.Collections.Generic;
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
    public partial class AddStudentPage : Window
    {
        /// <summary>
        ///
        /// </summary>
        private IStudentService studentService;

        public bool studentExists = false;

        /// <summary>
        ///
        /// </summary>
        public AddStudentPage()
        {
            InitializeComponent();
            //居中显示
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            studentService = GlobalContext.Resolve<IStudentService>();

            var paperTypeDic = new Dictionary<int, string>();
            paperTypeDic.Add(0, "本科");
            paperTypeDic.Add(1, "函授");
            paperTypeDic.Add(2, "成教");
            CmbStudentType.SelectedValuePath = "Key";
            CmbStudentType.DisplayMemberPath = "Value";
            CmbStudentType.ItemsSource = paperTypeDic;
            CmbStudentType.SelectedIndex = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
        /// 添加学生
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
                StudyType=1
            };

            var result = studentService.AddStudent(student);
            if (result.Success)
            {
                studentExists=result.Success;
                MessageBox.Show("添加成功");
            }
            else
            {
                MessageBox.Show($"添加失败:{result.Message}");
            }

            this.Close();
        }
    }
}