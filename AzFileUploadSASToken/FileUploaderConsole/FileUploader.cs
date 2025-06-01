using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderConsole;

public class FileUploader
{
    public async Task UploadFileAsync(string sasUrl, string filePath)
    {
        if (string.IsNullOrEmpty(sasUrl) || string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("SAS URL and file path must be provided.");
        }
        try
        {
            using var httpClient = new HttpClient();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var content = new StreamContent(fileStream);
            // Set the content type for the file being uploaded
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            // Upload the file to the SAS URL
            var response = await httpClient.PutAsync(sasUrl, content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("File uploaded successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to upload file. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while uploading the file: {ex.Message}");
        }
    }
}

public class SasResponse
{
    public string SasUrl { get; set; }
    public DateTimeOffset Expiry { get; set; }
}
