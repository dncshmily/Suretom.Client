using CTBClient.Common;
using CTBClient.DataPlatformModel.CutItemService;
using CTBClient.DataPlatformModel.ResourcePaperService;
using CTBClient.DataPlatformModel.TransferItemTraceService;
using CTBClient.DataPlatformServices;
using CTBClient.UI.Pages.ItemResource;
using CTBClient.UI.ViewModels.ItemResource;
using DocTool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// AutoResourcePaperWordCutItemContent.xaml 的交互逻辑
    /// </summary>
  
   public partial class AutoResourcePaperWordCutItemContent : UserControl
    {
        private static NLog.Logger ez_log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 资源试卷Guid
        /// </summary>
        private string ez_resourcePaperGuid;

        /// <summary>
        /// 资源试卷
        /// </summary>
        private ResourcePaper ez_resourcePaper;

        /// <summary>
        /// 当前切图保存的基础目录
        /// </summary>
        private string _saveBaseDir = string.Empty;

        /// <summary>
        /// 合并后的Word
        /// </summary>
        private string _mergeWordPath = string.Empty;

        /// <summary>
        /// 试卷里解析出的题目
        /// </summary>
        private ObservableCollection<WordItemNew> ez_wordItemList = new ObservableCollection<WordItemNew>();

        /// <summary>
        /// 切图处理类
        /// </summary>
        private DocTool.QuestionWord.ResourcePaperWordCutItemHandler _wordHandler;

        private ObservableCollection<BatchImportProcessInfo> _processInfoList = new ObservableCollection<BatchImportProcessInfo>();
        /// <summary>
        /// 移除处理控件
        /// </summary>
        public event EventHandler<RemoveCtlEventArgs> RemoveCtlEvent;
        public event EventHandler<StatusChangeEventArgs> StatusChangeEvent;
        /// <summary>
        /// 创建此控件时的WordFileName，用此作Key，移除控件
        /// </summary>
        private string _originWordFileName;
        /// <summary>
        /// 是否处于操作中，true表示在某个操作中， 比如检查
        /// 为true时，不能关闭当前的窗口
        /// </summary>
        private bool ez_isDealing = false;
        /// <summary>
        /// 当前的处理状态
        /// </summary>
        public BatchImportStatus CurrentStatus
        {
            get; private set;
        }
        /// <summary>
        /// 自动处理的状态
        /// </summary>
        private AutoDealStatus _autoDealStatus = AutoDealStatus.UnStart;

        #region 操作时间统计

        /// <summary>
        /// 批量处理时，处理的开始时间
        /// </summary>
        private DateTime _batchDealStartTime;
        /// <summary>
        /// 批量处理时，处理的结束时间
        /// </summary>
        private DateTime _batchDealEndTime;

        /// <summary>
        /// 单个操作，处理的开始时间
        /// </summary>
        private DateTime _operationStartTime;
        /// <summary>
        /// 单个操作，处理的结束时间
        /// </summary>
        private DateTime _operationEndTime;

        #endregion

        #region 切图设置参数

        /// <summary>
        /// 检查时是否显示Word,true显示，false不显示
        /// </summary>
        private bool _isCheckShowWord = false;
        /// <summary>
        /// 检查时是否修复段落
        /// </summary>
        private bool _isRepairParagraph = true;

        #endregion

        /// <summary>
        /// 资源试卷服务
        /// </summary>
        private readonly IResourcePaperService ez_resourcePaperService;

        private readonly ICutItemService ez_cutItemService;

        public AutoResourcePaperWordCutItemContent()
        {
            InitializeComponent();

            CurrentStatus = BatchImportStatus.未开始;

            #region 服务接口初始化

            var scope = GlobalContext.LifetimeScope();
            ez_resourcePaperService = GlobalContext.Resolve<IResourcePaperService>(scope);
            ez_cutItemService = GlobalContext.Resolve<ICutItemService>(scope);

            #endregion 服务接口初始化

            var guid = Guid.NewGuid().ToString();
            //每次选择新的word，都重新生成切图的目录
            _saveBaseDir = System.IO.Path.Combine(GlobalContext.LocalDirectoryConfig["CutItem"], guid);
            if (!System.IO.Directory.Exists(_saveBaseDir))
            {
                System.IO.Directory.CreateDirectory(_saveBaseDir);
            }

            dgProcessInfo.DataContext = _processInfoList;
            dgItemUpdate.DataContext = ez_wordItemList;
        }

        #region 对外接口


        public void InitParam(string wordFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(wordFileName))
                {
                    throw new ArgumentNullException("文件名为空");
                }

                _originWordFileName = wordFileName;

                //AddProcessError("测试信息--");
                AddProcessInfo("开始");

                //解析文件名
                WordFileNameParse(wordFileName);
                _mergeWordPath = wordFileName;

                //获取资源试卷信息
                Task.Run(() =>
                {
                    try
                    {
                        OperationBtnEnable(false);

                        GetResourcePaperAndItemData();
                    }
                    catch (Exception inEx)
                    {
                        AddProcessError($"{inEx.Message}");
                        ez_log.Error(inEx);
                    }
                    finally
                    {
                        OperationBtnEnable(true);
                    }
                });


                CurrentStatus = BatchImportStatus.未开始;
                OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
            }
            catch (Exception ex)
            {
                AddProcessError($"{ex.Message}");
                ez_log.Error(ex);

                CurrentStatus = BatchImportStatus.初始化失败;
                OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
            }
        }

        public async Task<bool> BatchDeal()
        {
            try
            {
                _batchDealStartTime = DateTime.Now;

                ez_isDealing = true;
                _autoDealStatus = AutoDealStatus.AllSuccess;

                ClearProcessMsg();
                AddProcessInfo("开始处理");

                OperationBtnEnable(false);

                await Task.Run(() =>
                {
                    #region 检查

                    try
                    {
                        CloseAllWordProcess();

                        InitWordHandler();

                        _wordHandler.SetOption(isCheckShowWord: _isCheckShowWord, isRepareParagraph: _isRepairParagraph);

                        if (ez_wordItemList.Count == 0)
                        {
                            throw new Exception("没有试卷结构");
                        }

                        foreach (var item in ez_wordItemList)
                        {
                            item.CheckInfo = "";
                            item.IsCheckSuccess = false;
                            item.IsCutImgSuccess = false;
                            item.ContentImgUpdateInfo = "";
                            item.AnswerImgUpdateInfo = "";
                            item.ContentPageImages = new List<PageImageUnit>();
                            item.AnswerPageImages = new List<PageImageUnit>();
                            item.ContentImages = new List<ImageUnit>();
                            item.AnswerImages = new List<ImageUnit>();
                            item.IsCutWordSuccess = false;
                            item.ContentWordPath = string.Empty;
                            item.AnswerWordPath = string.Empty;
                            item.ContentWordUpdateInfo = "";
                            item.AnswerWordUpdateInfo = "";

                            item.IsDataDealSuccess = false;
                        }

                        var result = _wordHandler.CheckWord();

                        if (!string.IsNullOrEmpty(result))
                        {
                            throw new Exception(result);
                        }

                    }
                    catch (Exception inEx)
                    {
                        var msg = $"检查Word出现错误，{inEx.Message}";

                        AddProcessError(msg);

                        CloseAllWordProcess();

                        //拷贝检查失败的word
                        //CopyCheckFailWord(msg);

                        _autoDealStatus = AutoDealStatus.CheckError;
                        return;
                    }

                    #endregion

                    #region 切图
                    try
                    {
                        //切图
                        _wordHandler.CutImage();

                    }
                    catch (Exception inEx)
                    {
                        var msg = $"分割试题出现错误，{inEx.Message}";

                        ez_log.Error(msg);

                        AddProcessError(msg);

                        CloseAllWordProcess();

                        _autoDealStatus = AutoDealStatus.CutImgError;

                        return;
                    }
                    #endregion

                    #region 保存

                    try
                    {
                        if (!ez_wordItemList.Any(x => x.ContentPageImages.Count > 0))
                        {
                            throw new Exception("没有可供保存的数据");
                        }

                        //进行过切题操作的试题
                        var wordItemList = ez_wordItemList.Where(x => x.IsCutImgSuccess && x.IsCutWordSuccess).ToList();

                        //清空原有的保存状态
                        foreach (var item in wordItemList)
                        {
                            item.IsDataDealSuccess = false;
                            item.ContentWordUpdateInfo = "已切Word";
                            item.AnswerWordUpdateInfo = "已切Word";
                            item.ContentImgUpdateInfo = "已切图片";
                            item.AnswerImgUpdateInfo = "已切图片";
                        }

                        //上传标记的Word
                        UpdateMarkedWordFile();

                        //更新试题的属性数据
                        WordCutItemSaveData(wordItemList);

                        //更新图片，需要把原有的图片先清除
                        WordCutItemSaveFile(wordItemList, true);
                    }
                    catch (Exception inEx)
                    {
                        var msg = $"保存试题出现错误，{inEx.Message}";

                        ez_log.Error(msg);

                        AddProcessError(msg);

                        _autoDealStatus = AutoDealStatus.UpdateError;

                        return;
                    }

                    #endregion

                }).ContinueWith(t =>
                {
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            OperationBtnEnable(true);

                            #region 统计批处理用时
                            _batchDealEndTime = DateTime.Now;
                            AddProcessInfo($"开始时间：{_batchDealStartTime}");
                            AddProcessInfo($"结束时间：{_batchDealEndTime}");
                            var timeSpan = _batchDealEndTime - _batchDealStartTime;
                            if (timeSpan.TotalMinutes > 10d)
                            {
                                var avgTime = "";
                                if (ez_wordItemList.Count == 0)
                                    avgTime = "无法计算";
                                else
                                    avgTime = (timeSpan.TotalMinutes / ez_wordItemList.Count).ToString("F2");

                                AddProcessInfo($"总计用时：{timeSpan.TotalMinutes.ToString("F2")}分钟，平均每题：{avgTime}分钟");
                            }
                            else
                            {
                                var avgTime = "";
                                if (ez_wordItemList.Count == 0)
                                    avgTime = "无法计算";
                                else
                                    avgTime = (timeSpan.TotalSeconds / ez_wordItemList.Count).ToString("F2");

                                AddProcessInfo($"总计用时：{timeSpan.TotalSeconds.ToString("F2")}秒，平均每题：{avgTime}秒");
                            }
                            #endregion
                        });
                    }
                    catch (Exception inEx)
                    {
                        ez_log.Error(inEx);
                    }
                });

                return _autoDealStatus == AutoDealStatus.AllSuccess;
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
                AddProcessError($"{ex.Message}");

                return false;
            }
            finally
            {
                ez_isDealing = false;

            }
        }

        public List<string> GetProcessError()
        {
            return new List<string>();
        }

        #endregion

        #region 上部按钮


        private void BtnPaperFileSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".docx";
                ofd.Filter = "Word 文件|*.docx";
                var originFileName = TxtWordFileName.Text.Trim();
                if (!string.IsNullOrEmpty(originFileName) && File.Exists(originFileName))
                {
                    ofd.InitialDirectory = System.IO.Path.GetDirectoryName(originFileName);
                }
                if (ofd.ShowDialog() == true)
                {
                    InitParam(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                CurrentStatus = BatchImportStatus.初始化失败;
                OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));

                MessageBox.Show(ex.Message);
                ez_log.Error(ex);
            }
        }

        private void BtnOpenPaperFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = TxtWordFileName.Text.Trim();
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("请先选择Word文件");
                    return;
                }

                if (System.IO.File.Exists(fileName))
                {
                    System.Diagnostics.Process.Start(fileName);
                }
                else
                {
                    MessageBox.Show("文件不存在");
                }
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnOpenPaperFileDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = TxtWordFileName.Text.Trim();
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("请先选择Word文件");
                    return;
                }

                if (System.IO.File.Exists(fileName))
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{fileName}\"");
                }
                else
                {
                    MessageBox.Show("文件不存在");
                }
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnOpenImgDir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnOpenWordDir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ez_isDealing)
                {
                    MessageBox.Show("正在处理中，不能关闭！");
                    return;
                }

                var result = MessageBox.Show("是否确定关闭？", "提示", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    OnRemoveCtlEvent(new RemoveCtlEventArgs(_originWordFileName));
                }
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 处理按钮


        private void OnRemoveCtlEvent(RemoveCtlEventArgs args)
        {
            this.RemoveCtlEvent?.Invoke(this, args);
        }

        private void OnStatusChangeEvent(StatusChangeEventArgs args)
        {
            this.StatusChangeEvent?.Invoke(this, args);
        }

        /// <summary>
        /// 标记检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCheckWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OperationBtnEnable(false);

                Task.Run(() =>
                {
                    try
                    {
                        _operationStartTime = DateTime.Now;
                        _autoDealStatus = AutoDealStatus.UnStart;

                        ez_isDealing = true;

                        ClearProcessMsg();
                        AddProcessInfo("开始处理");

                        CurrentStatus = BatchImportStatus.手动处理;
                        OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));

                        CloseAllWordProcess();

                        InitWordHandler();

                        _wordHandler.SetOption(isCheckShowWord: _isCheckShowWord, isRepareParagraph: _isRepairParagraph);

                        if (ez_wordItemList.Count == 0)
                        {
                            throw new Exception("没有试卷结构");
                        }

                        foreach (var item in ez_wordItemList)
                        {
                            item.CheckInfo = "";
                            item.IsCheckSuccess = false;
                            item.IsCutImgSuccess = false;
                            item.ContentImgUpdateInfo = "";
                            item.AnswerImgUpdateInfo = "";
                            item.ContentPageImages = new List<PageImageUnit>();
                            item.AnswerPageImages = new List<PageImageUnit>();
                            item.ContentImages = new List<ImageUnit>();
                            item.AnswerImages = new List<ImageUnit>();
                            item.IsCutWordSuccess = false;
                            item.ContentWordPath = string.Empty;
                            item.AnswerWordPath = string.Empty;
                            item.ContentWordUpdateInfo = "";
                            item.AnswerWordUpdateInfo = "";

                            item.IsDataDealSuccess = false;
                        }

                        var result = _wordHandler.CheckWord();
                        if (!string.IsNullOrEmpty(result))
                        {
                            throw new Exception(result);
                        }

                        _operationEndTime = DateTime.Now;
                    }
                    catch (Exception inEx)
                    {
                        _autoDealStatus = AutoDealStatus.CheckError;
                        _operationEndTime = DateTime.Now;

                        ez_log.Error(inEx);
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(inEx.Message);
                        });
                    }
                }).ContinueWith(t =>
                {
                    try
                    {
                        ez_isDealing = false;

                        ShowOperationTimeInfo();

                        CloseAllWordProcess();

                        OperationBtnEnable(true);
                    }
                    catch (Exception inEx)
                    {
                        ez_log.Error(inEx);
                    }
                });
            }
            catch (Exception ex)
            {
                ez_log.Error(ex.Message);
            }
        }

        /// <summary>
        /// 分割试题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCutImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_wordHandler == null)
                {
                    MessageBox.Show("请先标记检查！");
                    return;
                }
                if (CurrentStatus == BatchImportStatus.初始化失败)
                {
                    MessageBox.Show("初始化失败");
                    return;
                }
                if (_autoDealStatus == AutoDealStatus.CheckError)
                {
                    MessageBox.Show("检查失败，请重新检查");
                    return;
                }

                OperationBtnEnable(false);

                CloseAllWordProcess();

                Task.Run(() =>
                {
                    try
                    {
                        ez_isDealing = true;

                        _operationStartTime = DateTime.Now;


                        //判断是否进行了检查

                        if (ez_wordItemList.Count == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("没有可供切图的数据");
                            });
                            return;
                        }

                        //清空原有的切图状态
                        foreach (var item in ez_wordItemList)
                        {
                            item.IsCutImgSuccess = false;
                            item.ContentImgUpdateInfo = "";
                            item.AnswerImgUpdateInfo = "";
                            item.ContentPageImages = new List<PageImageUnit>();
                            item.AnswerPageImages = new List<PageImageUnit>();
                            item.ContentImages = new List<ImageUnit>();
                            item.AnswerImages = new List<ImageUnit>();

                            item.IsCutWordSuccess = false;
                            item.ContentWordPath = string.Empty;
                            item.AnswerWordPath = string.Empty;
                            item.ContentWordUpdateInfo = "";
                            item.AnswerWordUpdateInfo = "";

                            item.IsDataDealSuccess = false;
                        }

                        //切图
                        _wordHandler.CutImage();

                        _operationEndTime = DateTime.Now;

                    }
                    catch (Exception inEx)
                    {
                        _operationEndTime = DateTime.Now;

                        ez_log.Error(inEx);
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(inEx.Message);
                        });
                    }
                }).ContinueWith(t =>
                {
                    try
                    {
                        ez_isDealing = false;

                        ShowOperationTimeInfo();

                        CloseAllWordProcess();

                        OperationBtnEnable(true);
                    }
                    catch (Exception inEx)
                    {
                        ez_log.Error(inEx);
                    }

                });
            }
            catch (Exception ex)
            {
                ez_log.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 保存试题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdatePaper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OperationBtnEnable(false);

                CloseAllWordProcess();

                Task.Run(() =>
                {
                    try
                    {
                        ez_isDealing = true;

                        if (ez_wordItemList.Count == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("没有可供保存的数据");
                            });
                            return;
                        }
                        if (!ez_wordItemList.Any(x => x.ContentPageImages.Count > 0))
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("没有可供保存的数据");
                            });
                            return;
                        }

                        //进行过切题操作的试题
                        var wordItemList = ez_wordItemList.Where(x => x.IsCutImgSuccess && x.IsCutWordSuccess).ToList();
                        if (wordItemList.Count == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("没有可供保存的数据");
                            });
                            return;
                        }

                        //清空原有的保存状态
                        foreach (var item in wordItemList)
                        {
                            item.IsDataDealSuccess = false;
                            item.ContentWordUpdateInfo = "已切Word";
                            item.AnswerWordUpdateInfo = "已切Word";
                            item.ContentImgUpdateInfo = "已切图片";
                            item.AnswerImgUpdateInfo = "已切图片";
                        }

                        //上传标记的Word
                        UpdateMarkedWordFile();

                        //更新试题的属性数据
                        WordCutItemSaveData(wordItemList);

                        //更新图片，需要把原有的图片先清除
                        WordCutItemSaveFile(wordItemList, true);

                        _operationEndTime = DateTime.Now;
                        CurrentStatus = BatchImportStatus.成功;
                        OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));
                    }
                    catch (Exception inEx)
                    {
                        _operationEndTime = DateTime.Now;
                        CurrentStatus = BatchImportStatus.失败;
                        OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));


                        ez_log.Error(inEx);
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(inEx.Message);
                        });
                    }
                }).ContinueWith(t =>
                {
                    try
                    {
                        ez_isDealing = false;

                        ShowOperationTimeInfo();

                        CloseAllWordProcess();

                        OperationBtnEnable(true);
                    }
                    catch (Exception inEx)
                    {
                        ez_log.Error(inEx);
                    }
                });
            }
            catch (Exception ex)
            {
                ez_log.Error(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 自动处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAutoDeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentStatus == BatchImportStatus.初始化失败)
                {
                    MessageBox.Show("初始化失败");
                    return;
                }

                CurrentStatus = BatchImportStatus.手动处理;
                OnStatusChangeEvent(new StatusChangeEventArgs(CurrentStatus));

                var result = await BatchDeal();
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
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void ChkCheckShowWord_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkCheckShowWord_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkIsRepairParagraph_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkIsRepairParagraph_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        #endregion


        #region DataGrid按钮

        /// <summary>
        /// 单题切图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSingleItemCutImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //var cutImgLineNum = (int)SliderCutImgLineNum.Value;
                //var threadHeight = (int)SliderThreadHeight.Value;

                var button = sender as Button;
                if (button.Tag is WordItemNew wordItem)
                {
                    if (!wordItem.IsCheckSuccess)
                    {
                        MessageBox.Show("没有完成检查");

                        return;
                    }

                    button.IsEnabled = false;

                    Task.Run(() =>
                    {
                        try
                        {
                            CutImageBySelectRows(new List<WordItemNew> { wordItem });
                        }
                        catch (Exception inEx)
                        {
                            ez_log.Error(inEx);
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
                ez_log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 单题查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnItemView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var wordItem = button.Tag as WordItemNew;
                if (wordItem != null)
                {
                    var win = new WinItemView2(ref ez_wordItemList, wordItem);
                    win.WindowState = WindowState.Maximized;
                    win.Show();
                }
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnReUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var wordItem = button.Tag as WordItemNew;
                if (wordItem != null)
                {
                    if (!wordItem.IsCutImgSuccess)
                    {
                        MessageBox.Show("切图失败，不能上传");

                        return;
                    }

                    button.IsEnabled = false;

                    Task.Run(() =>
                    {
                        try
                        {
                            SaveDataBySelectRows(new List<WordItemNew> { wordItem });
                        }
                        catch (Exception inEx)
                        {
                            ez_log.Error(inEx);
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
                ez_log.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        #endregion

        #region 表格的右键菜单

        /// <summary>
        /// 选定的行切图
        /// </summary>
        /// <param name="wordItemNews"></param>
        private void CutImageBySelectRows(List<WordItemNew> wordItemNews)
        {
            CloseAllWordProcess();

            //
            foreach (var wordItem in wordItemNews.Where(x => x.IsCheckSuccess))
            {
                wordItem.ContentImgUpdateInfo = "";
                wordItem.AnswerImgUpdateInfo = "";
                wordItem.ContentWordUpdateInfo = "";
                wordItem.AnswerWordUpdateInfo = "";
            }

            if (_wordHandler != null)
            {
                _wordHandler.SetOption(isCheckShowWord: _isCheckShowWord, isRepareParagraph: _isRepairParagraph);

                foreach (var wordItem in wordItemNews.Where(x => x.IsCheckSuccess))
                {
                    _wordHandler.SingleWordItemCutDeal(wordItem, true);
                }
                CloseAllWordProcess();
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("请先进行检查");
                });
            }

        }

        /// <summary>
        /// 选定的行保存数据
        /// </summary>
        /// <param name="wordItemNews"></param>
        private void SaveDataBySelectRows(List<WordItemNew> wordItemNews)
        {
            var list = wordItemNews.Where(x => x.IsCutImgSuccess && x.IsCutWordSuccess).ToList();

            if (list.Count > 0)
            {
                foreach (var wordItem in list)
                {
                    wordItem.IsDataDealSuccess = false;
                    wordItem.ContentWordUpdateInfo = "已切Word";
                    wordItem.AnswerWordUpdateInfo = "已切Word";
                    wordItem.ContentImgUpdateInfo = "已切图片";
                    wordItem.AnswerImgUpdateInfo = "已切图片";
                }

                //保存DB数据
                WordCutItemSaveData(list);

                //保存文件数据
                WordCutItemSaveFile(list);
            }
        }

        /// <summary>
        ///  编辑题干，选定的行编辑第一个
        /// </summary>
        /// <param name="wordItemNews"></param>
        private void EditContentFirstRowBySelectRows(List<WordItemNew> wordItemNews)
        {
            var wordItem = wordItemNews.FirstOrDefault();
            if (wordItem != null)
            {
                if (_wordHandler != null)
                {
                    _wordHandler.OpenAndLocationItem(wordItem);
                }
            }
        }

        /// <summary>
        /// 编辑答案，选定的行编辑第一个
        /// </summary>
        /// <param name="wordItemNews"></param>
        private void EditAnswerFirstRowBySelectRows(List<WordItemNew> wordItemNews)
        {
            var wordItem = wordItemNews.FirstOrDefault();
            if (wordItem != null)
            {
                if (_wordHandler != null)
                {
                    _wordHandler.OpenAndLocationItem(wordItem, "answer");
                }
            }
        }

        #endregion 表格的右键菜单


        #region 界面显示的日志信息

        /// <summary>
        /// 显示操作处理的时间
        /// </summary>
        private void ShowOperationTimeInfo()
        {
            AddProcessInfo($"检查操作，开始时间：{_operationStartTime}");
            AddProcessInfo($"检查操作，结束时间：{_operationEndTime}");
            var timeSpan = _operationEndTime - _operationStartTime;
            #region old
            //if (timeSpan.TotalMinutes > 10d)
            //{
            //    var avgTime = "";
            //    if (_wordItemList.Count == 0)
            //        avgTime = "无法计算";
            //    else
            //        avgTime = (timeSpan.TotalMinutes / _wordItemList.Count).ToString("F2");

            //    AddProcessInfo($"总计用时：{timeSpan.TotalMinutes.ToString("F2")}分钟，平均每题：{avgTime}分钟");
            //}
            //else
            //{
            //    var avgTime = "";
            //    if (_wordItemList.Count == 0)
            //        avgTime = "无法计算";
            //    else
            //        avgTime = (timeSpan.TotalSeconds / _wordItemList.Count).ToString("F2");

            //    AddProcessInfo($"总计用时：{timeSpan.TotalSeconds.ToString("F2")}秒，平均每题：{avgTime}秒");
            //}
            #endregion

            //只按秒统计
            var avgTime = "";
            if (ez_wordItemList.Count == 0)
                avgTime = "无法计算";
            else
                avgTime = (timeSpan.TotalSeconds / ez_wordItemList.Count).ToString("F2");

            AddProcessInfo($"总计用时：{timeSpan.TotalSeconds.ToString("F2")}秒，平均每题：{avgTime}秒");
        }

        private void AddProcessError(string info)
        {
            AddProcessMsg(info, BatchImportProcessInfoType.错误);
        }

        private void AddProcessInfo(string info)
        {
            AddProcessMsg(info, BatchImportProcessInfoType.信息);
        }

        private void AddProcessWarn(string info)
        {
            AddProcessMsg(info, BatchImportProcessInfoType.警告);
        }

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

        /// <summary>
        /// 清空处理信息
        /// </summary>
        private void ClearProcessMsg()
        {
            this.Dispatcher.Invoke(() =>
            {
                _processInfoList.Clear();
            });
        }

        #endregion

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
                    Sp2.IsEnabled = isEnable;
                    Grid2.IsEnabled = isEnable;

                });
            }
            catch (Exception ex)
            {
                ez_log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Word文件名称解析
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="selectedKnowledgeGuid">选择的知识点优先使用</param>
        private void WordFileNameParse(string fileName)
        {

            TxtWordFileName.Text = fileName;

            var paperName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            var strArray = paperName.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 3)
            {
                throw new Exception("文件名不规范");
            }
            if (!Guid.TryParse(strArray[1], out Guid tempGuid))
            {
                throw new Exception("文件名不规范,取不到ResourcePaperGuid");
            }
            ez_resourcePaperGuid = strArray[1];
        }

        private void GetResourcePaperAndItemData()
        {
            ez_resourcePaper = null;
            OperationHelper.RetryAction(() =>
            {
                ez_resourcePaper = ez_resourcePaperService.GetResourcePaperForWordCutItem(ez_resourcePaperGuid);
            }, 3);

            this.Dispatcher.Invoke(() => {
                LblExamName.Content = $"考试名称：{ez_resourcePaper.Name ?? ""}";
                LblSubject.Content = $"科目：{ez_resourcePaper.Subject ?? ""}";
            });


            #region 验证试题的规范性

            if (ez_resourcePaper.ResouceItemList.Count == 0)
            {
                throw new Exception("试题列表为空");
            }
            if (ez_resourcePaper.ResouceItemList.Exists(x => string.IsNullOrWhiteSpace(x.ItemNumber)))
            {
                throw new Exception("存在空题号");
            }
            //验试是否存在重复的题号
            var groups = ez_resourcePaper.ResouceItemList
                .GroupBy(x => x.ItemNumber)
                .Where(x => x.Count() > 1)
                .ToList();
            if (groups.Count > 0)
            {
                throw new Exception($"存在重复的题号：{string.Join(" ; ", groups.ConvertAll(x => x.Key))}");
            }

            #endregion 验证试题的规范性

            this.Dispatcher.Invoke(() =>
            {
                ez_wordItemList.Clear();
                for (int i = 0; i < ez_resourcePaper.ResouceItemList.Count; i++)
                {
                    var item = ez_resourcePaper.ResouceItemList[i];
                    ez_wordItemList.Add(new WordItemNew
                    {
                        ItemGuid = item.Guid,
                        OrderNumber = i + 1,
                        Score = item.Score,
                        ItemNumber = item.ItemNumber,
                        OfficeItemNumber = item.OfficeItemNumber,
                        Subject = item.Subject,
                        ItemType = item.ItemTypeName,
                        ItemTypeGuid = item.ItemTypeGuid,
                        IsHaveWordInServer = item.IsHaveWord,
                        HistoryCutInfo = item.IsHaveWord ? "已保存上传" : "无",
                    });
                }
            });

            //DownloadPaperWord();

            //
        }

        /// <summary>
        /// 关闭所有的Word进程
        /// </summary>
        private void CloseAllWordProcess()
        {
            var list = System.Diagnostics.Process.GetProcessesByName("WINWORD");
            foreach (var item in list)
            {
                try
                {
                    item.Kill();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 初始化Word处理类
        /// </summary>
        /// <param name="isAfterCutImgAutoUpdate">切图后是否自动上传，</param>
        private void InitWordHandler()
        {
            if (_wordHandler == null)
            {
                _wordHandler = new DocTool.QuestionWord.ResourcePaperWordCutItemHandler(_mergeWordPath, _saveBaseDir, ez_wordItemList, false);
                _wordHandler.WordItemError += (s2, e2) =>
                {
                    var msg = $"第{e2.WordItem.OrderNumber}题{e2.OpType}失败，{e2.Exception.Message}";
                    ez_log.Error(msg);
                };
            }
        }

        /// <summary>
        /// 上传打过标签的文件
        /// </summary>
        private void UpdateMarkedWordFile()
        {
            if (System.IO.File.Exists(_mergeWordPath))
            {
                ez_resourcePaperService.ResourcePaperReplaceUploadMarkedWord(ez_resourcePaperGuid, _mergeWordPath);
            }
        }

        /// <summary>
        /// 保存DB数据
        /// </summary>
        private void WordCutItemSaveData(List<WordItemNew> wordItemNews)
        {
            //重置数据保存状态？

            //一次性保存所有数据
            var data = new
            {
                ResourcePaperGuid = ez_resourcePaper.Guid,

                CutResultInfoList = wordItemNews
                    .Select(x => new
                    {
                        Guid = x.ItemGuid,
                        ResourcePaperGuid = ez_resourcePaper.Guid,
                        ItemNumber = x.ItemNumber,
                        ItemTypeGuid = x.ItemTypeGuid,
                        Subject = x.Subject,
                        ContentText = GetImageText(x.ContentPageImages),
                        AnswerText = GetImageText(x.AnswerPageImages),
                        AnalysisText = "",//暂时不支持

                        SourceType = ez_resourcePaper.SourceType,
                    }).ToList(),
            };

            ez_cutItemService.WordCutSaveResourceItem(Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.None));

            foreach (var item in wordItemNews)
            {
                item.IsDataDealSuccess = true;
            }
        }

        /// <summary>
        /// 获取切图的内容
        /// </summary>
        /// <param name="imageUnits"></param>
        /// <returns></returns>
        private string GetImageText(List<PageImageUnit> imageUnits)
        {
            if (imageUnits.Count == 0)
                return "";

            var str = string.Join(" ", imageUnits.ConvertAll(x => x.Text ?? ""));
            if (!string.IsNullOrWhiteSpace(str))
            {
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
            }
            else
            {
                return "";
            }

            //return string.Join(" ", imageUnits.ConvertAll(x => x.Text ?? ""));
        }

        /// <summary>
        /// 保存文件数据(图片和Word)，因为每次保存为新增，所以，不需要删除原来的图片
        /// </summary>
        /// <param name="wordItemList"></param>
        /// <param name="isDeleteFile"></param>
        private void WordCutItemSaveFile(IList<WordItemNew> wordItemList, bool isDeleteFile = false)
        {
            foreach (var wordItem in wordItemList)
            {
                if (isDeleteFile)
                {
                    ez_cutItemService.WordCutItemDeleteFile2(wordItem.ItemGuid);
                }

                var wordCutItemSaveFileParamList = new List<WordCutItemSaveFileParam>();

                //if (_isStopUpdateFlag)
                //    break;

                //切图失败的不上传
                if (!wordItem.IsCutImgSuccess)
                    continue;
                //切Word失败的不上传
                if (!wordItem.IsCutWordSuccess)
                    continue;

                wordItem.ContentPageImages.ForEach(x =>
                {
                    wordCutItemSaveFileParamList.Add(new WordCutItemSaveFileParam
                    {
                        CutResultGuid = wordItem.ItemGuid,
                        ContentType = DataPlatformModel.TransferItemTraceService.ItemFileContentTypeEnum.Content,
                        FileType = DataPlatformModel.TransferItemTraceService.ItemFileTypeEnum.PageImg,
                        FilePath = x.ImagePath,
                        ImageIndex = x.Index,
                    });
                });
                wordItem.AnswerPageImages.ForEach(x =>
                {
                    wordCutItemSaveFileParamList.Add(new WordCutItemSaveFileParam
                    {
                        CutResultGuid = wordItem.ItemGuid,
                        ContentType = DataPlatformModel.TransferItemTraceService.ItemFileContentTypeEnum.Answer,
                        FileType = DataPlatformModel.TransferItemTraceService.ItemFileTypeEnum.PageImg,
                        FilePath = x.ImagePath,
                        ImageIndex = x.Index,
                    });
                });

                wordCutItemSaveFileParamList.Add(new WordCutItemSaveFileParam
                {
                    CutResultGuid = wordItem.ItemGuid,
                    ContentType = DataPlatformModel.TransferItemTraceService.ItemFileContentTypeEnum.Content,
                    FileType = DataPlatformModel.TransferItemTraceService.ItemFileTypeEnum.Word,
                    FilePath = wordItem.ContentWordPath,
                });
                wordCutItemSaveFileParamList.Add(new WordCutItemSaveFileParam
                {
                    CutResultGuid = wordItem.ItemGuid,
                    ContentType = DataPlatformModel.TransferItemTraceService.ItemFileContentTypeEnum.Answer,
                    FileType = DataPlatformModel.TransferItemTraceService.ItemFileTypeEnum.Word,
                    FilePath = wordItem.AnswerWordPath,
                });

                #region 并发上传

                Parallel.ForEach(wordCutItemSaveFileParamList, x =>
                {
                    try
                    {
                        var result = ez_cutItemService.WordCutItemSaveFile2(x);
                        if (result.Item1)
                        {
                            x.IsSaveSuccess = true;
                        }
                        else
                        {
                            x.IsSaveSuccess = false;
                            x.Messge = result.Item2;
                        }
                    }
                    catch (Exception inEx)
                    {
                        x.IsSaveSuccess = false;
                        x.Messge = inEx.Message;
                    }
                });

                #endregion 并发上传

                #region 同步上传

                //wordCutItemSaveFileParamList.ForEach(x =>
                //{
                //    try
                //    {
                //        var result = ez_cutItemService.WordCutItemSaveFile2(x);
                //        if (result.Item1)
                //        {
                //            x.IsSaveSuccess = true;
                //        }
                //        else
                //        {
                //            x.IsSaveSuccess = false;
                //            x.Messge = result.Item2;
                //        }
                //    }
                //    catch (Exception inEx)
                //    {
                //        x.IsSaveSuccess = false;
                //        x.Messge = inEx.Message;
                //    }
                //});

                #endregion 同步上传

                #region 题干图片

                var contentImageList = wordCutItemSaveFileParamList.Where(y => y.ContentType == ItemFileContentTypeEnum.Content
                                                                            && y.FileType == ItemFileTypeEnum.PageImg).ToList();
                if (contentImageList.All(x => x.IsSaveSuccess))
                {
                    wordItem.ContentImgUpdateInfo = $"全部上传成功,共{contentImageList.Count}个文件";
                }
                else
                {
                    if (contentImageList.All(x => !x.IsSaveSuccess))
                    {
                        wordItem.ContentImgUpdateInfo = $"全部上传失败,共{contentImageList.Count}个文件";
                    }
                    else
                    {
                        wordItem.ContentImgUpdateInfo = $"上传结果，成功：{contentImageList.Count(x => !x.IsSaveSuccess)}，失败：{contentImageList.Count(x => !x.IsSaveSuccess)}";
                    }

                    ez_log.Error("上传题干图片失败,{0}", string.Join(",", contentImageList.Where(x => !x.IsSaveSuccess).Select(x => x.Messge).ToList()));
                }

                #endregion 题干图片

                #region 答案图片

                var answerImageList = wordCutItemSaveFileParamList.Where(y => y.ContentType == ItemFileContentTypeEnum.Answer
                                                                            && y.FileType == ItemFileTypeEnum.PageImg).ToList();
                if (answerImageList.All(x => x.IsSaveSuccess))
                {
                    wordItem.AnswerImgUpdateInfo = $"全部上传成功,共{answerImageList.Count}个文件";
                }
                else
                {
                    if (answerImageList.All(x => !x.IsSaveSuccess))
                    {
                        wordItem.AnswerImgUpdateInfo = $"全部上传失败,共{answerImageList.Count}个文件";
                    }
                    else
                    {
                        wordItem.AnswerImgUpdateInfo = $"上传结果，成功：{answerImageList.Count(x => !x.IsSaveSuccess)}，失败：{answerImageList.Count(x => !x.IsSaveSuccess)}";
                    }

                    ez_log.Error("上传答案图片失败,{0}", string.Join(",", answerImageList.Where(x => !x.IsSaveSuccess).Select(x => x.Messge).ToList()));
                }

                #endregion 答案图片

                #region 题干Word

                var contentWordList = wordCutItemSaveFileParamList.Where(y => y.ContentType == ItemFileContentTypeEnum.Content
                                                                            && y.FileType == ItemFileTypeEnum.Word).ToList();
                if (contentWordList.All(x => x.IsSaveSuccess))
                {
                    wordItem.ContentWordUpdateInfo = $"全部上传成功,共{contentWordList.Count}个文件";
                }
                else
                {
                    if (contentWordList.All(x => !x.IsSaveSuccess))
                    {
                        wordItem.ContentWordUpdateInfo = $"全部上传失败,共{contentWordList.Count}个文件";
                    }
                    else
                    {
                        wordItem.ContentWordUpdateInfo = $"上传结果，成功：{contentWordList.Count(x => !x.IsSaveSuccess)}，失败：{contentWordList.Count(x => !x.IsSaveSuccess)}";
                    }

                    ez_log.Error("上传题干Word失败,{0}", string.Join(",", contentWordList.Where(x => !x.IsSaveSuccess).Select(x => x.Messge).ToList()));
                }

                #endregion 题干Word

                #region 答案Word

                var answerWordList = wordCutItemSaveFileParamList.Where(y => y.ContentType == ItemFileContentTypeEnum.Answer
                                                                            && y.FileType == ItemFileTypeEnum.Word).ToList();
                if (answerWordList.All(x => x.IsSaveSuccess))
                {
                    wordItem.AnswerWordUpdateInfo = $"全部上传成功,共{answerWordList.Count}个文件";
                }
                else
                {
                    if (answerWordList.All(x => !x.IsSaveSuccess))
                    {
                        wordItem.AnswerWordUpdateInfo = $"全部上传失败,共{answerWordList.Count}个文件";
                    }
                    else
                    {
                        wordItem.AnswerWordUpdateInfo = $"上传结果，成功：{answerWordList.Count(x => !x.IsSaveSuccess)}，失败：{answerWordList.Count(x => !x.IsSaveSuccess)}";
                    }

                    ez_log.Error("上传答案Word失败,{0}", string.Join(",", answerWordList.Where(x => !x.IsSaveSuccess).Select(x => x.Messge).ToList()));
                }

                #endregion 答案Word

                //试题所有文件上传成功，才是成功
                wordItem.IsUploadFileSuccess = wordCutItemSaveFileParamList.All(x => x.IsSaveSuccess);
            }
        }

    }
}
