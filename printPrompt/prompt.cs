using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace printPrompt
{
    class Prompt
    {
            public static string ShowDialog(string text, string caption)
            {
                var prompt = new Form();
                prompt.Width = 300;
                prompt.Height = 200;
                prompt.Text = caption;
                Label textLabel = new Label();
                textLabel.Left = 50;
                textLabel.Top = 10;
                textLabel.Text = text;
                var textBox1 =new RadSpinEditor(){ Left = 50, Top = 50, Width = 200 };
               
//                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                var confirmation = new RadButton() { Text = "Ok", Left = 50, Width = 100, Top = 80 };
//                confirmation.Click += (sender, e) => prompt.Close();
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox1);
                prompt.ShowDialog();
                return textBox1.Text;
            }
        
    }
}
