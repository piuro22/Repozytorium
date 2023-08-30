using System.IO;
using UnityEngine;

public static class FileUtils
{
    public static string FormatFileSize(long size)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double formattedSize = size;

        while (formattedSize >= 1024 && order < sizes.Length - 1)
        {
            order++;
            formattedSize /= 1024;
        }

        return $"{formattedSize:F2} {sizes[order]}";
    }

    public static bool CheckFileExist(string filePath)
    {
        bool exists = File.Exists(filePath);
        return exists;
    }
}