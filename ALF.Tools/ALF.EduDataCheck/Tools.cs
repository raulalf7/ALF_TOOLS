using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using ALF.EDU;
using ALF.EDU.DataModel;
using DataCheck.Info;
using MahApps.Metro.Controls;


namespace DataCheck
{
    public class Tools
    {
        public static int RecordYear = 2015;

        private static string _tmpResult;

        public static string TemplateConfigPath = @".\templateConfigFile.xml";

        public static string ArgConfigPath = @".\argConfigFile.xml";
        
        public static List<WordInfo> CreateReportFile(List<TemplateInfo> templateInfoList, List<return_getRegionTreeNodeList> regionList, int appType, out string result)
        {
            var resultList = new List<WordInfo>();
            result = "";
            foreach (var itemRegion in regionList)
            {
                string regionString = itemRegion.node1 + itemRegion.node2 + itemRegion.node3 + itemRegion.node4 +
                                      itemRegion.node5;
                foreach (var itemFile in templateInfoList)
                {
                    var wordInfo = new WordInfo { rowid = Guid.NewGuid(), templateID = itemFile.templateID, templateName = itemFile.templateName, regionPath = itemRegion.node1 + itemRegion.node2 + itemRegion.node3 + itemRegion.node4, regionType = itemRegion.nodeOrganizationType, wordID = Guid.NewGuid() };
                    
                    
                    var argInfoList = GetArgInfoList(itemFile,out result);

                    if (result != "")
                    {
                        Tools.showError(202, result);
                        return resultList;
                    }

                    var destFilePath = string.Format(@"{0}\reportFiles\{1}_{2}_{3}", Environment.CurrentDirectory, wordInfo.regionPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), itemFile.templateName);
                    File.Copy(itemFile.templatePath, destFilePath, true);

                    result = ReportOfficeTools.UpdateWord(argInfoList, regionString, appType, destFilePath);

                    if (result != "")
                    {
                        Tools.showError(203, result);
                        return resultList;
                    }

                    wordInfo.wordPath = destFilePath;
                    wordInfo.updatetime = DateTime.Now;
                    wordInfo.wordFileName = ALF.SYSTEM.WindowsTools.GetBasicName(wordInfo.wordPath);
                    resultList.Add(wordInfo);
                }
            }
            return resultList;
        }

        public static List<ArgInfo> GetArgInfoList(TemplateInfo templateInfo,out string result)
        {
            if (templateInfo == null)
            {
                result = "未发现模板对象";
                return null;
            }
            var origArgInfoList = ALF.SYSTEM.WindowsTools.XmlDeseerializer(typeof(List<ArgInfo>), ArgConfigPath, out result) as List<ArgInfo>;
            if (origArgInfoList == null)
            {
                return null;
            }
            return origArgInfoList.Where(p => p.templateName == templateInfo.templateName).OrderBy(p => p.argNo).ToList();
        }


        public static List<TemplateInfo> GetLocalTemplateFileList(out string result)
        {
            return ALF.SYSTEM.WindowsTools.XmlDeseerializer(typeof(List<TemplateInfo>), TemplateConfigPath, out result) as List<TemplateInfo>;
        }
        
        public static void ShowArgInfoControl(ArgInfo argInfo, TemplateInfoControl templateInfoControl, TemplateInfo templateInfo)
        {
            var w = new MetroWindow {Height = 430, Width = 600, EnableDWMDropShadow = true, ResizeMode = ResizeMode.NoResize};
            w.LostFocus += (ss, ee) => w.Focus();
            w.Title = "参数信息";
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var control = new ArgInfoControl();
            control.Load(argInfo);

            control.OnCancelClose += (sender, args) => w.Close();

            control.OnOkClose += (ss, ee) =>
            {
                _tmpResult = updateArgConfig(argInfo);
                if (_tmpResult != "")
                {
                    return;
                }
                templateInfoControl.Load(templateInfo, true);
                w.Close();
            };

            w.Content = control;
            w.ShowDialog();
        }
        
        public static string updateArgConfig(List<ArgInfo> argInfoList)
        {
            var origArgInfoList = ALF.SYSTEM.WindowsTools.XmlDeseerializer(typeof(List<ArgInfo>), ArgConfigPath, out _tmpResult) as List<ArgInfo>;
            if (origArgInfoList == null || _tmpResult != "")
            {
                return _tmpResult;
            }
            origArgInfoList = origArgInfoList.Where(p => p.templateName != argInfoList[0].templateName).ToList();
            origArgInfoList.AddRange(argInfoList);
            ALF.SYSTEM.WindowsTools.XmlSerialize(origArgInfoList, ArgConfigPath, out _tmpResult);
            return _tmpResult;
        }

        public static string updateArgConfig(ArgInfo argInfo)
        {
            var origArgInfoList = ALF.SYSTEM.WindowsTools.XmlDeseerializer(typeof(List<ArgInfo>), ArgConfigPath, out _tmpResult) as List<ArgInfo>;
            if (origArgInfoList == null || _tmpResult!="")
            {
                return _tmpResult;
            }
            origArgInfoList.Remove(origArgInfoList.Single(p => p.argID == argInfo.argID));
            origArgInfoList.Add(argInfo);
            origArgInfoList = origArgInfoList.OrderBy(p => p.templateID).ThenBy(p => p.argType).OrderBy(p => p.argName).ToList();
            ALF.SYSTEM.WindowsTools.XmlSerialize(origArgInfoList, ArgConfigPath, out _tmpResult);
            return _tmpResult;
        }


        public static string UpdateTemplateConfig(TemplateInfo templateInfo)
        {
            var origTemplateInfoList = ALF.SYSTEM.WindowsTools.XmlDeseerializer(typeof(List<TemplateInfo>), TemplateConfigPath, out _tmpResult) as List<TemplateInfo>;
            if (origTemplateInfoList == null || _tmpResult != "")
            {
                return _tmpResult;
            }
            origTemplateInfoList = origTemplateInfoList.Where(p => p.templateName != templateInfo.templateName).ToList();
            origTemplateInfoList.Add(templateInfo);
            ALF.SYSTEM.WindowsTools.XmlSerialize(origTemplateInfoList, TemplateConfigPath, out _tmpResult);
            return _tmpResult;
        }


        public static void showError(int errorCode, string errorContent = "")
        {
            string msg = "发生错误";
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
}
