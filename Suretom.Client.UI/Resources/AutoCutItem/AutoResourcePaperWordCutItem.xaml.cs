using CTBClient.UI.ViewModels.ItemResource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CTBClient.DataTools.Pages.AutoCutItem
{
    /// <summary>
    /// AutoResourcePaperWordCutItem.xaml 的交互逻辑
    /// </summary>
    public partial class AutoResourcePaperWordCutItem : UserControl
    {
        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private ObservableCollection<BatchImportInfo> _batchImprotInfoList = new ObservableCollection<BatchImportInfo>();

        /// <summary>
        /// 是否停止处理
        /// </summary>
        private bool _isStopDeal = false;


        public AutoResourcePaperWordCutItem()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ListBoxWord.DataContext = _batchImprotInfoList;

                this.ListBoxWord.SelectionChanged += (s2, e3) =>
                {
                    try
                    {
                        var showCtlList = _batchImprotInfoList
                            .Where(x => ((UIElement)x.Ctl).Visibility == Visibility.Visible)
                            .Select(x => (UIElement)x.Ctl).ToList();
                        foreach (var ctl in showCtlList)
                        {
                            ctl.Visibility = Visibility.Collapsed;
                        }

                        if (ListBoxWord.SelectedValue != null)
                        {
                            var currentCtl = (UIElement)((BatchImportInfo)ListBoxWord.SelectedValue).Ctl;
                            currentCtl.Visibility = Visibility.Visible;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        MessageBox.Show(ex.Message);
                    }
                };

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
                    AddWords(fileNames);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void AddWords(string[] fileNames)
        {
            if (fileNames != null && fileNames.Length > 0)
            {
                if (_batchImprotInfoList.Count + fileNames.Length > 200)
                {
                    MessageBox.Show("最多同时处理200个Word");
                    return;
                }

                var existedList = new List<string>();
                var maxId = _batchImprotInfoList.Count > 0 ? _batchImprotInfoList.Max(x => x.Id) : 0;
                foreach (var fileName in fileNames)
                {
                    var wordFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);

                    //word临时文件
                    if (wordFileName.StartsWith("~$"))
                        continue;

                    if (_batchImprotInfoList.Any(x => x.WordFileFullPath == fileName))
                    {
                        existedList.Add(fileName);
                    }
                    else
                    {
                        var batchImprotInfo = new BatchImportInfo
                        {
                            Id = ++maxId,
                            WordFileFullPath = fileName,
                            WordFileName = wordFileName,
                            Status = BatchImportStatus.未开始,
                        };
                        var ctl = new AutoResourcePaperWordCutItemContent()
                        {
                            Visibility = Visibility.Collapsed
                        };
                        ctl.RemoveCtlEvent += (s2, e2) =>
                        {
                            try
                            {
                                var temp = _batchImprotInfoList.FirstOrDefault(x => x.WordFileFullPath == e2.WordFileName);
                                if (temp != null)
                                {
                                    var index = _batchImprotInfoList.IndexOf(temp);
                                    this.DealCtlContainer.Children.Remove((UIElement)temp.Ctl);
                                    _batchImprotInfoList.Remove(temp);

                                    if (index - 1 >= 0)
                                    {
                                        this.ListBoxWord.SelectedIndex = index - 1;
                                    }
                                    else if (_batchImprotInfoList.Count > 0)
                                    {
                                        this.ListBoxWord.SelectedIndex = 0;
                                    }

                                    ShowNumber();

                                }
                            }
                            catch (Exception ex2)
                            {
                                log.Error(ex2);
                                MessageBox.Show(ex2.Message);
                            }
                        };
                        ctl.StatusChangeEvent += (s2, e2) =>
                        {
                            try
                            {
                                batchImprotInfo.Status = e2.Status;
                                ShowNumber();
                            }
                            catch (Exception ex2)
                            {
                                log.Error(ex2);
                            }
                        };
                        ctl.InitParam(fileName);
                        batchImprotInfo.Ctl = ctl;

                        _batchImprotInfoList.Add(batchImprotInfo);

                        this.DealCtlContainer.Children.Add((UIElement)batchImprotInfo.Ctl);
                    }

                    ShowNumber();
                }

                if (_batchImprotInfoList.Count > 0)
                {
                    this.ListBoxWord.SelectedIndex = 0;
                }

                if (existedList.Count > 0)
                {
                    MessageBox.Show($"已在列表中的文档：{string.Join(",", existedList)}");
                }
            }
        }

        private void BtnAddWordPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fbWnd = new System.Windows.Forms.FolderBrowserDialog();
                fbWnd.Description = "选择Word目录";
                fbWnd.SelectedPath = "C:";
                //fbWnd.ShowNewFolderButton = true;
                if (fbWnd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var fileNames = System.IO.Directory.GetFiles(fbWnd.SelectedPath, "*.docx", System.IO.SearchOption.AllDirectories);

                    AddWords(fileNames);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 清空处理列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearAllWords_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var unDealList = _batchImprotInfoList.Where(x => x.Status == BatchImportStatus.未开始).ToList();
                if (unDealList.Count > 0)
                {
                    var result = MessageBox.Show($"存在未处理的文件：{string.Join("，", unDealList.ConvertAll(x => x.ShowName))}，只会移除完成处理的，是否继续？", "提示", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        var toRemoveList = _batchImprotInfoList.Where(x => x.Status == BatchImportStatus.成功).ToList();
                        foreach (var item in toRemoveList)
                        {
                            this.DealCtlContainer.Children.Remove((UIElement)item.Ctl);
                            _batchImprotInfoList.Remove(item);
                        }
                    }
                }
                else
                {
                    var result = MessageBox.Show($"是否清空处理列表？", "提示", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        foreach (var item in _batchImprotInfoList)
                        {
                            this.DealCtlContainer.Children.Remove((UIElement)item.Ctl);
                        }

                        _batchImprotInfoList.Clear();
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
                this.BtnStopDeal.Visibility = Visibility.Visible;

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

                    var result = await ((AutoResourcePaperWordCutItemContent)item.Ctl).BatchDeal();
                    if (result)
                        item.Status = BatchImportStatus.成功;
                    else
                        item.Status = BatchImportStatus.失败;

                    ShowNumber();
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
                this.BtnStopDeal.Visibility = Visibility.Hidden;
                this.BtnStopDeal.IsEnabled = true;
                this.BtnStopDeal.Content = "停止处理";
            }
        }


        /// <summary>
        /// 停止处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStopDeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isStopDeal = true;
                this.BtnStopDeal.IsEnabled = false;
                this.BtnStopDeal.Content = "正在停止...";
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 关闭处理成功的窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCloseAllSuccess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var successList = _batchImprotInfoList.Where(x => x.Status == BatchImportStatus.成功).ToList();
                if (successList.Count > 0)
                {
                    var result = MessageBox.Show($"是否关闭列表中处理成功的Word？", "提示", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        foreach (var item in successList)
                        {
                            this.DealCtlContainer.Children.Remove((UIElement)item.Ctl);
                            _batchImprotInfoList.Remove(item);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("无处理成功的Word");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void ShowNumber()
        {
            var allCount = _batchImprotInfoList.Count;
            var unDealCount = _batchImprotInfoList.Count(x => x.Status == BatchImportStatus.未开始);
            var initFailCount = _batchImprotInfoList.Count(x => x.Status == BatchImportStatus.初始化失败);
            var manualCount = _batchImprotInfoList.Count(x => x.Status == BatchImportStatus.手动处理);
            var successCount = _batchImprotInfoList.Count(x => x.Status == BatchImportStatus.成功);
            var failCount = _batchImprotInfoList.Count(x => x.Status == BatchImportStatus.失败);

            this.Dispatcher.Invoke(() =>
            {
                LblAllCount.Content = $"总计：{allCount}";
                LblUnDealCount.Content = $"未处理：{unDealCount}";
                LblManualCount.Content = $"手动处理：{manualCount}";
                LblInitFailCount.Content = $"初始化失败：{initFailCount}";
                LblSuccessCount.Content = $"成功：{successCount}";
                LblFailCount.Content = $"失败：{failCount}";
            });
        }

        /// <summary>
        /// 导出错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExportError_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fbWnd = new System.Windows.Forms.FolderBrowserDialog();
                fbWnd.Description = "选择导出的位置";
                fbWnd.SelectedPath = "C:";
                fbWnd.ShowNewFolderButton = true;
                if (fbWnd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var exportPath = fbWnd.SelectedPath;

                    //错误信息
                    var errorTxt = System.IO.Path.Combine(exportPath, "错误信息.txt");
                    foreach (var item in _batchImprotInfoList.Where(x => x.Status == BatchImportStatus.初始化失败 || x.Status == BatchImportStatus.失败))
                    {
                        try
                        {
                            var lines = new List<string>();
                            lines.Add("");
                            lines.Add("");
                            lines.Add(item.WordFileFullPath);
                            lines.Add(item.Status.ToString());
                            lines.Add("********************");

                            var errorList = ((AutoResourcePaperWordCutItemContent)item.Ctl).GetProcessError();
                            if (errorList.Count > 0)
                                lines.AddRange(errorList);
                            else
                                lines.Add("");
                            lines.Add("");
                            lines.Add("-----------------------------");
                            System.IO.File.AppendAllLines(errorTxt, lines, Encoding.UTF8);

                            //失败的Word复制到这个目录
                            var saveFileName = System.IO.Path.Combine(exportPath, System.IO.Path.GetFileName(item.WordFileFullPath));
                            System.IO.File.Copy(item.WordFileFullPath, saveFileName);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
