namespace MMImageTool
{
    using System;
    using System.Drawing;
    using System.IO;
    using Texim.Media.Image;
    using Texim.Media.Image.Processing;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length > 3 || args.Length < 2)
            {
                Console.Write("Wrong arguments.");
                ShowUsage();
            }
            else
            {
                ShowCredits();
                switch (args[0])
                {
                    case "-export":

                        string fileToExtract = args[1];

                        Export(new BinaryFormat(fileToExtract), fileToExtract + ".png");

                    break;

                    case "-exportDir":

                        string dirToExtract = args[1];

                        Node folder = NodeFactory.FromDirectory(dirToExtract);

                        foreach (Node child in folder.Children)
                        {
                            Export(child.GetFormatAs<BinaryFormat>(), dirToExtract + Path.DirectorySeparatorChar + child.Name + ".png");
                        }

                        break;

                    case "-import":

                        string originalFile = args[1];
                        string pngPath = args[2];

                        Import(new BinaryFormat(originalFile), originalFile + ".png", originalFile + "_TEX");

                    break;

                    case "-importDir":

                        string dirToImport = args[1];

                        Node f = NodeFactory.FromDirectory(dirToImport, "*?.TEX");

                        foreach (Node child in f.Children)
                        {
                            Import(child.GetFormatAs<BinaryFormat>(), dirToImport + Path.DirectorySeparatorChar + child.Name + ".png", dirToImport + Path.DirectorySeparatorChar + child.Name + "_new");
                        }

                        break;
                }
            }
        }

        private static void Import(BinaryFormat input, string pngPath, string output)
        {
            MmTex texture = input.ConvertWith<Binary2MmTex, BinaryFormat, MmTex>();
            input.Dispose();

            // Import the new PNG file
            Bitmap newImage = (Bitmap)Image.FromFile(pngPath);
            var quantization = new FixedPaletteQuantization(texture.Palette.GetPalette(0));
            Texim.Media.Image.ImageConverter importer = new Texim.Media.Image.ImageConverter
            {
                Format = ColorFormat.Indexed_A5I3,
                PixelEncoding = PixelEncoding.Lineal,
                Quantization = quantization
            };
            (Palette _, PixelArray pixelInfo) = importer.Convert(newImage);
            texture.Pixels = pixelInfo;

            // Save the texture
            texture.ConvertWith<Binary2MmTex, MmTex, BinaryFormat>()
                  .Stream.WriteTo(output);
        }

        private static void Export(BinaryFormat input, string output) {
            MmTex mmtex = input.ConvertWith<Binary2MmTex, BinaryFormat, MmTex>();
            input.Dispose();

            // To export the image:
            mmtex.Pixels.CreateBitmap(mmtex.Palette, 0).Save(output);
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: MMImageTool.exe -export <fileToExtract>");
            Console.WriteLine("Usage: MMImageTool.exe -exportDir <dirToExtract>");
            Console.WriteLine("Usage: MMImageTool.exe -import <originalFile> <png>");
            Console.WriteLine("Usage: MMImageTool.exe -import <dirToImport> // Remember to name .TEX and .png equally");
        }

        private static void ShowCredits()
        {
            Console.WriteLine("=========================");
            Console.WriteLine("== MM Image Tool by Pleonex ==");
            Console.WriteLine("== Modifications by Nex ==");
            Console.WriteLine("=========================");
        }
    }
}
