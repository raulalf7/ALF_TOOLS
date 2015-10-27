using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ALF.MSSQL;
using ALF.MSSQL.DataModel;
using ALF.SYSTEM;
using ALF.SYSTEM.DataModel;

namespace ALF.EduBusinessCellCheck
{
    internal class Program
    {
        private static void Main()
        {
            while (true)
            {
                SetConn();
            }
        }

        private static void SetConn()
        {
            Console.WriteLine("输入数据库连接编号:1. sqlexpress  2. sqlserver  3.远程数据库");
            var t = Console.ReadLine();
            if (t == "1")
            {
                Tools.DataBaseType = DataBaseEngineType.SqlExpress;
            }
            else if (t == "2")
            {
                Tools.DataBaseType = DataBaseEngineType.MsSqlServer;
            }
            else if (t == "3")
            {
                Console.WriteLine("请输入IP地址：");
                var ip = Console.ReadLine();
                Console.WriteLine("请输入密码：");
                var password = Console.ReadLine();
                Tools.ConnInfo = new ConnInfo {ConnIp = ip, ConnPw = password};
                Tools.DataBaseType = DataBaseEngineType.Remote;
            }
            else
            {
                Console.WriteLine("输入错误");
                return;
            }
            Console.WriteLine("开始生成核查语句");
            DataCheck();
            Console.WriteLine("生成完成，请见D盘sql.txt");
        }

        private static void DataCheck()
        {
            string tmp;
            var sql =
                @" SELECT templateNo,businessTypeNo,did,columnTag FROM [eduData2015DB].[dbo].[excelTemplatePower] where len(templateNo)<5 order by templateNo,columnTag,did,businessTypeNo";
            var result = Tools.GetSqlDataView(sql, out tmp).Table;
            var currentTemplateNo = "";
            var currentColumnnTag = "";
            var currentRow = "";
            var currentCondition = " ";
            var sqlList = new List<string>();
            foreach (DataRow row in result.Rows)
            {
                if ((row[0].ToString() != currentTemplateNo || row[2].ToString() != currentRow ||
                     row[3].ToString() != currentColumnnTag))
                {
                    sqlList.Add(
                        string.Format(
                            "select '{0}',organizationNo from eduData2015DB..{0}_R where did ={1} and {2}!=0 and businessTypeNo not in ({3}) \n",
                            currentTemplateNo, currentRow, currentColumnnTag,
                            currentCondition.Substring(0, currentCondition.Length - 1)));
                    currentCondition = row[1] + ",";
                }
                else
                {
                    currentCondition += string.Format("{0},", row[1]);
                }
                currentTemplateNo = row[0].ToString();
                currentRow = row[2].ToString();
                currentColumnnTag = row[3].ToString();
            }

            sqlList.RemoveAt(0);
            var sqlString = sqlList.Aggregate("", (current, sqlItem) => current + sqlItem);
            WindowsTools.WriteToTxt(@"d:\sql.txt", sqlString);
        }
    }
}
