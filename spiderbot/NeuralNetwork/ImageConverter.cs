using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialIntelligence
{
    internal class ImageConverter
    {
        public Bitmap NeuronMapToImage(byte[] array, uint mask)
        {
            int xyValue = (int)(Math.Sqrt(array.Length));
            Bitmap myBitmap = new Bitmap(xyValue, xyValue);
            byte[,] matrix = ConvertMatrix(array.Take((int)Math.Pow(xyValue, 2)).ToArray(), xyValue, xyValue);


            // Set each pixel in myBitmap to black.
            for (int Xcount = 0; Xcount < myBitmap.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < myBitmap.Height; Ycount++)
                {

                    myBitmap.SetPixel(Xcount, Ycount, Color.FromArgb((int)(matrix[Xcount, Ycount] | mask)));


                }
            }
            return myBitmap;
        }

        static byte[,] ConvertMatrix(byte[] flat, int m, int n)
        {
            if (flat.Length != m * n)
            {
                throw new ArgumentException("Invalid length");
            }
            byte[,] ret = new byte[m, n];
            // BlockCopy uses byte lengths: a double is 8 bytes
            Buffer.BlockCopy(flat, 0, ret, 0, flat.Length * sizeof(byte));
            return ret;
        }
    }
}
