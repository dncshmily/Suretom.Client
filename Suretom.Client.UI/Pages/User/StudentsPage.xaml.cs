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
    /// 学员管理
    /// </summary>
    public partial class StudentsPage : Window
    {
        /// <summary>
        ///
        /// </summary>
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///学生列表
        /// </summary>
        private ObservableCollection<Student> ez_StudentList = new ObservableCollection<Student>();

        /// <summary>
        ///
        /// </summary>
        public bool studentExists = false;

        /// <summary>
        ///
        /// </summary>
        private Student ez_student = null;

        /// <summary>
        ///
        /// </summary>
        private IStudentService studentService;

        /// <summary>
        ///
        /// </summary>
        public StudentsPage()
        {
            InitializeComponent();

            //居中显示
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            studentService = GlobalContext.Resolve<IStudentService>();

            var userInfos = GlobalContext.UserInfo.studentInfos;

            for (int i = 0; i < userInfos.Count; i++)
            {
                var userInfo = userInfos[i];

                userInfo.List.ForEach(s =>
                {
                    ez_StudentList.Add(s);
                });
            }

            dgStudents.DataContext= ez_StudentList;
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
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEditStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var student = button.Tag as Student;

                if (student!=null)
                {
                    EditStudentPage edit = new EditStudentPage();

                    edit.student=student;
                    edit.ShowDialog();

                    if (edit.studentExists)
                    {
                    }
                }
                else
                {
                    MessageBox.Show("111");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        ///删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var student = button.Tag as Student;

                if (student!=null)
                {
                    var result = studentService.DeleteStudent(student.Id);

                    if (result.Success)
                    {
                        studentExists=result.Success;
                        MessageBox.Show("操作成功");
                    }
                    else
                    {
                        MessageBox.Show($"操作失败:{result.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseUpClick(object sender, MouseButtonEventArgs e)
        {
            ez_student= ((System.Windows.Controls.DataGrid)e.Source).CurrentItem as Student;

            if (ez_student != null)
            {
                gb2.IsEnabled = false;
                try
                {
                }
                catch
                {
                }
                gb2.IsEnabled = true;
            }
        }
    }
}