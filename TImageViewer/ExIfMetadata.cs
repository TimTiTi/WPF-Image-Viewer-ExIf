using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TImageViewer
{
    public enum ColorRepresentation
    {
        sRGB,
        Uncalibrated
    }

    public enum FlashMode
    {
        FlashFired,
        FlashDidNotFire
    }

    public enum ExposureMode
    {
        Manual,
        NormalProgram,
        AperturePriority,
        ShutterPriority,
        LowSpeedMode,
        HighSpeedMode,
        PortraitMode,
        LandscapeMode,
        Unknown
    }

    public enum WhiteBalanceMode
    {
        Daylight,
        Fluorescent,
        Tungsten,
        Flash,
        StandardLightA,
        StandardLightB,
        StandardLightC,
        D55,
        D65,
        D75,
        Other,
        Unknown
    }

   public class ExIfMetadata
    {
        BitmapFrame frame;
        BitmapMetadata metadata;
        BitmapImage _bitmapImage;
        FileInfo _fileinfo;

        public ExIfMetadata(String filename, BitmapImage bitmapImage)
        {          
            try
            {
                _bitmapImage = bitmapImage;
                _fileinfo = new FileInfo(filename);
                 frame = BitmapFrame.Create(new Uri(filename), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                metadata = (BitmapMetadata)frame.Metadata;                
            }
            catch (System.NotSupportedException)
            {
                try {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true))
                    {
                        DateTime localDate = DateTime.Now;
                        file.WriteLine(localDate.ToString() + " " + "Format nije podrzan! -> ExIfMetadata");
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    DirectoryInfo di = Directory.CreateDirectory("C:\\Users\\Public\\ITVC\\TImageViewer");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\ITVC\TImageViewer\ErrorLog.txt", true))
                    {
                        DateTime localDate = DateTime.Now;
                        file.WriteLine(localDate.ToString() + " " + "Format nije podrzan! -> ExIfMetadata: " + filename);
                    }
                }
            }
          
        }
        private decimal ParseUnsignedRational(ulong exifValue)
        {
            return (decimal)(exifValue & 0xFFFFFFFFL) / (decimal)((exifValue & 0xFFFFFFFF00000000L) >> 32);
        }

        private decimal ParseSignedRational(long exifValue)
        {
            return (decimal)(exifValue & 0xFFFFFFFFL) / (decimal)((exifValue & 0x7FFFFFFF00000000L) >> 32);
        }
        private object QueryMetadata(string query)
        {
            try
            {
                if (metadata.ContainsQuery(query))
                    return metadata.GetQuery(query);
                else
                    return null;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                //MessageBox.Show("Nije se moglo System.Runtime.InteropServices.COMException");
                return null;
            }
            catch (System.NullReferenceException)
            {
                //MessageBox.Show("Nije se moglo System.NullReferenceException");
                return null;
            }
        }
        public string Name
        {
            get
            {
                return _fileinfo.Name;
            }
        }
        public string CreationTime
        {
            get
            {
                return _fileinfo.CreationTime.ToString();
            }
        }
        public string FullName
        {
            get
            {
                return _fileinfo.FullName;
            }
        }
        public string Extension
        {
            get
            {
                return _fileinfo.Extension;
            }
        }
        public string Size
        {
            get
            {
                return _fileinfo.Length / 1024 + "KB";
            }
        }
        public uint? ActualWidth
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif/subifd:{uint=40962}");
                if (val == null)
                {
                    return System.Convert.ToUInt32(_bitmapImage.Width);
                }
                else
                {
                    if (val.GetType() == typeof(UInt32))
                    {
                        return (uint?)val;
                    }
                    else
                    {
                        return System.Convert.ToUInt32(val);
                    }
                }
            }
        }

        public uint? ActualHeight
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif/subifd:{uint=40963}");
                if (val == null)
                {
                    return System.Convert.ToUInt32(_bitmapImage.Height);
                }
                else
                {
                    if (val.GetType() == typeof(UInt32))
                    {
                        return (uint?)val;
                    }
                    else
                    {
                        return System.Convert.ToUInt32(val);
                    }
                }
            }
        }

        public decimal? lResolutioHorizontal
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif:{uint=282}");
                if (val != null)
                {
                    return ParseUnsignedRational((ulong)val);
                }
                else
                {
                    return System.Convert.ToDecimal(_bitmapImage.DpiX);
                }
            }
        }

        public decimal? ResolutionVertical
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif:{uint=283}");
                if (val != null)
                {
                    return ParseUnsignedRational((ulong)val);
                }
                else
                {
                    return System.Convert.ToDecimal(_bitmapImage.DpiY);
                }
            }
        }

        public string EquipmentManufacturer
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif:{uint=271}");
                return (val != null ? (string)val : String.Empty);
            }
        }

        public string CameraModel
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif:{uint=272}");
                return (val != null ? (string)val : String.Empty);
            }
        }

        public string CreationSoftware
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif:{uint=305}");
                return (val != null ? (string)val : String.Empty);
            }
        }
        public ColorRepresentation ColorRepresentation
        {
            get
            {
                if ((ushort?)QueryMetadata("/app1/ifd/exif/subifd:{uint=40961}") == 1)
                    return ColorRepresentation.sRGB;
                else
                    return ColorRepresentation.Uncalibrated;
            }
        }

        public decimal? ExposureTime
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif/subifd:{uint=33434}");
                if (val != null)
                {
                    return ParseUnsignedRational((ulong)val);
                }
                else
                {
                    return null;
                }
            }
        }

        public decimal? ExposureCompensation
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif/subifd:{uint=37380}");
                if (val != null)
                {
                    return ParseSignedRational((long)val);
                }
                else
                {
                    return null;
                }
            }
        }

        public decimal? LensAperture
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif/subifd:{uint=33437}");
                if (val != null)
                {
                    return ParseUnsignedRational((ulong)val);
                }
                else
                {
                    return null;
                }
            }
        }

        public decimal? FocalLength
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif/subifd:{uint=37386}");
                if (val != null)
                {
                    return ParseUnsignedRational((ulong)val);
                }
                else
                {
                    return null;
                }
            }
        }

        public ushort? IsoSpeed
        {
            get
            {
                return (ushort?)QueryMetadata("/app1/ifd/exif/subifd:{uint=34855}");
            }
        }

        public FlashMode FlashMode
        {
            get
            {
                if ((ushort?)QueryMetadata("/app1/ifd/exif/subifd:{uint=37385}") % 2 == 1)
                    return FlashMode.FlashFired;
                else
                    return FlashMode.FlashDidNotFire;
            }
        }

        public ExposureMode ExposureMode
        {
            get
            {
                ushort? mode = (ushort?)QueryMetadata("/app1/ifd/exif/subifd:{uint=34850}");

                if (mode == null)
                {
                    return ExposureMode.Unknown;
                }
                else
                {
                    switch ((int)mode)
                    {
                        case 1: return ExposureMode.Manual;
                        case 2: return ExposureMode.NormalProgram;
                        case 3: return ExposureMode.AperturePriority;
                        case 4: return ExposureMode.ShutterPriority;
                        case 5: return ExposureMode.LowSpeedMode;
                        case 6: return ExposureMode.HighSpeedMode;
                        case 7: return ExposureMode.PortraitMode;
                        case 8: return ExposureMode.LandscapeMode;
                        default: return ExposureMode.Unknown;
                    }
                }
            }
        }

        public WhiteBalanceMode WhiteBalanceMode
        {
            get
            {
                ushort? mode = (ushort?)QueryMetadata("/app1/ifd/exif/subifd:{uint=37384}");

                if (mode == null)
                {
                    return WhiteBalanceMode.Unknown;
                }
                else
                {
                    switch ((int)mode)
                    {
                        case 1: return WhiteBalanceMode.Daylight;
                        case 2: return WhiteBalanceMode.Fluorescent;
                        case 3: return WhiteBalanceMode.Tungsten;
                        case 10: return WhiteBalanceMode.Flash;
                        case 17: return WhiteBalanceMode.StandardLightA;
                        case 18: return WhiteBalanceMode.StandardLightB;
                        case 19: return WhiteBalanceMode.StandardLightC;
                        case 20: return WhiteBalanceMode.D55;
                        case 21: return WhiteBalanceMode.D65;
                        case 22: return WhiteBalanceMode.D75;
                        case 255: return WhiteBalanceMode.Other;
                        default: return WhiteBalanceMode.Unknown;
                    }
                }
            }
        }

        public DateTime? DateImageTaken
        {
            get
            {
                object val = QueryMetadata("/app1/ifd/exif/subifd:{uint=36867}");
                if (val == null)
                {
                    return null;
                }
                else
                {
                    string date = (string)val;
                    try
                    {
                        return new DateTime(
                            int.Parse(date.Substring(0, 4)),    // year
                            int.Parse(date.Substring(5, 2)),    // month
                            int.Parse(date.Substring(8, 2)),    // day
                            int.Parse(date.Substring(11, 2)),   // hour
                            int.Parse(date.Substring(14, 2)),   // minute
                            int.Parse(date.Substring(17, 2))    // second
                        );
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                    catch (OverflowException)
                    {
                        return null;
                    }
                    catch (ArgumentNullException)
                    {
                        return null;
                    }
                    catch (NullReferenceException)
                    {
                        return null;
                    }
                }
            }

        }
    }
}
