using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage;

namespace ZhangZiShiRssRead
{
    public class FileStoreHelper
    {
        private const string RssFileName = "RssFile.xml";

        public async Task<bool> SaveRssFileAsync(string content)
        {
            bool isWriteSuccess = true;
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile rssFile = await storageFolder.CreateFileAsync(RssFileName, CreationCollisionOption.ReplaceExisting);

                var buffer = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);
                await FileIO.WriteBufferAsync(rssFile, buffer);
            }
            catch (Exception)
            {
                isWriteSuccess = false;
            }

            return isWriteSuccess;
        }

        public async Task<string> ReadRssFileAsync()
        {
            string content = string.Empty;

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile rssFile = await storageFolder.TryGetItemAsync(RssFileName) as StorageFile;
            if (rssFile != null)
            {
                content = await FileIO.ReadTextAsync(rssFile);
            }

            return content;
        }
    }
}
