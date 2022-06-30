using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileUploader.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTrolleyTours.Models
{
	public class TourScheduleChanges : IStorage
	{
        private readonly AzureStorageConfig storageConfig;

        public TourScheduleChanges(AzureStorageConfig storageConfig)
        {
            this.storageConfig = storageConfig;
        }

        public Task Initialize()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.FileContainerName);
            return containerClient.CreateIfNotExistsAsync();
        }

        public Task Save(Stream fileStream, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetNames()
        {
            List<string> names = new List<string>();

            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);

            // Get the container the blobs are saved in
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.FileContainerName);

            // This gets the info about the blobs in the container
            AsyncPageable<BlobItem> blobs = containerClient.GetBlobsAsync();

            await foreach (var blob in blobs)
            {
                names.Add(blob.Name);
            }
            return names;
        }

        public Task<Stream> Load(string name)
        {
            throw new NotImplementedException();
        }
    }
}

