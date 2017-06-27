using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ALF.DocGen
{
    public class Tools
    {
        public static string folder = Environment.CurrentDirectory;

        public static string Gen(Dictionary<string, string> valueList, int appType,bool isPreview)
        {
            var filePath = string.Format(@"{0}\tmpPng\{1}.docx", Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMddhhmmss"));

            var tempPath = folder + @"\Template\签报模板.docx";
            if (appType == 0)
            {
                tempPath = folder + @"\Template\发文稿纸模板.docx";
            }
            if (appType == 2)
            {
                tempPath = folder + @"\Template\部发文模板.docx";
            }
            if (appType == 3)
            {
                tempPath = folder + @"\Template\厅发文模板.docx";
            }
            if (appType == 4)
            {
                tempPath = folder + @"\Template\司发文模板.docx";
            }
            if (!isPreview)
            {
                tempPath = tempPath.Replace(".docx", "文字.docx");
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
                MainWindow.coverGrid.Visibility = Visibility.Collapsed;
                return "";
            }
            Console.WriteLine("拷贝模板");
            OFFICE.WordTools.OpenDoc(filePath);
            foreach (var valueItem in valueList)
            {
                OFFICE.WordTools.ReplaceContent(valueItem.Value, valueItem.Key, filePath,true);
            }
            OFFICE.WordTools.SaveAndClose(null, true);

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

        //public static void StartWordApp()
        //{
        //    Console.WriteLine("打开Word应用");
        //    wordApp =
        //        (Microsoft.Office.Interop.Word.Application)
        //            Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
        //}

        //public static void CloseWordApp()
        //{
        //    try
        //    {
        //        wordApp.Quit(WdSaveOptions.wdSaveChanges, WdOriginalFormat.wdWordDocument, false);
        //    }
        //    catch
        //    {
        //    }
        //}

        //public static void CopyWordDoc(object sorceDocPath, object destDocPath,bool isPasteAtLast)
        //{
        //    object readOnly = false;
        //    object isVisible = false;

        //    Document destWordDoc = wordApp.Documents.Open(ref destDocPath, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis);
        //    Document openWord = wordApp.Documents.Open(ref sorceDocPath, ref _mis, ref readOnly, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref isVisible, ref _mis, ref _mis, ref _mis, ref _mis);

        //    openWord.Select();
        //    openWord.Sections[1].Range.Copy();

        //    if (isPasteAtLast)
        //    {
        //        destWordDoc.Select();
        //        object objUnit = WdUnits.wdStory;
        //        wordApp.Selection.EndKey(ref objUnit);
        //        wordApp.ActiveWindow.Selection.PasteAndFormat(WdRecoveryType.wdPasteDefault);
        //    }
        //    else
        //    {
        //        foreach (Bookmark bm in destWordDoc.Bookmarks)
        //        {
        //            if (bm.Name == "ZW")
        //            {
        //                bm.Select();
        //                bm.Range.PasteAndFormat(WdRecoveryType.wdPasteDefault);
        //            }
        //        }
        //    }

        //    openWord.Save();
        //    openWord.Close(ref _mis, ref _mis, ref _mis);
        //    destWordDoc.Save();
        //    destWordDoc.Close(ref _mis, ref _mis, ref _mis);
        //}

        //private static string EditDocArg(string newValue, string oldValue, Document wordDoc)
        //{
        //    var result = "";
        //    try
        //    {
        //        var wfnd = wordDoc.Range().Find;
        //        wfnd.Text = oldValue;
        //        wfnd.ClearFormatting();
        //        if (wfnd.Execute(ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, ref _mis, newValue, RpAll, ref _mis, ref _mis, ref _mis, ref _mis))
        //        {
        //            Console.WriteLine("Find");
        //            return "";
        //        }

        //        StoryRanges sr = wordDoc.StoryRanges;
        //        foreach (Range r in sr)
        //        {
        //            Range r1 = r;
        //            if (WdStoryType.wdTextFrameStory == r.StoryType) //这句话用来判断什么的？
        //            {
        //                do
        //                {
        //                    r1.Find.ClearFormatting();
        //                    r1.Find.Text = oldValue;

        //                    r1.Find.Replacement.ClearFormatting();
        //                    r1.Find.Replacement.Text = newValue;

        //                    if (r1.Find.Execute(
        //                                    ref _mis, ref _mis, ref _mis, ref _mis, ref _mis,
        //                                    ref _mis, ref _mis, ref _mis, ref _mis, ref _mis,
        //                                    RpAll, ref _mis, ref _mis, ref _mis, ref _mis))
        //                    {
        //                        break;//找到并替换后跳出循环，省点时间
        //                    }
        //                    else
        //                    {
        //                        r1 = r1.NextStoryRange;
        //                    }  //加上这个if语句可以提高一点效率，但还是比较慢
        //                } while (r1 != null);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.Message;
        //        Console.WriteLine("Error:" + ex.Message);
        //    }

        //    return result;
        //}


        public static void ConvertWordToImage(string docPath, Dictionary<string, string> valueList, int genType)
        {
            var list = OFFICE.WordTools.ConvertWordToImage(docPath);
            Console.WriteLine("预览图片生成完成");
            var window = new PicWindow();
            window.Load(list, valueList, genType);
            window.ShowDialog();
        }

    }
}
