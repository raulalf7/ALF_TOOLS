using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using ALF.EDU;
using ALF.EDU.DataModel;
using ALF.SYSTEM;

namespace DataReport_XP
{
    public static class Tools
    {
        public static int RecordYear = 2016;

        public static string Ver = "5.1";
        public static string Title = "";

        private static string _tmpResult;

        public static string TemplateConfigPath = @".\templateConfigFile.xml";

        public static string SystemConfigPath = @".\systemConfigFile.xml";

        public static string ArgConfigPath = @".\argConfigFile.xml";

        public static List<KeyInfo> KeyInfoList
        {
            get
            {
                string result;
                var tmp =
                    WindowsTools.XmlDeseerializer(typeof(List<KeyInfo>), SystemConfigPath, out result) as
                        List<KeyInfo>;
                if (result != "")
                {
                    ShowError(200);
                    return null;
                }
                return tmp;
            }
        }

        public static List<WordInfo> CreateReportFile(List<TemplateInfo> templateInfoList,
            List<return_getRegionTreeNodeList> regionList, int appType, out string result)
        {
            var resultList = new List<WordInfo>();
            result = "";
            foreach (var itemRegion in regionList)
            {
                var regionString = itemRegion.node1 + itemRegion.node2 + itemRegion.node3 + itemRegion.node4 +
                                   itemRegion.node5;
                foreach (var itemFile in templateInfoList)
                {
                    var wordInfo = new WordInfo
                    {
                        rowid = Guid.NewGuid(),
                        templateID = itemFile.templateID,
                        templateName = itemFile.templateName,
                        regionPath = itemRegion.node1 + itemRegion.node2 + itemRegion.node3 + itemRegion.node4,
                        regionType = itemRegion.nodeOrganizationType,
                        wordID = Guid.NewGuid()
                    };


                    var argInfoList = GetArgInfoList(itemFile, out result);

                    if (result != "")
                    {
                        ShowError(202, result);
                        return resultList;
                    }

                    var destFilePath = string.Format(@"{0}\reportFiles\{1}_{2}_{3}", Environment.CurrentDirectory,
                        wordInfo.regionPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), itemFile.templateName);
                    File.Copy(itemFile.templatePath, destFilePath, true);

                    result = ReportOfficeTools.UpdateWord(argInfoList, regionString, appType, destFilePath,false);

                    if (result != "")
                    {
                        ShowError(203, result);
                        return resultList;
                    }

                    wordInfo.wordPath = destFilePath;
                    wordInfo.updatetime = DateTime.Now;
                    wordInfo.wordFileName = WindowsTools.GetBasicName(wordInfo.wordPath);
                    resultList.Add(wordInfo);
                }
            }
            return resultList;
        }

        
        public static List<KeyInfo> GetKeyInfoList(out string result)
        {
            return
                WindowsTools.XmlDeseerializer(typeof(List<KeyInfo>), SystemConfigPath, out result) as
                    List<KeyInfo>;
        }

        public static List<ArgInfo> GetArgInfoList(TemplateInfo templateInfo, out string result)
        {
            if (templateInfo == null)
            {
                result = "未发现模板对象";
                return null;
            }
            var origArgInfoList =
                WindowsTools.XmlDeseerializer(typeof (List<ArgInfo>), ArgConfigPath, out result) as List<ArgInfo>;
            if (origArgInfoList == null)
            {
                return null;
            }
            return
                origArgInfoList.Where(p => p.templateName == templateInfo.templateName).OrderBy(p => p.argNo).ToList();
        }


        public static List<TemplateInfo> GetLocalTemplateFileList(out string result)
        {
            return
                WindowsTools.XmlDeseerializer(typeof (List<TemplateInfo>), TemplateConfigPath, out result) as
                    List<TemplateInfo>;
        }

        public static string UpdateArgConfig(List<ArgInfo> argInfoList)
        {
            var origArgInfoList =
                WindowsTools.XmlDeseerializer(typeof (List<ArgInfo>), ArgConfigPath, out _tmpResult) as List<ArgInfo>;
            if (origArgInfoList == null || _tmpResult != "")
            {
                return _tmpResult;
            }
            origArgInfoList = origArgInfoList.Where(p => p.templateName != argInfoList[0].templateName).ToList();
            origArgInfoList.AddRange(argInfoList);
            WindowsTools.XmlSerialize(origArgInfoList, ArgConfigPath, out _tmpResult);
            return _tmpResult;
        }

        public static string UpdateArgConfig(ArgInfo argInfo)
        {
            var origArgInfoList =
                WindowsTools.XmlDeseerializer(typeof (List<ArgInfo>), ArgConfigPath, out _tmpResult) as List<ArgInfo>;
            if (origArgInfoList == null || _tmpResult != "")
            {
                return _tmpResult;
            }
            origArgInfoList.Remove(origArgInfoList.Single(p => p.argID == argInfo.argID));
            origArgInfoList.Add(argInfo);
            origArgInfoList =
                origArgInfoList.OrderBy(p => p.templateID).ThenBy(p => p.argType).OrderBy(p => p.argName).ToList();
            WindowsTools.XmlSerialize(origArgInfoList, ArgConfigPath, out _tmpResult);
            return _tmpResult;
        }


        public static string UpdateTemplateConfig(TemplateInfo templateInfo)
        {
            var origTemplateInfoList =
                WindowsTools.XmlDeseerializer(typeof (List<TemplateInfo>), TemplateConfigPath, out _tmpResult) as
                    List<TemplateInfo>;
            if (origTemplateInfoList == null || _tmpResult != "")
            {
                return _tmpResult;
            }
            origTemplateInfoList = origTemplateInfoList.Where(p => p.templateName != templateInfo.templateName).ToList();
            origTemplateInfoList.Add(templateInfo);
            WindowsTools.XmlSerialize(origTemplateInfoList, TemplateConfigPath, out _tmpResult);
            return _tmpResult;
        }


        public static void ShowError(int errorCode, string errorContent = "")
        {
            var msg = "发生错误";
            if (errorCode <= 203)
            {
                switch (errorCode)
                {
                    case 101:
                    {
                        msg = "数据库服务没有开启";
                        break;
                    }
                    case 102:
                    {
                        msg = "数据库没有挂载";
                        break;
                    }
                    case 103:
                    {
                        msg = "数据库中没有区划表";
                        break;
                    }
                    default:
                    {
                        switch (errorCode)
                        {
                            case 200:
                            {
                                msg = "缺少配置文件";
                                break;
                            }
                            case 201:
                            {
                                msg = "文档中没有占位参数";
                                break;
                            }
                            case 202:
                            {
                                msg = "分析文档参数错误：" + errorContent;
                                break;
                            }
                            case 203:
                            {
                                msg = "SQL参数查询参数错误：" + errorContent;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                if (errorCode != 302)
                {
                    if (errorCode != 401)
                    {
                        if (errorCode == 999)
                        {
                            msg = errorContent;
                        }
                    }
                    else
                    {
                        msg = "数据类型错误：" + errorContent;
                    }
                }
                else
                {
                    msg = "文档更新错误：" + errorContent;
                }
            }
            MessageBox.Show(msg);
        }
    }

    public class KeyInfo
    {
        public string Key
        {
            get
            { return _key; }
            set
            { _key = value; }
        }

        public string Value
        {
            get
            { return _value; }
            set
            { _value = value; }
        }

        private string _key;
        private string _value;
    }
}
