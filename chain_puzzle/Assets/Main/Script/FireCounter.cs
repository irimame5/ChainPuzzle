using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FireCounter : MonoSingleton<FireCounter> {

    [SerializeField]
    Image[] fireImages;
    [SerializeField]
    int count = 5;

    void Start()
    {
        //var a = fireImages.Skip(count).Select(x => x.enabled = false);
        for(int i = 0; i < fireImages.Length; i++)
        {
            if (count <= i)
            {
                fireImages[i].enabled = false;
            }else
            {
                fireImages[i].enabled = true;
            }
        }
    }

    public void AddFire()
    {
        if (count == fireImages.Length) { return; }
        count++;
        fireImages[count - 1].enabled = true;
    }

    public void RemoveFire()
    {
        if (count == 0) { return; }
        if(fireImages.Length<=count - 1) { return; }
        fireImages[count-1].enabled = false;
        count--;
    }
}
