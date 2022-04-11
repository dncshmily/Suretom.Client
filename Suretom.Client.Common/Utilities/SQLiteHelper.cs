using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Suretom.Client.Common
{
    public class SQLiteHelper
    {
        public string connstr = Path.Combine(Environment.CurrentDirectory, "User.db");//没有数据库会创建数据库
        public SQLiteConnection db;

        public SQLiteHelper()
        {
            db = new SQLiteConnection(connstr);
            //db.CreateTable<Stock>();//表已存在不会重复创建
            //db.CreateTable<Valuation>();
        }

        public int Add<T>(T model)
        {
            return db.Insert(model);
        }

        public int Update<T>(T model)
        {
            return db.Update(model);
        }

        public int Delete<T>(T model)
        {
            return db.Update(model);
        }

        public List<T> Query<T>(string sql) where T : new()
        {
            return db.Query<T>(sql);
        }

        public int Execute(string sql)
        {
            return db.Execute(sql);
        }
    }

    public class Valuation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed("ValuationStockId2", 1)] //索引，注意，该索引在表创建时，会创建，如果索引改名，旧索引依然存在，并未被删除
        public int StockId { get; set; }

        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }

    public class Stock
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Symbol { get; set; }
    }
}