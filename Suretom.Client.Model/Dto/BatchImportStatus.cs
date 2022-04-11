namespace Suretom.Client.Entity
{
    public enum BatchImportStatus
    {
        未开始 = 0,
        初始化失败 = 1,
        正在处理 = 2,
        成功 = 3,
        失败 = 4,
        手动处理 = 5,
        部分成功 = 6,
    }
}