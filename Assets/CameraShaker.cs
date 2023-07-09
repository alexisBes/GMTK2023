using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShaker : MonoBehaviour
{
    public Transform camera;
    public Vector3 positionStrength;

    public Vector3 rotationStrength;

    private static event Action shake;

    public static void Invoke() {
        shake?.Invoke();
    }

    private void OnEnable() => shake += CameraShake;
    private void OnDisable() => shake -= CameraShake;

    public void CameraShake() {
        camera.DOComplete();
        camera.DOShakePosition(0.3f, positionStrength);
        camera.DOShakeRotation(0.3f, rotationStrength);
    }
}
