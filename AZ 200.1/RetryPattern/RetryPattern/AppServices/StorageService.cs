using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//install "WindowsAzure.Storage" from nuget
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace RetryPattern.AppServices
{
    public class StorageService
    {
        private CloudStorageAccount _storageAccount;

        public StorageService()
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=az200t1storage;AccountKey=57/YGAibE1ZQcq/PpihsfTA8Cv8eSb8+/CK6tnNmbAsPW0EtOBTcNFezbN2kY+g36917fz5NDPBnqOnPS+Inxw==;EndpointSuffix=core.windows.net";
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public string ReadWriteToStorage()
        {
            try
            {
                //Create a blob client to talk to
                CloudBlobClient blobClient = CreateBlobClient();

                //Look for a blob container where blobs lives.
                CloudBlobContainer newContainer = blobClient.GetContainerReference("newcontainer");
                 newContainer.CreateIfNotExists();
                //Get the reference to a blob called mynewblob
                CloudBlockBlob myNewBlob = newContainer.GetBlockBlobReference("mynewblob");
                if (!myNewBlob.Exists())
                {
                   myNewBlob.UploadText("This is the text in the blob!");
                }
                return  myNewBlob.DownloadText();
            }
            catch (Exception e)
            {
                return "No Connection!";
            }
        }

        private CloudBlobClient CreateBlobClient()
        {
            //Storage account client creation
            CloudBlobClient client = _storageAccount.CreateCloudBlobClient();

            //Custom Retry Process
            client.DefaultRequestOptions = new BlobRequestOptions
            {
                //Will try the request every 3 second for 4 times.
                RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(3), 4),
                LocationMode = LocationMode.PrimaryThenSecondary,
                MaximumExecutionTime = TimeSpan.FromSeconds(20)
            };

            return client;
        }
    }
}
