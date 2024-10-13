using NetMQ;
using NetMQ.Sockets;
using System.Text;

namespace File_Processor
{
    internal class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.WriteLine("Please provide a processor id.");
                return;
            }
            string processorId = args[0];
            Console.WriteLine("Connecting Processor {0} and wait on 127.0.0.1:5556", processorId);
            using (var subscriber = new SubscriberSocket())
            {
                subscriber.Connect("tcp://127.0.0.1:5556");               
                subscriber.Subscribe("A");

                while (true)
                {
                    var topic = subscriber.ReceiveFrameString();                  

                    byte[] textBytes = Convert.FromBase64String(subscriber.ReceiveFrameString());
                    string decodedText = Encoding.UTF8.GetString(textBytes);    

                    Console.WriteLine("{2}\t: From Publisher: {0} {1}", topic, decodedText, processorId);
                }
            }
        }
    }
}
