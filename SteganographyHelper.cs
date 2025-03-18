using System;
using SkiaSharp;
using System.Text;

public class SteganographyHelper
{
    public static SKBitmap EncodeMessage(SKBitmap bmp, string message)
    {
        // 🛑 Check if image is null
        if (bmp == null)
        {
            throw new ArgumentNullException(nameof(bmp), "Error: The input image is null. Please check if the image is loaded correctly.");
        }

        // 🛑 Convert message to bytes
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        int messageLength = messageBytes.Length;

        // 🛑 Ensure the message fits inside the image
        if (messageLength == 0)
        {
            throw new ArgumentException("Error: The message cannot be empty.");
        }

        if (messageLength > bmp.Width * bmp.Height - 1)
        {
            throw new Exception("Error: Message too large for the image. Try a smaller message.");
        }

        SKBitmap newBitmap = bmp.Copy();
        int index = 0;

        // 🛑 Encode the message into the image pixels
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                if (index >= messageLength) break;

                SKColor pixel = newBitmap.GetPixel(x, y);
                byte r = (byte)((pixel.Red & 0xFE) | ((messageBytes[index] >> 7) & 1));
                byte g = (byte)((pixel.Green & 0xFE) | ((messageBytes[index] >> 6) & 1));
                byte b = (byte)((pixel.Blue & 0xFE) | ((messageBytes[index] >> 5) & 1));

                newBitmap.SetPixel(x, y, new SKColor(r, g, b));

                messageBytes[index] <<= 3;
                index++;
            }
        }

        // 🛑 Store message length in the last pixel
        SKColor lastPixel = newBitmap.GetPixel(bmp.Width - 1, bmp.Height - 1);
        newBitmap.SetPixel(bmp.Width - 1, bmp.Height - 1, new SKColor(lastPixel.Red, lastPixel.Green, (byte)messageLength));

        return newBitmap;
    }

    public static string DecodeMessage(SKBitmap bmp)
    {
        // 🛑 Check if image is null
        if (bmp == null)
        {
            throw new ArgumentNullException(nameof(bmp), "Error: The input image is null. Please check if the image is loaded correctly.");
        }

        // 🛑 Read the last pixel to get the message length
        SKColor lastPixel = bmp.GetPixel(bmp.Width - 1, bmp.Height - 1);
        int messageLength = lastPixel.Blue;

        // 🛑 Ensure message length is valid
        if (messageLength <= 0 || messageLength > bmp.Width * bmp.Height)
        {
            throw new Exception("Error: Invalid message length detected. The image may not contain hidden data.");
        }

        // 🛑 Initialize a byte array for the message
        byte[] messageBytes = new byte[messageLength];
        int index = 0;

        // 🛑 Extract message bits from each pixel
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                if (index >= messageLength) break;

                SKColor pixel = bmp.GetPixel(x, y);
                messageBytes[index] = (byte)((pixel.Red & 1) << 7 | (pixel.Green & 1) << 6 | (pixel.Blue & 1) << 5);

                index++;
            }
        }

        // 🛑 Convert bytes to text
        return Encoding.UTF8.GetString(messageBytes);
    }
}
