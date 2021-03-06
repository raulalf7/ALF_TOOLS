﻿using System.Windows;
using ALF.EDU.DataModel;
using DataReport.Info;
using DataReport_XP;
using MahApps.Metro.Controls;

namespace DataReport
{
    public class DataReportTools
    {
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
                if (Tools.UpdateArgConfig(argInfo) != "")
                {
                    return;
                }
                templateInfoControl.Load(templateInfo, true);
                w.Close();
            };

            w.Content = control;
            w.ShowDialog();
        }


    }
}
