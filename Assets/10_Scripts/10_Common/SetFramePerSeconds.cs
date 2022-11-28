using UnityEngine;

public class SetFramePerSeconds : MonoBehaviour
{
    void Awake() => Application.targetFrameRate = 60; // 目標フレームにを60に設定
}
