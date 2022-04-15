using System;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 操作帮助类，封装Action和Func的一些常用操作
    /// </summary>
    public static class OperationHelper
    {
        /// <summary>
        /// 异常重试操作
        /// </summary>
        /// <param name="action">要重试的函数</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="interval">重试间隔</param>
        public static void RetryExceptionAction(Action action, int retryCount, int interval, ref bool isResult)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    action();

                    //执行成功，则直接返回
                    isResult=false;
                }
                catch (Exception inEx)
                {
                    //只抛出最后一次的失败
                    if (i == retryCount - 1)
                    {
                        isResult =true;
                        //throw inEx;
                    }
                }

                System.Threading.Thread.Sleep(interval);
            }
        }

        /// <summary>
        /// 重复调用
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryCount"></param>
        /// <param name="interval"></param>
        /// <param name="isResult"></param>
        public static void RetrySucessAction(Action action, int retryCount, int interval, ref bool isResult)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    action();

                    //执行成功
                    isResult=false;

                    //只抛出最后一次的失败
                    if (i == retryCount - 1)
                    {
                        isResult =true;
                    }
                }
                catch
                {
                }

                System.Threading.Thread.Sleep(interval);
            }
        }
    }
}