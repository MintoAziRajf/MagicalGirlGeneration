using UnityEngine;

public class SetFramePerSeconds : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
