using System;
using System.Windows.Forms;

namespace IntroProject.Presentation.Controls
{
    public class LazyLabel : Label
    {
        public Func<string> LazyText { get; set; } = () => "";
        public override string Text { get => LazyText(); }
    }
}
