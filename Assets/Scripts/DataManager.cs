using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class DataManager : MonoBehaviour
{
    public List<string> URLs = new List<string>();
    public GameObject loading;
    public RawImage[] rawImage;

    string pathToSaveImage;
    string fetchFromCache;
    List<string> paths = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        loading.SetActive(true);
        if (URLs != null)
        {
            pathToSaveImage = Application.persistentDataPath;
            downloadImage(URLs, pathToSaveImage);
        }
    }

    public void downloadImage(List<string> urls, string pathToSaveImage)
    {
        foreach (var item in urls)
        {
            WWW www = new WWW(item);
            fetchFromCache = Path.Combine(pathToSaveImage, Path.GetFileName(item));
            StartCoroutine(_downloadImage(www, fetchFromCache));

        }
        loading.SetActive(false);
    }

    private IEnumerator _downloadImage(WWW www, string savePath)
    {
        yield return www;

        //Check if we failed to send
        if (string.IsNullOrEmpty(www.error))
        {
            UnityEngine.Debug.Log("Success");

            //Save Image
            saveImage(savePath, www.bytes);
            paths.Add(savePath);
        }
        else
        {
            UnityEngine.Debug.Log("Error: " + www.error);
        }
    }

    void saveImage(string path, byte[] imageBytes)
    {

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, imageBytes);
            Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }

        catch (System.Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    public void LoadTexture()
    {
        if (paths != null)
            LoadImage(paths);

    }

    public byte[] LoadImage(List<string> path)
    {
        byte[] dataByte = null;

        //Exit if Directory or File does not exist
        for (int i = 0; i < path.Count; i++)
        {


            if (!Directory.Exists(Path.GetDirectoryName(path[i])))
            {
                Debug.LogWarning("Directory does not exist");
                return null;
            }

            if (!File.Exists(path[i]))
            {
                Debug.Log("File does not exist");
                return null;
            }

            try
            {
                dataByte = File.ReadAllBytes(path[i]);
                Texture2D sampleTexture = new Texture2D(2, 2);
                bool isLoaded = sampleTexture.LoadImage(dataByte);
                // apply this texure as per requirement on image or material
               
                if (isLoaded)
                {
                    rawImage[i].texture = sampleTexture;
                }
                Debug.Log("Loaded Data from: " + path[i].Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To Load Data from: " + path[i].Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }
        }
        return dataByte;

    }

}
