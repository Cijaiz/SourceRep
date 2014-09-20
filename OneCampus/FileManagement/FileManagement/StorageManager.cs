using FileManagement.StorageSkeleton;
using FileManagement.StorageWorkers;
using FileManagement.Utility;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace FileManagement
{
    public class StorageManager
    {
        public static IFileStorageProvider storageProvider;

        public static string RecentErrors { get; private set; }

        public static bool LoadCurrentProvider()
        {
            bool isStorageProviderLoaded = false;

            try
            {
                //Load the appropriate factory based on the environment conditions.
                storageProvider = RoleEnvironment.IsAvailable ? AzureSystemStorage.Instance(null) : storageProvider = FileSystemStorage.Instance(null);
                isStorageProviderLoaded = true;

                //Clear Errors..
                RecentErrors = string.Empty;
            }
            catch (Exception ex)
            {
                RecentErrors = ex.Message + ex.StackTrace + (ex.InnerException == null ? "NA" : ex.InnerException.Message);
                throw new ApplicationException(RecentErrors); ;
            } //Error mode so Return false..
            return isStorageProviderLoaded;
        }

        #region Constants
        /// <summary>
        /// Image Size : 39X38
        /// </summary>
        public const string PROFILESUMMARY_SIZE = "39X38";

        /// <summary>
        /// IMage Size : 205X190
        /// </summary>
        public const string PROFILE_SIZE = "205X190";

        /// <summary>
        /// Image Size : 49X49
        /// </summary>
        public const string SHARE_SIZE = "49X49";

        /// <summary>
        /// Image Size : 32X32
        /// </summary>
        public const string COMMENT_SIZE = "32X32";

        /// <summary>
        /// Seperator while retriving the resized images from the cloud.
        /// </summary>
        public const string IMAGESIZE_SEPERATOR = "_";
        #endregion

        /// <summary>
        /// Gets the Media file from the given location.
        /// </summary>
        /// <param name="relativePath">The path of the file to be located and retrived.</param>
        /// <returns>IStorage file retrived from the given location.</returns>
        public static IStorageFile GetMediaFile(string relativePath)
        {
            Guard.IsNotBlank(relativePath, "Relative Path");

            if (storageProvider == null)
                LoadCurrentProvider();

            return storageProvider.GetFile(relativePath);
        }

        /// <summary>
        /// Returns the Public Path of the media file.
        /// </summary>
        /// <param name="relativePath">relative path of the media file location.</param>
        /// <param name="imageSizeToRetrive">Size of the IMage to retrive from the Cloud based on the constatns.</param>
        /// <returns>Public path of the media file.</returns>
        public static string GetPublicMediaPath(string relativePath, string imageSizeToRetrive)
        {
            Guard.IsNotBlank(relativePath, "Relative path");

            if (storageProvider == null)
                LoadCurrentProvider();

            string sizedImagePath = string.Empty;
            string originalFileName = Path.GetFileNameWithoutExtension(relativePath);
            string originalExtension = Path.GetExtension(relativePath);

            sizedImagePath = relativePath.Substring(0, relativePath.LastIndexOf("/") + 1)
                                                        + originalFileName
                                                        + IMAGESIZE_SEPERATOR
                                                        + imageSizeToRetrive
                                                        + originalExtension;

            return storageProvider.GetPublicURL(string.IsNullOrEmpty(imageSizeToRetrive) ? relativePath : sizedImagePath);
        }

        /// <summary>
        /// Returns the Public Path of the media file.
        /// </summary>
        /// <param name="relativePath">relative path of the media file location.</param>
        /// <returns>Public path of the media file.</returns>
        public static string GetPublicMediaPath(string relativePath)
        {
            Guard.IsNotBlank(relativePath, "Relative path");
            if (storageProvider == null)
                LoadCurrentProvider();

            return storageProvider.GetPublicURL(relativePath);
        }

        /// <summary>
        /// Lists all the files in the location.
        /// </summary>
        /// <param name="relativePath">Location to traverse to get the file names..</param>
        /// <returns>List of Istorage file which contains all the files from the given location.</returns>
        public static IEnumerable<IStorageFile> ListAllMediaFiles(string relativePath)
        {
            if (storageProvider == null)
                LoadCurrentProvider();

            return storageProvider.ListFiles(relativePath);
        }

        /// <summary>
        /// Lists all the folders in the location.
        /// </summary>
        /// <param name="relativePath">Location to traverse to get the folder names..</param>
        /// <returns>List of IStorageFolder which contains all the folders from the given location.</returns>
        public static IEnumerable<IStorageFolder> ListAllMediaFolders(string relativePath)
        {
            if (storageProvider == null)
                LoadCurrentProvider();

            return storageProvider.ListFolders(relativePath);
        }

        /// <summary>
        /// Uplloads the Media File into the Storage system based on the environment.
        /// </summary>
        /// <param name="folderPath">The folder path where to upload the file.</param>
        /// <param name="postedFile">The file to be uploaded.</param>
        /// <param name="persistThumbNails">If enabled all the Thumbnails will be automatically resized and uploaded..</param>
        /// <returns>Uploaded File path. {New file is automatically generated when the file name already exists.}</returns>
        public static string UploadMediaFile(string folderPath, HttpPostedFileBase postedFile, bool persistThumbNails = false)
        {
            Guard.IsNotNull(postedFile, "Uploaded File");
            Guard.IsNotBlank(folderPath, "Folder Path");

            folderPath = folderPath.EndsWith("/") ? folderPath : folderPath + "/";

            //Upload the Mail file and return the uploaded path..
            string uploadedMainFilePath = UploadMediaFile(folderPath, Path.GetFileName(postedFile.FileName), postedFile.InputStream, false);

            //Upload all the thumb nails if enabled.....
            if (persistThumbNails)
            {
                UploadMediaFile(folderPath, Path.GetFileName(uploadedMainFilePath), postedFile, StorageManager.COMMENT_SIZE, persistThumbNails);
                UploadMediaFile(folderPath, Path.GetFileName(uploadedMainFilePath), postedFile, StorageManager.PROFILE_SIZE, persistThumbNails);
                UploadMediaFile(folderPath, Path.GetFileName(uploadedMainFilePath), postedFile, StorageManager.PROFILESUMMARY_SIZE, persistThumbNails);
                UploadMediaFile(folderPath, Path.GetFileName(uploadedMainFilePath), postedFile, StorageManager.SHARE_SIZE, persistThumbNails);
            }
            return uploadedMainFilePath;
        }

        /// <summary>
        /// Uploads a media file based on an array of bytes.
        /// </summary>
        /// <param name="folderPath">The path to the folder where to upload the file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="bytes">The array of bytes with the file's contents.</param>
        /// <param name="extractZip">Boolean value indicating weather zip files should be extracted.</param>
        /// <returns>The path to the uploaded file.{New file is automatically generated when the file name already exists.}</returns>
        public static string UploadMediaFile(string folderPath, string fileName, byte[] bytes, bool extractZip)
        {
            Guard.IsNotBlank(folderPath, "folderPath");
            Guard.IsNotBlank(fileName, "fileName");
            Guard.IsNotNull(bytes, "bytes");

            return UploadMediaFile(folderPath, fileName, new MemoryStream(bytes), extractZip);
        }

        /// <summary>
        /// Uploads a media file based on memory stream.
        /// </summary>
        /// <param name="folderPath">The path to the folder where to upload the file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="inputStream">The input stream with file's contents.</param>
        /// <param name="extractZip">Boolean value indicating wheather zip files should be extracted.</param>
        /// <returns>The path of the uploaded file.</returns>
        public static string UploadMediaFile(string folderPath, string fileName, Stream inputStream, bool extractZip)
        {
            Guard.IsNotBlank(folderPath, "Folder Path");
            Guard.IsNotBlank(fileName, "File Name");
            Guard.IsNotNull(inputStream, "Input Stream");

            string uploadedFilePath = string.Empty;

            if (storageProvider == null)
                LoadCurrentProvider();

            storageProvider.TrySaveStream(folderPath + "/" + fileName, inputStream, ref uploadedFilePath);
            return uploadedFilePath;
        }

        /// <summary>
        /// Uploads the Image based on decisions for upload Thumbnails or not..
        /// </summary>
        /// <param name="folderPath">The folder path where to upload the file.</param>
        /// <param name="postedFile">The file to be uploaded.</param>
        /// <param name="newImageSize">Image size to be resized.. Select from StorageManger Constants..</param>
        /// <param name="saveThumbnail">Saves thumnais and default=false.</param>
        /// <returns>Uploaded File path. {New file is automatically generated when the file name already exists.}</returns>
        private static string UploadMediaFile(string folderPath, string fileName, HttpPostedFileBase postedFile, string newImageSize, bool saveThumbnail = false)
        {
            Guard.IsNotNull(postedFile, "Uploaded File");
            Guard.IsNotBlank(folderPath, "Folder Path");

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);
            string newFileName = fileNameWithoutExtension + "_" + newImageSize + fileExtension;
            MemoryStream resizedImage = ResizeImage(postedFile.InputStream, newImageSize, fileExtension);

            folderPath = folderPath.EndsWith("/") ? folderPath : folderPath + "/";
            return UploadMediaFile(folderPath, newFileName, resizedImage.ToArray(), false);
        }

        /// <summary>
        /// Resizes the Image based on the Sizes provided..
        /// </summary>
        /// <param name="uploadedImage">Uploaded source Image..</param>
        /// <param name="newImageSize">Image size to be resized.. Ex. {20X20}</param>
        /// <param name="fileExtension">Extension of the file for which new image should be resized.</param>
        /// <returns>Resized Image stream..</returns>
        private static MemoryStream ResizeImage(Stream uploadedImage, string newImageSize, string fileExtension)
        {
            MemoryStream resizedImage = null;
            Image sourceImage = null;

            //Load the source image.. from stream.
            sourceImage = Image.FromStream(uploadedImage);

            //Apply dimensions dynamically..
            string[] splittedDimensions = newImageSize.Split(new char[] { 'X' });
            int[] newdimensions = new int[splittedDimensions.Length];
            for (int i = 0; i < splittedDimensions.Length; i++)
            {
                int defaultDimension = 0;
                newdimensions[i] = int.TryParse(splittedDimensions[i], out defaultDimension) ? defaultDimension : defaultDimension;
            }

            //Start transformation process.
            int newHeight, newWidth = 0;
            if (newdimensions.Length.Equals(2))
            {
                newHeight = newdimensions[0]; //First itme in array is always height..
                newWidth = newdimensions[1];
            }
            else
                throw new ArgumentOutOfRangeException("Dimensions were invalid");

            //Determine height and width factors..
            if (newHeight == 0 || newWidth == 0)
                throw new ArgumentOutOfRangeException("New dimension height and width were invalid");

            float heightFactor, widthFactor = 0;
            heightFactor = sourceImage.Height / (float)newWidth;
            widthFactor = sourceImage.Width / (float)newHeight;

            if (widthFactor != heightFactor)
            {
                if (widthFactor > heightFactor)
                    newHeight = Convert.ToInt32(sourceImage.Height / widthFactor);
                else
                    newWidth = Convert.ToInt32(sourceImage.Width / heightFactor);
            }

            //Start Resizing process..
            Image targedImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(targedImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
            }

            //Save New resized image to stream..
            ImageFormat imageFormat;
            switch (fileExtension.ToUpper())
            {
                case ".BMP":
                    imageFormat = ImageFormat.Bmp;
                    break;
                case ".GIF":
                    imageFormat = ImageFormat.Gif;
                    break;
                case ".JPG":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                default:
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }

            using (resizedImage = new MemoryStream())
                targedImage.Save(resizedImage, imageFormat);

            return resizedImage;
        }
    }
}
