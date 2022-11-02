using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public IEnumerator Move()
    {
        yield return new WaitUntil(() => !Input.GetKeyDown(KeyCode.A));
    }
}
