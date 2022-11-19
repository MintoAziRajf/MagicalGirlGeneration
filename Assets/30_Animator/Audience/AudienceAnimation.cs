using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceAnimation : MonoBehaviour
{
    [SerializeField] private Animator audienceAnim;
    public void StartAnimation()
    {
        audienceAnim.SetTrigger("Skill");
    }
}
