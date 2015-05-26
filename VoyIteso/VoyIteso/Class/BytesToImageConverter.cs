using Microsoft.Phone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using VoyIteso.Class;
using VoyIteso.Resources;
using Windows.Storage.Streams;

namespace VoyIteso.Converters
{
    public class BytesToImageConverter : IValueConverter
    {
        public byte[] bytes;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value != null && value is byte[])
            {
                bytes = value as byte[];
                WriteableBitmap image = new WriteableBitmap(20, 20);
                //BitmapImage image = new BitmapImage();
                
                using (MemoryStream ms = new MemoryStream(bytes,0,bytes.Length,true,true))
                {
                    
                    ms.Position = 0;
                    image.SetSource(ms);
                    
                    //image.SaveJpeg(ms, 70, 70, 0, 100);
                }
                
                return image;
                 
            }
            return null;
        }
        public object ConvertBack(object value, Type taretType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
