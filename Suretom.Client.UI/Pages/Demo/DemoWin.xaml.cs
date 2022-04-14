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
        private IUserService userService;

        /// <summary>
        /// 是否停止处理
        /// </summary>
        private bool _isStopDeal = false;

        /// <summary>
        ///
        /// </summary>
        private DemoData demoData;

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
        ///学生课程信息
        /// </summary>
        private List<CourseDto> ez_coursesList = new List<CourseDto>();

        //声明CancellationTokenSource对象
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
        public DemoWin()
        {
            InitializeComponent();

            userService = GlobalContext.Resolve<IUserService>();
            studentService = GlobalContext.Resolve<IStudentService>();
            dgProcessInfo.DataContext = _processInfoList;
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
                strInfo = $"{ez_student.SchoolName}-{ez_student.ClassName}-{ez_student.StudentName}-";
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

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
        public void StudentDataBind()
        {
            labNane.Content = ez_student.StudentName;
            labIdCard.Content = ez_student.IdCard;
            labNo.Content = ez_student.StudyCode;
            labClass.Content = ez_student.MoviePwd;
            labIdType.Content = StudyTypeConverter(ez_student.StudyType);

            demoData = new DemoData(ez_student);
            //课程信息
            ez_coursesList= demoData.GetCourseList().List;
            //学生信息
            //ez_studentInfo = demoData.GetStudentInfo();

            if (ez_coursesList!=null&&ez_coursesList.Count>0)
            {
                dgCourseInfo.DataContext = ez_coursesList;

                int i = 0;

                ez_coursesList.ForEach(f =>
                {
                    f.ExpiredTime = UtilityHelper.ToConvertTime(f.ExpiredTime).ToString();

                    switch (i)
                    {
                        case 0:
                            f.imgStr = "imgA";
                            dgrA.Visibility = Visibility.Visible;
                            labA1.Content = f.CourseName;
                            labA2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labA3.Content = f.ExpiredTime;
                            pbA.Value = f.Schedule;
                            labA4.Content = $"{(f.Schedule / 100) * 100}%";
                            labA5.Content = f.DisplayName;
                            break;

                        case 1:
                            f.imgStr = "imgB";
                            dgrB.Visibility = Visibility.Visible;
                            labB1.Content = f.CourseName;
                            labB2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labB3.Content = f.ExpiredTime;
                            pbB.Value = f.Schedule;
                            labB4.Content = $"{(f.Schedule / 100) * 100}%";
                            labB5.Content = f.DisplayName;
                            break;

                        case 2:
                            f.imgStr = "imgC";
                            dgrC.Visibility = Visibility.Visible;
                            labC1.Content = f.CourseName;
                            labC2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labC3.Content = f.ExpiredTime;
                            pbC.Value = f.Schedule;
                            labC4.Content = $"{(f.Schedule / 100) * 100}%";
                            labC5.Content = f.DisplayName;
                            break;

                        case 3:
                            f.imgStr = "imgD";
                            dgrD.Visibility = Visibility.Visible;
                            labD1.Content = f.CourseName;
                            labD2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labD3.Content = f.ExpiredTime;
                            pbD.Value = f.Schedule;
                            labD4.Content = $"{(f.Schedule / 100) * 100}%";
                            labD5.Content = f.DisplayName;
                            break;

                        case 4:
                            f.imgStr = "imgE";
                            dgrE.Visibility = Visibility.Visible;
                            labE1.Content = f.CourseName;
                            labE2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labE3.Content = f.ExpiredTime;
                            pbE.Value = f.Schedule;
                            labE4.Content = $"{(f.Schedule / 100) * 100}%";
                            labE5.Content = f.DisplayName;
                            break;

                        case 5:
                            f.imgStr = "imgF";
                            dgrF.Visibility = Visibility.Visible;
                            labF1.Content = f.CourseName;
                            labF2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labF3.Content = f.ExpiredTime;
                            pbF.Value = f.Schedule;
                            labF4.Content = $"{(f.Schedule / 100) * 100}%";
                            labF5.Content = f.DisplayName;
                            break;

                        case 6:
                            f.imgStr = "imgG";
                            dgrG.Visibility = Visibility.Visible;
                            labG1.Content = f.CourseName;
                            labG2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labG3.Content = f.ExpiredTime;
                            pbG.Value = f.Schedule;
                            labG4.Content = $"{(f.Schedule / 100) * 100}%";
                            labG5.Content = f.DisplayName;
                            break;

                        case 7:
                            f.imgStr = "imgH";
                            dgrH.Visibility = Visibility.Visible;
                            labH1.Content = f.CourseName;
                            labH2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labH3.Content = f.ExpiredTime;
                            pbH.Value = f.Schedule;
                            labH4.Content = $"{(f.Schedule / 100) * 100}%";
                            labH5.Content = f.DisplayName;
                            break;

                        case 8:
                            f.imgStr = "imgI";
                            dgrI.Visibility = Visibility.Visible;
                            labI1.Content = f.CourseName;
                            labI2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labI3.Content = f.ExpiredTime;
                            pbI.Value = f.Schedule;
                            labI4.Content = $"{(f.Schedule / 100) * 100}%";
                            labI5.Content = f.DisplayName;
                            break;

                        case 9:
                            f.imgStr = "imgJ";
                            dgrJ.Visibility = Visibility.Visible;
                            labJ1.Content = f.CourseName;
                            labJ2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labJ3.Content = f.ExpiredTime;
                            pbJ.Value = f.Schedule;
                            labJ4.Content = $"{(f.Schedule / 100) * 100}%";
                            labJ5.Content = f.DisplayName;
                            break;

                        case 10:
                            f.imgStr = "imgK";
                            dgrK.Visibility = Visibility.Visible;
                            labK1.Content = f.CourseName;
                            labK2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labK3.Content = f.ExpiredTime;
                            pbK.Value = f.Schedule;
                            labK4.Content = $"{(f.Schedule / 100) * 100}%";
                            labK5.Content = f.DisplayName;
                            break;

                        case 11:
                            f.imgStr = "imgL";
                            dgrL.Visibility = Visibility.Visible;
                            labL1.Content = f.CourseName;
                            labL2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labL3.Content = f.ExpiredTime;
                            pbL.Value = f.Schedule;
                            labL4.Content = $"{(f.Schedule / 100) * 100}%";
                            labL5.Content = f.DisplayName;
                            break;

                        case 12:
                            f.imgStr = "imgM";
                            dgrM.Visibility = Visibility.Visible;
                            labM1.Content = f.CourseName;
                            labM2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labM3.Content = f.ExpiredTime;
                            pbM.Value = f.Schedule;
                            labM4.Content = $"{(f.Schedule / 100) * 100}%";
                            labM5.Content = f.DisplayName;
                            break;

                        case 13:
                            f.imgStr = "imgN";
                            dgrN.Visibility = Visibility.Visible;
                            labN1.Content = f.CourseName;
                            labN2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labN3.Content = f.ExpiredTime;
                            pbN.Value = f.Schedule;
                            labN4.Content = $"{(f.Schedule / 100) * 100}%";
                            labN5.Content = f.DisplayName;
                            break;

                        case 14:
                            f.imgStr = "imgO";
                            dgrO.Visibility = Visibility.Visible;
                            labO1.Content = f.CourseName;
                            labO2.Content = $"{f.StudyYear}-{f.StudyTerm}";
                            labO3.Content = f.ExpiredTime;
                            pbO.Value = f.Schedule;
                            labO4.Content = $"{(f.Schedule / 100) * 100}%";
                            labO5.Content = f.DisplayName;
                            break;

                        default:
                            break;
                    }
                    i++;
                });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dgrStr"></param>
        /// <returns></returns>
        public async Task<bool> BatchCourseStart(string dgrStr = "")
        {
            try
            {
                AddProcessInfo($"{strInfo}-开始学习");

                await Task.Run(() =>
                 {
                     AddProcessInfo($"{strInfo}-学习中...");

                     OperationBtnEnable(false);

                     _isStopDeal=true;

                     //未完成的课程
                     var coursesInfos = ez_coursesList.Where(f => f.Schedule < 100 && string.IsNullOrEmpty(dgrStr) ? true : (f.imgStr == dgrStr)).ToList();

                     if (coursesInfos.Count > 0)
                     {
                         demoData.PostCourseStart(coursesInfos);
                     }
                 }, token).ContinueWith(t =>
                 {
                     try
                     {
                         this.Dispatcher.Invoke(() =>
                         {
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
                AddProcessWarn($"{strInfo}停止学习");

                tokenSource.Cancel();

                OperationBtnEnable(true);

                AddProcessWarn($"{strInfo}学习结束");
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
                _isStopDeal = false;

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
                _isStopDeal = false;
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

                    foreach (var fileName in fileNames)
                    {
                        var dataTable = new ExcelHelper(fileName).ExcelToDataTable("Sheet1", true);

                        var studentList = JsonHelper.DeserializeObject<List<Student>>(dataTable.ToJson());

                        //学生信息
                        studentList.ForEach(s =>
                        {
                            if (!string.IsNullOrEmpty(s.IdCard)&&s.IdCard.Length>7)
                            {
                                var student = new Student()
                                {
                                    IdCard=s.IdCard,
                                    MoviePwd= s.IdCard.Substring(s.IdCard.Length-6),
                                };

                                ez_studentInfo = new DemoData(student).GetStudentInfo();

                                student.ClassName=ez_studentInfo.info.ClassName;
                                student.SchoolName=ez_studentInfo.info.SchoolName;
                                student.StudyType=LearnTypeConverter(ez_studentInfo.info.LearnType);
                                student.StudyCode=ez_studentInfo.info.StudentNumber;

                                var result = studentService.AddStudent(s);

                                if (!result.Success)
                                {
                                    sucessCount++;
                                    //exMsgs.Add(new EXMessageDto()
                                    //{
                                    //    Id = s.Id,
                                    //    Message = result.Message,
                                    //});
                                    AddProcessError($"新增学生:{s.StudentName}_{s.IdCard}失败，{result.Message}");
                                }
                                else
                                {
                                    AddProcessInfo($"新增学生:{s.StudentName}_{s.IdCard}成功");
                                }
                            }
                            else
                            {
                                AddProcessError($"新增学生:{s.StudentName}_{s.IdCard}失败，数据格式错误");

                                //exMsgs.Add(new EXMessageDto()
                                //{
                                //    Id = s.IdCard,
                                //    Message ="格式错误",
                                //});
                            }
                        });
                    }

                    if (sucessCount>0)
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

                    CurrentStatus = BatchImportStatus.手动处理;
                    OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));

                    //var t = Task.Run(() =>
                    //{
                    //    OperationBtnEnable(false);

                    //    Thread.Sleep(5000);
                    //    return "Hello I am TimeConsumingMethod";
                    //});

                    //textBox1.Text = await t;

                    //await Task.Factory.StartNew(MyTask, cancelTokenSource.Token);

                    var resultData = await BatchCourseStart();

                    if (resultData)
                    {
                        CurrentStatus = BatchImportStatus.成功;
                        OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
                    }
                    else
                    {
                        CurrentStatus = BatchImportStatus.失败;
                        OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
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

                var result = await BatchCourseStart(name);

                if (result)
                {
                    CurrentStatus = BatchImportStatus.成功;
                    OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
                }
                else
                {
                    CurrentStatus = BatchImportStatus.失败;
                    OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
                }

                MessageBox.Show("双击");
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
                    //BtnAutoStart.IsEnabled = isEnable;
                    grid1.IsEnabled = isEnable;
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

        private ObservableCollection<BatchImportProcessInfo> _processInfoList = new ObservableCollection<BatchImportProcessInfo>();

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
                _processInfoList.Add(new BatchImportProcessInfo
                {
                    Id = _processInfoList.Count,
                    Info = msg,
                    Type = type,
                });
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
                ez_student=student;

                StudentDataBind();

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
            treeView.BeginInit();
            treeView.Items.Clear();
            int i = 0;

            studentInfos.ForEach(student =>
            {
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
                if (studentCount == 0)
                {
                    ez_student=student;
                    StudentDataBind();
                }
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