using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneDriveSimple.Helpers
{
    public static class LongExtensions
    {
        public static string ConvertSize(this long sizeInBytes)
        {
            if (sizeInBytes > 1024 * 1024 * 1024)
            {
                return $"{((sizeInBytes / 1024.0) / 1024.0 / 1024):N2} GB";
            }

            if (sizeInBytes > 1024 * 1024)
            {
                return $"{((sizeInBytes / 1024.0) / 1024.0):N2} MB";
            }

            if (sizeInBytes > 1024)
            {
                return $"{(sizeInBytes / 1024.0):N2} kB";
            }

            return $"{sizeInBytes} bytes";
        }
    }
}
