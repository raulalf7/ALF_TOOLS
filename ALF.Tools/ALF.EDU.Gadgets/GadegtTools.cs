using System;
using System.Globalization;
using System.Linq;
using ALF.MSSQL;
using ALF.MSSQL.DataModel;
using ALF.SYSTEM.DataModel;

namespace ALF.EDU.Gadgets
{
    public static class GadegtTools
    {
        public static string CheckConn(string ip, string pw)
        {
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(pw))
            {
                return @"请填写链接和密码";
            }
            Tools.DataBaseType = DataBaseEngineType.Remote;
            Tools.ConnInfo = new ConnInfo { ConnIp = ip, ConnPw = pw };
            if (!Tools.IsDBOpen())
            {
                return @"请确认链接、密码正确";
            }
            return "";
        }

        public static string CreateInstanceID(string organizationNo, string templateNo, string templateName, string templateNoDisplay)
        {
            string str = organizationNo + "0000";
            string text = templateNo.ToUpper().Aggregate("", delegate(string current, char c)
            {
                var num = (int)c;
                return current + num.ToString(CultureInfo.InvariantCulture);
              
            }
            );
            text = string.Format("{0:X}", long.Parse(text));
            string text2 = str  + text;
            string str3 = new string('0', 32 - text2.Length);
            text2 += str3;
            var instanceID = Guid.Parse(text2);


            string insertString = string.Format(@"
INSERT INTO eduData2015DB..[instanceTable]
           ([rowID]
           ,[state]
           ,[description]
           ,[updateTime]
           ,[recordYear]
           ,[instanceID]
           ,[organizationNo]
           ,[businessID]
           ,[businessTypeNo]
           ,[templateNo]
           ,[templateName]
           ,[templateNoDisplay]
           ,[dataVer]
           ,[approveState])
     VALUES
           ('{0}'
           ,NULL
           ,NULL
           ,'{1}'
           ,2015
           ,'{2}'
           ,{3}
           ,'00000000-0000-0000-0000-000000000000'
           ,'管理'
           ,'{4}'
           ,'{5}'
           ,'{6}'
           ,0
           ,1)
", Guid.NewGuid(), DateTime.Now, instanceID, organizationNo,templateNo, templateName, templateNoDisplay);
            var query = string.Format("exec eduData2015DB..sys_createInstanceData 2015 , '{0}','{1}'", templateNo, instanceID);
            var tmp = Tools.ExecSql(insertString);
            return tmp != "" ? tmp : Tools.ExecSql(query);
        }
    }
}
