using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

using Alf7.Tools.Library;
using Alf7.Tools.Library.DataModel;
using Alf7.UI.Library;
using DataCheck_XP.Info;



namespace DataCheck_XP
{
    class Tools
    {
        private static string _tmpResult;

        public static string TemplateConfigPath = @".\templateConfigFile.xml";

        public static string ArgConfigPath = @".\argConfigFile.xml";

        public static List<WordInfo> createReportFile(List<TemplateInfo> templateInfoList, List<return_getRegionTreeNodeList> regionList, int appType, out string result)
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
                    
                    
                    var argInfoList = getArgInfoList(itemFile,out result);

                    if (result != "")
                    {
                        SystemTools.showError(202, result);
                        return resultList;
                    }

                    var destFilePath = string.Format(@"{0}\reportFiles\{1}_{2}_{3}", Environment.CurrentDirectory, wordInfo.regionPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), itemFile.templateName);
                    File.Copy(itemFile.templatePath, destFilePath, true);

                    result = ReportOfficeTools.updateWord(argInfoList, regionString, appType, destFilePath);

                    if (result != "")
                    {
                        SystemTools.showError(203, result);
                        return resultList;
                    }

                    wordInfo.wordPath = destFilePath;
                    wordInfo.updatetime = DateTime.Now;
                    wordInfo.wordFileName = SystemTools.getBasicName(wordInfo.wordPath);
                    resultList.Add(wordInfo);
                }
            }
            return resultList;
        }

        public static List<ArgInfo> getArgInfoList(TemplateInfo templateInfo,out string result)
        {
            if (templateInfo == null)
            {
                result = "未发现模板对象";
                return null;
            }
            var origArgInfoList = SystemTools.xmlDeseerializer(typeof(List<ArgInfo>), ArgConfigPath, out result) as List<ArgInfo>;
            if (origArgInfoList == null)
            {
                return null;
            }
            return origArgInfoList.Where(p => p.templateName == templateInfo.templateName).OrderBy(p => p.argNo).ToList();
        }


        public static List<TemplateInfo> getLocalTemplateFileList(out string result)
        {
           return  SystemTools.xmlDeseerializer(typeof(List<TemplateInfo>), TemplateConfigPath, out result) as List<TemplateInfo>;
        }
        
        public static void showArgInfoControl(ArgInfo argInfo, TemplateInfoControl templateInfoControl, TemplateInfo templateInfo)
        {
            var w = new Window() {Height = 430, Width = 600, ResizeMode = ResizeMode.NoResize};
            w.LostFocus += (ss, ee) => w.Focus();
            w.Title = "参数信息";
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var control = new ArgInfoControl();
            control.load(argInfo);

            control.onCancelClose += (sender, args) => w.Close();

            control.onOkClose += (ss, ee) =>
            {
                _tmpResult = updateArgConfig(argInfo);
                if (_tmpResult != "")
                {
                    return;
                }
                templateInfoControl.load(templateInfo, true);
                w.Close();
            };

            w.Content = control;
            w.ShowDialog();
        }
        
        public static string updateArgConfig(List<ArgInfo> argInfoList)
        {
            var origArgInfoList = SystemTools.xmlDeseerializer(typeof(List<ArgInfo>), ArgConfigPath, out _tmpResult) as List<ArgInfo>;
            if (origArgInfoList == null || _tmpResult != "")
            {
                return _tmpResult;
            }
            origArgInfoList = origArgInfoList.Where(p => p.templateName != argInfoList[0].templateName).ToList();
            origArgInfoList.AddRange(argInfoList);
            SystemTools.xmlSerialize(origArgInfoList, ArgConfigPath, out _tmpResult);
            return _tmpResult;
        }

        public static string updateArgConfig(ArgInfo argInfo)
        {
            var origArgInfoList = SystemTools.xmlDeseerializer(typeof(List<ArgInfo>), ArgConfigPath,out _tmpResult) as List<ArgInfo>;
            if (origArgInfoList == null || _tmpResult!="")
            {
                return _tmpResult;
            }
            origArgInfoList.Remove(origArgInfoList.Single(p => p.argID == argInfo.argID));
            origArgInfoList.Add(argInfo);
            origArgInfoList = origArgInfoList.OrderBy(p => p.templateID).ThenBy(p => p.argType).OrderBy(p => p.argName).ToList();
            SystemTools.xmlSerialize(origArgInfoList, ArgConfigPath, out _tmpResult);
            return _tmpResult;
        }


        public static string updateTemplateConfig(TemplateInfo templateInfo)
        {
            var origTemplateInfoList = SystemTools.xmlDeseerializer(typeof(List<TemplateInfo>), TemplateConfigPath, out _tmpResult) as List<TemplateInfo>;
            if (origTemplateInfoList == null || _tmpResult != "")
            {
                return _tmpResult;
            }
            origTemplateInfoList = origTemplateInfoList.Where(p => p.templateName != templateInfo.templateName).ToList();
            origTemplateInfoList.Add(templateInfo);
            SystemTools.xmlSerialize(origTemplateInfoList, TemplateConfigPath, out _tmpResult);
            return _tmpResult;
        }
    }
}
