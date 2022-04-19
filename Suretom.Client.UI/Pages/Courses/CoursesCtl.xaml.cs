using Suretom.Client.Entity;
using Suretom.Client.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Suretom.Client.IService;
using Suretom.Client.Common;
using System.Threading.Tasks;
using Suretom.Client.UI.Others;
using Microsoft.Win32;
using Suretom.Client.UI.Pages.User;
using System.Threading;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Threading;
using System.ComponentModel;

/// <summary>
///
/// </summary>
namespace Suretom.Client.UI.Pages.Courses
{
    /// <summary>
    /// DemoWin.xaml 的交互逻辑
    /// </summary>
    public partial class CoursesCtl : UserControl
    {
        /// <summary>
        ///
        /// </summary>
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///
        /// </summary>
        private IUserService userService;

        /// <summary>
        /// 是否停止处理
        /// </summary>
        private bool _isStopDeal = false;

        /// <summary>
        ///
        /// </summary>
        private IStudentService studentService;

        /// <summary>
        ///学生信息
        /// </summary>
        private Student ez_student = new Student();

        /// <summary>
        ///抓取学生信息
        /// </summary>
        private StudentDto ez_studentInfo = new StudentDto();

        /// <summary>
        ///
        /// </summary>
        private ObservableCollection<BatchImportProcessInfo> ez_processInfoList = new ObservableCollection<BatchImportProcessInfo>();

        /// <summary>
        ///学生列表
        /// </summary>
        private ObservableCollection<Student> ez_StudentList = new ObservableCollection<Student>();

        /// <summary>
        ///当前学生课程信息
        /// </summary>
        private ObservableCollection<CourseDto> ez_AllCoursesList = new ObservableCollection<CourseDto>();

        /// <summary>
        ///当前学生课程信息
        /// </summary>
        private ObservableCollection<CourseDto> ez_coursesList = new ObservableCollection<CourseDto>();

        /// <summary>
        ///当前学生-已完成课程信息
        /// </summary>
        private ObservableCollection<CourseDto> ez_CompletedCoursesList = new ObservableCollection<CourseDto>();

        /// <summary>
        ///当前学生-未完成课程信息
        /// </summary>
        private ObservableCollection<CourseDto> ez_UndoneCoursesList = new ObservableCollection<CourseDto>();

        /// <summary>
        /// 当前学生-刷课课程
        /// </summary>
        private ObservableCollection<CourseDto> do_CoursesList = new ObservableCollection<CourseDto>();

        /// <summary>
        /// 学习中的课程
        /// </summary>
        private ObservableCollection<CourseDto> do_AllCoursesList = new ObservableCollection<CourseDto>();

        /// <summary>
        /// 声明CancellationTokenSource对象
        /// </summary>
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();

        /// <summary>
        ///
        /// </summary>
        private CancellationToken token = tokenSource.Token;

        /// <summary>
        ///
        /// </summary>
        private DispatcherTimer ez_timer1;

        /// <summary>
        ///
        /// </summary>
        private DispatcherTimer ez_timer2;

        /// <summary>
        ///
        /// </summary>
        private string strInfo = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public CoursesCtl()
        {
            InitializeComponent();

            //
            userService = GlobalContext.Resolve<IUserService>();
            //
            studentService = GlobalContext.Resolve<IStudentService>();
            //日志信息
            dgProcessInfo.DataContext = ez_processInfoList;
            //未完成课程
            dgCourseInfo.DataContext = ez_UndoneCoursesList;
            //已完成课程
            dgFinishCourseInfo.DataContext =ez_CompletedCoursesList;
            //学习课程列表
            dgCourseList.DataContext = do_CoursesList;
            //
            dgStudents.DataContext = ez_StudentList;

            ez_timer1 = new DispatcherTimer();
            ez_timer1.Interval = TimeSpan.FromMinutes(0.1);
            ez_timer1.Tick += Timer1_Tick;
            ez_timer1.Start();

            //ez_timer2 = new DispatcherTimer();
            //ez_timer2.Interval = TimeSpan.FromMinutes(0.1);
            //ez_timer2.Tick += Timer2_Tick;
            //ez_timer2.Start();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBindStudentList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        #region 事件

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
            var student = ((System.Windows.Controls.DataGrid)e.Source).CurrentItem as Student;

            if (student != null)
            {
                DataBindStudentCourses(student);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDropDownClosed_CmbSemester(object sender, EventArgs e)
        {
            if (CmbSchool.IsDropDownOpen == true)
            {
                var s = CmbSemester.Text;
            }
        }

        /// <summary>
        /// CmbSchool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDropDownClosed_CmbSchool(object sender, EventArgs e)
        {
            if (CmbSchool.IsDropDownOpen == false)
            {
                DataBindStudentList(string.IsNullOrEmpty(CmbSchool.Text) ? "" : CmbSchool.Text);
            }
        }

        #endregion

        #region 按钮

        /// <summary>
        ///添加学员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddStudentPage addStudentPage = new AddStudentPage();

                addStudentPage.ShowDialog();

                if (addStudentPage.studentExists)
                {
                    GlobalContext.UserInfo.studentInfos = studentService.GetStudentList();

                    DataBindStudentList();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Sp1.IsEnabled = true;
            }
        }

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBatchImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Sp1.IsEnabled = false;

                var dialog = new OpenFileDialog()
                {
                    DefaultExt = ".xlsx",
                    Filter = "Excel文件|*.xlsx;",
                    Multiselect = true
                };

                if (dialog.ShowDialog() == true)
                {
                    var fileNames = dialog.FileNames;

                    foreach (var item in fileNames)
                    {
                        var ext = System.IO.Path.GetExtension(item);

                        if (ext.ToLower() != ".xlsx")
                        {
                            MessageBox.Show($"不支持{ext.ToLower() }的后缀名！");
                            return;
                        }
                    }

                    //var exSucess = new List<EXMessageDto>();
                    var sucessCount = 0;

                    List<Student> addStudents = new List<Student>();
                    foreach (var fileName in fileNames)
                    {
                        var dataTable = new ExcelHelper(fileName).ExcelToDataTable("Sheet1", true);

                        var studentList = JsonHelper.DeserializeObject<List<Student>>(dataTable.ToJson());

                        //学生信息
                        studentList.ForEach(s =>
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(s.IdCard) && s.IdCard.Length > 7)
                                {
                                    var student = new Student()
                                    {
                                        IdCard = s.IdCard,
                                        MoviePwd = s.IdCard.Substring(s.IdCard.Length - 6),
                                    };

                                    ez_studentInfo = new CoursesData(student).GetStudentInfo();

                                    if (ez_studentInfo != null)
                                    {
                                        student.ClassName = ez_studentInfo.info.ClassName;
                                        student.SchoolName = ez_studentInfo.info.SchoolName;
                                        student.StudyType = LearnTypeConverter(ez_studentInfo.info.LearnType);
                                        student.StudyCode = ez_studentInfo.info.StudentNumber;
                                        student.StudentName = ez_studentInfo.info.DisplayName.ToString();
                                        addStudents.Add(student);
                                    }
                                    else
                                    {
                                        string ex = $"新增学生:{s.StudentName}_{s.IdCard} 失败，跳过";
                                        AddProcessError(ex);
                                        log.Error(ex);
                                    }
                                }
                                else
                                {
                                    AddProcessError($"新增学生:{s.StudentName}_{s.IdCard} 失败，数据格式错误");
                                }
                            }
                            catch (Exception ex)
                            {
                                AddProcessError($"新增学生:{s.StudentName}_{s.IdCard} 异常，{ex.Message}");
                                throw;
                            }
                        });
                    }

                    addStudents.ForEach(s =>
                    {
                        var result = studentService.AddStudent(s);
                        if (!result.Success)
                        {
                            sucessCount++;
                            AddProcessError($"新增学生:{s.StudentName}_{s.IdCard}失败，{result.Message}");
                        }
                        else
                        {
                            AddProcessInfo($"新增学生:{s.StudentName}_{s.IdCard}成功");
                        }
                    });

                    if (sucessCount > 0)
                    {
                        GlobalContext.UserInfo.studentInfos = studentService.GetStudentList();

                        DataBindStudentList();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                AddProcessError(ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Sp1.IsEnabled = true;
            }
        }

        /// <summary>
        /// 开始学习-智能刷课
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAutoStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("是否确定开始学习？", "提示", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    if (CurrentStatus == BatchImportStatus.初始化失败)
                    {
                        MessageBox.Show("初始化失败");
                        return;
                    }
                    if (do_AllCoursesList.Count(f => f.Status == 0) > 0)
                    {
                        if (!_isStopDeal)
                        {
                            AddProcessInfo($"————————开始学习————————");

                            AddProcessInfo($"数据初始化...");

                            _isStopDeal = true;
                        }

                        OperationBtnEnable(false);
                        CurrentStatus = BatchImportStatus.手动处理;
                        OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
                    }
                    else
                    {
                        AddProcessError($"请选择要学习的学生！");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 停止学习-自动处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAutoStopClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("是否确定停止学习？", "提示", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    if (CurrentStatus == BatchImportStatus.初始化失败)
                    {
                        MessageBox.Show("初始化失败");
                        return;
                    }

                    CurrentStatus = BatchImportStatus.手动处理;
                    OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));

                    await StopMyTask();

                    _isStopDeal = false;

                    CurrentStatus = BatchImportStatus.成功;
                    OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///学员管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BtnStudentManageClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        //DoWork
                    }
                    catch (Exception inEx)
                    {
                        log.Error(inEx);
                        MessageBox.Show(inEx.Message);
                    }
                }).ContinueWith(t =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                    });
                });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///个人中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BtnPersonCenterClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        //DoWork
                    }
                    catch (Exception inEx)
                    {
                        log.Error(inEx);
                        MessageBox.Show(inEx.Message);
                    }
                }).ContinueWith(t =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                    });
                });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var name = ((FrameworkElement)sender).Name;

            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (CurrentStatus == BatchImportStatus.初始化失败)
                {
                    MessageBox.Show("初始化失败");
                    return;
                }

                CurrentStatus = BatchImportStatus.手动处理;
                OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));

                //var doCourses = do_CoursesList.FirstOrDefault(f => f.course.Status==0);

                //if (doCourses != null)
                //{
                //    var result = await SigneCourseStudy(doCourses);

                //    if (result)
                //    {
                //        CurrentStatus = BatchImportStatus.成功;
                //        OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
                //    }
                //    else
                //    {
                //        CurrentStatus = BatchImportStatus.失败;
                //        OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
                //    }
                //}

                //MessageBox.Show("双击");
                e.Handled = true;
            }
        }

        /// <summary>
        /// 单课程添加-刷课
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddCourse_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var course = button.Tag as CourseDto;

            if (course!=null)
            {
                if (do_CoursesList.Count(f => f.Student.IdCard == ez_student.IdCard && f.CourseOpenId == course.CourseOpenId&&f.CourseName==course.CourseName)==0)
                {
                    do_CoursesList.Add(course);

                    AddProcessInfo($"添加刷课—{course.CourseName} 成功");
                }
                else
                {
                    MessageBox.Show("课程已存在，不可重复添加");
                }
            }
        }

        /// <summary>
        /// 一键添加-刷课
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddCourses_Click(object sender, RoutedEventArgs e)
        {
            AddProcessInfo($"一键添加刷课...");

            //当前学生-未完成课程信息
            foreach (var course in ez_UndoneCoursesList)
            {
                if (do_CoursesList.Count(f => f.Student.IdCard == ez_student.IdCard && f.CourseOpenId == course.CourseOpenId&&f.CourseName==course.CourseName)==0)
                {
                    do_CoursesList.Add(course);

                    AddProcessInfo($"添加刷课—{course.CourseName} 成功");
                }
            }
        }

        /// <summary>
        /// 单课程-开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnCourseStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("是否确定开始学习？", "提示", MessageBoxButton.OKCancel);

                if (CurrentStatus == BatchImportStatus.初始化失败)
                {
                    MessageBox.Show("初始化失败");
                    return;
                }
                var button = sender as Button;
                var course = button.Tag as CourseDto;

                if (course != null)
                {
                    button.IsEnabled=false;

                    if (do_AllCoursesList.Count(f => f.Student.IdCard == ez_student.IdCard && f.CourseOpenId == course.CourseOpenId&& f.CourseName == course.CourseName)==0)
                    {
                        course.Status=0;

                        do_AllCoursesList.Add((course));
                    }

                    try
                    {
                        var idx = do_AllCoursesList.IndexOf(course);

                        AddProcessInfo($"{strInfo}-开始学习{course.CourseName}");

                        await Task.Run(() =>
                        {
                            AddProcessInfo($"{strInfo}-{course.CourseName}-学习中...");

                            OperationBtnEnable(false);

                            this.Dispatcher.Invoke(() =>
                            {
                                do_AllCoursesList[idx].Status = 1;
                            });

                            //开始学习
                            new CoursesData(course.Student).SingeSyudentStart(course);
                        }, course.Token).ContinueWith(t =>
                        {
                            try
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    do_AllCoursesList[idx].Status = 2;
                                });
                            }
                            catch (Exception inEx)
                            {
                                log.Error(inEx);
                            }
                        });

                        AddProcessInfo($"{strInfo}-{course.CourseName}-学习结束");
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        AddProcessError($"{strInfo}-{course.CourseName}-{ex.Message}");
                    }
                    finally
                    {
                        button.IsEnabled=true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 一键开始-当前学生-刷课
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCoursesStart_Click(object sender, RoutedEventArgs e)
        {
            AddProcessInfo($"一键开始刷课...");

            //当前学生-待刷课课程
            var studenCourses = do_CoursesList.Where(c => c.Student.IdCard==ez_student.IdCard&&c.Student.StudentName==ez_student.StudentName&&c.Schedule<100).ToList();

            foreach (var course in studenCourses)
            {
                if (do_AllCoursesList.Count(f => f.Student.IdCard == ez_student.IdCard && f.CourseOpenId == course.CourseOpenId)==0)
                {
                    course.Status=0;

                    do_AllCoursesList.Add((course));

                    AddProcessInfo($"开始刷课—{course.CourseName} 启动成功");
                }
            };
        }

        /// <summary>
        /// 单课程-暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCoursePause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var course = button.Tag as CourseDto;

                if (course != null)
                {
                    button.IsEnabled = false;

                    course.TokenSource.Cancel();

                    button.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 一键停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCoursesStop_Click(object sender, RoutedEventArgs e)
        {
            AddProcessInfo($"一键停止刷课...");

            //当前学生-未完成课程
            var studenCourses = do_CoursesList.Where(c => c.Student.IdCard==ez_student.IdCard&&c.Student.StudentName==ez_student.StudentName&&c.Schedule<100).ToList();

            foreach (var course in studenCourses)
            {
                var do_AllCourses = do_AllCoursesList.FirstOrDefault(f => f.CourseName==course.CourseName&&f.Student.IdCard==course.Student.IdCard);

                if (do_AllCourses!=null)
                {
                    course.TokenSource.Cancel();

                    AddProcessInfo($"停止刷课—{course.CourseName} 成功");
                }
            };
        }

        #endregion 按钮

        #region Timer

        /// <summary>
        ///Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer1_Tick(object sender, EventArgs e)
        {
            var doCoursesList = do_AllCoursesList.Where(f => f.Status == 0).ToList();

            if (doCoursesList.Count > 0)
            {
                try
                {
                    foreach (var course in doCoursesList)
                    {
                        await SigneCourseStudy(course);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.Sp1.IsEnabled = true;
                }
            }
            else
            {
                OperationBtnEnable(true);

                this.Dispatcher.Invoke(() =>
                {
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer2_Tick(object sender, EventArgs e)
        {
            var studenCourses = do_AllCoursesList.Where(c => c.Student.IdCard==ez_student.IdCard&&c.Student.StudentName==ez_student.StudentName).ToList();

            if (studenCourses.Count > 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    do_CoursesList.Clear();

                    studenCourses.ForEach(c =>
                    {
                        do_CoursesList.Add(c);
                    });
                });
            }
        }

        #endregion

        #region DoWork

        /// <summary>
        ///切换学生绑定
        /// </summary>
        /// <param name="student"></param>
        public void DataBindStudentList(string schoolName = "")
        {
            var userInfos = GlobalContext.UserInfo.studentInfos;

            var cmbSchoolDic = new Dictionary<int, string>();
            var cmbSemesterDic = new Dictionary<int, string>();

            int selectIndex = 0;

            if (!string.IsNullOrEmpty(schoolName))
            {
                selectIndex = CmbSchool.SelectedIndex;
                userInfos= userInfos.Where(f => f.SchoolName==schoolName).ToList();
            }

            ez_StudentList.Clear();

            //CmbSemester.Items.Clear();

            for (int i = 0; i < userInfos.Count; i++)
            {
                var userInfo = userInfos[i];

                userInfo.List.ForEach(s =>
                {
                    ez_StudentList.Add(s);
                });

                if (!cmbSchoolDic.ContainsValue(schoolName))
                {
                    cmbSchoolDic.Add(i, userInfo.SchoolName);
                }
            }

            CmbSchool.SelectedValuePath = "Key";
            CmbSchool.DisplayMemberPath = "Value";
            CmbSchool.ItemsSource = cmbSchoolDic;
            CmbSchool.SelectedIndex = selectIndex;

            CmbSemester.SelectedValuePath = "Key";
            CmbSemester.DisplayMemberPath = "Value";
            CmbSemester.ItemsSource = cmbSemesterDic;
            CmbSemester.SelectedIndex = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="student"></param>
        public void DataBindStudentCourses(Student student)
        {
            do_CoursesList.Clear();
            ez_coursesList.Clear();

            ez_student = student;

            strInfo = $"{ez_student.SchoolName}-{ez_student.StudentName}-{ez_student.IdCard}";

            labNane.Content = ez_student.StudentName;
            labIdCard.Content = ez_student.IdCard;
            labNo.Content = ez_student.StudyCode;
            labClass.Content = ez_student.MoviePwd;
            labIdType.Content = StudyTypeConverter(ez_student.StudyType);

            //课程信息
            var studentCourses = ez_AllCoursesList.Where(f => f.Student.IdCard==student.IdCard).ToList();

            if (studentCourses.Count>0)
            {
                foreach (var course in studentCourses)
                {
                    ez_coursesList.Add(course);
                }
            }
            else
            {
                ez_coursesList = new CoursesData(student).GetCourseInfoList();

                foreach (var course in ez_coursesList)
                {
                    //所有学生课程
                    if (ez_AllCoursesList.Count(f => f.Student.IdCard==student.IdCard&&f.CourseName==course.CourseName)==0)
                    {
                        ez_AllCoursesList.Add(course);
                    }
                }
            }

            for (int i = 0; i < ez_coursesList.Count; i++)
            {
                var time = ez_coursesList[i].ExpiredTime.Contains("Date") ? UtilityHelper.ToConvertTime(ez_coursesList[i].ExpiredTime).ToString() : ez_coursesList[i].ExpiredTime;
                ez_coursesList[i].ExpiredTime = time;
                ez_coursesList[i].Id = i+1;
            }

            ObservableHelper.ObservableMySort(ez_coursesList, 1);

            //未完成的课程
            var coursesList = ez_coursesList.Where(f => f.Schedule < 100).ToList();

            if (ez_coursesList != null && ez_coursesList.Count > 0)
            {
                ez_UndoneCoursesList.Clear();
                ez_CompletedCoursesList.Clear();

                foreach (var courses in ez_coursesList)
                {
                    if (courses.Schedule < 100)
                    {
                        //未完成课程
                        ez_UndoneCoursesList.Add(courses);
                    }
                    else
                    {
                        //已完成课程
                        ez_CompletedCoursesList.Add(courses);
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dgrStr"></param>
        /// <returns></returns>
        public async Task<bool> SigneCourseStudy(CourseDto course)
        {
            try
            {
                var idx = do_AllCoursesList.IndexOf(course);

                AddProcessInfo($"{strInfo}-开始学习{course.CourseName}");

                await Task.Run(() =>
                {
                    AddProcessInfo($"{strInfo}-学习中...");

                    OperationBtnEnable(false);

                    this.Dispatcher.Invoke(() =>
                    {
                        do_AllCoursesList[idx].Status = 1;
                    });

                    //开始学习
                    new CoursesData(course.Student).SingeSyudentStart(course);
                }, course.Token).ContinueWith(t =>
                {
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            do_AllCoursesList[idx].Status = 2;
                            OperationBtnEnable(true);
                        });
                    }
                    catch (Exception inEx)
                    {
                        log.Error(inEx);
                    }
                });

                AddProcessInfo($"{strInfo}学习结束");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                AddProcessError($"{strInfo}{ex.Message}");
                return false;
            }
            finally
            {
                OperationBtnEnable(true);
            }
            return true;
        }

        /// <summary>
        ///全部停止
        /// </summary>
        /// <param name="dgrStr"></param>
        /// <returns></returns>
        public async Task<bool> StopMyTask()
        {
            try
            {
                AddProcessError($"{strInfo}停止学习");

                foreach (var course in do_AllCoursesList)
                {
                    course.TokenSource.Cancel();
                }

                if (ez_timer1 != null)
                {
                    ez_timer1.Stop();
                }

                this.Dispatcher.Invoke(() =>
                {
                    ez_coursesList.Clear();
                    do_AllCoursesList.Clear();
                });

                OperationBtnEnable(true);

                AddProcessError($"{strInfo}学习结束");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                AddProcessError($"{ex.Message}");
                return false;
            }
            finally
            {
                OperationBtnEnable(true);
            }
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="studyType"></param>
        /// <returns></returns>
        public string StudyTypeConverter(int studyType)
        {
            switch (studyType)
            {
                case 0:
                    return LearnTypeEnum.本科.ToString();

                case 1:
                    return LearnTypeEnum.函授.ToString();

                case 2:
                    return LearnTypeEnum.成教.ToString();

                default:
                    return LearnTypeEnum.其它.ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="learnTypeEnum"></param>
        /// <returns></returns>
        public int LearnTypeConverter(string learnType)
        {
            switch (learnType)
            {
                case "本科":
                    return LearnTypeEnum.本科.GetHashCode();

                case "函授":
                    return LearnTypeEnum.函授.GetHashCode();

                case "成教":
                    return LearnTypeEnum.成教.GetHashCode();

                default:
                    return LearnTypeEnum.其它.GetHashCode();
            }
        }

        #endregion

        #region 更新状态

        /// <summary>
        /// 当前的处理状态
        /// </summary>
        public BatchImportStatus CurrentStatus
        {
            get; private set;
        }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler<StatusChangeEventArgs> StatusChangeEvent;

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        private void OnStatusChangeEvent(StatusChangeEventArgs args)
        {
            this.StatusChangeEvent?.Invoke(this, args);
        }

        /// <summary>
        /// 操作按钮是否可用
        /// </summary>
        /// <param name="isEnable"></param>
        private void OperationBtnEnable(bool isEnable)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    Sp1.IsEnabled = isEnable;
                    BtnAutoStart.IsEnabled = isEnable;
                    gb1.IsEnabled = isEnable;
                });
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        private void AddProcessError(string info)
        {
            AddProcessMsg(info, BatchImportProcessInfoType.错误);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        private void AddProcessInfo(string info)
        {
            AddProcessMsg(info, BatchImportProcessInfoType.信息);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        private void AddProcessWarn(string info)
        {
            AddProcessMsg(info, BatchImportProcessInfoType.警告);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        private void AddProcessMsg(string msg, BatchImportProcessInfoType type)
        {
            this.Dispatcher.Invoke(() =>
            {
                ez_processInfoList.Add(new BatchImportProcessInfo
                {
                    Id = ez_processInfoList.Count,
                    Info = msg,
                    Type = type
                });

                ObservableHelper.ObservableMySort(ez_processInfoList);
            });
        }

        #endregion
    }
}