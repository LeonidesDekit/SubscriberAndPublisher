using System.Net.Http.Json;
using Subscriber.Dtos;

Console.WriteLine("Press ESC to stop");
do
{
    HttpClient client = new HttpClient();
    Console.WriteLine("Listening....");
   

    while(!Console.KeyAvailable)
    {
         List<int> ackids = await GetMessageAsync(client);
         Thread.Sleep(2000);
         
         if(ackids.Count > 0)
         {
            await AskMessageAsync(client,ackids);
         }
    }

}while(Console.ReadKey(true).Key != ConsoleKey.Escape);

static async Task<List<int>> GetMessageAsync(HttpClient httpClient)
{
    List<int> ackids = new List<int>();
    List<MessageReadDto>? messages = new List<MessageReadDto>();

    try
    {
        messages = await httpClient.GetFromJsonAsync<List<MessageReadDto>>("http://localhost:5147/api/subscriptions/1/messages");
    }
    catch
    {
        return ackids;
    }

    foreach(MessageReadDto msg in messages!)
    {
        Console.WriteLine($"{msg.Id}-{msg.TopicMessage}-{msg.MessageStatus}");
        ackids.Add(msg.Id);
    }

    return ackids;
}

static async Task AskMessageAsync(HttpClient httpClient, List<int> ackids)
{
    var response = await httpClient.PostAsJsonAsync("http://localhost:5147/api/subscriptions/1/messages",ackids);
    var returnMessage = await response.Content.ReadAsStringAsync();

    Console.WriteLine(returnMessage);
}