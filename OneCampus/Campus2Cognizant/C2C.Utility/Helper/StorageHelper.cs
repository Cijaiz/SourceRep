using FileManagement;
using FileManagement.StorageSkeleton;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace C2C.Core.Helper
{
    public static class StorageHelper
    {
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
        #endregion

        /// <summary>
        /// Helper for Storage Manager.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="postedFile"></param>
        /// <param name="persistThumbNail"></param>
        /// <returns></returns>
        public static string UploadMediaFile(string folderPath, HttpPostedFileBase postedFile, bool persistThumbNail = false)
        {
            return StorageManager.UploadMediaFile(folderPath, postedFile, persistThumbNail);
        }

        /// <summary>
        /// Helper for Storage Manager.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="postedFile"></param>
        /// <param name="persistThumbNail"></param>
        /// <returns></returns>
        public static string UploadMediaFile(string folderPath, string postedFileName, byte[] postedStream)
        {
            return StorageManager.UploadMediaFile(folderPath, postedFileName, postedStream, false);
        }

        /// <summary>
        /// Gets the Media file from the given location.
        /// </summary>
        /// <param name="relativePath">The path of the file to be located and retrived.</param>
        /// <returns>Returned File Path.</returns>
        public static string GetMediaFilePath(string relativePath, string imageSizeToRetrieve = "")
        {
            return StorageManager.GetPublicMediaPath(relativePath, imageSizeToRetrieve);
        }

        /// <summary>
        /// Retrives all the mediafiles from the given location.
        /// </summary>
        /// <param name="folderName">Folder Name to retrive files.</param>
        /// <returns>List of public URLS.</returns>
        public static List<string> GetMediaFilesUnderContainer(string folderName)
        {
            List<string> mediaFileUrls = new List<string>();

            var mediaFiles = StorageManager.ListAllMediaFiles(folderName);
            foreach (var file in mediaFiles)
                mediaFileUrls.Add(GetMediaFilePath(file.GetPath()));

            return mediaFileUrls;
        }
    }
}
