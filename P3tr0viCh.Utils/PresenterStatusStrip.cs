using System;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public class PresenterStatusStrip<T> where T : Enum
    {
        public interface IPresenterStatusStrip
        {
            ToolStripStatusLabel GetLabel(T label);
        }

        public IPresenterStatusStrip View { get; private set; }

        public PresenterStatusStrip(IPresenterStatusStrip view)
        {
            View = view;
        }
    }
}