using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// なんかうまく揺らせないので保留
/// </summary>
public class CameraEffects : MonoSingleton<CameraEffects> {

    [SerializeField]
    Animator frontestColorPanel;

	void Start () {

	}

    /// <summary>
    /// 引数があるとContextMenuで呼べないのでこれで呼ぶ
    /// </summary>
    [ContextMenu("ShakeTest")]
    void ShakeTest()
    {
        Shake();
    }

    public void Shake(float shakeTime = 0.3f)
    {
        transform.DOShakePosition(shakeTime);
    }

    public void PlayerDamage()
    {
        Shake();
        frontestColorPanel.SetTrigger("Damage");
    }
}
