using static System.Net.Mime.MediaTypeNames;
using System.Text;
using NetMQ.Sockets;
using NetMQ;
using System.Text.Json;

namespace File_listing
{
    public class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.WriteLine("Please provide a folder path.");
                return;
            }
            string path = args[0];

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"The folder path '{path}' does not exist.");
                return;
            }
            
            string[] jpgFiles = Directory.GetFiles(path, "*.jpg");

            using (var publisher = new PushSocket()) {

                publisher.Bind("tcp://127.0.0.1:5556");
                //publisher.Bind("tcp://*:5556");
                int messageId = 0;

                foreach (string file in jpgFiles)
                {
                    var msg = new
                    {
                        id = messageId,
                        message = file
                    };

                    byte[] textBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
                    string encodedText = Convert.ToBase64String(textBytes);
                    Console.WriteLine("{0} - {1}" , messageId,encodedText);

                    publisher                   
                    .SendFrame(encodedText); // Message

                    messageId++;

                    Thread.Sleep(500);

                }
            }
        }
    }
}
