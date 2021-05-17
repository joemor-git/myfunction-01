#r "Microsoft.Azure.EventHubs"
#r "Microsoft.Azure.WebJobs"
#r "Microsoft.Azure.WebJobs.Extensions"
#r "Microsoft.Azure.WebJobs.Extensions.Storage"


using System;
using System.Text;
using Microsoft.Azure.EventHubs;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;

public static async Task Run(EventData[] events, String inputBlob, ILogger log)
{
    var exceptions = new List<Exception>();

    foreach (EventData eventData in events)
    {
        try
        {
            string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

            // Replace these two lines with your processing logic.
            log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
            log.LogInformation("Blob content: " + inputBlob);

            var username =  Environment.GetEnvironmentVariable("UsernameFromKeyVault", EnvironmentVariableTarget.Process);
            var password =  Environment.GetEnvironmentVariable("PasswordFromKeyVault", EnvironmentVariableTarget.Process);

            log.LogInformation($"Username: {username}");
            log.LogInformation($"Password: {password}");

            await Task.Yield();
        }
        catch (Exception e)
        {
            // We need to keep processing the rest of the batch - capture this exception and continue.
            // Also, consider capturing details of the message that failed processing so it can be processed again later.
            exceptions.Add(e);
        }
    }

    // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

    if (exceptions.Count > 1)
        throw new AggregateException(exceptions);

    if (exceptions.Count == 1)
        throw exceptions.Single();
}
