using System;
using Unity.VisualScripting;
using UnityEngine;

public enum DiceState { Idle, Rolling, Dragging, Slotted }

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Dice : MonoBehaviour
{
    [Header("Runtime")]
     public DiceState state { get; private set; } = DiceState.Idle;
     public Rigidbody rb {get; private set;}
    public Collider col {get; private set;}
    public DiceSlot CurrentSlot {get; set;}

     public bool isInteractable => state == DiceState.Idle || state == DiceState.Slotted;

    [SerializeField] private float idleCheckDelay = 0.5f;
    float _idleCheckTimer;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    
    public void SetState(DiceState newState)
    {
        state = newState;
    }

}
