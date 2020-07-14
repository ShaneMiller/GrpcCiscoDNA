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

            Channel channel = new Channel("partners.dnaspaces.io:443", ChannelCredentials.Insecure);
            var client = new FirehoseClient(channel);
            
            var eventsStreamRequest = new EventsStreamRequest();
            
            Metadata metadata = new Metadata();
            metadata.Add(new Metadata.Entry("X-API-KEY", apiKey));
            var callOptions = new CallOptions(metadata);

            AsyncServerStreamingCall<EventRecord> records = client.GetEvents(eventsStreamRequest, callOptions);

            Console.WriteLine("Requesting records...");

            try
            {
                while (await records.ResponseStream.MoveNext())
                {
                    var current = records.ResponseStream.Current;
                    Console.Write("Record received.");
                    System.Diagnostics.Debugger.Break();
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
            }
        }
    }
}
