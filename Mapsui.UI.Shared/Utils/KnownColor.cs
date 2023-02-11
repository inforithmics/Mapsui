using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui.UI.Extensions;

namespace Mapsui.UI.Utils;

#if __MAUI__    
    /// <summary> Known Color Helper for Maui </summary>
    public static class KnownColor
    {
        public static Color White => Mapsui.Styles.Color.White.ToNative();
        public static Color Red => Mapsui.Styles.Color.Red.ToNative();
        public static Color Black => Mapsui.Styles.Color.Black.ToNative();
        public static Color DarkGray => Mapsui.Styles.Color.DarkGray.ToNative();
    }
}
#endif

