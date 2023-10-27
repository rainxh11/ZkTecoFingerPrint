using System.IO;
using System.Runtime.InteropServices;

namespace ZkTecoFingerPrint
{
    public class BitmapFormat
    {
        public struct Bitmapfileheader
        {
            public ushort BfType;
            public int BfSize;
            public ushort BfReserved1;
            public ushort BfReserved2;
            public int BfOffBits;
        }

        public struct Mask
        {
            public byte Redmask;
            public byte Greenmask;
            public byte Bluemask;
            public byte RgbReserved;
        }

        public struct Bitmapinfoheader
        {
            public int BiSize;
            public int BiWidth;
            public int BiHeight;
            public ushort BiPlanes;
            public ushort BiBitCount;
            public int BiCompression;
            public int BiSizeImage;
            public int BiXPelsPerMeter;
            public int BiYPelsPerMeter;
            public int BiClrUsed;
            public int BiClrImportant;
        }

        public static byte[] RotatePic(byte[] bmpBuf, int width, int height, int returnBufferLength)
        {
            
            var bmpBuflen = width * height;
            var resBuf = new byte[returnBufferLength];
            for (var rowLoop = 0; rowLoop < bmpBuflen;)
            {
                for (var colLoop = 0; colLoop < width; colLoop++)
                    resBuf[rowLoop + colLoop] = bmpBuf[bmpBuflen - rowLoop - width + colLoop];

                rowLoop += width;
            }
            return resBuf;
        }

        public static byte[] StructToBytes(object structObj, int size)
        {
            var structSize = Marshal.SizeOf(structObj);
            var getBytes = new byte[structSize];

            var structPtr = Marshal.AllocHGlobal(structSize);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, getBytes, 0, structSize);
            Marshal.FreeHGlobal(structPtr);

            if (size != 14) return getBytes;
            var newBytes = new byte[size];
            var count = 0;

            for (var loop = 0; loop < structSize; loop++)
                if (loop != 2 && loop != 3)
                {
                    newBytes[count] = getBytes[loop];
                    count++;
                }

            return newBytes;
        }
        public static MemoryStream GetBitmap(byte[] buffer, int width, int height)
        {
            var ms = new MemoryStream();
            const ushort mNBitCount = 8;
            const int mNColorTableEntries = 256;

            var bmpHeader = new Bitmapfileheader();
            var bmpInfoHeader = new Bitmapinfoheader();
            var colorMask = new Mask[mNColorTableEntries];

            var w = (width + 3) / 4 * 4;

            bmpInfoHeader.BiSize = Marshal.SizeOf(bmpInfoHeader);
            bmpInfoHeader.BiWidth = width;
            bmpInfoHeader.BiHeight = height;
            bmpInfoHeader.BiPlanes = 1;
            bmpInfoHeader.BiBitCount = mNBitCount;
            bmpInfoHeader.BiCompression = 0;
            bmpInfoHeader.BiSizeImage = 0;
            bmpInfoHeader.BiXPelsPerMeter = 0;
            bmpInfoHeader.BiYPelsPerMeter = 0;
            bmpInfoHeader.BiClrUsed = mNColorTableEntries;
            bmpInfoHeader.BiClrImportant = mNColorTableEntries;

            bmpHeader.BfType = 0x4D42;
            bmpHeader.BfOffBits = 14 + Marshal.SizeOf(bmpInfoHeader) + bmpInfoHeader.BiClrUsed * 4;
            bmpHeader.BfSize = bmpHeader.BfOffBits +
                               (w * bmpInfoHeader.BiBitCount + 31) / 32 * 4 * bmpInfoHeader.BiHeight;
            bmpHeader.BfReserved1 = 0;
            bmpHeader.BfReserved2 = 0;

            ms.Write(StructToBytes(bmpHeader, 14), 0, 14);
            ms.Write(StructToBytes(bmpInfoHeader, Marshal.SizeOf(bmpInfoHeader)), 0, Marshal.SizeOf(bmpInfoHeader));

            for (var colorIndex = 0; colorIndex < mNColorTableEntries; colorIndex++)
            {
                colorMask[colorIndex].Redmask = (byte)colorIndex;
                colorMask[colorIndex].Greenmask = (byte)colorIndex;
                colorMask[colorIndex].Bluemask = (byte)colorIndex;
                colorMask[colorIndex].RgbReserved = 0;

                ms.Write(StructToBytes(colorMask[colorIndex], Marshal.SizeOf(colorMask[colorIndex])), 0,
                         Marshal.SizeOf(colorMask[colorIndex]));
            }

            var resBuf = RotatePic(buffer, width, height, width * height * 2);

            for (var i = 0; i < height; i++)
            {
                ms.Write(resBuf, i * width, width);
                if (w - width > 0) ms.Write(resBuf, 0, w - width);
            }

            return ms;
        }
    }
}