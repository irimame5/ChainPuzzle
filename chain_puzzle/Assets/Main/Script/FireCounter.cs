using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireCounter : MonoSingleton<FireCounter> {

    [SerializeField]
    Image[] fireImages;
    [SerializeField]
    int count = 5;
	
    public void AddFire()
    {
        if (count == fireImages.Length) { return; }
        count++;
        fireImages[count - 1].enabled = true;
    }

    public void RemoveFire()
    {
        if (count == 0) { return; }
        fireImages[count-1].enabled = false;
        count--;
    }
}
