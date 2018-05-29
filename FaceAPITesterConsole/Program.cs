using Microsoft.ProjectOxford.Face;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FaceAPITesterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Azure Storage connection string
            string storageConnectionString = "";

            //Face API connection variables
            string SubscriptionKey = "";
            string SubscriptionRegion = "";

            //image from local Files for testing
            string fileName = "testimage.jpg";
            string file = @"C:\images\" + fileName;

            Console.WriteLine("Uploading Blob");
            string imageUrl = UploadToBlob(storageConnectionString, file, fileName);
            Console.WriteLine(imageUrl);
            DetectFace(imageUrl, SubscriptionKey, SubscriptionRegion);

            Console.ReadKey();
        }

        private static string UploadToBlob(string connectionString, string file, string fileName)
        {
            // Create a CloudStorageAccount instance pointing to your storage account.
            CloudStorageAccount storageAccount =
              CloudStorageAccount.Parse(connectionString);

            // Create the CloudBlobClient that is used to call the Blob Service for that storage account.
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container called 'quickstartblobs'. 
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");

            // Upload the file 
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            cloudBlockBlob.Properties.ContentType = "image/jpg";

            cloudBlockBlob.UploadFromFileAsync(file);

            return cloudBlockBlob.StorageUri.PrimaryUri.ToString();
        }

        private static async void DetectFace(string imageUrl, string subscriptionKey, string subscriptionRegion)
        {
            
            FaceServiceClient _faceServiceClient = new FaceServiceClient(subscriptionKey, subscriptionRegion);
            var faces = await _faceServiceClient.DetectAsync(imageUrl);
            Console.WriteLine(faces);
        }

        public static void DetectFaceWithREST(string imageUrl, string subscriptionKey, string subscriptionRegion)
        {
            string requestBody = $"{{\"url\":\"{imageUrl}\"}}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                HttpResponseMessage response;

                HttpContent hc = new StringContent(requestBody);
                hc.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = client.PostAsync($"https://{subscriptionRegion}.api.cognitive.microsoft.com/face/v1.0/detect", hc).Result;

                response.EnsureSuccessStatusCode();

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }

        }
    }
}
