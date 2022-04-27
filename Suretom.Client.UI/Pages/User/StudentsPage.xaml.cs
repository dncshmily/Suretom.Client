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
        public ObservableCollection<Student> ez_StudentList = new ObservableCollection<Student>();

        /// <summary>
        ///
        /// </summary>
        public bool IsClose = false;

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
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var userInfos = GlobalContext.UserInfo.studentInfos;

            for (int i = 0; i < userInfos.Count; i++)
            {
                userInfos[i].List.ForEach(s =>
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("确定是退出吗？", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question);
            ////关闭窗口
            //if (result == MessageBoxResult.Yes)
            //    e.Cancel = false;
            ////不关闭窗口
            //if (result == MessageBoxResult.No)
            //    e.Cancel = true;

            IsClose =true;
            GlobalContext.UserInfo.studentInfos[0].List= ez_StudentList.ToList();
        }

        /// <summary>
        ///编辑
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
                        var students = ez_StudentList.ToList();
                        students[ez_StudentList.IndexOf(student)]=edit.newStudent;

                        ez_StudentList.Clear();

                        foreach (var item in students)
                        {
                            ez_StudentList.Add(item);
                        }
                    }
                }
                else
                {
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
                    ez_StudentList.Remove(student);

                    //var result = studentService.DeleteStudent(student.Id);

                    //if (result.Success)
                    //{
                    //    ez_StudentList.Remove(student);

                    //    studentExists =result.Success;
                    //    MessageBox.Show("操作成功");
                    //}
                    //else
                    //{
                    //    MessageBox.Show($"操作失败:{result.Message}");
                    //}
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