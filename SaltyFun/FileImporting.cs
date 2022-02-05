using System.IO;
using UnityEngine;

namespace SaltyFun
{
    public class FileImporting
    {
        public static Texture2D ImportImage(string filePath)
        {
            byte[] imageBytes = File.ReadAllBytes(filePath);
            Texture2D image = new Texture2D(64, 64);
            image.LoadImage(imageBytes);
            return image;
        }

        public static Mesh ImportObj(string filePath)
        {
            return ObjImporter.ImportFile(filePath);
        }
    }
}
