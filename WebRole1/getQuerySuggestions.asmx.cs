using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WebRole1
{
    /// <summary>
    /// Summary description for getQuerySuggestions
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class getQuerySuggestions : System.Web.Services.WebService
    {
        private static MyTrie storage;

        [WebMethod]
        public string DownloadFile()
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("first");
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            //string fileName = System.IO.Path.GetTempFileName();
            // Loop over items within the container and output the length and URI.
            string fileName = "";
            if (container.Exists())
            {
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference("pagecountfilter");
                        //fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "\\text.txt";
                        using (var fileStream = System.IO.File.OpenWrite(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + "\\text.txt"))
                        {
                            blockBlob.DownloadToStream(fileStream);
                        }
                        //Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                        //using (var fileStream = System.IO.File.OpenWrite("@C:\Users\"))
                    }
                }
            }
            return fileName;
        }

        [WebMethod]
        public string BuildTrie()
        {
            storage = new MyTrie();
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("first");

            // Retrieve reference to a blob named "myblob.txt"
            CloudBlockBlob blob = container.GetBlockBlobReference("pagecountfilter");
            using (var stream = blob.OpenRead())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    System.Diagnostics.Debug.WriteLine(ramCounter.NextValue());
                    while (!reader.EndOfStream && ramCounter.NextValue() > 30)
                    {
                        string result = "";
                        try
                        {
                            string line = reader.ReadLine();
                            string[] lineComponent = line.Split('|');
                            string word = lineComponent[0];
                            int pageCount = Int32.Parse(lineComponent[1]);
                            word = word.Trim();
                            result = word;
                            storage.Add(word, pageCount);
                        }
                        catch (Exception e)
                        {
                            return ("{0} Exception caught." + e + "at word " + result);
                        }
                    }
                    if (reader.EndOfStream)
                        return "end of file";
                }
            }
            return "hello";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SearchTrie(string prefix)
        {
            List<string> result = storage.GetWords(prefix);
            return new JavaScriptSerializer().Serialize(result.ToArray());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SearchSuggestions(string prefix)
        {
            List<string> result = storage.GetSuggestions(prefix);
            return new JavaScriptSerializer().Serialize(result.ToArray());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddWord(string prefix)
        {
            storage.Add(prefix, 0);
            return "success";
        }
    }
}

