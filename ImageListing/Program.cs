using Common;
using NetMQ;
using NetMQ.Sockets;
using ProtoBuf;
using System.Text;
using System.Text.Json;

namespace ImageListing
{
    public class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 3)
            {
                Console.WriteLine("Required parameters not set. input-folder output-folder resize-percentage");
                return;
            }

            string path;
            string outputPath ;
            decimal resize;
            try
            {
                 path = args[0];
                 outputPath = args[1];
                 resize = decimal.Parse(args[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Arguments not correctly passed");
                return ;
            }

            

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"The folder path '{path}' does not exist.");
                return;
            }

            if (!Directory.Exists(outputPath))
            {
                Console.WriteLine($"The folder path '{outputPath}' does not exist.");
                return;
            }

            if (string.IsNullOrEmpty(args[2]))
            {
                Console.WriteLine($"The resize parameter is required.");
                return;
            }

            string[] jpgFiles = Directory.GetFiles(path, "*.jpg");

            using (var publisher = new PushSocket()) {

                publisher.Bind("tcp://127.0.0.1:5556");

                int messageId = 0;

                foreach (string file in jpgFiles)
                {
                    var msg = new ImageDefProto
                    {
                        Id = messageId.ToString(),
                        Filename = file,
                        InputFolder = path,
                        OutputFolder = outputPath,
                        Resize = resize
                    };
                    
                    string encodedText = Convert.ToBase64String(ProotoSerialize(msg));
                    Console.WriteLine("{0} - {1}" , messageId,encodedText);

                    publisher.SendFrame(encodedText); // Message                   

                    messageId++;

                    Thread.Sleep(500);

                }
            }
        }

        private static byte[] ProotoSerialize<T>(T record) where T : class
        {

            using var stream = new MemoryStream();
            Serializer.Serialize(stream, record);
            return stream.ToArray();
        }
    }
}
