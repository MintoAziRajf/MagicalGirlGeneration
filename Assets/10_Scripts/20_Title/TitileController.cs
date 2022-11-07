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
        bool isCheck = false;
        // Start is called before the first frame update
        protected virtual void Start()
        {
            getHorizontalValue = 0f;
            decisionGameStart.SetActive(true);
            decisionEXIT.SetActive(false);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            GetHorizontal();

            if (Input.GetButtonDown("Submit") && isCheck)
            {
                LoadManager.instance.LoadScene("30_CharacterSelect");
            }
        }

        public virtual void GetHorizontal()
        {
            getHorizontalValue = Input.GetAxis("Vertical");

            if (getHorizontalValue == 1)
            {
                isCheck = true;
                decisionGameStart.SetActive(true);
                decisionEXIT.SetActive(false);                
            }

            if (getHorizontalValue == -1)
            {
                isCheck= false;
                decisionEXIT.SetActive(true);
                decisionGameStart.SetActive(false);
            }
        }
    }
}