using System.Collections.Generic;
using System.Windows.Forms;

namespace Keyboard
{
    public class ModifierExtractor
    {
        private readonly bool _alt;
        private readonly bool _ctrl;
        private readonly bool _shift;

        public ModifierExtractor(Keys modifiedKey)
        {
            _shift = modifiedKey.HasFlag(Keys.Shift);
            _ctrl = modifiedKey.HasFlag(Keys.Control);
            _alt = modifiedKey.HasFlag(Keys.Alt);
        }

        public IEnumerable<Keys> AsEnumerable()
        {
            if (_shift) yield return Keys.ShiftKey;
            if (_ctrl) yield return Keys.ControlKey;
            if (_alt) yield return Keys.Menu;
            yield break;
        }
    }
}