using UnityEditor;
using UnityEngine;

public abstract class DBEditor : Editor
{
    protected string _dataName = "";
    protected string resourcesPath = "Assets/Resources/";
    protected string _dataPath = "Data/";

    protected abstract void CreateButton();

    protected abstract void DeleteButton();

    protected abstract void RefreshData(string path);
    
    protected abstract string GetAssetPath();
}