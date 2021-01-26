using System;
using System.Drawing;
using System.Windows.Forms;

using IntroProject.Core.Utils;
using Microsoft.Win32;

namespace IntroProject.Presentation.Controls
{
    public class HelpMenu : UserControl
    {
        MultipleLanguages translator = MultipleLanguages.Instance;
        Button exit;
        EventHandler _exitMenu, _HomeExit;
        private RichTextBox textBox = new RichTextBox();

        public HelpMenu(int w, int h, EventHandler exitMenu, EventHandler HomeExit)
        {
            this.translator = new MultipleLanguages();
            this.BackColor = Color.FromArgb(123, 156, 148);
            this.Size = new Size(w, h);
            _exitMenu = exitMenu;
            _HomeExit = HomeExit;

            exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(50, 50);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;

            textBox.BackColor = Color.FromArgb(123, 156, 148);
            textBox.BorderStyle = BorderStyle.None;
            textBox.Size = new Size(800, 300);
            textBox.Location = new Point(this.Width / 2 - 400, 40);
            textBox.Font = new Font("Arial", 22, FontStyle.Regular);
            textBox.Text = translator.DisplayText("helpText") + " https://github.com/Informatica-Introproject-Lesser-Dim/Introproject-Form/wiki";
            textBox.BackColor = Color.FromArgb(123, 156, 148);
            textBox.ReadOnly = true;
            textBox.DetectUrls = true;
            textBox.ScrollBars = RichTextBoxScrollBars.None;
            textBox.LinkClicked += (object sender, LinkClickedEventArgs e) =>
                System.Diagnostics.Process.Start(GetSystemDefaultBrowser(), e.LinkText);

            this.Controls.Add(textBox);
            this.Controls.Add(exit);
        }
    // All this to find IE
    private string GetSystemDefaultBrowser()
    {
        string name = string.Empty;
        RegistryKey regKey = null;
        try
        {
            regKey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);

            name = regKey.GetValue(null).ToString().ToLower().Replace("" + (char)34, "");

            if (!name.EndsWith("exe"))
                name = name.Substring(0, name.LastIndexOf(".exe") + 4);

        }
        catch (Exception ex)
        {
            name = string.Format("ERROR: An exception of type: {0} occurred in method: {1} in the following module: {2}", ex.GetType(), ex.TargetSite, this.GetType());
        }
        finally
        {
            if (regKey != null)
                regKey.Close();
        }
        return name;
        }
        public void backHome()
        {
            exit.Click -= _exitMenu;
            exit.Click += _HomeExit;
        }
        public void backExit()
        {
            exit.Click -= _HomeExit;
            exit.Click += _exitMenu;
        }
    }
}
