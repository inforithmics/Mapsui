using System;
using System.Collections.Generic;
using System.Text;

namespace Mapsui.Nts.Objects;

[Flags]
public enum FontAttributes
{
    /// <summary>The font is unmodified.</summary>
    None = 0,
    /// <summary>The font is bold.</summary>
    Bold = 1,
    /// <summary>The font is italic.</summary>
    Italic = 2,
}
