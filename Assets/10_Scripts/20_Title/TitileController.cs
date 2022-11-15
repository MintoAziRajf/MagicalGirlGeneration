using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NTitleController
{ 
    interface ITitileController
    {
       void GetHorizontal(); 
    }


    public class TitileController : MonoBehaviour,ITitileController
    {
        [SerializeField]GameObject decisionGameStart;
        [SerializeField]GameObject decisionEXIT;

        public static LoadManager instance;
        [SerializeField] private GameObject loadPrefab = null;

        float getHorizontalValue = 0f;
        bool isGameStartCheck = true;
        bool isExitGameCheck = true;
        bool checkIf = true;

        [SerializeField]FadeImage fi;
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
          
            StartCoroutine(wait());
        }

        public virtual void GetHorizontal()
        {
            getHorizontalValue = Input.GetAxis("Vertical");

            if (getHorizontalValue == 1)
            {
                isGameStartCheck = true;
                isExitGameCheck = false;
                decisionGameStart.SetActive(true);
                decisionEXIT.SetActive(false);
            }

            if (getHorizontalValue == -1)
            {
                isGameStartCheck = false;
                isExitGameCheck = true;
                decisionEXIT.SetActive(true);
                decisionGameStart.SetActive(false);
            }
        }

        IEnumerator wait()
        {
            yield return new WaitUntil(() => fi.CutoutRange == 0f);

            GetHorizontal();
            if (Input.GetButtonDown("Submit") && isGameStartCheck && checkIf)
            {
                checkIf = false;
                LoadManager.instance.LoadScene("30_CharacterSelect");
                SoundManager.instance.PlaySE(SoundManager.SE_Type.Submit);
            }

            if (Input.GetButtonDown("Submit") && isExitGameCheck && checkIf)
            {
                checkIf = false;
                
            #if UNITY_EDITOR
                         UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
            #else
                         Application.Quit();//ゲームプレイ終了
            #endif
            }
        }
    }
}