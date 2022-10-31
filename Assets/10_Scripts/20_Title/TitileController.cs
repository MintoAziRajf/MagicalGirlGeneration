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
        [SerializeField]GameObject DecisionGameStart;
        [SerializeField]GameObject DecisionEXIT;

        float getHorizontalValue = 0f;
        // Start is called before the first frame update
        protected virtual void Start()
        {
            getHorizontalValue = 0f;
            DecisionGameStart.SetActive(true);
            DecisionEXIT.SetActive(false);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            GetHorizontal();
        }

        public virtual void GetHorizontal()
        {
            getHorizontalValue = Input.GetAxis("Vertical");

            if (getHorizontalValue == 1)
            {
                DecisionGameStart.SetActive(true);
                DecisionEXIT.SetActive(false);
            }

            if (getHorizontalValue == -1)
            {
                DecisionEXIT.SetActive(true);
                DecisionGameStart.SetActive(false);
            }
        }
    }
}