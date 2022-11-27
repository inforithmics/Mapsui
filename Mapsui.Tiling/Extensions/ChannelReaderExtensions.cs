using System.Threading.Channels;

namespace Mapsui.Tiling.Extensions
{
    public static class ChannelReaderExtensions
    {
        public static void Clear<T>(this ChannelReader<T> reader)
        {
            // read all items to clear the channel
            while (reader.TryRead(out var _))
            {
            }
        }
    }
}