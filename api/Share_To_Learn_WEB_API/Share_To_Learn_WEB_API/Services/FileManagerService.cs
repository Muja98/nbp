using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Share_To_Learn_WEB_API.DTOs;

namespace Share_To_Learn_WEB_API.Services
{
    public class FileManagerService
    {
        public static string SaveImageToFile(string base64Image, string imagefileId)
        {
            //string imageCounter = File.ReadAllText(@"..\..\UserImages\tempImgCounter.txt");
            byte[] bytes = Convert.FromBase64String(base64Image);
            string imagePath = @"..\..\UserImages\userImg" + imagefileId + ".png";
            File.WriteAllBytes(imagePath, bytes);
            //File.WriteAllText(@"..\..\UserImages\tempImgCounter.txt", (int.Parse(imageCounter) + 1).ToString());
            return imagePath;
        }

        public static string LoadImageFromFile(string path)
        {
            string base64Image = "";
            if(!string.IsNullOrEmpty(path))
            { 
                byte[] bytes = File.ReadAllBytes(path);
                base64Image = Convert.ToBase64String(bytes);
            }
            return base64Image;
        }

        public static string SaveDocumentToFile(string base64Document, string documentFileId)
        {
            //string documentCounter = File.ReadAllText(@"..\..\Documents\tempDocCounter.txt");
            byte[] bytes = Convert.FromBase64String(base64Document);
            string documentpath = @"..\..\Documents\" + documentFileId + ".pdf";
            File.WriteAllBytes(documentpath, bytes);
            //File.WriteAllText(@"..\..\Documents\tempDocCounter.txt", (int.Parse(documentCounter) + 1).ToString());
            return documentpath;
        }

        public static PDFContent LoadDocumentFromFile(string path)
        {
            string base64Document = "";
            if (!string.IsNullOrEmpty(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                base64Document = Convert.ToBase64String(bytes);
            }

            PDFContent document = new PDFContent();
            document.Base64Content = base64Document;

            return document;
        }

        public static int GetFileCount(bool isImage)
        {
            if(isImage)
                return  (from file in Directory.EnumerateFiles(@"..\..\UserImages", "*.png", SearchOption.AllDirectories)
                          select file).Count();
            else
                return  (from file in Directory.EnumerateFiles(@"..\..\Documents", "*.pdf", SearchOption.AllDirectories)
                             select file).Count();
        }
    }
}
