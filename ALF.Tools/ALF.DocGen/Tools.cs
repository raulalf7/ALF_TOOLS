using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace ALF.DocGen
{
    public class Tools
    {
        private static object _mis = Type.Missing;
        private static readonly object RpAll = WdReplace.wdReplaceAll;
        public static Microsoft.Office.Interop.Word.Application wordApp;
        public static string folder = @".\Template";
       // public static string folder = @"E:\文档\5_Code\ALF_TOOL\ALF.Tools\ALF.DocGen\bin\Debug\Template";

        public static string Gen(Dictionary<string, string> valueList, int appType)
        {
            var filePath = string.Format(@"{0}\tmp\{1}.docx", Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMddhhmmss"));

            var tempPath = folder + @"\签报模板.docx";
            if (appType == 0)
            {
                tempPath = folder + @"\发文稿纸模板.docx";
            }
            if (appType == 2)
            {
                tempPath = folder + @"\部发文模板.docx";
            }
            if (appType == 3)
            {
                tempPath = folder + @"\厅发文模板.docx";
            }
            if (appType == 4)
            {
                tempPath = folder + @"\司发文模板.docx";
            }
            

            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            if (File.Exists(tempPath))
            {
                File.Copy(tempPath, filePath, true);
            }
            else
            {
                MessageBox.Show("缺少模板！");
                return "";
            }

            UpdateWord(valueList, filePath);

            return filePath;
        }

        public static void PrepareAttach(string filePath,List<string> attachList,string contextPath)
        {
            var fileInfo = new FileInfo(filePath);

            foreach (var item in attachList)
            {
                File.Copy(item, string.Format(@"{0}\{1}_3_附件_{2}",
                    fileInfo.DirectoryName, 
                    SYSTEM.WindowsTools.GetBasicName(fileInfo.Name).Replace(".docx",""),
                    SYSTEM.WindowsTools.GetBasicName(item)),true);
            }
        }

        public static void StartWordApp()
        {
            wordApp =
                (Microsoft.Office.Interop.Word.Application)
                    Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
        }

        public static void CloseWordApp()
        {
            try
            {
                wordApp.Quit(WdSaveOptions.wdSaveChanges, WdOriginalFormat.wdWordDocument, false);
            }
            catch
            {
            }
        }

        public static void CopyWordDoc(object sorceDocPath, object destDocPath,bool isPasteAtLast)
        {
            object readOnly = false;
            object isVisible = false;

            Document destWordDoc = wordApp.Documents.Open(ref destDocPath, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis);
            Document openWord = wordApp.Documents.Open(ref sorceDocPath, ref _mis, ref readOnly, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref isVisible, ref _mis, ref _mis, ref _mis, ref _mis);

            openWord.Select();
            openWord.Sections[1].Range.Copy();

            if (isPasteAtLast)
            {
                destWordDoc.Select();
                object objUnit = WdUnits.wdStory;
                wordApp.Selection.EndKey(ref objUnit);
                wordApp.ActiveWindow.Selection.PasteAndFormat(WdRecoveryType.wdPasteDefault);
            }
            else
            {
                foreach (Bookmark bm in destWordDoc.Bookmarks)
                {
                    if (bm.Name == "ZW")
                    {
                        bm.Select();
                        bm.Range.PasteAndFormat(WdRecoveryType.wdPasteDefault);
                    }
                }
            }

            openWord.Save();
            openWord.Close(ref _mis, ref _mis, ref _mis);
            destWordDoc.Save();
            destWordDoc.Close(ref _mis, ref _mis, ref _mis);
        }
        
        private static void ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private static string EditDocArg(string newValue, string oldValue, Document wordDoc)
        {
            var result = "";
            try
            {
                var wfnd = wordDoc.Range().Find;
                wfnd.Text = oldValue;
                wfnd.ClearFormatting();
                if (wfnd.Execute(ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, newValue, RpAll, ref _mis, ref _mis, ref _mis, ref _mis))
                {
                    Console.WriteLine("Find");
                    return "";
                }

                StoryRanges sr = wordDoc.StoryRanges;
                foreach (Range r in sr)
                {
                    Range r1 = r;
                    if (WdStoryType.wdTextFrameStory == r.StoryType) //这句话用来判断什么的？
                    {
                        do
                        {
                            r1.Find.ClearFormatting();
                            r1.Find.Text = oldValue;

                            r1.Find.Replacement.ClearFormatting();
                            r1.Find.Replacement.Text = newValue;

                            if (r1.Find.Execute(
                                            ref _mis, ref _mis, ref _mis, ref _mis, ref _mis,
                                            ref _mis, ref _mis, ref _mis, ref _mis, ref _mis,
                                            RpAll, ref _mis, ref _mis, ref _mis, ref _mis))
                            {
                                break;//找到并替换后跳出循环，省点时间
                            }
                            else
                            {
                                r1 = r1.NextStoryRange;
                            }  //加上这个if语句可以提高一点效率，但还是比较慢
                        } while (r1 != null);
                    }
                }

            //    foreach (Range range in storyRanges)
            //    {
            //        Range rangeFlag = range;
            //        rangeFlag.Find.ClearFormatting();
            //        rangeFlag.Find.Replacement.ClearFormatting();
            //        rangeFlag.Find.Text = oldValue;
            //        rangeFlag.Find.Replacement.Text = newValue;
            //        if (rangeFlag.Find.Execute(ref _mis, ref _mis, ref _mis,
            //                               ref _mis, ref _mis, ref _mis,
            //                               ref _mis, ref _mis, ref _mis,
            //                               ref _mis, RpAll, ref _mis,
            //                               ref _mis, ref _mis, ref _mis))
            //        {
            //            Console.WriteLine("Find");
            //            return "";
            //        }
            //        //if (WdStoryType.wdTextFrameStory == rangeFlag.StoryType)
            //        //{
            //        //    while (rangeFlag != null)
            //        //    {
            //        //        rangeFlag.Find.ClearFormatting();
            //        //        rangeFlag.Find.Replacement.ClearFormatting();
            //        //        rangeFlag.Find.Text = oldValue;
            //        //        rangeFlag.Find.Replacement.Text = newValue;
            //        //        rangeFlag.Find.Execute(ref _mis, ref _mis, ref _mis,
            //        //                               ref _mis, ref _mis, ref _mis,
            //        //                               ref _mis, ref _mis, ref _mis,
            //        //                               ref _mis, RpAll, ref _mis,
            //        //                               ref _mis, ref _mis, ref _mis);
            //        //        rangeFlag = range.NextStoryRange;
            //        //    }
            //        //}

            //    }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                Console.WriteLine("Error:" + ex.Message);
            }

            return result;
        }

        private static void UpdateWord(Dictionary<string,string> valueList, object filePath, bool isQuitWhenError = true)
        {
            Document wordDoc =new Document();
            try
            {
                wordDoc = wordApp.Documents.Open(ref filePath, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis);
            }
            catch
            {
                StartWordApp();
                wordDoc = wordApp.Documents.Open(ref filePath, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis);
            }
            wordDoc.SpellingChecked = true;
            wordDoc.GrammarChecked = true;
            wordDoc.ShowSpellingErrors = false;
            var i = 1;
            foreach (var valueItem in valueList)
            {
                EditDocArg(valueItem.Value, valueItem.Key, wordDoc);
                i++;
            }

            wordDoc.Save();
            wordDoc.Close(ref _mis, ref _mis, ref _mis);
        }
    }
}
