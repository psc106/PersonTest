using System.IO;
using UnityEditor;
using UnityEngine;

public static class Setup
{
    [MenuItem("Tool/Setup/Create Default Folder")]
    public static void CreateDefaultFolders()
    {

        Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "Settings");
        AssetDatabase.Refresh();
    }

    [MenuItem("Tool/Setup/Import My Favorite Assets")]
    public static void ImportMyFavoriteAssets()
    {
        Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
    }
    static class Folders
    {
        public static void CreateDefault(string root, params string[] folders)
        {
            var fullPath = Path.Combine(Application.dataPath, root);
            foreach(var folder in folders)
            {
                var path = Path.Combine(fullPath, folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }

    public class Assets
    {
        public static void ImportAsset(string asset, string subFolder, string folder = "C:/Users/a/AppData/Roaming/Unity/Asset Store-5.x")
        {
            AssetDatabase.ImportPackage(Path.Combine(folder, subFolder, asset), false);
        }
    }

}
