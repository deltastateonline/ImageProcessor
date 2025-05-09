using Common;
using NetMQ;
using NetMQ.Sockets;
using ProtoBuf;
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
                    int i = 1;
                    while (true)
                    {
                    
                        if (!subscriber.TryReceiveFrameString(TimeSpan.FromMilliseconds(5000), out string receivedFrame))
                        {
                            Console.WriteLine("Timeout reached, no more messages. Exiting...");
                            break;
                        }

                        byte[] textBytes = Convert.FromBase64String(receivedFrame); 
                        var imageDetails = ProtobufHelpers.DeserializeObject<ImageDefProto>(textBytes);  

                        Console.WriteLine(JsonSerializer.Serialize(imageDetails, jsonSerializerOptions));
                        await ResizeImage(imageDetails);                   

                        i++;
                    }

                    Console.WriteLine($"{i} messages processed.");

                }
        }

        static async Task ResizeImage(ImageDefProto imageDef) 
        {

           await Task.Run(() => { 

                
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
            });

            return ;
        }        
    }
}
