using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 本地目录管理
    /// </summary>
    public class LocalDirectoryConfig
    {
        /// <summary>
        /// 添加目录
        /// </summary>
        /// <param name="dir"></param>
        public void AddDirectory(string dir)
        {
            if (DirectoryDic.ContainsKey(dir))
            {
                throw new CtbException("目录已存在");
            }
            else
            {
                DirectoryDic.Add(dir, Path.Combine(Environment.CurrentDirectory, "Temp", dir));
            }
        }

        /// <summary>
        /// 获取指定的目录
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                if (!System.IO.Directory.Exists(DirectoryDic[name]))
                {
                    System.IO.Directory.CreateDirectory(DirectoryDic[name]);
                }

                return DirectoryDic[name];
            }
        }

        /// <summary>
        /// 获取目录字典
        /// </summary>
        public IDictionary<string, string> DirectoryDic { get; } = new Dictionary<string, string>();

        /// <summary>
        /// 清理本地的目录
        /// </summary>
        public void ClearDirectory()
        {
            Task.Run(() =>
            {
                var deleteTime = DateTime.Now.AddDays(-1);
                //var deleteTime = DateTime.Now.AddSeconds(-10);

                foreach (var item in DirectoryDic.Values)
                {
                    try
                    {
                        if (Directory.Exists(item))
                        {
                            var directories = Directory.GetDirectories(item);
                            Parallel.ForEach(directories, dir =>
                            {
                                try
                                {
                                    var dirInfo = new DirectoryInfo(dir);
                                    if (dirInfo.Exists && dirInfo.CreationTime < deleteTime)
                                    {
                                        dirInfo.Delete(true);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            });

                            var files = Directory.GetFiles(item);
                            Parallel.ForEach(files, file =>
                            {
                                try
                                {
                                    var fileInfo = new FileInfo(file);
                                    if (fileInfo.Exists && fileInfo.CreationTime < deleteTime)
                                    {
                                        fileInfo.Delete();
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }
    }
}