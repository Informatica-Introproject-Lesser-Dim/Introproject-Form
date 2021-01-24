using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security;
using System.Windows.Forms;

using IntroProject.Core.Utils;

namespace IntroProject.Presentation.Controls
{
    public class SettingsMenu : UserControl
    {
        MultipleLanguages multipleLanguages = new MultipleLanguages();
        public bool changed, newMap = false, warned = true;

        internal OpenFileDialog openFileDialog = new OpenFileDialog();
        internal SaveFileDialog SaveFileDialog = new SaveFileDialog();
        private CheckBox AddHeat;
        private ComboBox languageIndex;
        private List<TrackBar> trackBars = new List<TrackBar>();
        private int TE, SE, HS, GG, GMF;
        private float Sp, MH, MC, WE, JE, PE;

        public SettingsMenu(int w, int h, EventHandler exitMenu)
        {
            BackColor = Color.FromArgb(123, 156, 148);
            Size = new Size(w, h);
            int edge = Math.Max(Size.Width, Size.Height) / 36;

            StartSettings();

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(edge, edge);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;

            //Makes the Import Settings Button
            int i = 2 * Size.Width / 3 + 40, j = Size.Height / 10 + 60;

            Button importSet = new Button();
            importSet.Location = new Point(i, j + edge);
            importSet.Size = new Size(edge * 2, edge * 2);
            importSet.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            importSet.BackColor = Color.White;
            importSet.Text = "Import Settings";
            importSet.Click += ImportSettings;
            importSet.Click += ImplementChanges;


            //Makes the Export Settings Button
            int k = 2 * Size.Width / 3 + 40, l = 60;

            Button ExportSetts = new Button();
            ExportSetts.Location = new Point(k, l);
            ExportSetts.Size = new Size(edge * 2, edge * 2);
            ExportSetts.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            ExportSetts.BackColor = Color.White;
            ExportSetts.Text = "Export Settings";
            ExportSetts.Click += ExportSettings;

            //Makes the Save Settings Button
            int m = (int)(1.5 * Size.Width) / 3, n = 6 * Size.Height / 10;

            Button SaveSetts = new Button();
            SaveSetts.Location = new Point(m, n);
            SaveSetts.Size = new Size(edge * 2, edge * 2);
            SaveSetts.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            SaveSetts.BackColor = Color.White;
            SaveSetts.Text = "Save Settings";
            SaveSetts.Click += ImplementChanges;

            this.Controls.Add(exit);
            this.Controls.Add(importSet);
            this.Controls.Add(SaveSetts);
            this.Controls.Add(ExportSetts);

            //Resize Method for the buttons
            Resize += (object o, EventArgs ea) =>
            {
                int i = 2 * Size.Width / 3 + 40, j = Size.Height / 10 + 60, k = 2 * Size.Width / 3 + 40, l = 60, m = (int)(1.5 * Size.Width) / 3, n = 6 * Size.Height / 10 + 10;
                edge = Math.Max(Size.Width, Size.Height) / 36;
                exit.Size = new Size(edge, edge);
                importSet.Size = new Size(edge * 2, edge * 2);
                importSet.Location = new Point(i, j + edge);
                ExportSetts.Size = new Size(edge * 2, edge * 2);
                ExportSetts.Location = new Point(k, l);
                SaveSetts.Size = new Size(edge * 2, edge * 2);
                SaveSetts.Location = new Point(m, n);
            };
            /* Each triple sentence is an slider group, First the construnction, where after the TrackBar specific event and textbox equivalent handlers.
               Don't forget, slider values are integers. So there is a scale in every thing
               If the scale is a visual addition, don't forget to remove it from the trackbar value.
               MakeSlider takes: x & y, name, Scaled value, flat minimum, flat maximum, scalar, explenation.
            */
            int r = 100, s = 10000, t = 1000000, value;
            float fvalue;

            (TrackBar TrackBarSpeed, TextBox TextBoxSpeed, ToolTip ToolTipSpeed) = MakeSlider(0, 0, () => multipleLanguages.DisplayText("SDforSpeed"), Settings.StepSize, 25, 400, 100, multipleLanguages.DisplayText("SDforSpeedExplenation"));
            TrackBarSpeed.ValueChanged += (object o, EventArgs ea) => { Sp = TrackBarSpeed.Value / (float)r; };
            TextBoxSpeed.Leave += (object o, EventArgs ea) => { Sp = float.Parse(TextBoxSpeed.Text); };
            trackBars.Add(TrackBarSpeed); //0

            (TrackBar TrackBarTotalEntities, TextBox TextBoxTotalEntities, ToolTip ToolTipTotalEntities) = MakeSlider(0, 1,() => multipleLanguages.DisplayText("SDstartingCountCarnivore"), Settings.StartCarnivore, 0, 10, 1, "bijbehorende uitleg");
            TrackBarTotalEntities.ValueChanged += (object o, EventArgs ea) => { TE = TrackBarTotalEntities.Value; };
            TextBoxTotalEntities.Leave += (object o, EventArgs ea) => { TE = int.Parse(TextBoxTotalEntities.Text); };
            trackBars.Add(TrackBarTotalEntities); //1

            (TrackBar TrackBarStartEntities, TextBox TextBoxStartEntities, ToolTip ToolTipSTartEntities) = MakeSlider(0, 2,() => multipleLanguages.DisplayText("SDstartingCountHerbivore"), Settings.StartHerbivore, 10, 40, 1, "bijbehorende uitleg");
            TrackBarStartEntities.ValueChanged += (object o, EventArgs ea) => { SE = TrackBarStartEntities.Value; };
            TextBoxStartEntities.Leave += (object o, EventArgs ea) => { SE = int.Parse(TextBoxStartEntities.Text); };
            trackBars.Add(TrackBarStartEntities); //2

            (TrackBar TrackBarHatchSpeed, TextBox TextBoxHatchSpeed, ToolTip ToolTipHatchSpeed) = MakeSlider(0, 3,() => multipleLanguages.DisplayText("SDhatchSpeed"), Settings.HatchSpeed, 10, 100, 1, "bijbehorende uitleg");
            TrackBarHatchSpeed.ValueChanged += (object o, EventArgs ea) => { HS = TrackBarHatchSpeed.Value; };
            TextBoxHatchSpeed.Leave += (object o, EventArgs ea) => { HS = int.Parse(TextBoxHatchSpeed.Text); };
            trackBars.Add(TrackBarHatchSpeed); //3

            fvalue = (Settings.MiddleHeight + 1f) / 2f;
            (TrackBar TrackBarMiddleHeight, TextBox TextBoxMiddleHeight, ToolTip ToolTipMiddleHeight) = MakeSlider(0, 4,() => multipleLanguages.DisplayText("SDseaLevel"), fvalue, 0, 99, 100, "bijbehorende uitleg");
            TrackBarMiddleHeight.ValueChanged += (object o, EventArgs ea) => { MH = (TrackBarMiddleHeight.Value/50f) - 0.99f; };
            TextBoxMiddleHeight.Leave += (object o, EventArgs ea) => { MH = float.Parse(TextBoxMiddleHeight.Text) - 0.99f; };
            trackBars.Add(TrackBarMiddleHeight); //4

            (TrackBar TrackBarMatingCost, TextBox TextBoxMatingCost, ToolTip ToolTipMatingCost) = MakeSlider(0, 5,() => multipleLanguages.DisplayText("SDmatingCost"), Settings.MatingCost, 20, 150, 100, "bijbehorende uitleg");
            TrackBarMatingCost.ValueChanged += (object o, EventArgs ea) => { MC = TrackBarMatingCost.Value / (float)r; };
            TextBoxMatingCost.Leave += (object o, EventArgs ea) => { MC = float.Parse(TextBoxMatingCost.Text); };
            trackBars.Add(TrackBarMatingCost); //5

            (TrackBar TrackBarGrassGrowth, TextBox TextBoxGrassGrowth, ToolTip ToolTipGrassGrowth) = MakeSlider(0, 6,() => multipleLanguages.DisplayText("SDgrassGrowSpeed"), Settings.GrassGrowth, 100, 500, 1, "bijbehorende uitleg");
            TrackBarGrassGrowth.ValueChanged += (object o, EventArgs ea) => { GG = TrackBarGrassGrowth.Value; };
            TextBoxGrassGrowth.Leave += (object o, EventArgs ea) => { GG = int.Parse(TextBoxGrassGrowth.Text); };
            trackBars.Add(TrackBarGrassGrowth); //6

            (TrackBar TrackBarGrassMaxFeed, TextBox TextBoxGrassMaxFeed, ToolTip ToolTipGrassMaxFeed) = MakeSlider(1, 0,() => multipleLanguages.DisplayText("SDgrassMaxEnergy"), Settings.GrassMaxFeed, 1000, 3000, 1, "bijbehorende uitleg");
            TrackBarGrassMaxFeed.ValueChanged += (object o, EventArgs ea) => { GMF = TrackBarGrassMaxFeed.Value; };
            TextBoxGrassMaxFeed.Leave += (object o, EventArgs ea) => { GMF = int.Parse(TextBoxGrassMaxFeed.Text); };
            trackBars.Add(TrackBarGrassMaxFeed); //7

            value = (int)(Settings.WalkEnergy * (float)r);
            (TrackBar TrackBarWalkEnergy, TextBox TextBoxWalkEnergy, ToolTip ToolTipWalkEnergy) = MakeSlider(1, 1,() => multipleLanguages.DisplayText("SDwalkEnergyCost"), value, 5, 30, 1, "bijbehorende uitleg");
            TrackBarWalkEnergy.ValueChanged += (object o, EventArgs ea) => { WE = TrackBarWalkEnergy.Value / (float)r; };
            TextBoxWalkEnergy.Leave += (object o, EventArgs ea) => { WE = float.Parse(TextBoxWalkEnergy.Text); };
            trackBars.Add(TrackBarWalkEnergy); //8

            value = (int)(Settings.JumpEnergy * (float)s);
            (TrackBar TrackBarJumpEnergy, TextBox TextBoxJumpEnergy, ToolTip ToolTipJumpEnergy) = MakeSlider(1, 2,() => multipleLanguages.DisplayText("SDjumpEnergyCost"), value, 5, 30, 1, "bijbehorende uitleg");
            TrackBarJumpEnergy.ValueChanged += (object o, EventArgs ea) => { JE = TrackBarJumpEnergy.Value / (float)s; };
            TextBoxJumpEnergy.Leave += (object o, EventArgs ea) => { JE = float.Parse(TextBoxJumpEnergy.Text); };
            trackBars.Add(TrackBarJumpEnergy); //9

            value = (int)(Settings.PassiveEnergy * (float)t);
            (TrackBar TrackBarPassiveEnergy, TextBox TextBoxPassiveEnergy, ToolTip ToolTipPassiveEnergy) = MakeSlider(1, 3,() => multipleLanguages.DisplayText("SDpassiveEnergyCost"), value, 5, 30, 1, "bijbehorende uitleg");
            TrackBarPassiveEnergy.ValueChanged += (object o, EventArgs ea) => { PE = TrackBarPassiveEnergy.Value / (float)t; };
            TextBoxPassiveEnergy.Leave += (object o, EventArgs ea) => { PE = float.Parse(TextBoxPassiveEnergy.Text); };
            trackBars.Add(TrackBarPassiveEnergy); //10

            makeHeatBox(1, 4,() => multipleLanguages.DisplayText("CBheatMap"), "Turn Heat Map coloring on/off");

            makeLanguageBox(1, 5, () => "Language Selection", "Select the which language.");

            Controls.Add(exit);
        }

        internal class LazyLabel : Label
        {
            public Func<string> LazyText { get; set; } = () => "";
            public override string Text { get => LazyText(); }
        }
        //Makes a TrackBar with connected Textbox, which are returned for specialzation
        private (TrackBar, TextBox, ToolTip) MakeSlider(int x, int y, Func<string> name, float basevalue, int minvalue, int maxvalue, int scale, string uitleg)
        { 
            Size sliderTextBoxSize = new Size(30, 20);
            int edge = Math.Max(Size.Width, Size.Height) / 36, w = (x * (Size.Width / 3)) + 40, h = (y * (Size.Height / 10)) + edge;

            LazyLabel label = MakeLazySliderLabel(name, w, h);

            TrackBar trackBar = new CustomTrackbar(w, (h+20), minvalue, maxvalue);
            TextBox textBox = new TextBox();
            textBox.BackColor = Color.FromArgb(123, 156, 148);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.ForeColor = Color.White;
            trackBar.Value = (int) (basevalue * scale);
            trackBar.Size = new Size((Size.Width / 4), 40);
            textBox.Text = basevalue.ToString();
            trackBar.ValueChanged += (object o, EventArgs ea) => { textBox.Text = (Math.Round(trackBar.Value * (1.0/scale), 2)).ToString(); changed = true; };
            textBox.Leave += (object o, EventArgs ea) => { trackBar.Value = Convert.ToInt32(double.Parse(textBox.Text) * scale); changed = true; };
            textBox.Location = new Point((w + (Size.Width / 4)), (h+30));
            textBox.Size = sliderTextBoxSize;
            trackBar.AutoSize = false;

            Resize += (object o, EventArgs ea) =>
            {
                int edge = Math.Max(Size.Width, Size.Height) / 36, w = (x * (Size.Width / 3)) + 40, h = (y * (Size.Height / 10)) + edge + 10;
                label.Location = new Point(w, h);
                trackBar.Size = new Size((Size.Width / 4), 40);
                trackBar.Location = new Point(w, (h + 20));
                textBox.Location = new Point((w + (Size.Width / 4)), (h + 30));
            };

            ToolTip toolTip = new ToolTip();
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = name();
            toolTip.SetToolTip(label, uitleg);
            toolTip.SetToolTip(trackBar, uitleg);

            Controls.Add(trackBar);
            Controls.Add(textBox);
            Controls.Add(label);
            
            return (trackBar, textBox, toolTip);
        }

        private void makeHeatBox(int x, int y, Func<string> name, string uitleg)
        {
            int edge = Math.Max(Size.Width, Size.Height) / 36, w = (x * (Size.Width / 3)) + 40, h = (y * (Size.Height / 10)) + edge + 10;
            AddHeat = new CheckBox();
            LazyLabel label = MakeLazySliderLabel(name, w, h);

            AddHeat.Location = new Point((w+10), (h+20));
            AddHeat.AutoSize = false;
            AddHeat.Checked = false;

            ToolTip toolTip = new ToolTip();
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = name();
            toolTip.SetToolTip(label, uitleg);
            toolTip.SetToolTip(AddHeat, uitleg);

            Resize += (object o, EventArgs ea) =>
            {
                int edge = Math.Max(Size.Width, Size.Height) / 36, w = (x * (Size.Width / 3)) + 40, h = (y * (Size.Height / 10)) + edge + 10;
                label.Location = new Point(w, h);
                AddHeat.Location = new Point((w + 10), (h + 20));
            };
            
            Controls.Add(AddHeat);
            Controls.Add(label);
        }

        public void makeLanguageBox(int x, int y, Func<string> name, string uitleg)
        {
            int edge = Math.Max(Size.Width, Size.Height) / 36, w = (x * (Size.Width / 3)) + 40, h = (y * (Size.Height / 10)) + edge + 10;
            languageIndex = new ComboBox();

            LazyLabel label = MakeLazySliderLabel(name, w, h);

            languageIndex.Location = new Point((w + 10), (h + 20));
            languageIndex.AutoSize = false;
            languageIndex.DropDownStyle = ComboBoxStyle.DropDownList;
            string[] languages = new string[multipleLanguages.translations.Keys.Count];
            multipleLanguages.translations.Keys.CopyTo(languages, index: 0);
            languageIndex.Items.AddRange(languages);
            languageIndex.SelectedIndex = Settings.LanguageIndex;

            ToolTip toolTip = new ToolTip();
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = name();
            toolTip.SetToolTip(label, uitleg);
            toolTip.SetToolTip(languageIndex, uitleg);

            Resize += (object o, EventArgs ea) =>
            {
                int edge = Math.Max(Size.Width, Size.Height) / 36, w = (x * (Size.Width / 3)) + 40, h = (y * (Size.Height / 10)) + edge + 10;
                label.Location = new Point(w, h);
                languageIndex.Location = new Point((w + 10), (h + 20));
            };

            Controls.Add(languageIndex);
            Controls.Add(label);
        }
        public void ImportSettings(object o, EventArgs ea) //Imports settings from a given txt file
        { 
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                try
                {
                    if (!(openFileDialog.FileName.EndsWith(".txt")))
                        MessageBox.Show($"Wrong File Type error.\n\nError message: {String.Format("{0} is not of the correct type", openFileDialog.FileName)}\n\n" +
                        "It must be an .txt file");
                    else
                    {
                        StreamReader sr = new StreamReader(openFileDialog.FileName);

                        string[] settings = new string[15];
                        string lineInfo;
                        int i = 0;

                        while ((lineInfo = sr.ReadLine()) != null)
                        {
                            settings[i] = lineInfo;
                            i++;
                        }

                        sr.Close();
                        Import(settings);
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
        }

        private LazyLabel MakeLazySliderLabel(Func<string> name, int w, int h)
        {
            LazyLabel label = new LazyLabel();
            label.Size = new Size(400, 20);
            label.LazyText = name;
            label.ForeColor = Color.White;
            label.Location = new Point(w, h);
            label.AutoSize = false;
            return label;
        }

        private void Import(string[] settings) //imports all the values to the sliders
        { 
            trackBars[0].Value = (int) float.Parse(settings[0]) * 100;
            trackBars[1].Value = int.Parse(settings[1]);
            trackBars[2].Value = int.Parse(settings[2]);
            trackBars[3].Value = int.Parse(settings[3]);
            trackBars[4].Value = (int)float.Parse(settings[4]) * 100;
            trackBars[5].Value = (int) (float.Parse(settings[5]) * 100);
            trackBars[6].Value = int.Parse(settings[6]);
            trackBars[7].Value = int.Parse(settings[7]);
            trackBars[8].Value = (int)(float.Parse(settings[8]) * 100);
            trackBars[9].Value = (int)(float.Parse(settings[9]) * 10000);
            trackBars[10].Value = (int)(float.Parse(settings[10]) * 1000000);
            AddHeat.Checked = bool.Parse(settings[11]);
            languageIndex.SelectedIndex = int.Parse(settings[11]);
        }

        public void ImplementChanges(object o, EventArgs ea)
        {
            if (changed || languageIndex.SelectedIndex != Settings.LanguageIndex || AddHeat.Checked != Settings.AddHeatMap)
            {
                DialogResult implement = MessageBox.Show(multipleLanguages.DisplayText("DRchangedSettings0"), multipleLanguages.DisplayText("DRchangedSettings1"), MessageBoxButtons.YesNo);
                
                if (implement == DialogResult.Yes)
                {
                    ImplementSettings();

                    if (TE != Settings.StartCarnivore || SE != Settings.StartHerbivore || MH != Settings.MiddleHeight)
                    {
                        DialogResult restart = MessageBox.Show(multipleLanguages.DisplayText("DRdoYouWantToRestart0"), multipleLanguages.DisplayText("DRdoYouWantToRestart1"), MessageBoxButtons.YesNo);
                        
                        if (restart == DialogResult.Yes)
                        {
                            Settings.StartCarnivore = TE;
                            Settings.StartHerbivore = SE;
                            Settings.MiddleHeight = MH;
                            newMap = true;
                        }
                        else if (restart == DialogResult.No)
                        {
                            trackBars[1].Value = Settings.StartCarnivore;
                            trackBars[2].Value = Settings.StartHerbivore;
                            trackBars[4].Value = (int) Settings.MiddleHeight * 100;
                        }
                    }
                }
            }
            this.Refresh();
        }

        public void ExportSettings(object o, EventArgs e)
        {
            StreamWriter stream;

            SaveFileDialog.CreatePrompt = true;
            SaveFileDialog.OverwritePrompt = true;
            SaveFileDialog.FileName = "HoL Settings";
            SaveFileDialog.DefaultExt = "txt";
            SaveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            SaveFileDialog.FilterIndex = 2;
            SaveFileDialog.InitialDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                stream =new StreamWriter(SaveFileDialog.OpenFile());
                stream.Write(Export());
                stream.Close();
            }
        }

    private string Export()
        {
            string export;
            export = Sp.ToString() + "\n";
            export += TE + "\n";
            export += SE + "\n";
            export += HS + "\n";
            export += MH + "\n";
            export += MC + "\n";
            export += GG + "\n";
            export += GMF + "\n";
            export += WE + "\n";
            export += JE + "\n";
            export += PE + "\n";
            export += AddHeat.Checked + "\n";
            export += languageIndex.SelectedIndex;
            return export;
        }
        private void StartSettings() //Gives all abstract between values their start value, preventing implementation problems
        {
            TE = Settings.StartCarnivore;
            SE = Settings.StartHerbivore;
            HS = Settings.HatchSpeed;
            GG = Settings.GrassGrowth;
            GMF = Settings.GrassMaxFeed;
            Sp = Settings.StepSize;
            MH = Settings.MiddleHeight;
            MC = Settings.MatingCost;
            WE = Settings.WalkEnergy;
            JE = Settings.JumpEnergy;
            PE = Settings.PassiveEnergy;
        }

        private void ImplementSettings()
        {
            Settings.StepSize = Sp;
            Settings.HatchSpeed = HS;
            Settings.MatingCost = MC;
            Settings.GrassGrowth = GG;
            Settings.GrassMaxFeed = GMF;
            Settings.WalkEnergy = WE;
            Settings.JumpEnergy = JE;
            Settings.PassiveEnergy = PE;
            Settings.AddHeatMap = AddHeat.Checked;
            Settings.LanguageIndex = languageIndex.SelectedIndex;
            changed = false;
        }

        public void RevertSettings()
        {
            trackBars[0].Value = (int) (Settings.StepSize * 100f);
            trackBars[1].Value = Settings.StartCarnivore;
            trackBars[2].Value = Settings.StartHerbivore;
            trackBars[3].Value = Settings.HatchSpeed;
            trackBars[4].Value = (int) ((Settings.MiddleHeight + 1f) * 50f);
            trackBars[5].Value = (int) (Settings.MatingCost * 100f);
            trackBars[6].Value = Settings.GrassGrowth;
            trackBars[7].Value = Settings.GrassMaxFeed;
            trackBars[8].Value = (int) (Settings.WalkEnergy * 100f);
            trackBars[9].Value = (int) (Settings.JumpEnergy * 10000f);
            trackBars[10].Value = (int) (Settings.PassiveEnergy * 1000000f);
            AddHeat.Checked = Settings.AddHeatMap;
            languageIndex.SelectedIndex = Settings.LanguageIndex;
        }
    }
}
