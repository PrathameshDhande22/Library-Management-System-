using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace MVC_Task.Utils
{
    public class Utilities
    {
        /// <summary>
        /// Adds the Cookie into the MVC application.
        /// </summary>
        /// <param name="response">The HTTP Response to which the Cookie to be added </param>
        /// <param name="key">The key of the Cookie to add</param>
        /// <param name="value">The Value for the Cookie</param>
        public static void AddCookie(HttpResponseBase response, string key, string value)
        {
            response.AppendCookie(new HttpCookie(key, value));
        }

        /// <summary>
        /// Checks if the Uploaded File is of Valid Extension.
        /// </summary>
        /// <param name="file">The File to validate</param>
        /// <returns>True if the file is Valid otherwise false.</returns>
        public static bool IsFileValid(HttpPostedFileBase file)
        {
            List<string> extensionsupported = new List<string>() { ".jpg", ".jpeg", ".png" };
            string extension = Path.GetExtension(file.FileName);
            return extensionsupported.Contains(extension);
        }

        /// <summary>
        /// Saves the Files into the Specified path and hashes the Filename.
        /// </summary>
        /// <param name="savePath">The Path Where the file will be saved.</param>
        /// <param name="file">The File Which want to save.</param>
        /// <param name="server">The Server utility for mapping the Path.</param>
        /// <returns>URL of the Saved File otherwise null.</returns>
        public static string SaveFile(string savePath, HttpPostedFileBase file, HttpServerUtilityBase server)
        {
            try
            {
                string newguidfile = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(file.FileName);
                string newfilename = newguidfile + extension;
                string serverTempPath = Path.Combine(server.MapPath(savePath), newfilename);
                file.SaveAs(serverTempPath);
                return savePath + "/" + newfilename;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Moves the File from Temporary location to Permanent Location.
        /// </summary>
        /// <param name="destPath">the Destination path where the file will be moved.</param>
        /// <param name="fileurl">The URL of the File to be moved.</param>
        /// <param name="server">The Server Utility for mapping the paths.</param>
        /// <returns>Url of the Moved File otherwise returns null.</returns>
        public static string SaveFileToPermanent(string destPath, string fileurl, HttpServerUtilityBase server)
        {
            try
            {
                string srcFile = server.MapPath(fileurl);
                string filename = Path.GetFileName(srcFile);
                string destpathfile = Path.Combine(server.MapPath(destPath), filename);
                File.Move(srcFile, destpathfile);
                return destPath + "/" + filename;
            }
            catch (Exception)
            {
                return null;

            }
        }

        /// <summary>
        /// Deletes the File from the System. 
        /// </summary>
        /// <param name="fileurl">The Url of the File which want to delete.</param>
        /// <param name="server">The Server Utility for mapping the paths.</param>
        /// <returns>true if the file deletion is successful otherwise false.</returns>
        public static bool DeleteFile(string fileurl, HttpServerUtilityBase server)
        {
            try
            {
                string fullPath = server.MapPath(fileurl);
                File.Delete(fullPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// To get the List of SelectListItem of the Publication year from 1990 to Current Year. 
        /// </summary>
        /// <returns>List of SelectListItem of Publication year from 1990 to Current Year</returns>
        public static List<SelectListItem> GetPublicationYear()
        {
            List<SelectListItem> publicationyear = new List<SelectListItem>();

            for (int i = 1990; i <= DateTime.Now.Year; i++)
            {
                publicationyear.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            return publicationyear;
        }


    }
}