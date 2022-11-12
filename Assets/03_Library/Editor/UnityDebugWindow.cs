using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class UnityDebugWindow : EditorWindow
{
 
    string text = "";
    /// <summary>
    /// 第一関数unityのメニュー上に表示される関数
    /// 第二関数このボタンが使えるかの判定
    /// 第三関数メニューアイテムの並び順のの指定
    /// </summary>
    [MenuItem("UnityGameLib/Debug/DebugWindow",false,1)]
    private static void ShowWindow()
    {
        UnityDebugWindow window = (UnityDebugWindow)GetWindow(typeof(UnityDebugWindow));
        window.titleContent = new GUIContent("UnityDebugWindow");
    }

    private void OnGUI()
    {
        GUILayout.Label("文字列出力");
       text = EditorGUILayout.TextArea(text, GUILayout.Height(100));
      

        if (GUILayout.Button("コンソールに出力"))
        {
            Debug.Log(text);           
        }
        
   
    }
}
