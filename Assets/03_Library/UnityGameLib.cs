using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AddressableAssets;
using NUnityGameLib.NDesignPattern.NSingleton;

//名前空間
//この中のclassや関数を使うときはusingをつける
namespace NUnityGameLib
{
    interface IUnityGameLib
    {
        void UpdateLib();
        T[] ArrayAdd<T>(int arrayLength);
        int ShortIf(bool b, int a);
        float ShortIf(bool b, float a);
        double ShortIf(bool b, double a);
        char ShortIf(bool b, char a);
        string ShortIf(bool b, string a);
        T[] ArrayAssignment<T>(T[] arrayReturn, T[] arrayAssigined, int num);
        
    }

    /// <summary>
    /// UnityGameLib
    /// Unityのライブラリ 便利なクラスや関数などが入っています
    /// </summary>    
    public abstract class UnityGameLib : MonoBehaviour, IUnityGameLib
    {
              
        void Update()
        {
            UpdateLib();
        }

        /// <summary>
        /// Update関数をUnityGameLibで管理する関数
        /// 使い方はUpdate関数と同じです
        /// </summary>
        public virtual void UpdateLib(){}

        /// <summary>
        /// 配列を追加する関数
        /// 引数(int 配列の長さ)
        /// </summary>
        public virtual T[] ArrayAdd<T>(int arrayLength)
        {
            T[] array = new T[arrayLength];
            return array;
        }

        /// <summary>
        /// if文の省略系
        /// 引数(bool 条件を入力,int 整数) 
        /// </summary>  
        public virtual int ShortIf(bool b, int a)
        {
            if (b) { return a; }
            return 0;
        }

        /// <summary>
        /// if文の省略系
        /// 引数(bool 条件を入力,float 範囲が小さい小数) 
        /// </summary> 
        public virtual float ShortIf(bool b, float a)
        {
            if (b) { return a; }
            return 0f;
        }

        /// <summary>
        /// if文の省略系
        /// 引数(bool 条件を入力,double 範囲が大きい小数) 
        /// </summary> 
        public virtual double ShortIf(bool b, double a)
        {
            if (b) { return a; }
            return 0d;
        }

        /// <summary>
        /// if文の省略系
        /// 引数(bool 条件を入力,char 文字) 
        /// </summary> 
        public virtual char ShortIf(bool b, char a)
        {
            if (b) { return a; }
            return '0';
        }

        /// <summary>
        /// if文の省略系
        /// 引数(bool 条件を入力,string 文字列) 
        /// </summary> 
        public virtual string ShortIf(bool b, string a)
        {
            if (b) { return a; }
            return "";
        }

        /// <summary>
        /// 型が同じである二つの配列を一方に代入する関数
        /// 引数(T[] 代入されて返される配列 ,T[] 代入する配列,int 代入する数) 
        /// </summary> 
        public virtual T[] ArrayAssignment<T>(T[] arrayReturn,T[] arrayAssigined,int num)
        {
            for(int i = 0; i < arrayReturn.Length;++i)
            {
                arrayReturn[i] = arrayAssigined[i];
            }
            return arrayReturn;
        }

        /// <summary>
        /// ある配列から条件にあった値を返す関数
        /// 引数(T[] 配列,bool 条件)
        /// </summary>
        public virtual T[] WhereLineQ<T>(T[] array,bool b)
        {
            IEnumerable<T> query = array.Where(s => b);
            return query.ToArray();
        }

        /// <summary>
        /// あるリストから条件にあった値を返す関数
        /// 引数(ListT リスト,bool 条件)
        /// </summary>
        public virtual List<T> WhereLineQ<T>(List<T> array, bool b)
        {
            IEnumerable<T> query = array.Where(s => b);
            return query.ToList();
        }

        /// <summary>
        /// 二つの文字列を連結させる関数
        /// 引数(string 文字列,string 文字列)
        /// </summary>
        public String AddString(string str,string str1)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(str);
            sb.Append(str1);
           string s =  sb.ToString();
            return s;
        }

        /// <summary>
        /// 複数の文字列を連結させる関数
        /// 引数(string[]文字列の配列)
        /// </summary>
        public String AddString(string[] str)
        {
            StringBuilder sb = new StringBuilder();
            foreach(string word in str)
            {
                sb.Append(word);
            }
            var text = sb.ToString();
            return text;
        }

        /// <summary>
        /// 画像を指定のフォルダから読み込む関数
        /// 引数(Image 返される画像, string 画像があるパス,bool 条件があれば)
        /// </summary>
        public Image ImageLoadingAsync(Image image, string imagePath, bool check = true)
        {
            if (check)
            {
                Addressables.LoadAssetAsync<Sprite>(imagePath).Completed += sprite =>
                {
                     image.sprite = Instantiate(sprite.Result);                     
                };
                return image;
            }
            return image;
        }

        /*void OnTriggerEnter(Collider collider)
        {
            this.transform.parent = collider.gameObject.transform;
        }*/
    }

    namespace NDesignPattern
    {
        namespace NSingleton
        {
            interface ISingleton
            {
                bool DestroyTragetGameObject { get;}
                void Init();
                void OnDestroy();
                void OnRelease();
            }

            /// <summary>
            /// シングルトンクラス
            /// クラスのインスタンスを一つに制限する
            /// グローバルなアクセスポイントを提供する
            /// デザインパターンの一種(Singleton)
            /// </summary>
            public abstract class Singleton<T> : UnityGameLib,ISingleton where T : Singleton<T>
            {
                public virtual bool DestroyTragetGameObject => false;
                public static T Instance { get; private set; } = null;

                ///<summary>
                ///Singletonが有効かどうか
                /// </summary>
                public static bool IsValid() => Instance != null;

                private void Awake()
                {
                    if (Instance == null)
                    {
                        Instance = this as T;
                        Instance.Init();
                        return;
                    }
                    if (DestroyTragetGameObject)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        Destroy(this);
                    }
                }

                ///<summary>
                ///派生クラス用のAwake
                /// </summary>
                public virtual void Init() { }

                public void OnDestroy()
                {
                    if (Instance == this)
                    {
                        Instance = null;
                    }
                    OnRelease();
                }

                public virtual void OnRelease() { }
            }
        }
    }

    namespace NPlayerController
    {
        namespace NControllerPC
        {
            interface IControllerPC
            {
                Transform GetKeyPositionMoveUp(GameObject obj, float speed);
                Transform GetKeyPositionMoveDown(GameObject obj, float speed);
                Transform GetKeyPositionMoveRight(GameObject obj, float speed);
                Transform GetKeyPositionMoveLeft(GameObject obj, float speed);
            }

            /// <summary>
            /// PCのコントローラークラス
            /// PCのキー入力関係の関数が入っています
            /// </summary>
            public abstract class ControllerPC : UnityGameLib,IUnityGameLib,IControllerPC
            {
                public virtual Transform GetKeyPositionMoveUp(GameObject obj, float speed)
                {
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    {
                        obj.transform.position += Vector3.forward * speed * Time.deltaTime;
                    }
                    return obj.transform;
                }

                public virtual Transform GetKeyPositionMoveDown(GameObject obj, float speed)
                {
                    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    {
                        obj.transform.position += Vector3.back * speed * Time.deltaTime;
                    }
                    return obj.transform;
                }

                public virtual Transform GetKeyPositionMoveRight(GameObject obj, float speed)
                {
                    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    {
                        obj.transform.position += transform.right * speed * Time.deltaTime;
                    }
                    return obj.transform;
                }

                public virtual Transform GetKeyPositionMoveLeft(GameObject obj, float speed)
                {
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    {
                        obj.transform.position += Vector3.left * speed * Time.deltaTime;
                    }
                    return obj.transform;
                }
            }

        }

        namespace NControllerMobile
        {

        }
    }

    namespace NGameManager
    {
        
        namespace NSoundManager
        {

            interface ISoundManager
            {
                void PlayBGM(AudioClip bgmManager);
                void PlaySE(int seNumber);
            }

            [RequireComponent(typeof(AudioSource))]
            public abstract class SoundManager : Singleton<SoundManager>,IUnityGameLib,ISingleton,ISoundManager
            {
                [SerializeField]AudioSource audioSource;
                [SerializeField]AudioClip[] sound;

                public virtual void PlayBGM(AudioClip bgmNumber)
                {
                    audioSource.clip = bgmNumber;
                    audioSource.Play();
                }

                public virtual void PlaySE(int seNumber)
                {
                    audioSource.PlayOneShot(sound[seNumber]);
                }
            }
        }

        namespace NSceneManager
        {
            interface ISceneManager
            {
                
            }

            public abstract class SceneManagerLib : Singleton<SceneManagerLib>,IUnityGameLib,ISingleton,ISceneManager
            {
                
            }
        }

        namespace NScenarioManager
        {
            interface IScenarioManager
            {

            }
            public abstract class ScenarioManager : Singleton<ScenarioManager>,IUnityGameLib,IScenarioManager
            {

            }
        }

        namespace NDebugManager
        {
            interface IDebugManager
            {

            }

            public abstract class DebugManager : Singleton<DebugManager>,IUnityGameLib,IDebugManager
            {
               
            }
        }
    }
}