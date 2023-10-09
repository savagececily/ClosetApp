using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MyCloset.Services.Interfaces
{
	public interface IBlobStorageService
	{
        /// <summary>
        /// Upload image to Azure Blob Storage 
        /// </summary>
        /// <param name="userId"> current user guid </param>
        /// <param name="image"> base64 image string </param>
        /// <returns></returns>
        public Task<string> UploadImageAsync(Guid userId, string image);

        /// <summary>
        /// Get image from Azure Blob Storage
        /// </summary>
        /// <param name="blobUri"> blob Uri </param>
        /// <returns></returns>
        public Task<byte[]> GetImageAsync(string blobUri);
    }
}

