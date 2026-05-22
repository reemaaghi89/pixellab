using System.Collections.Generic;

namespace pixellab
{
    public class ColorChannel
    {
        public string Name { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public static class ColorSpaceRegistry
    {
        private static readonly Dictionary<string, List<ColorChannel>> _colorSpaces = new Dictionary<string, List<ColorChannel>>();

        static ColorSpaceRegistry()
        {
            Initialize();
        }

        private static void Initialize()
        {
            _colorSpaces["RGB"] = new List<ColorChannel>
            {
                new ColorChannel { Name="Red", Min=0, Max=255 },
                new ColorChannel { Name="Green", Min=0, Max=255 },
                new ColorChannel { Name="Blue", Min=0, Max=255 }
            };

            _colorSpaces["HSV"] = new List<ColorChannel>
            {
                new ColorChannel { Name="Hue", Min=0, Max=360 },
                new ColorChannel { Name="Saturation", Min=0, Max=100 },
                new ColorChannel { Name="Value", Min=0, Max=100 }
            };

            _colorSpaces["CMYK"] = new List<ColorChannel>
            {
                new ColorChannel { Name="Cyan", Min=0, Max=100 },
                new ColorChannel { Name="Magenta", Min=0, Max=100 },
                new ColorChannel { Name="Yellow", Min=0, Max=100 },
                new ColorChannel { Name="Black", Min=0, Max=100 }
            };

            _colorSpaces["LAB"] = new List<ColorChannel>
            {
                new ColorChannel { Name="L", Min=0, Max=100 },
                new ColorChannel { Name="A", Min=-128, Max=127 },
                new ColorChannel { Name="B", Min=-128, Max=127 }
            };

            _colorSpaces["YUV"] = new List<ColorChannel>
            {
                new ColorChannel { Name="Y", Min=0, Max=255 },
                new ColorChannel { Name="U", Min=-128, Max=127 },
                new ColorChannel { Name="V", Min=-128, Max=127 }
            };

            _colorSpaces["YCbCr"] = new List<ColorChannel>
            {
                new ColorChannel { Name="Y", Min=16, Max=235 },
                new ColorChannel { Name="Cb", Min=16, Max=240 },
                new ColorChannel { Name="Cr", Min=16, Max=240 }
            };
        }

        public static List<ColorChannel> GetChannels(string systemName)
        {
            return _colorSpaces.ContainsKey(systemName) ? _colorSpaces[systemName] : new List<ColorChannel>();
        }
    }
}