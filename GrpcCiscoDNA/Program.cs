using FirehoseApiV1;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FirehoseApiV1.Firehose;

namespace GrpcCiscoDNA
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter API Key:");
            var apiKey = Console.ReadLine();

            Console.WriteLine("Creating channel...");

            ChannelCredentials channelCredentials = new SslCredentials(System.IO.File.ReadAllText("CA.pem"));

            Channel channel = new Channel("partners.dnaspaces.io:443", channelCredentials);
            var client = new FirehoseClient(channel);
            
            var eventsStreamRequest = new EventsStreamRequest();
            
            Metadata metadata = new Metadata();
            metadata.Add(new Metadata.Entry("X-API-KEY", apiKey));
            var callOptions = new CallOptions(metadata);

            Console.WriteLine("Requesting records...");
            AsyncServerStreamingCall<EventRecord> records = client.GetEvents(eventsStreamRequest, callOptions);
            try
            {
                while (await records.ResponseStream.MoveNext())
                {
                    var current = records.ResponseStream.Current;
                    Console.Write(current.ToString());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception thrown!");
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Console.WriteLine("Shutting down channel.");
                channel.ShutdownAsync().Wait();
                Console.ReadLine();
            }
        }
    }
}
