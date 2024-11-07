using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Turn True to disable screen timeout during using the app")]
    bool _sleepTimeoutNeverSleep = true;

    // Awake is called before Start, on initialization

    private void Awake()
    {
        if (_sleepTimeoutNeverSleep)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
