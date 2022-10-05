using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DumpApplic
{
    public static class Helper
    {

        public static string database = "DumpApplic";// use this declaration in all classes
        public static string CnnValue(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static CheckTapes CreateCheckTapesWindow(List<string> todayTapes)
        {
            CheckTapes CT = new CheckTapes();//new check tapes window
            List<Label> labelsList = new List<Label>();
            List<TextBox> tbList = new List<TextBox>();
            foreach (string item in todayTapes)// create label+textbox for each tape
            {
                Label lbl = new Label(); lbl.Height = 40; lbl.Width = 120; lbl.VerticalContentAlignment = VerticalAlignment.Center;
                lbl.FontSize = 18; lbl.FontWeight = FontWeights.Bold; lbl.Content = item;
                TextBox tb = new TextBox(); tb.Height = 40; tb.Width = 200; tb.FontSize = 18; tb.FontWeight = FontWeights.Bold; tb.VerticalContentAlignment = VerticalAlignment.Center;
                labelsList.Add(lbl); tbList.Add(tb);
            }

            //add all labels and textboxes to the new window
            for (int i = 0; i < labelsList.Count; i++)
            {
                CT.TapesToCheck.Children.Add(labelsList[i]);
                CT.TapesToCheck.Children.Add(tbList[i]);
            }
            return CT;
        }

        
            
    
    }
}
