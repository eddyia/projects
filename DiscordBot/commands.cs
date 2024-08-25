using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Interactions;
using System.Collections.Generic;

namespace IB_Bot
{
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

public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
{
  [SlashCommand("randomquestion", "Finds random question(s) from a specified lesson and adds it to the current instance of a test")]
  public async Task RandomQuestion(string schoolclass, string lesson, string numquestions)
  {
    string directory;
    
    if (schoolclass == "biology")
    {
      directory = "QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/43-dp-biology/OrganizedQuestions/" + lesson;
    }
    else if (schoolclass == "math_hl")
    {
      directory = "";
    }
    else if (schoolclass == "chemistry")
    {
      directory = "QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/45-dp-chemistry/OrganizedQuestions/" + lesson;
    }
    else if (schoolclass == "mathsl")
    {
      directory = "QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/50-dp-mathematics/OrganizedQuestions/" + lesson;
    }
    else
    {
      await RespondAsync("Inputted class `" + schoolclass + "` was not found. Current supported classes are 'biology', 'chemistry', and 'mathsl'.");
      return;
    }

    string[] files = Directory.GetFiles(directory);
    int numquestion = 0;
    
    try{
       numquestion = Int32.Parse(numquestions);
    }
    catch (FormatException)
    {
      if (numquestions == "all")
      {
        numquestion = files.Length;
      }
    else
    {
      await RespondAsync("Invalid input for number of questions");
    }
  }

  List<string> file = new List<string>();
  file.AddRange(files);

  IBDatabaseReader IBDatabaseReader = new IBDatabaseReader();
  IBDatabaseReader.paperdirectory = @"QuestionBank/GeneratedPapers/GeneratedPaper.html";

  Random rand = new Random();

  for (int i = 0; i < numquestion; i++)
  {
    int random_number = rand.Next(0, file.Count);
    
    string htmlfile = File.ReadAllText(file[random_number]);
    string completed_file = IBDatabaseReader.TrimHTMLFile(htmlfile);
  
    if (File.Exists(IBDatabaseReader.paperdirectory) == false)
    {
      StreamWriter Initialwriter = new StreamWriter(IBDatabaseReader.paperdirectory);
  
      Initialwriter.Write(completed_file);
      Initialwriter.Close();
      
      file.RemoveAt(random_number);
    }
    else
    {
      FileInfo HTMLFileinfo = new FileInfo(IBDatabaseReader.paperdirectory);
      long start_position = HTMLFileinfo.Length;

      string NewHtmlFile = File.ReadAllText(IBDatabaseReader.paperdirectory) + trimmed_file;
      
      StreamWriter Additionalwriter = new StreamWriter(IBDatabaseReader.paperdirectory);
      Additionalwriter.Write(String.Empty);
      Additionalwriter.Write(NewHtmlFile);
      Additionalwriter.Close();
     
      file.RemoveAt(random_number);
    }
  }

  await RespondAsync("Succesfully added `" + numquestions + "` questions from lesson `" + lesson + "` of " + schoolclass + " to the current instance of the test.");
  return;
}

  [SlashCommand("addquestion", "Add a question to a current instance of an IB test.")]
  public async Task AddQuestion(string schoolclass, int questionid)
  {
    IBDatabaseReader IBDatabaseReader = new IBDatabaseReader();
    IBDatabaseReader.paperdirectory = @"QuestionBank/GeneratedPapers/GeneratedPaper.html";
    
    if (schoolclass == "biology")
    {
      IBDatabaseReader.questiondirectory = @"QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/43-dp-biology/questions/";
    }
    else if (schoolclass == "mathsl")
    {
      IBDatabaseReader.questiondirectory = @"QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/50-dp-mathematics/questions/";
    }
    else if (schoolclass == "chemistry")
    {
      IBDatabaseReader.questiondirectory = @"QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/45-dp-chemistry/questions/";
    }
    else
    {
      await RespondAsync("Inputted class `" + schoolclass + "` was not found. Current supported classes are 'biology', 'chemistry', and 'mathsl'.");
      return;
    }
    
    string FileDirectory = IBDatabaseReader.questiondirectory + questionid + ".html";
    string htmlfile = File.ReadAllText(FileDirectory);
    string completed_file = IBDatabaseReader.TrimHTMLFile(htmlfile);
                  
    if (File.Exists(IBDatabaseReader.paperdirectory) == false)
    {
      StreamWriter Initialwriter = new StreamWriter(IBDatabaseReader.paperdirectory);
      Initialwriter.Write(completed_file);
      Initialwriter.Flush();
      Initialwriter.Close();
      
      await RespondAsync("Question of id `" + IBDatabaseReader.SearchQuestionBank.FilterLink(FileDirectory) + "` has been added");
      return;                
    }
    else
    {
      FileInfo HTMLFileinfo = new FileInfo(IBDatabaseReader.paperdirectory);
      long start_position = HTMLFileinfo.Length;

      string NewHtmlFile = File.ReadAllText(IBDatabaseReader.paperdirectory) + trimmed_file;

      StreamWriter Additionalwriter = new StreamWriter(IBDatabaseReader.paperdirectory);
      Additionalwriter.Write(String.Empty);
      Additionalwriter.Write(NewHtmlFile);
      Additionalwriter.Flush();
      Additionalwriter.Close();
     
      await RespondAsync("Question of id `" + IBDatabaseReader.SearchQuestionBank.FilterLink(FileDirectory) + "` has been added");
      return;
    }
  }

  [SlashCommand("generatepaper", "Automatically generates an IB-style test based on the questions added.")]
  public async Task GeneratePaper()
  {
    await RespondAsync("Generating paper...");
    
    System.Diagnostics.ProcessStartInfo process = new System.Diagnostics.ProcessStartInfo();
    process.UseShellExecute = false;
    process.FileName = "node";
    process.Arguments = "start.js";
    process.RedirectStandardOutput = true;
    
    System.Diagnostics.Process cmd =  System.Diagnostics.Process.Start(process);
    string url = System.Environment.GetEnvironmentVariable("url");

    await FollowupAsync("Paper has been generated!");
    await FollowupAsync(url);
  }

  [SlashCommand("resetpaper", "Deletes all of the questions on the current generated test.")]
  public async Task ResetPaper()
  {
    File.Delete("QuestionBank/GeneratedPapers/GeneratedPaper.html");
    await RespondAsync("Paper has been succesfully reset");
  }
  
  [SlashCommand("searchquestionbank", "Search the V4 IB QuestionBank using a specific phrase or word.")]
  public async Task SearchQuestionBank(string schoolclass, [Remainder] string keyterm, string addquestions)
  {
    IBDatabaseReader IBDatabaseReader = new IBDatabaseReader();
    
    if (schoolclass == "biology")
    {
      IBDatabaseReader.questiondirectory = @"QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/43-dp-biology/questions/";
    }
    else if (schoolclass == "mathsl")
    {
      IBDatabaseReader.questiondirectory = "QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/50-dp-mathematics/questions/";
    }
    else if (schoolclass == "chemistry")
    {
      IBDatabaseReader.questiondirectory = @"QuestionBank/questionbank.ibo.org/en/teachers/00000/questionbanks/45-dp-chemistry/questions/";
    }
    else
    {
    await RespondAsync("Inputted class `" + schoolclass + "` was not found. Current supported classes are 'biology', 'chemistry', and 'mathsl'.");
    return;
    }

    await RespondAsync("Searching the IB " + schoolclass + " Databases for `" + keyterm + "`... (this might take awhile)");

    IBDatabaseReader.SearchQuestionBank.ReadFiles(keyterm);
    int placeholder = IBDatabaseReader.SearchQuestionBank.num_matches;

    if (IBDatabaseReader.SearchQuestionBank.num_matches > 30)
    {
      await FollowupAsync("`" + placeholder + "` results have been found. " + IBDatabaseReader.SearchQuestionBank.CalculateImportance(placeholder) + " To avoid getting rate limited, I can only send results with a maximum of 30 questions at a time.");
      return;
    }
    else if (IBDatabaseReader.SearchQuestionBank.num_matches == 0)
    {
      await FollowupAsync("No results were found. Either there were no questions associated with your inputed phrase, or the question was not included in the V4 Questionbank.");
      return;
    }
    else
    {
      await FollowupAsync("The IB has asked `" + placeholder + "` questions on the inputted phrase. " + IBDatabaseReader.SearchQuestionBank.CalculateImportance(placeholder));
    }

    string file_content;
    string trimmed_file;

    foreach (string current_file in Directory.GetFiles(IBDatabaseReader.questiondirectory))
    {
      file_content = File.ReadAllText(current_file);
      trimmed_file = IBDatabaseReader.TrimHTMLFile(file_content);
      trimmed_file = IBDatabaseReader.AddFeatures(trimmed_file);

      if (trimmed_file.Contains(keyterm))
      {
        string id = IBDatabaseReader.SearchQuestionBank.FilterLink(current_file);
        string url = IBDatabaseReader.SearchQuestionBank.CreateLink(schoolclass, current_file);
      
        if (Convert.ToBoolean(addquestions) == true)
        {
          AddQuestion(schoolclass, Int32.Parse(id));
          await FollowupAsync("Adding question with id: `" + id + "` to current test");
        }                   
      }
    }
  }
}
