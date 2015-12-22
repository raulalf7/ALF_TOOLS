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
        }

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
  where instanceID is null and left(statisticsOrganizationNo,2)<>'23'
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
            if(tmp!="")
            {
                infoText.Text += string.Format("{0}n", tmp);
                return;
            }
            var count = 0;
            foreach (DataRow row in data.Table.Rows)
            {
                await GetData(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString());
                count++;
                if (count%100 != 0) continue;
                infoText.Text += string.Format("已补充{0}条\n", count);
            }
            infoText.Text += string.Format("补充完成，已补充{0}条\n", count);
        }

        private void QuitButton_OnClick(object sender, RoutedEventArgs e)
        {
            var control = Parent as ContentControl;
            if (control != null) control.Visibility=Visibility.Collapsed;
        }

        private async Task<string> GetData(string organizationNo, string templateNo, string templateName, string templateNoDisplay)
        {
            return await Task.Run(() => GadegtTools.CreateInstanceID(organizationNo, templateNo, templateName, templateNoDisplay));
        }
    }
}