using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace Resizer.Utils
{
    public class ImageUtils
    {
        public static byte[] Resize(byte[] file, float percent)
        {
            using (Image<Rgba32> image = Image.Load(file))
            {
                image.Mutate(x => x
                     .Resize((int) (image.Width / 100 * percent), (int)(image.Height / 100 * percent))
                     .Grayscale());
                MemoryStream memoryStream = new MemoryStream();
                image.SaveAsJpeg(memoryStream);
                return memoryStream.ToArray();
            }
        }


    }
}
