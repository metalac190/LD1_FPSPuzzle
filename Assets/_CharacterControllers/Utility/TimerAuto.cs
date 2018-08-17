using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// automatically starts a timer that runs designated functions at an interval
public class TimerAuto : MonoBehaviour {

    [SerializeField] float timerInterval = 1.5f;       // length of timer loop, in seconds
    [SerializeField] bool immediateExecute;         // whether function should call immediately when this script is created
    [SerializeField] bool isLooping;            // should this timer loop?

    public UnityEvent OnTimerInterval;       // event handler for functions that subscribe

    // using OnEnable to start timer so that it works with Object Pooling
    void OnEnable()
    {
        // make sure timer is > 0, don't start the timer. otherwise things will get insane
        if(timerInterval <= 0)
        {
            return;
        }

        // immediately call timer if specified
        if (immediateExecute)
        {
            Invoke("Execute", 0);
        }
        // if it's looping, start the loop
        if (isLooping)
        {
            InvokeRepeating("Execute", timerInterval, timerInterval);
        }
        // if it's not looping, just activate the Event after the timer interval
        else
        {
            Invoke("Execute", timerInterval);
        }
    }

    // make sure Invokes cancel if this script (or object) is disabled
    void OnDisable()
    {
        CancelInvoke();
    }

    // run all of the functions inside of the UnityEvent
    void Execute()
    {
        OnTimerInterval.Invoke();
    }
}
