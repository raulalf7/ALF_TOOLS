using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ALF.MSSQL;

namespace ALF.EDU.Gadgets.UserControl
{
    /// <summary>
    ///     Interaction logic for CountyInstanceSupply.xaml
    /// </summary>
    public partial class CountyInstanceSupply
    {
        public CountyInstanceSupply()
        {
            InitializeComponent();
            Tools.DBName = "eduData2015DB";
        }

        //private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{

        //}

        //private void QuitButton_OnClick(object sender, RoutedEventArgs e)
        //{

        //}

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var tmp = GadegtTools.CheckConn(ipText.Value, pwText.Value);
            if (tmp != "")
            {
                infoText.Text += tmp + "\n";
                MessageBox.Show(tmp);
                return;
            }
            var query = @"
 
 select distinct  a.statisticsOrganizationNo,b.templateNo,b.templateName,b.templateNoDisplay,statisticsOrganizationName,statisticsRegionA,statisticsRegionB,statisticsRegionC from
(select statisticsOrganizationNo,statisticsOrganizationName,statisticsRegionA,statisticsRegionB,statisticsRegionC from eduData2015DB..statisticsEntity 
  where CASE WHEN  isImmediacy=1 THEN  statisticsOrganizationLevel  ELSE statisticsOrganizationLevel-1 END =2)a    
  left JOIN  
(SELECT TEMPLATENO,templateName,templateNoDisplay FROM eduData2015DB..excelTemplateStatisticsRelation where templateNo in ('ZX311','ZX211')) b
     ON 1=1
  left join
(select * from eduData2015DB..instanceTable where templateNo in ('ZX311','ZX211'))c
     on a.statisticsOrganizationNo=c.organizationNo
    and b.templateNo=c.templateNo
  where instanceID is null 
  UNION 
 select distinct a.statisticsOrganizationNo,b.templateNo,b.templateName,b.templateNoDisplay,statisticsOrganizationName,statisticsRegionA,statisticsRegionB,statisticsRegionC from
 (select statisticsOrganizationNo,statisticsOrganizationName,statisticsRegionA,statisticsRegionB,statisticsRegionC from eduData2015DB..statisticsEntity 
  where  CASE WHEN  isImmediacy=1 THEN  statisticsOrganizationLevel  ELSE statisticsOrganizationLevel-1 END =2)a    
  left JOIN  
(SELECT TEMPLATENO,templateName,templateNoDisplay FROM eduData2015DB..excelTemplateStatisticsRelation where templateNo in ('JX31','JX32','JX2')) b
     ON 1=1
  left join
(select * from eduData2015DB..instanceTable where templateNo in ('JX31','JX32','JX2'))c
     on a.statisticsOrganizationNo=c.organizationNo
    and b.templateNo=c.templateNo
  where instanceID is null and left(statisticsOrganizationNo,2)<>'23'
  order by templateNo,statisticsOrganizationNo
";

            var data = Tools.GetSqlDataView(query, out tmp);
            if (tmp != "")
            {
                infoText.Text += string.Format("{0}\n", tmp);
                return;
            }
            var count = 0;
            foreach (DataRow row in data.Table.Rows)
            {
                await GetData(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString());
                count++;
                if (count % 100 != 0) continue;
                infoText.Text += string.Format("已补充{0}条\n", count);
            }
            infoText.Text += string.Format("补充完成，已补充{0}条\n", count);




        }

        private void QuitButton_OnClick(object sender, RoutedEventArgs e)
        {
            var control = Parent as ContentControl;
            if (control != null) control.Visibility = Visibility.Collapsed;
        }

        private async Task<string> GetData(string organizationNo, string templateNo, string templateName, string templateNoDisplay)
        {
            return await Task.Run(() => GadegtTools.CreateInstanceID(organizationNo, templateNo, templateName, templateNoDisplay));
        }

        private async void DoubleButton_OnClick(object sender, RoutedEventArgs e)
        {

            var tmp = GadegtTools.CheckConn(ipText.Value, pwText.Value);
            if (tmp != "")
            {
                infoText.Text += tmp + "\n";
                MessageBox.Show(tmp);
                return;
            }

            infoText.Text += "\n\n\n ***********开始清理数据表中重复实例数据\n";
            var tableList = Tools.GetSqlListString(
                @"select   templateNo  from  eduData2015DB..excelTemplateTable where templateGroup='基表' and templateOwner='' and templateCategory<>'浮动行表' order by templateNo",
                out tmp);
            if (tmp != "")
            {
                infoText.Text += string.Format("{0}\n", tmp);
                return;
            }

            if (tmp != "")
            {
                infoText.Text += string.Format("{0}\n", tmp);
                return;
            }

            foreach (var table in tableList)
            {
                var sql =
                    string.Format(
                        "select instanceID,did  as templateNo from eduData2015DB..{0} group by instanceID,did having count(1)>1", table);
                var dataList = Tools.GetSqlDataView(sql, out tmp);
                if (tmp != "")
                {
                    infoText.Text += string.Format("{0}\n", tmp);
                    return;
                }
                if (dataList==null ||dataList.Table.Rows.Count == 0)
                {
                    continue;
                }



                var colList =
                    Tools.GetSqlListString(
                        string.Format(
                            @"SELECT [字段名]  FROM [eduData2015DB].[dbo].[表信息] where [表名] = '{0}' and  [字段名] <>'rowid'  ORDER BY 字段序号",
                            table), out tmp);

                var selectString = "select  distinct  ";
                foreach (var colName in colList)
                {
                    selectString +=  colName +"," ;
                }
                selectString = selectString.Substring(0, selectString.Length - 1);
                selectString +=
                    string.Format(
                        " into eduData2015DB..MultiData_{0} from  eduData2015DB..{0} \n  truncate table eduData2015DB..{0} \n  insert into  eduData2015DB..{0} select newid() as rowid,* from eduData2015DB..MultiData_{0} ",
                        table);
                var t = Tools.ExecSql(selectString);
                infoText.Text += string.Format("{0}处理完成\n", table);
                if(t!="")
                {
                    infoText.Text += string.Format("{0}错误：{1}\n", table,t);
                }

            }
        }

        private async Task<string> ExecSqlAsync(string sql)
        {
            return await Task.Run(() => Tools.ExecSql(sql));
        }
    }
}