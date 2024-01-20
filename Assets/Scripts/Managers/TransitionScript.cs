using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{

    public void Transition(string sceneName,TransitionSettings transition,float loadDelay)
    {
        TransitionManager.Instance().Transition(sceneName,transition,loadDelay);
    }
}
