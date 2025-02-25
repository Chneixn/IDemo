using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class FileDataService : IDataService
{
    ISerializer serializer;
    string dataPath;
    string fileExtension;

    public FileDataService(ISerializer serializer)
    {
        this.dataPath = Application.persistentDataPath;
        this.fileExtension = "json";
        this.serializer = serializer;
    }

    string GetPathToFile(string fileName)
    {
        return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
    }

    public void Save(ISaveData data, bool overwrite = true)
    {
        string fileLocation = GetPathToFile(data.FileName);

        // 检查是否存在同名文件，若不允许覆盖则抛出异常
        if (!overwrite && File.Exists(fileLocation))
        {
            throw new IOException($"The file '{data.FileName}.{fileExtension}' already exists and cannot be overwritten.");
        }

        File.WriteAllText(fileLocation, serializer.Serialize(data));
    }

    public ISaveData Load(string name)
    {
        string fileLocation = GetPathToFile(name);

        // 检查文件是否存在，若不存在则抛出异常
        if (!File.Exists(fileLocation))
        {
            throw new ArgumentException($"No persisted GameData with name '{name}'");
        }

        return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
    }

    public void Delete(string name)
    {
        string fileLocation = GetPathToFile(name);

        if (File.Exists(fileLocation))
        {
            File.Delete(fileLocation);
        }
    }

    public void DeleteAll()
    {
        foreach (string filePath in Directory.GetFiles(dataPath))
        {
            File.Delete(filePath);
        }
    }

    public IEnumerable<string> ListSaves()
    {
        // 遍历数据目录下的所有文件，返回所有扩展名为 fileExtension 的文件名
        foreach (string path in Directory.EnumerateFiles(dataPath))
        {
            if (Path.GetExtension(path) == fileExtension)
            {
                yield return Path.GetFileNameWithoutExtension(path);
            }
        }
    }
}