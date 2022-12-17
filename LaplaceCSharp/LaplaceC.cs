//namespace LaplaceCSharp
//{
//    public class LaplaceC
//    {
//        public void processImage(byte[] pixels, byte[] processed, int[] filter)
//        {
//            int j = 0; // for 1px
//                       //{
//            int blue = 0;
//            int green = 0;
//            int red = 0;

//            blue += pixels[i + j - stride - bpp] * filter[0]; // top left
//            blue += pixels[i + j - stride] * filter[1]; // top middle
//            blue += pixels[i + j - stride + bpp] * filter[2]; // top right
//            blue += pixels[i + j - bpp] * filter[3]; // middle left
//            blue += pixels[i + j] * filter[4]; // middle
//            blue += pixels[i + j + bpp] * filter[5]; // middle right
//            blue += pixels[i + j + stride - bpp] * filter[6]; // bottom left
//            blue += pixels[i + j + stride] * filter[7]; //bottom middle
//            blue += pixels[i + j + stride + bpp] * filter[8]; // bottom right

//            green += pixels[i + j - stride - bpp + 1] * filter[0];
//            green += pixels[i + j - stride + 1] * filter[1];
//            green += pixels[i + j - stride + bpp + 1] * filter[2];
//            green += pixels[i + j - bpp + 1] * filter[3];
//            green += pixels[i + j + 1] * filter[4];
//            green += pixels[i + j + bpp + 1] * filter[5];
//            green += pixels[i + j + stride - bpp + 1] * filter[6];
//            green += pixels[i + j + stride + 1] * filter[7];
//            green += pixels[i + j + stride + bpp + 1] * filter[8];

//            red += pixels[i + j - stride - bpp + 2] * filter[0];
//            red += pixels[i + j - stride + 2] * filter[1];
//            red += pixels[i + j - stride + bpp + 2] * filter[2];
//            red += pixels[i + j - bpp + 2] * filter[3];
//            red += pixels[i + j + 2] * filter[4];
//            red += pixels[i + j + bpp + 2] * filter[5];
//            red += pixels[i + j + stride - bpp + 2] * filter[6];
//            red += pixels[i + j + stride + 2] * filter[7];
//            red += pixels[i + j + stride + bpp + 2] * filter[8];

//            if (red < 0) red = 0;
//            else if (red > 255) red = 255;
//            if (green < 0) green = 0;
//            else if (green > 255) green = 255;
//            if (blue < 0) blue = 0;
//            else if (blue > 255) blue = 255;
//            processed[i + j] = (byte)blue;
//            processed[i + j + 1] = (byte)green;
//            processed[i + j + 2] = (byte)red;
//            processed[i + j + 2] = (byte)red;
//            processed[i + j + 3] = pixels[i + j + 3]; // alpha
//                                                        // }
//                                                        //}
//        }
//    }
//}
namespace LaplaceCSharp
{
    public class LaplaceC
    {
        public void processImage(byte[] original, byte[] processed, int[] filter, int stride, int bpp, int begStride)
        {
            //for (int i = stride; i < original.Length - stride; i += stride)
            //{
            int i = begStride;
            //for (int j = bpp; j != stride - bpp; j += bpp) // for full stride
            int j = 0; // for 1px
                       //{
            int blue = 0;
            int green = 0;
            int red = 0;

            blue += original[i + j - stride - bpp] * filter[0]; // top left
            blue += original[i + j - stride] * filter[1]; // top middle
            blue += original[i + j - stride + bpp] * filter[2]; // top right
            blue += original[i + j - bpp] * filter[3]; // middle left
            blue += original[i + j] * filter[4]; // middle
            blue += original[i + j + bpp] * filter[5]; // middle right
            blue += original[i + j + stride - bpp] * filter[6]; // bottom left
            blue += original[i + j + stride] * filter[7]; //bottom middle
            blue += original[i + j + stride + bpp] * filter[8]; // bottom right

            green += original[i + j - stride - bpp + 1] * filter[0];
            green += original[i + j - stride + 1] * filter[1];
            green += original[i + j - stride + bpp + 1] * filter[2];
            green += original[i + j - bpp + 1] * filter[3];
            green += original[i + j + 1] * filter[4];
            green += original[i + j + bpp + 1] * filter[5];
            green += original[i + j + stride - bpp + 1] * filter[6];
            green += original[i + j + stride + 1] * filter[7];
            green += original[i + j + stride + bpp + 1] * filter[8];

            red += original[i + j - stride - bpp + 2] * filter[0];
            red += original[i + j - stride + 2] * filter[1];
            red += original[i + j - stride + bpp + 2] * filter[2];
            red += original[i + j - bpp + 2] * filter[3];
            red += original[i + j + 2] * filter[4];
            red += original[i + j + bpp + 2] * filter[5];
            red += original[i + j + stride - bpp + 2] * filter[6];
            red += original[i + j + stride + 2] * filter[7];
            red += original[i + j + stride + bpp + 2] * filter[8];

            if (red < 0) red = 0;
            else if (red > 255) red = 255;
            if (green < 0) green = 0;
            else if (green > 255) green = 255;
            if (blue < 0) blue = 0;
            else if (blue > 255) blue = 255;
            processed[i + j] = (byte)blue;
            processed[i + j + 1] = (byte)green;
            processed[i + j + 2] = (byte)red;
            processed[i + j + 2] = (byte)red;
            processed[i + j + 3] = original[i + j + 3]; // alpha
                                                        // }
                                                        //}
        }
    }
}

//version 1.0
//int filterCounter = 0;
//for (int k = -stride; k != 2 * stride; k += stride)
//{
//    for (int l = -3; l != 6; l += 3)
//    {
//        if (filter[filterCounter] != 0)
//        {
//            blue += original[i + j + k + l] * filter[filterCounter];
//            green += original[i + j + 1 + k + l] * filter[filterCounter];
//            red += original[i + j + 2 + k + l] * filter[filterCounter];
//            
//        }
//        filterCounter++;
//    }
//}
