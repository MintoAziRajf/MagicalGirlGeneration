using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace NTitleController
{ 
    //インターフェース
    interface ITitileController
    {
       void GetHorizontal(); 
    }


    public class TitileController : MonoBehaviour,ITitileController
    {
        [SerializeField]MediaController mC;
        [SerializeField]GameObject decisionGameStart;
        [SerializeField]GameObject decisionEXIT;
        [SerializeField] private GameObject loadPrefab = null;
        [SerializeField] float timer = 0f;
        [SerializeField] FadeImage fi;

        public static LoadManager instance;

        float getHorizontalValue = 0f;
        bool isGameStartCheck = true;
        bool isExitGameCheck = true;
        bool checkIf = true;

    　　//プロパティ
        public float Timer
        {
            set { timer = value;}
            get { return timer;}
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            SoundManager.instance.PlayBGM(SoundManager.BGM_Type.Title);
            getHorizontalValue = 0f;
            decisionGameStart.SetActive(true);
            decisionEXIT.SetActive(false);
        }

        // Update is called once per frame
        protected virtual void Update()
        {         
            timer += Time.deltaTime;

            StartCoroutine(wait());
        }

        

        IEnumerator wait()
        {
            yield return new WaitUntil(() => fi.CutoutRange == 0f);

            GetHorizontal();

            //Startボタンを押したら
            if (Input.GetButtonDown("Submit") && isGameStartCheck && checkIf && mC.WaitCheck)
            {
                checkIf = false;
                LoadManager.instance.LoadScene("30_CharacterSelect");
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
            }

            //EXITボタンを押したら
            if (Input.GetButtonDown("Submit") && isExitGameCheck && checkIf && mC.WaitCheck)
            {
                checkIf = false;
                
            #if UNITY_EDITOR
                         UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
            #else
                         Application.Quit();//ゲームプレイ終了
            #endif
            }
        }

        public virtual void GetHorizontal()
        {
            getHorizontalValue = Input.GetAxis("Vertical");

            //Startボタンを選択していたら
            if (getHorizontalValue == 1)
            {
                timer = 0f;
                isGameStartCheck = true;
                isExitGameCheck = false;
                decisionGameStart.SetActive(true);
                decisionEXIT.SetActive(false);
            }

            //EXITボタンを選択していたら
            if (getHorizontalValue == -1)
            {
                timer = 0f;
                isGameStartCheck = false;
                isExitGameCheck = true;
                decisionEXIT.SetActive(true);
                decisionGameStart.SetActive(false);
            }
        }
    }
}