using Common;
using NetMQ;
using NetMQ.Sockets;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Xml;

namespace ImageProcessor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            if (args.Length != 1) 
            {
                Console.WriteLine("Please provide a processor id.");
                return;
            }
            string processorId = args[0];
            Console.WriteLine("Connecting Processor {0} and wait on 127.0.0.1:5556", processorId);
           
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };         

                using (var subscriber = new PullSocket())
                {
                    subscriber.Connect("tcp://127.0.0.1:5556");  
                    int i = 0;
                    while (true)
                    {                                

                        byte[] textBytes = Convert.FromBase64String(subscriber.ReceiveFrameString());
                        string decodedText = Encoding.UTF8.GetString(textBytes);                          

                        var imageDetails = JsonSerializer.Deserialize<ImageDef>(decodedText);

                        Console.WriteLine(JsonSerializer.Serialize(imageDetails, jsonSerializerOptions));
                        ResizeImage(imageDetails);                   

                        i++;
                    }                
                }
        }

        static void ResizeImage(ImageDef imageDef)
        {

            string inputPath = "";
            string outputPath = imageDef.OutputFolder + "\\"+Path.GetFileName(imageDef.Filename);
            using (Image image = Image.Load(imageDef.Filename))
            {

                var resizer = decimal.ToInt16(imageDef.Resize) * 0.01;

                // Calculate new dimensions
                int newWidth = (int) (image.Width * resizer);
                int newHeight = (int)(image.Height * resizer);

                // Resize the image
                image.Mutate(x => x.Resize(newWidth, newHeight));

                // Save the output
                image.Save(outputPath);
            }


        }
    }
}
