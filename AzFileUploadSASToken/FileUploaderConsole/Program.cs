// See https://aka.ms/new-console-template for more information
using FileUploaderConsole;
using System.Net.Http.Json;

Console.WriteLine("Hello, World!");

string filePath = @"C:\Maran\Study\Documents\Nextjs.txt"; // Replace with your file path

string functionUrl = "http://localhost:7223/api/api/files/generate-sas?fileName=test.txt";

using var httpClient = new HttpClient();
try
{
    var response = await httpClient.PostAsync(functionUrl, null);

    response.EnsureSuccessStatusCode();
    
    var result = await response.Content.ReadFromJsonAsync<SasResponse>();

    if (result == null || string.IsNullOrEmpty(result.SasUrl))
    {
        Console.WriteLine("Failed to retrieve SAS URL from the function.");
        return;
    }
    FileUploader uploader = new FileUploader();

    await uploader.UploadFileAsync(result.SasUrl, filePath);

    Console.WriteLine($"File upload successfully. SAS URL expires at {result.Expiry}");
    
}
catch(Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}


