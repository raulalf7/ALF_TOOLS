using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ALF.MSSQL;

namespace ALF.EDU.Gadgets.UserControl
{
    /// <summary>
    ///     Interaction logic for DataProvinceView.xaml
    /// </summary>
    public partial class DataProvinceView
    {
        public DataProvinceView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CreateData(GetSelectList());
        }

        private void TypeCombo_OnSelectionChanged(object sender, EventArgs e)
        {
        }

        private void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            var tmp = GadegtTools.CheckConn(ipText.Value, pwText.Value);
            if (tmp != "")
            {
                infoText.Text += tmp + "\n";
                MessageBox.Show(tmp);
                return;
            }
            if (typeCombo.SelectedIndex == 0)
            {
                LoadData("");
                return;
            }
            LoadData(typeCombo.SelectedItem.ToString());
        }

        private void LoadData(string type)
        {
            var tmp = "";
            if (type == "")
            {
                dataGrid.ItemsSource = Tools.GetSqlDataView(@"SELECT [templateNo],[templateName],[templateNoDisplay],executeSqlString  
                                              FROM [eduData2015DB].[dbo].[excelTemplateTable]
                                             where templateGroup in ('综表','县综')
                                               and templateNoDisplay not like '%分项%'
                                               and templateOwner=''
                                             order by templateName", out tmp);
                return;
            }
            dataGrid.ItemsSource =
                Tools.GetSqlDataView(string.Format(@"SELECT [templateNo],[templateName],[templateNoDisplay],executeSqlString  
                                              FROM [eduData2015DB].[dbo].[excelTemplateTable]
                                             where templateGroup in ('综表','县综')
                                               and templateNoDisplay not like '%分项%'
                                               and templateOwner=''
                                               and templateType='{0}'
                                               and templateNo not like '%1122%'
                                             order by templateName", type), out tmp);
        }

        private string createSqlString(string templateNo, string templateName, string region, string tmpColString,
            string tableColString, string execStringFormat, string regionANO)
        {



            var execString = string.Format(execStringFormat, 0, region);

            var createTmpString = string.Format(@"
                create table #{1}_tmp
                        (regionA nvarchar(50)
                        ,regionANo nvarchar(50)
                        ,templateNo nvarchar(50)
                        ,DID int
                        ,headType nvarchar(50)
                        ,A int
                        ,B nvarchar(50)
                        ,C nvarchar(50)
                        ,D nvarchar(50)
                        {2})", templateNo, templateName, tableColString);

            string tmp;
            var list = Tools.GetSqlListString("select distinct columnTag from eduData2015DB..excelTemplateCell where templateNo='" + templateNo + "' and showBackgroundColor in('FFFFFF', '8DB4E3')", out tmp);

            var name = list[0];
            if (name == "D")
            {
                createTmpString = createTmpString.Replace(",D nvarchar(50)", ",D numeric(18,2)");
            }

            var insertTmpString = string.Format(@"                        

             insert into #{1}_tmp
             (templateNo,DID,headType,A,B,C,D{2}) 
             execute eduData2015DB..{0} {3}", templateNo, templateName, tmpColString, execString);


            var insertFinalString = string.Format(@"

                 insert into eduTotal2015DB..{3}
                 select '{0}' as regionA,'{1}' as regionANo,templateNo,DID,headType,A,B,C,D{2}
                   from #{3}_tmp", region, regionANO, tmpColString, templateName);

            var dropTmpTableString = string.Format(@"                        
              drop table #{1}_tmp", templateNo, templateName);


            return createTmpString + insertTmpString + insertFinalString + dropTmpTableString;
        }

        private List<DataRowView> GetSelectList()
        {
            return (from object item in dataGrid.SelectedItems select item as DataRowView).ToList();
        }

        private void CreateData(List<DataRowView> tableList)
        {
            var tmp = "";

            var regionAList =
                Tools.GetSqlDataView("select regionA,regionANo from eduData2015DB..codeRegionA order by regionANo",
                    out tmp);

            foreach (var item in tableList)
            {
                Console.WriteLine(@"生成{0}", item[1]);
                var tableColString = "";
                var tmpColString = "";
                Tools.ExecSql(string.Format("drop table eduTotal2015DB..{0}", item[1]));

                List<string> colNameList = Tools.GetSqlListString(
                    string.Format(@"SELECT expression
                                      FROM [eduData2015DB].[dbo].[excelTemplateCell]
                                     where templateNo='{0}'
                                       and row=1", item[0]),out tmp);


                string result;

                var list = Tools.GetSqlListString(string.Format("select columnTag from (select distinct columnTag,len(columnTag) colLength from eduData2015DB..excelTemplateCell where templateNo='{0}' and showBackgroundColor in('FFFFFF', '8DB4E3') )t  order by colLength,columnTag",item[0]), out result);

                var name = list[0];




                foreach (var col in colNameList)
                {
                    if (col.ToUpper() == "A" || col.ToUpper() == "B" || col.ToUpper() == "C" || col.ToUpper() == "D")
                    {
                        continue;
                    }
                    tmpColString += "," + col;
                    tableColString += string.Format(@"
                        ,{0} numeric(18,2)
                        ", col);
                }


                var createFinalTable = string.Format(@"
                create table eduTotal2015DB..{0}
                        (regionA nvarchar(50)
                        ,organizationNo nvarchar(50)
                        ,templateNo nvarchar(50)
                        ,DID int
                        ,headType nvarchar(50)
                        ,A int
                        ,B nvarchar(50)
                        ,C nvarchar(50)
                        ,D nvarchar(50)
                        {1})", item[1], tableColString);


                if (name == "D")
                {
                    createFinalTable = createFinalTable.Replace(",D nvarchar(50)", ",D numeric(18,2)");
                }


                Tools.ExecSql(createFinalTable);

                foreach (DataRow region in regionAList.Table.Rows)
                {
                    var finalString = createSqlString(item[0].ToString(), item[1].ToString(), region[0].ToString(),
                        tmpColString, tableColString, item[3].ToString(), region[1].ToString());
                    tmp = Tools.ExecSql(finalString);
                    if (tmp != "")
                    {
                        Console.WriteLine(@"生成{0}发生错误：{1}", item[1],tmp);
                    }
                }
                var edufinalString = createSqlString(item[0].ToString(), item[1].ToString(), "", tmpColString,
                    tableColString, item[2].ToString(), "360");
                Tools.ExecSql(edufinalString);
                Console.WriteLine(@"生成{0}完成", item[1]);
            }
        }

        private void QuitButton_OnClick(object sender, RoutedEventArgs e)
        {
            var control = Parent as ContentControl;
            if (control != null) control.Visibility = Visibility.Collapsed;
        }
    }
}