using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Imaging.Filters;
using AForge;
using AForge.Imaging;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Interop;
using System.Windows;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace WpfApplication1
{
    class Analizer
    {
        // U ovom zadatku koristi se aforge biblioteka za obradu slika u C# jeziku
        // Za zadanu sliku rjesenje je 5.
        // Rezultatna slika nalazi se na web adresi:
        // http://imageshack.us/scaled/landing/221/merged.png
        public static Bitmap FindObjects(System.Drawing.Bitmap image, int hueMin, int hueMax, float saturationMin, float saturationMax, float luminanceMin, float luminanceMax, int blobWidth, int blobHeight)
        {
            // HSL filter
            HSLFiltering filter = new HSLFiltering();

            // Zelimo zadrzati samo crvenu
            filter.Hue = new IntRange(hueMin, hueMax);
            filter.Saturation = new Range(saturationMin, saturationMax);
            filter.Luminance = new Range(luminanceMin, luminanceMax);

            // primjenimo filter
            filter.ApplyInPlace(image);

            // algoritam za trazenje nakupina (eng. blob)
            BlobCounterBase bc = new BlobCounter();
            // postavljamo filter za karakteristike nakupina koje trazimo
            bc.FilterBlobs = true;
            // Trazimo da budu odredene velicine kako bi uklonili sum na slici
            bc.MinWidth = blobWidth;
            bc.MinHeight = blobHeight;
            bc.ObjectsOrder = ObjectsOrder.Size;


            Bitmap redBlobs = image;
            bc.ProcessImage(redBlobs);

            //Trazimo crvene nakupine, tj crvene objekte na slici
            Blob[] blobs = bc.GetObjectsInformation();
            //Za zadanu sliku nasli smo ih pet, i spremljeni su u polje blobs, duzina polja je broj crvenih objekata na slici
            //Console.WriteLine(blobs.Length);
            bc.ExtractBlobsImage(redBlobs, blobs[0], true);

            //DODATNO: Radimo sliku samo sa pronadenim objektima, ovo nam sluzi samo kao provjera
            Merge roses = new Merge();
            Bitmap resultImage = blobs[0].Image.ToManagedImage();

            for (int j = 1; j < blobs.Length; j++)
            {
                bc.ExtractBlobsImage(redBlobs, blobs[j], true);
                roses.OverlayImage = blobs[j].Image.ToManagedImage();
                resultImage = roses.Apply(resultImage);
            }
            //Odkomentirati sljedecu liniju, kako bi slika sa pronadenim objektima bila spremljena na disk, te se onda moze pregledati
            //resultImage.Save(@"C:\merged.jpg");
            return resultImage;

        }

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            if (Application.Current.Dispatcher == null)
                return null; // Is it possible?

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // You need to specify the image format to fill the stream. 
                    // I'm assuming it is PNG
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Make sure to create the bitmap in the UI thread
                    if (InvokeRequired)
                        return (BitmapSource)Application.Current.Dispatcher.Invoke(
                            new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
                            DispatcherPriority.Normal,
                            memoryStream);

                    return CreateBitmapSourceFromBitmap(memoryStream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        private static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
        {
            BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            // This will disconnect the stream from the image completely...
            WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            return writable;
        }
    }
}
