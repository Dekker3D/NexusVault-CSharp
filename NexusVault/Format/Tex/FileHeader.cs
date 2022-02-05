using System.Runtime.InteropServices;

namespace NexusVault.Format.Tex
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileHeader
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ChannelInfo
        {
            public byte quality;
            public byte hasDefaultValue;
            public byte defaultValue;
        }

        public uint signature; // " GFX"
        public uint version;

        public uint width;
        public uint height;
        public uint deepth;
        public uint sides;
        public uint mipmapsCount; // max 13

        public uint format; // argb(0), argb(1), rgb(5), grayscale(6), dxt1(13), dxt3(14), dxt5(15), not used for jpg
        [MarshalAs(UnmanagedType.Bool)]
        public bool isJpg;
        public uint jpgType; // 0, 1, 2

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ChannelInfo[] channelInfos;

        public uint jpgSizeCount; // max 13
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public uint[] jpgSize;

        private readonly uint padding;
    }
}
