using System;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 操作帮助类，封装Action和Func的一些常用操作
    /// </summary>
    public static class OperationHelper
    {
        /// <summary>
        /// 重试操作
        /// </summary>
        /// <param name="action">要重试的函数</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="interval">重试间隔</param>
        public static void RetryAction(Action action, int retryCount, int interval = 100)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    action();

                    //执行成功，则直接返回
                    return;
                }
                catch (Exception inEx)
                {
                    //只抛出最后一次的失败
                    if (i == retryCount - 1)
                        throw inEx;
                }

                System.Threading.Thread.Sleep(interval);
            }
        }
    }
}