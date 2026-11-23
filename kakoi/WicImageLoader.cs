using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace kakoi
{
    /// <summary>
    /// Windows Imaging Component (WIC: windowscodecs.dll) を利用して
    /// WebP / PNG / JPEG / GIF / BMP 等をデコードし、円形アバター PNG を生成するヘルパー。
    /// 外部 NuGet パッケージ・ネイティブ DLL 同梱不要。
    /// </summary>
    internal static class WicImageLoader
    {
        private const uint CLSCTX_INPROC_SERVER = 0x1;
        private const uint WICDecodeMetadataCacheOnLoad = 0x1;
        private const int MaxDimension = 16384;

        private static readonly Guid CLSID_WICImagingFactory = new("cacaf262-9370-4615-a13b-9f5539da4c0a");
        private static readonly Guid IID_IWICImagingFactory = new("ec5ec8a9-c395-4314-9c77-54d7a935ff70");
        private static readonly Guid GUID_WICPixelFormat32bppBGRA = new("6fddc324-4e03-4bfe-b185-3d77768dc90f");

        [DllImport("shlwapi.dll", ExactSpelling = true)]
        private static extern IntPtr SHCreateMemStream(byte[] pInit, uint cbInit);

        [DllImport("ole32.dll", ExactSpelling = true)]
        private static extern int CoCreateInstance(
            ref Guid rclsid,
            IntPtr pUnkOuter,
            uint dwClsContext,
            ref Guid riid,
            out IntPtr ppv);

        [DllImport("windowscodecs.dll", ExactSpelling = true)]
        private static extern int WICConvertBitmapSource(
            ref Guid dstFormat,
            IntPtr pISrc,
            out IntPtr ppIDst);

        /// <summary>
        /// 画像バイト配列を WIC（失敗時は GDI+ フォールバック）で Bitmap にデコードする。
        /// 非対応形式（SVG 等）や破損データの場合は null を返す。
        /// </summary>
        public static Bitmap? DecodeBitmap(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            Bitmap? bitmap = DecodeWithWic(imageBytes);
            if (bitmap != null)
            {
                return bitmap;
            }

            // フォールバック: 標準 GDI+
            try
            {
                using var ms = new MemoryStream(imageBytes);
                using var img = Image.FromStream(ms, useEmbeddedColorManagement: false, validateImageData: false);
                if (img.Width <= 0 || img.Height <= 0 || img.Width > MaxDimension || img.Height > MaxDimension)
                {
                    return null;
                }
                return new Bitmap(img);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GDI+ デコードスキップ: {ex.Message}");
                return null;
            }
        }

        private static unsafe Bitmap? DecodeWithWic(byte[] imageBytes)
        {
            IntPtr pStream = IntPtr.Zero;
            IntPtr pFactory = IntPtr.Zero;
            IntPtr pDecoder = IntPtr.Zero;
            IntPtr pFrame = IntPtr.Zero;
            IntPtr pConverted = IntPtr.Zero;

            try
            {
                pStream = SHCreateMemStream(imageBytes, (uint)imageBytes.Length);
                if (pStream == IntPtr.Zero)
                {
                    return null;
                }

                Guid clsid = CLSID_WICImagingFactory;
                Guid iid = IID_IWICImagingFactory;
                int hr = CoCreateInstance(ref clsid, IntPtr.Zero, CLSCTX_INPROC_SERVER, ref iid, out pFactory);
                if (hr < 0 || pFactory == IntPtr.Zero)
                {
                    return null;
                }

                // Store 拡張コーデック (WebP 等) は RCW 経由の QueryInterface で E_NOINTERFACE を返すことがあるため、
                // COM vtable を直接呼び出す。
                // IWICImagingFactory vtable[4] = CreateDecoderFromStream
                IntPtr* factoryVtbl = *(IntPtr**)pFactory;
                var createDecoderFromStream = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, uint, out IntPtr, int>)factoryVtbl[4];
                hr = createDecoderFromStream(pFactory, pStream, IntPtr.Zero, WICDecodeMetadataCacheOnLoad, out pDecoder);
                if (hr < 0 || pDecoder == IntPtr.Zero)
                {
                    return null;
                }

                // IWICBitmapDecoder vtable[13] = GetFrame
                IntPtr* decoderVtbl = *(IntPtr**)pDecoder;
                var getFrame = (delegate* unmanaged[Stdcall]<IntPtr, uint, out IntPtr, int>)decoderVtbl[13];
                hr = getFrame(pDecoder, 0, out pFrame);
                if (hr < 0 || pFrame == IntPtr.Zero)
                {
                    return null;
                }

                Guid targetFormat = GUID_WICPixelFormat32bppBGRA;
                hr = WICConvertBitmapSource(ref targetFormat, pFrame, out pConverted);
                if (hr < 0 || pConverted == IntPtr.Zero)
                {
                    return null;
                }

                // IWICBitmapSource vtable[3] = GetSize, vtable[7] = CopyPixels
                IntPtr* sourceVtbl = *(IntPtr**)pConverted;
                var getSize = (delegate* unmanaged[Stdcall]<IntPtr, out uint, out uint, int>)sourceVtbl[3];
                hr = getSize(pConverted, out uint width, out uint height);
                if (hr < 0 || width == 0 || height == 0 || width > MaxDimension || height > MaxDimension)
                {
                    return null;
                }

                var bitmap = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
                try
                {
                    var rect = new Rectangle(0, 0, (int)width, (int)height);
                    BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    try
                    {
                        if (data.Stride <= 0)
                        {
                            hr = -1;
                        }
                        else
                        {
                            uint stride = (uint)data.Stride;
                            uint bufferSize = stride * height;
                            var copyPixels = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, uint, uint, IntPtr, int>)sourceVtbl[7];
                            hr = copyPixels(pConverted, IntPtr.Zero, stride, bufferSize, data.Scan0);
                        }
                    }
                    finally
                    {
                        bitmap.UnlockBits(data);
                    }

                    if (hr < 0)
                    {
                        bitmap.Dispose();
                        return null;
                    }
                    return bitmap;
                }
                catch
                {
                    bitmap.Dispose();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WIC デコード失敗: {ex.Message}");
                return null;
            }
            finally
            {
                if (pConverted != IntPtr.Zero) Marshal.Release(pConverted);
                if (pFrame != IntPtr.Zero) Marshal.Release(pFrame);
                if (pDecoder != IntPtr.Zero) Marshal.Release(pDecoder);
                if (pFactory != IntPtr.Zero) Marshal.Release(pFactory);
                if (pStream != IntPtr.Zero) Marshal.Release(pStream);
            }
        }

        /// <summary>
        /// 画像バイト配列を中央正方形クロップ＋指定サイズにリサイズ＋円形切り抜きして PNG 保存する。
        /// </summary>
        public static bool SaveCircularAvatarPng(byte[] imageBytes, int targetSize, string outputPath)
        {
            if (targetSize <= 0)
            {
                return false;
            }

            using Bitmap? source = DecodeBitmap(imageBytes);
            if (source == null)
            {
                return false;
            }

            using var result = new Bitmap(targetSize, targetSize, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                using var clipPath = new GraphicsPath();
                clipPath.AddEllipse(0, 0, targetSize, targetSize);
                g.SetClip(clipPath);

                int cropSize = Math.Min(source.Width, source.Height);
                int cropX = (source.Width - cropSize) / 2;
                int cropY = (source.Height - cropSize) / 2;

                using var attrs = new ImageAttributes();
                attrs.SetWrapMode(WrapMode.TileFlipXY);
                g.DrawImage(
                    source,
                    new Rectangle(0, 0, targetSize, targetSize),
                    cropX,
                    cropY,
                    cropSize,
                    cropSize,
                    GraphicsUnit.Pixel,
                    attrs);
            }

            string? dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string tempPath = $"{outputPath}.{Guid.NewGuid():N}.tmp";
            try
            {
                result.Save(tempPath, ImageFormat.Png);
                File.Move(tempPath, outputPath, overwrite: true);
                return true;
            }
            catch
            {
                try
                {
                    if (File.Exists(tempPath)) File.Delete(tempPath);
                }
                catch
                {
                }
                throw;
            }
        }
    }
}
