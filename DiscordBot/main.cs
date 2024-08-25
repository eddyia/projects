using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.ComponentModel;
using Discord.Interactions;

class IBDatabaseReader
{
    public static string questiondirectory;
    public static string paperdirectory;

    public static string TrimHTMLFile(string file_content)
    {
        string trimmed_file;

        int start_position = file_content.IndexOf("pull-right screen_only") - 12;
        int end_position = file_content.IndexOf("<div class='row'>", file_content.IndexOf("<img style")) - 2;
        file_content = file_content.Remove(start_position, end_position - start_position);

        start_position = 0;
        end_position = file_content.IndexOf("<h2>Syllabus sections</h2>");

        if (end_position == -1)
        {
            return file_content;
        }

        trimmed_file = file_content.Substring(start_position, end_position - start_position);

        return trimmed_file;
    }

    public static class SearchQuestionBank
    {
        public static int num_matches = 0;

        public static void ReadFiles(string key_term)
        {
          num_matches = 0;
          foreach (string current_file in Directory.GetFiles(questiondirectory))
          {
              string file_content = File.ReadAllText(current_file);
              string trimmed_file = IBDatabaseReader.TrimHTMLFile(file_content);
    
              if (trimmed_file.Contains(key_term))
              {
                  num_matches++;
              }
          }
          return;
        }

      public static string FilterLink(string file_name)
      {
        int first_position = file_name.IndexOf("questions") + 10;
        int final_position = file_name.IndexOf("html") - 1;
        string questionid = file_name.Substring(first_position, final_position - first_position);
        
        return questionid;
      }

      public static string CalculateImportance(int num_matches)
      {
        string response;
        
        if (num_matches <= 5)
        {
          response = "This concept is not important.";
          return response;
        }
        else if (num_matches > 5 && num_matches <= 10)
        {
          response = "This concept is semi-important.";
          return response;
        }
        else if (num_matches > 10 && num_matches <= 20)
        {
          response = "This concept is very important.";
          return response;
        }
        else if (num_matches > 20)
        {
          response = "This concept is extremely important.";
          return response;
        }
        return String.Empty;
      }

      public static string CreateLink(string school_class, string filename)
      {
        string URI = string.Empty;
        int first_position = filename.IndexOf("questions") + 10;
        int final_position = filename.IndexOf("html") + 4;
        string html_file = filename.Substring(first_position, final_position - first_position);

        if (school_class == "biology")
        {
            URI = "http://140.143.240.119/QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/43-dp-biology/questions/";
        }
        else
        {
            URI = "http://140.143.240.119/QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/45-dp-chemistry/questions/";
        }

        Console.WriteLine(URI + html_file);
        return URI + html_file;

      }
    }
}

#region Discord.Net
class Program
{
     static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
     private DiscordSocketClient _client;
     private CommandService _commands;
     private IServiceProvider _services;
     private InteractionService _interactionservices;
        
     BackgroundWorker bw = new BackgroundWorker();
     BackgroundWorker reactionworker;
     BackgroundWorker emailworker;

     public async Task RunBotAsync()
     {
        _client = new DiscordSocketClient();
        _commands = new CommandService();
        _services = new ServiceCollection()
        .AddSingleton(_client)
        .AddSingleton(_commands)
        .BuildServiceProvider();

        string token = System.Environment.GetEnvironmentVariable("token");      
        _client.Log += _client_Log;
        _client.Ready += ClientReady;
        
        await _client.SetGameAsync("Studying for the IB Exams");
        
        System.Diagnostics.ProcessStartInfo process = new System.Diagnostics.ProcessStartInfo();
        process.UseShellExecute = false;
        process.FileName = "node";
        process.Arguments = "start.js";
        process.RedirectStandardOutput = true;

        System.Diagnostics.Process cmd =  System.Diagnostics.Process.Start(process);

        await RegisterCommandsAsync();
        await _client.LoginAsync(Discord.TokenType.Bot, token);
        await _client.StartAsync();
        
        await Task.Delay(-1);
     }

     private async Task ClientReady()
     {
        _interactionservices = new InteractionService(_client);
        await _interactionservices.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _interactionservices.RegisterCommandsGloballyAsync();
        
        _client.InteractionCreated += async interaction =>
         {
             var scope = _services.CreateScope();
             var ctx = new SocketInteractionContext(_client, interaction);
             await _interactionservices.ExecuteCommandAsync(ctx, scope.ServiceProvider);
         };
     }

     private Task _client_Log(LogMessage arg)
     {
        Console.WriteLine(arg);
        return Task.CompletedTask;
     }

     public async Task RegisterCommandsAsync()
     {
        _client.MessageReceived += HandleCommandAsync;
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
     }

     private async Task HandleCommandAsync(SocketMessage arg)
     {
        var message = arg as SocketUserMessage;
        
        SocketCommandContext Context = new SocketCommandContext(_client, message);
        
        if (message.Author.IsBot) return;
        
        int argPos = 0;
        if (message.HasStringPrefix(">", ref argPos))
        {
             var result = await _commands.ExecuteAsync(Context, argPos, _services);
             if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
        }
     }
}
#endregion
