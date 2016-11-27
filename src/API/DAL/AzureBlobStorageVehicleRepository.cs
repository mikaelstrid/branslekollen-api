using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

// https://docs.microsoft.com/en-gb/azure/storage/vs-storage-aspnet5-getting-started-blobs

namespace API.DAL
{
    public class AzureBlobStorageVehicleRepository : IVehicleRepository
    {
        private const string CONTAINER_NAME = "vehicles";
        private const string BLOB_PREFIX = "vehicle-";
        private readonly CloudBlobContainer _container;

        public AzureBlobStorageVehicleRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("ConnectionStrings:AzureStorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(CONTAINER_NAME);
        }

        public async void Add(IVehicle vehicle)
        {
            var blockBlob = GetBlockBlob(vehicle.Id);
            await blockBlob.UploadTextAsync(JsonConvert.SerializeObject(vehicle));
        }

        public async Task<IEnumerable<IVehicle>> GetAll()
        {
            BlobContinuationToken token = null;
            var result = new List<IVehicle>();
            do
            {
                var resultSegment = await _container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;
                foreach (var item in resultSegment.Results)
                {
                    var blockBlob = (CloudBlockBlob)item;
                    var text = await blockBlob.DownloadTextAsync();
                    var vehicle = JsonConvert.DeserializeObject<Vehicle>(text);
                    result.Add(vehicle);
                }
            } while (token != null);

            return result;
        }

        public async Task<IVehicle> Find(string id)
        {
            try
            {
                var blockBlob = GetBlockBlob(id);
                var text = await blockBlob.DownloadTextAsync();
                var vehicle = JsonConvert.DeserializeObject<Vehicle>(text);
                return vehicle;
            }
            catch (Exception)
            {
                return null;
            }        }

        public async void Remove(string id)
        {
            var blockBlob = GetBlockBlob(id);
            await blockBlob.DeleteAsync();
        }

        public async void Update(IVehicle vehicle)
        {
            var blockBlob = GetBlockBlob(vehicle.Id);
            await blockBlob.UploadTextAsync(JsonConvert.SerializeObject(vehicle));
        }

        
        private CloudBlockBlob GetBlockBlob(string id)
        {
            var blobName = $"{BLOB_PREFIX}{id}";
            var blockBlob = _container.GetBlockBlobReference(blobName);
            return blockBlob;
        }
    }
}