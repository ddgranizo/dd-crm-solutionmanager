using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SolutionManagerUI.Models;
using SolutionManagerUI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Services
{
    public class BlobStorageService
    {
        public const string IsEnabledIntegrationWithBlobStorageSettingKey = "IS_ENABLED_BLOB_STORAGE";
        public const string KeyIntegrationWithBlobStorageSettingKey = "BLOB_STORAGE_KEY";
        public const string BlobStorageContainerSettingKey = "BLOB_STORAGE_CONTAINER";
        public const string DefaultBlobStorageDownloadPathSettingKey = "BLOB_STORAGE_DOWNLOAD_PATH";
        public List<Setting> Settings { get; set; }
        public BlobStorageService(List<Setting> settings)
        {
            this.Settings = settings;
            //almbackupsolutions
        }


        public string GetDefaultPathForDownload()
        {
            return SettingsManager
                .GetSetting<string>
                    (this.Settings, DefaultBlobStorageDownloadPathSettingKey, null);
        }

        public List<string> GetBlobsInContainer()
        {
            List<string> blobs = new List<string>();
            if (IsEnabledBlobStorage())
            {
                var storageAccount = GetStorageAccount();
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                var container = GetContainerKey();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
                var blobList = blobContainer.ListBlobs();

                foreach (var blob in blobList)
                {
                    if (blob is CloudBlockBlob)
                    {
                        blobs.Add(((CloudBlockBlob)blob).Name);
                    }
                }
            }
            return blobs.OrderBy(k=>k).ToList();
        }

        public void Download(string solutionName, string path)
        {
            if (IsEnabledBlobStorage())
            {
                var storageAccount = GetStorageAccount();
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                var container = GetContainerKey();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(solutionName);
                blockBlob.DownloadToFile(path, System.IO.FileMode.CreateNew);
            }
            else
            {
                throw new Exception("Cannot connect with blobStorage. Set up the settings before");
            }

        }
        public void Upload(string solutionName, string path)
        {
            if (IsEnabledBlobStorage())
            {
                var storageAccount = GetStorageAccount();
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                var container = GetContainerKey();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
                var blobFileName = $"{solutionName}.zip";
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobFileName);
                using (var fileStream = System.IO.File.OpenRead(path))
                {
                    blockBlob.UploadFromStream(fileStream);
                }
            }

        }

        public void Delete(string solutionName)
        {
            if (IsEnabledBlobStorage())
            {
                var storageAccount = GetStorageAccount();
                var container = GetContainerKey();
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(solutionName);

                var extension = Path.GetExtension(solutionName);
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(solutionName);


                string newName =
                    string.Format(
                        "rollback/{0}__rolledback__{1}{2}",
                        nameWithoutExtension,
                        DateTime.Now.ToString()
                            .Replace('/', '_')
                            .Replace('\\', '_')
                            .Replace(':', '_')
                            .Replace(' ', '_')
                            .Replace('-', '_'),
                        extension);

                CloudBlockBlob blobCopy = blobContainer.GetBlockBlobReference(newName);
                if (blockBlob.Exists())
                {
                    blobCopy.StartCopy(blockBlob);
                    blockBlob.DeleteIfExists();
                }
            }
            else
            {
                throw new Exception("Cannot connect with blobStorage. Set up the settings before");
            }

        }

        private CloudStorageAccount GetStorageAccount()
        {
            var key = GetBlobKey();
            return CloudStorageAccount.Parse(key);
        }




        public bool IsEnabledBlobStorage()
        {
            bool settingIsEnabledValue = GetIsEnabledKey();
            string settingKeyValue = GetBlobKey();
            string settingContainerValue = GetContainerKey();

            return settingIsEnabledValue
                    && !string.IsNullOrEmpty(settingKeyValue)
                    && !string.IsNullOrEmpty(settingContainerValue);
        }

        private string GetContainerKey()
        {
            return SettingsManager
                   .GetSetting<string>
                       (this.Settings, BlobStorageContainerSettingKey, null);
        }

        private bool GetIsEnabledKey()
        {
            return SettingsManager
                            .GetSetting<bool>
                                (this.Settings, IsEnabledIntegrationWithBlobStorageSettingKey, false);
        }

        private string GetBlobKey()
        {
            return SettingsManager
                            .GetSetting<string>
                                (this.Settings, KeyIntegrationWithBlobStorageSettingKey, null);
        }
    }
}
