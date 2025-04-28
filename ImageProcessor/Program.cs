using Common;
using NetMQ;
using NetMQ.Sockets;
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

            var imageDefChannel = Channel.CreateUnbounded<ImageDef>();
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };


            using (var subscriber = new PullSocket())
            {
                subscriber.Connect("tcp://127.0.0.1:5556");  
                int i = 0;
                while (true)
                {                                

                    byte[] textBytes = Convert.FromBase64String(subscriber.ReceiveFrameString());
                    string decodedText = Encoding.UTF8.GetString(textBytes);    

                    Console.WriteLine("{0}:{1}", processorId, i);

                    var imageDetails = JsonSerializer.Deserialize<ImageDef>(decodedText);

                    Console.WriteLine(JsonSerializer.Serialize(imageDetails, jsonSerializerOptions));

                    //await imageDefChannel.Writer.WriteAsync(imageDetails);

                    //imageDefChannel.Writer.Complete();


                    i++;
                }

                
            }

            
            
            Random rnd = new Random();
            await foreach (ImageDef current in imageDefChannel.Reader.ReadAllAsync())
            {
                int x = rnd.Next(1, 10);
                Console.WriteLine(x);
                await Task.Delay(x * 500);
                Console.WriteLine(JsonSerializer.Serialize(current, jsonSerializerOptions));

            }


        }
    }
}
