using PrintManager.Sql.BLL;
using PrintManager.Sql.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintManager.Sql
{
    public class SqlSugarHelper
    {
        #region 单列模式

        private static SqlSugarHelper instance = null;
        private static readonly object padlock = new object();
        public static SqlSugarHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SqlSugarHelper();
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new SqlSugarHelper();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        //private string ConnectionString = "Server=localhost\\MYSQLSERVER;Database=db_printmanager;Trusted_Connection=True";//连接符字串
        //private string ConnectionString = "Data Source=localhost\\MYSQLSERVER;Initial Catalog=db_client;Trusted_Connection=True;User Id=sa;Password=Admin123;";//连接符字串
        private string ConnectionString = "Data Source=localhost;Initial Catalog=db_client;Trusted_Connection=True;User Id=sa;Password=Admin123;";//连接符字串
        //用单例模式
        public SqlSugarScope db;

        public SqlSugarHelper()
        {
            SqlConfig();
        }

        private void SqlConfig()
        {
            db = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,//连接符字串
                                                    
                DbType = DbType.SqlServer,//数据库类型
                IsAutoCloseConnection = true, //不设成true要手动close,
            },
            db => {
                //(A)全局生效配置点，一般AOP和程序启动的配置扔这里面 ，所有上下文生效
                //调试SQL事件，可以删掉
                //db.Aop.OnLogExecuting = (sql, pars) =>
                //{
                //    Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响


                //    //5.0.8.2 获取无参数化 SQL  对性能有影响，特别大的SQL参数多的，调试使用
                //    //UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
                //};

                //多个配置就写下面
                //db.Ado.IsDisableMasterSlaveSeparation=true;
                //db.Ado.CommandTimeOut = 2;//超时配置 单位秒
                //如果时间长，可以在连接字符串配置 连接超时时间
            });
        }

        public void SetConnectString(string connectString)
        {
            ConnectionString = connectString;
            SqlConfig();
        }
        public void ConnSql()
        {
            SqlConfig();
        }

        public bool IsConnection
        {
            get => db.Ado.IsValidConnection();
        }

        public List<T> Find<T>(Expression<Func<T, bool>> funWhere)
        {
            return db.Queryable<T>().Where(funWhere).ToList();
        }


        public bool CreateDatabase()
        {
            db.DbMaintenance.CreateDatabase();//没有数据库则新建
            return true;
        }

        public bool CreateTable()
        {
            if (!db.DbMaintenance.IsAnyTable("ProductOrder", false))//判断状态表是否存在 
            {
                //db.CodeFirst.SetStringDefaultLength(250).BackupTable().InitTables(new Type[]
                //{
                //    typeof(Pr{oductOrder),         //根据CodeFirstTable1实体创建表
                //});
                db.CodeFirst.SetStringDefaultLength(250).BackupTable().InitTables<ProductOrder>();
            }
            if (!db.DbMaintenance.IsAnyTable("HistoryLog", false))
            {
                db.CodeFirst.BackupTable().InitTables<HistoryLog>();
            }
            if (!db.DbMaintenance.IsAnyTable("Customer", false))
            {
                db.CodeFirst.SetStringDefaultLength(250).BackupTable().InitTables<Customer>();
            }
            if (!db.DbMaintenance.IsAnyTable("ProductOrderLog", false))
            {
                db.CodeFirst.SetStringDefaultLength(250).BackupTable().InitTables<ProductOrderLog>();
            }
            if (!db.DbMaintenance.IsAnyTable("User", false))
            {
                db.CodeFirst.SetStringDefaultLength(250).BackupTable().InitTables<User>();
            }
            if (!db.DbMaintenance.IsAnyTable("LanguageText", false))
            {
                db.CodeFirst.SetStringDefaultLength(250).BackupTable().InitTables<LanguageText>();
            }
            if (!db.DbMaintenance.IsAnyTable("Analog", false))
            {
                db.CodeFirst.SetStringDefaultLength(250).BackupTable().InitTables<Analog>();
            }
            return true;
        }

        public void InitAdminUser()
        {
            //db.Insertable(new User() 
            //{ 
            //    Name = "Admin", 
            //    UserName = "Admin", 
            //    Password = "123456", 
            //    IsAdmin = true, 
            //    Gender = 1, 
            //    Desc = "Administrator" 
            //}).ExecuteCommand();
        }

        public bool DeleteTable(string tableName)
        {
            try
            {
                db.DbMaintenance.DeleteTableRemark(tableName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteAllTable()
        {
            try
            {
                var tables = db.DbMaintenance.GetTableInfoList();
                foreach (var table in tables)
                {
                    db.Deleteable(table);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
