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
            if (args.Length > 4 || args.Length < 2)
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

                        string fileToExtract = args[2];

                        if (args[1] == "-a5i3")
                        {
                            Export(new BinaryFormat(fileToExtract), fileToExtract + ".png", ColorFormat.Indexed_A5I3);
                        }
                        else
                        {
                            Export(new BinaryFormat(fileToExtract), fileToExtract + ".png", ColorFormat.Indexed_A3I5);
                        }

                    break;

                    case "-exportDir":

                        string dirToExtract = args[2];

                        Node folder = NodeFactory.FromDirectory(dirToExtract);

                        foreach (Node child in folder.Children)
                        {
                            if (args[1] == "-a5i3")
                            {
                                Export(child.GetFormatAs<BinaryFormat>(), dirToExtract + Path.DirectorySeparatorChar + child.Name + ".png", ColorFormat.Indexed_A5I3);
                            }
                            else
                            {
                                Export(child.GetFormatAs<BinaryFormat>(), dirToExtract + Path.DirectorySeparatorChar + child.Name + ".png", ColorFormat.Indexed_A3I5);
                            }

                        }

                        break;

                    case "-import":

                        string originalFile = args[2];
                        string pngPath = args[3];

                        if (args[1] == "-a5i3")
                        {
                            Import(new BinaryFormat(originalFile), originalFile + ".png", originalFile + "_TEX", ColorFormat.Indexed_A5I3);
                        }
                        else
                        {
                            Import(new BinaryFormat(originalFile), originalFile + ".png", originalFile + "_TEX", ColorFormat.Indexed_A3I5);
                        }

                    break;

                    case "-importDir":

                        string dirToImport = args[2];

                        Node f = NodeFactory.FromDirectory(dirToImport, "*?.TEX");

                        foreach (Node child in f.Children)
                        {
                            if (args[1] == "-a5i3")
                            {
                                Import(child.GetFormatAs<BinaryFormat>(), dirToImport + Path.DirectorySeparatorChar + child.Name + ".png", dirToImport + Path.DirectorySeparatorChar + child.Name + "_new", ColorFormat.Indexed_A5I3);
                            }
                            else
                            {
                                Import(child.GetFormatAs<BinaryFormat>(), dirToImport + Path.DirectorySeparatorChar + child.Name + ".png", dirToImport + Path.DirectorySeparatorChar + child.Name + "_new", ColorFormat.Indexed_A3I5);
                            }

                        }

                        break;
                }
            }
        }

        private static void Import(BinaryFormat input, string pngPath, string output, ColorFormat format)
        {

            MmTex texture;
            if (format == ColorFormat.Indexed_A5I3)
            {
                texture = input.ConvertWith<Binary2MmTex, BinaryFormat, MmTex>();
            }
            else
            {
                texture = input.ConvertWith<Binary2MmTexA3, BinaryFormat, MmTex>();
            }

            input.Dispose();

            // Import the new PNG file
            Bitmap newImage = (Bitmap)Image.FromFile(pngPath);
            var quantization = new FixedPaletteQuantization(texture.Palette.GetPalette(0));
            Texim.Media.Image.ImageConverter importer = new Texim.Media.Image.ImageConverter
            {
                Format = format,
                PixelEncoding = PixelEncoding.Lineal,
                Quantization = quantization
            };
            (Palette _, PixelArray pixelInfo) = importer.Convert(newImage);
            texture.Pixels = pixelInfo;

            // Save the texture
            texture.ConvertWith<Binary2MmTex, MmTex, BinaryFormat>()
                  .Stream.WriteTo(output);
        }

        private static void Export(BinaryFormat input, string output, ColorFormat format) {
            MmTex mmtex;
            if (format == ColorFormat.Indexed_A5I3)
            {
                mmtex = input.ConvertWith<Binary2MmTex, BinaryFormat, MmTex>();
            }
            else
            {
                mmtex = input.ConvertWith<Binary2MmTexA3, BinaryFormat, MmTex>();
            }

            input.Dispose();

            // To export the image:
            mmtex.Pixels.CreateBitmap(mmtex.Palette, 0).Save(output);
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: MMImageTool.exe -export -format <fileToExtract>");
            Console.WriteLine("Usage: MMImageTool.exe -exportDir -format <dirToExtract>");
            Console.WriteLine("Usage: MMImageTool.exe -import -format <originalFile> <png>");
            Console.WriteLine("Usage: MMImageTool.exe -import -format <dirToImport> // Remember to name .TEX and .png equally");
            Console.WriteLine("Formats available: -a5i3 -a3i5");
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
