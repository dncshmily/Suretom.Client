using SQLite;

namespace Suretom.Client.UI.Extensions
{
    public static class SQLiteConnectionExtension
    {
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool TableExist(this SQLiteConnection conn, string tableName)
        {
            var count = conn.ExecuteScalar<int>("select count(*) from sqlite_master where type = 'table' and name = ?", tableName);
            return count > 0;
        }

        /// <summary>
        /// 清空表数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        //public static long ClearTable(this SQLiteConnection conn, string tableName)
        //{
        //    return conn.Execute($"delete from {tableName}");
        //}
    }
}