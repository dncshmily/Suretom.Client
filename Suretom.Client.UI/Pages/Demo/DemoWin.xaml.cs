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

namespace Suretom.Client.UI.Pages.Demo
{
    /// <summary>
    /// DemoWin.xaml 的交互逻辑
    /// </summary>
    public partial class DemoWin : UserControl
    {
        /// <summary>
        ///
        /// </summary>
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///
        /// </summary>
        private ObservableCollection<BatchImportInfo> _batchImprotInfoList = new ObservableCollection<BatchImportInfo>();

        /// <summary>
        ///
        /// </summary>
        private ObservableCollection<BatchImportProcessInfo> ez_processInfoList = new ObservableCollection<BatchImportProcessInfo>();

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
        ///当前学生课程信息
        /// </summary>
        private List<CourseDto> ez_coursesList = new List<CourseDto>();

        /// <summary>
        /// 学习中的课程
        /// </summary>
        private List<CoursesInfo> do_coursesList = new List<CoursesInfo>();

        /// <summary>
        /// 试卷里解析出的题目
        /// </summary>
        private ObservableCollection<CourseDto> ez_courseDtoList = new ObservableCollection<CourseDto>();

        /// <summary>
        ///
        /// </summary>
        public class CoursesInfo
        {
            /// <summary>
            ///
            /// </summary>
            public Guid guid { get; set; } = Guid.NewGuid();

            /// <summary>
            ///学生信息
            /// </summary>
            public Student student { get; set; } = new Student();

            /// <summary>
            ///课程信息
            /// </summary>
            public CourseDto course = new CourseDto();
        }

        /// <summary>
        /// 声明CancellationTokenSource对象
        /// </summary>
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();

        /// <summary>
        ///
        /// </summary>
        private string strInfo = string.Empty;

        /// <summary>
        ///
        /// </summary>
        private CancellationToken token = tokenSource.Token;

        /// <summary>
        ///
        /// </summary>
        private DispatcherTimer ez_timer;

        /// <summary>
        ///
        /// </summary>
        public DemoWin()
        {
            InitializeComponent();

            userService = GlobalContext.Resolve<IUserService>();
            studentService = GlobalContext.Resolve<IStudentService>();
            dgProcessInfo.DataContext = ez_processInfoList;
        }

        #region MyRegion

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                FillTreeView(GlobalContext.UserInfo.studentInfos);
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
        /// <param name="studyType"></param>
        /// <returns></returns>
        public string StudyTypeConverter(int studyType)
        {
            switch (studyType)
            {
                case 0:
                    return LearnTypeEnum.本科.ToString();

                    break;

                case 1:
                    return LearnTypeEnum.函授.ToString();

                    break;

                case 2:
                    return LearnTypeEnum.成教.ToString();
                    break;

                default:
                    return LearnTypeEnum.其它.ToString();

                    break;
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
                    break;

                case "函授":
                    return LearnTypeEnum.函授.GetHashCode();

                    break;

                case "成教":
                    return LearnTypeEnum.成教.GetHashCode();
                    break;

                default:
                    return LearnTypeEnum.其它.GetHashCode();
                    break;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="student"></param>
        public void StudentDataBind(Student student)
        {
            ez_student = student;

            strInfo = $"{ez_student.SchoolName}-{ez_student.StudentName}-{ez_student.IdCard}";

            labNane.Content = ez_student.StudentName;
            labIdCard.Content = ez_student.IdCard;
            labNo.Content = ez_student.StudyCode;
            labClass.Content = ez_student.MoviePwd;
            labIdType.Content = StudyTypeConverter(ez_student.StudyType);

            //课程信息
            ez_coursesList = new CoursesData(ez_student).GetCourseList();

            //未完成的课程
            var coursesList = ez_coursesList.Where(f => f.Schedule < 100).ToList();

            coursesList.ForEach(course =>
            {
                if (!do_coursesList.Exists(f => f.student.IdCard == ez_student.IdCard && f.course.CourseOpenId == course.CourseOpenId))
                {
                    do_coursesList.Add(new CoursesInfo()
                    {
                        student = ez_student,
                        course = course
                    });
                }
            });

            if (ez_coursesList != null && ez_coursesList.Count > 0)
            {
                //未完成课程
                dgCourseInfo.DataContext = ez_coursesList.Where(f => f.Schedule < 100).ToList();

                //已完成课程
                dgFinishCourseInfo.DataContext = ez_coursesList.Where(f => f.Schedule == 100).ToList();

                //课程列表
                dgCourseList.DataContext = ez_coursesList;

                ez_coursesList.ForEach(course =>
                {
                    ez_courseDtoList.Add(course);
                });

                int i = 0;

                ez_coursesList = ez_coursesList.OrderBy(f => f.Completed).ToList();

                ez_coursesList.ForEach(f =>
                {
                    f.ExpiredTime = UtilityHelper.ToConvertTime(f.ExpiredTime).ToString();

                    i++;
                    f.Id = i;
                });

                dgStudents.DataContext = ez_coursesList;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dgrStr"></param>
        /// <returns></returns>
        public async Task<bool> SigneCourseStart(CoursesInfo info)
        {
            try
            {
                var idx = do_coursesList.IndexOf(info);

                AddProcessInfo($"{strInfo}-开始学习{info.course.CourseName}");

                await Task.Run(() =>
                {
                    AddProcessInfo($"{strInfo}-学习中...");

                    OperationBtnEnable(false);

                    do_coursesList[idx].course.Status = 1;

                    this.Dispatcher.Invoke(() =>
                    {
                        labFinish.Content = do_coursesList.Count(f => f.course.Status == 2);
                        pbProcess.Value = do_coursesList.Count(f => f.course.Status == 2);
                    });

                    //开始学习
                    new CoursesData(info.student).SingeSyudentStart(info.course);
                }, token).ContinueWith(t =>
                {
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            do_coursesList[idx].course.Status = 2;
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
        ///
        /// </summary>
        /// <param name="dgrStr"></param>
        /// <returns></returns>
        public async Task<bool> StopMyTask()
        {
            try
            {
                AddProcessError($"{strInfo}停止学习");

                tokenSource.Cancel();
                if (ez_timer != null)
                {
                    ez_timer.Stop();
                }

                labFinish.Content = 0;
                labTotal.Content = 0;
                ez_coursesList = new List<CourseDto>();
                do_coursesList = new List<CoursesInfo>();

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

        #endregion

        #region 按钮

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer1_Tick(object sender, EventArgs e)
        {
            var doCoursesList = do_coursesList.Where(f => f.course.Status == 0).ToList();

            labTitle.Visibility = Visibility.Visible;
            labTotal.Content = do_coursesList.Count;
            pbProcess.Maximum = do_coursesList.Count;

            if (doCoursesList.Count > 0)
            {
                try
                {
                    foreach (var course in doCoursesList)
                    {
                        await SigneCourseStart(course);
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
            AddProcessInfo($"数据初始化...");
        }

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

                    FillTreeView(GlobalContext.UserInfo.studentInfos);
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

                        FillTreeView(GlobalContext.UserInfo.studentInfos);
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
        /// 开始学习-自动处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAutoStart_Click(object sender, RoutedEventArgs e)
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
                    if (do_coursesList.Count(f => f.course.Status == 0) > 0)
                    {
                        if (!_isStopDeal)
                        {
                            AddProcessInfo($"————————开始学习————————");

                            AddProcessInfo($"数据初始化...");

                            _isStopDeal = true;
                        }

                        ez_timer = new DispatcherTimer();
                        ez_timer.Interval = TimeSpan.FromMinutes(0.1);
                        ez_timer.Tick += Timer1_Tick;
                        ez_timer.Start();

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
        /// 开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnItemStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var wordItem = button.Tag as CourseDto;
                if (wordItem != null)
                {
                    if (!wordItem.IsStartSuccess)
                    {
                        MessageBox.Show("111");

                        return;
                    }

                    button.IsEnabled = false;

                    Task.Run(() =>
                    {
                        try
                        {
                            //
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
                            button.IsEnabled = true;
                        });
                    });
                }

                ez_timer = new DispatcherTimer();
                ez_timer.Interval = TimeSpan.FromMinutes(0.1);
                ez_timer.Tick += Timer2_Tick;
                ez_timer.Start();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnItemPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var wordItem = button.Tag as CourseDto;
                if (wordItem != null)
                {
                    if (!wordItem.IsPauseSuccess)
                    {
                        MessageBox.Show("切图失败，不能上传");

                        return;
                    }

                    button.IsEnabled = false;

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
                            button.IsEnabled = true;
                        });
                    });
                }
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
        private async void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
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

                //var doCourses = do_coursesList.FirstOrDefault(f => f.course.Status==0);

                //if (doCourses != null)
                //{
                //    var result = await SigneCourseStart(doCourses);

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

        #endregion 按钮

        #region 更新状态

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

        #region TreeView Event Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem selItem = treeView.SelectedItem as TreeViewItem;
            if (selItem == null || selItem.Tag == null)
            {
                return;
            }

            var student = selItem.Tag as Student;

            if (student == null)
            {
                return;
            }

            e.Handled = true;
            treeView.IsEnabled = false;

            try
            {
                StudentDataBind(student);

                this.Cursor = Cursors.Wait;
                this.ForceCursor = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                this.ForceCursor = false;

                treeView.IsEnabled = true;
                treeView.Focus();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewItemUnselected(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewItemCollapsed(object sender, RoutedEventArgs e)
        {
            if (_folderClose == null)
            {
                return;
            }

            TreeViewItem treeItem = e.OriginalSource as TreeViewItem;
            if (treeItem == null || (treeItem.Tag != null
                && !string.IsNullOrWhiteSpace(treeItem.Tag.ToString())))
            {
                return;
            }

            BulletDecorator decorator = treeItem.Header as BulletDecorator;
            if (decorator == null)
            {
                return;
            }
            Image headerImage = decorator.Bullet as Image;
            if (headerImage == null)
            {
                return;
            }
            headerImage.Source = _folderClose;

            e.Handled = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            if (_folderOpen == null)
            {
                return;
            }

            TreeViewItem treeItem = e.OriginalSource as TreeViewItem;
            if (treeItem == null || (treeItem.Tag != null
                && !string.IsNullOrWhiteSpace(treeItem.Tag.ToString())))
            {
                return;
            }

            BulletDecorator decorator = treeItem.Header as BulletDecorator;
            if (decorator == null)
            {
                return;
            }
            Image headerImage = decorator.Bullet as Image;
            if (headerImage == null)
            {
                return;
            }
            headerImage.Source = _folderOpen;

            e.Handled = true;
        }

        #endregion

        #region FillTreeView Methods

        private ImageSource _folderClose;
        private ImageSource _folderOpen;
        private ImageSource _fileThumbnail;

        /// <summary>
        ///
        /// </summary>
        /// <param name="list"></param>
        private void FillTreeView(List<StudentInfo> studentInfos)
        {
            var cmbDic = new Dictionary<int, string>();

            treeView.BeginInit();
            treeView.Items.Clear();
            int i = 0;

            studentInfos.ForEach(student =>
            {
                cmbDic.Add(i, student.SchoolName);

                TextBlock headerText = new TextBlock();
                headerText.Text = student.SchoolName;
                headerText.Margin = new Thickness(3, 0, 0, 0);

                BulletDecorator decorator = new BulletDecorator();
                if (_folderClose != null)
                {
                    Image image = new Image();
                    image.Source = _folderClose;
                    decorator.Bullet = image;
                }
                else
                {
                    Ellipse bullet = new Ellipse();
                    bullet.Height = 10;
                    bullet.Width = 10;
                    bullet.Fill = Brushes.LightSkyBlue;
                    bullet.Stroke = Brushes.DarkGray;
                    bullet.StrokeThickness = 1;

                    decorator.Bullet = bullet;
                }
                decorator.Margin = new Thickness(0, 0, 10, 0);
                decorator.Child = headerText;
                decorator.Tag = string.Empty;

                TreeViewItem categoryItem = new TreeViewItem();
                categoryItem.Tag = string.Empty;
                categoryItem.Header = decorator;
                categoryItem.Margin = new Thickness(0);
                categoryItem.Padding = new Thickness(3);
                categoryItem.FontSize = 14;
                categoryItem.FontWeight = FontWeights.Bold;

                treeView.Items.Add(categoryItem);

                FillTreeView(student, categoryItem);

                categoryItem.IsExpanded = (i == 0);
                i++;
            });

            treeView.EndInit();

            CmbSchool.SelectedValuePath = "Key";
            CmbSchool.DisplayMemberPath = "Value";
            CmbSchool.ItemsSource = cmbDic;
            CmbSchool.SelectedIndex = 0;
        }

        private int studentCount = 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="studentInfo"></param>
        /// <param name="treeItem"></param>
        private void FillTreeView(StudentInfo studentInfo, TreeViewItem treeItem)
        {
            int itemCount = 0;

            studentInfo.List.ForEach(student =>
            {
                //if (studentCount == 0)
                //{
                //    StudentDataBind(student);
                //}
                TextBlock itemText = new TextBlock();
                itemText.Text = $"{student.StudentName}";
                itemText.Margin = new Thickness(3, 0, 0, 0);

                BulletDecorator fileItem = new BulletDecorator();
                if (_fileThumbnail != null)
                {
                    Image image = new Image();
                    image.Source = _fileThumbnail;
                    image.Height = 16;
                    image.Width = 16;

                    fileItem.Bullet = image;
                }
                else
                {
                    Ellipse bullet = new Ellipse();
                    bullet.Height = 10;
                    bullet.Width = 10;
                    bullet.Fill = Brushes.Goldenrod;
                    bullet.Stroke = Brushes.DarkGray;
                    bullet.StrokeThickness = 1;

                    fileItem.Bullet = bullet;
                }
                fileItem.Margin = new Thickness(0, 0, 10, 0);
                fileItem.Child = itemText;

                TreeViewItem item = new TreeViewItem();
                item.Tag = student;
                item.Header = fileItem;
                item.Margin = new Thickness(0);
                item.Padding = new Thickness(2);
                item.FontSize = 12;
                item.FontWeight = FontWeights.Normal;

                treeItem.Items.Add(item);

                studentCount++;
                itemCount++;
            });
        }

        #endregion
    }

    /// <summary>
    ///
    /// </summary>
    public class BatchImportProcessInfoConverter : IValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var infoType = (BatchImportProcessInfoType)value;
            switch (infoType)
            {
                case BatchImportProcessInfoType.信息:
                    return Brushes.Black;

                case BatchImportProcessInfoType.警告:
                    return Brushes.Green;

                case BatchImportProcessInfoType.错误:
                    return Brushes.Red;

                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}