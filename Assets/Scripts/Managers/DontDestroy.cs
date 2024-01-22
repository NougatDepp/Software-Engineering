using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Start()
    {
        DontDestroyThis();
    }

    private void DontDestroyThis()
    {
        DontDestroyOnLoad(this);

    }

}
