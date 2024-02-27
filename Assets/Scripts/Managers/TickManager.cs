using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    public static TickManager instance;
    public static TickManager GetInstance()
    {
        return instance;
    }

    public delegate void TickDelegate();
    public delegate void PrepTickDelegate();
    public TickDelegate onTick;
    public PrepTickDelegate onPrepTick;


    
    public float ticksPerSecond = 1f;
    public bool paused = false;


    private float tickTimer = 0f;
    private int tickCount = 0;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Too many tickmanagers, deleting...", gameObject);
            Destroy(this);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (paused) return;

        tickTimer += Time.deltaTime;

        if (tickTimer >= 1f / ticksPerSecond)
        {
            tickTimer -= 1f / ticksPerSecond;
            tickCount++;
            
            onTick?.Invoke();
        }
    }
}
