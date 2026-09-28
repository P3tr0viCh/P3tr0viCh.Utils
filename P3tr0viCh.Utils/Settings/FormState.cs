using System.Drawing;

namespace P3tr0viCh.Utils.Settings
{
    public class FormState
    {
        public Rectangle Bounds { get; set; } = default;
        public bool Maximized { get; set; } = false;
    }
}