using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class SingletonGenerate : EditorWindow
{

    // @""とすることで、複数行を書ける
    // ただ「"」は「""」として書きます
    private const string CODE =
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnityGameLib;
using NUnityGameLib.NDesignPattern.NSingleton;

  public class Noname : Singleton<Noname>,IUnityGameLib
  {
      void Start() 
      {
            Debug.Log(""UnityGameStart"");
      }

      public override void UpdateLib()
      {

      }
  }
    ";

    // メニューのUnityGameLib選択するとGenerateメソッドが呼ばれる
    [MenuItem("UnityGameLib/Generate/DesignPattern/Singleton")]
    private static void Generate()
    {
        // 作成するアセットのパス
        string filePath = "Assets/Noname.cs";

        // もし名前(パス)が重複していた場合に、自動で語尾に「Sample1.cs」みたく数字をつけてくれる
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(filePath);

        // アセット(.cs)を作成する
        File.WriteAllText(assetPath, CODE);

        // 変更があったアセットをインポートする(UnityEditorの更新)
        AssetDatabase.Refresh();
    }
}
