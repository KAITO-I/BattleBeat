using UnityEngine;
using UnityEditor;

//==============================
// PlayerPrefsの初期化ツール
//==============================
public static class PlayerPrefsResetter
{
    [MenuItem("Tools/Reset PlayerPrefs")]
    public static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefsを初期化しました");
    }
}
