using System;
using SkiaSharp;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine("C# Steganography - Hide and Extract Text in Images");
        Console.WriteLine("1. Encode Message into Image");
        Console.WriteLine("2. Decode Message from Image");
        Console.Write("Choose an option (1/2): ");

        int choice = int.Parse(Console.ReadLine());

        if (choice == 1)
        {
            Console.Write("Enter text message to hide: ");
            string message = Console.ReadLine();

            Console.Write("Enter input image path: ");
            string inputImagePath = Console.ReadLine();

            Console.Write("Enter output image path: ");
            string outputImagePath = Console.ReadLine();

            using var inputImage = SKBitmap.Decode(inputImagePath);
            var encodedImage = SteganographyHelper.EncodeMessage(inputImage, message);

            using var image = SKImage.FromBitmap(encodedImage);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(outputImagePath);
            data.SaveTo(stream);

            Console.WriteLine("Message hidden successfully in " + outputImagePath);
        }
        else if (choice == 2)
        {
            Console.Write("Enter encoded image path: ");
            string encodedImagePath = Console.ReadLine();

            using var encodedImage = SKBitmap.Decode(encodedImagePath);
            string extractedMessage = SteganographyHelper.DecodeMessage(encodedImage);

            Console.WriteLine("Extracted Message: " + extractedMessage);
        }
        else
        {
            Console.WriteLine("Invalid option! Please restart the program.");
        }
    }
}
