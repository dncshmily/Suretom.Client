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

namespace Suretom.Client.UI.Pages.Demo
{
    /// <summary>
    /// DemoWin.xaml 的交互逻辑
    /// </summary>
    public partial class DemoWin : UserControl
    {
        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private ObservableCollection<BatchImportInfo> _batchImprotInfoList = new ObservableCollection<BatchImportInfo>();

        private List<string> schoolList = new List<string>() { "安徽大学", "安徽师范大学", "阜阳师范学院", "北京大学", "清华大学", "科技大学", "中科院" };

        private List<string> studentList = new List<string>() { "张三", "李四", "王五", "赵六", "孙七" };

        private IUserService userService;

        /// <summary>
        /// 是否停止处理
        /// </summary>
        private bool _isStopDeal = false;

        public DemoWin()
        {
            InitializeComponent();

            userService = GlobalContext.Resolve<IUserService>();

            FillTreeView(schoolList);
        }

        /// <summary>
        ///
        /// </summary>
        public class CourseInfo
        {
            public string Name { get; set; }
            public string State { get; set; }
            public string DeadlineTime { get; set; }
        }

        private List<CourseInfo> courseInfos = new List<CourseInfo>()
        {
            new CourseInfo()
            {
                Name=".Net",
                State="已学完",
                DeadlineTime=DateTime.Now.ToString()
            }, new CourseInfo()
            {
                Name="Java",
                State="学习中",
                DeadlineTime=DateTime.Now.ToString()
            }, new CourseInfo()
            {
                Name="Python",
                State="未开始",
                DeadlineTime=DateTime.Now.ToString()
            }
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //学生
                var studentInfo = new StudentInfo()
                {
                };

                //labNane.Content = studentInfo.Name;
                //labIdCard.Content = studentInfo.IdCard;
                //labNo.Content = studentInfo.No;
                //labClass.Content = studentInfo.ClassName;
                //labIdType.Content = studentInfo.Type;

                //课程
                var courseList = new DemoData(studentInfo).GetCourseList();

                var courseInfos = courseList.List.ToList();

                dgCourseInfo.DataContext = courseList.List.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        #region 按钮

        private void BtnAddWords_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".docx";
                ofd.Filter = "Word 文件|*.docx";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == true)
                {
                    var fileNames = ofd.FileNames;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 开始处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnStartDeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isStopDeal = false;
                this.Sp1.IsEnabled = false;

                foreach (var item in _batchImprotInfoList)
                {
                    if (_isStopDeal)
                        break;
                    //初始化失败的，不处理
                    if (item.Status == BatchImportStatus.初始化失败)
                        continue;
                    //处理成功的，不处理，只支持手动单独点击
                    if (item.Status == BatchImportStatus.成功)
                        continue;
                    //处于手动处理状态的，只能手动完成，检查开启此状态,上传成功->成功，上传失败->失败
                    if (item.Status == BatchImportStatus.手动处理)
                        continue;

                    item.Status = BatchImportStatus.正在处理;
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

        #endregion 按钮

        private ImageSource _folderClose;
        private ImageSource _folderOpen;
        private ImageSource _fileThumbnail;

        #region TreeView Event Handlers

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem selItem = treeView.SelectedItem as TreeViewItem;
            if (selItem == null || selItem.Tag == null)
            {
                return;
            }

            string selectedName = selItem.Tag as string;
            if (string.IsNullOrWhiteSpace(selectedName))
            {
                return;
            }

            e.Handled = true;

            treeView.IsEnabled = false;

            try
            {
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

        private void OnTreeViewItemUnselected(object sender, RoutedEventArgs e)
        {
        }

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="list"></param>
        private void FillTreeView(List<string> schoolList)
        {
            if (schoolList.Count == 0)
            {
                return;
            }

            treeView.BeginInit();
            treeView.Items.Clear();

            for (int i = 0; i < schoolList.Count; i++)
            {
                TextBlock headerText = new TextBlock();
                headerText.Text = schoolList[i];
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

                FillTreeView(studentList, categoryItem);

                categoryItem.IsExpanded = (i == 0);
            }

            treeView.EndInit();
        }

        private void FillTreeView(List<string> studentList, TreeViewItem treeItem)
        {
            if (studentList == null || studentList.Count == 0)
            {
                return;
            }

            int itemCount = 0;

            foreach (var student in studentList)
            {
                TextBlock itemText = new TextBlock();
                itemText.Text = $"{student}";
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

                itemCount++;
            }
        }

        #endregion
    }
}