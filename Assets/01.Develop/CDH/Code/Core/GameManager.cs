using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public UnityEvent StartRopeCharge;
    public UnityEvent EndRopeCharge;

    private bool isRopeCharging;
    private bool isRopeChargeEnd;

    private void Awake()
    {
        isRopeCharging = false;
        isRopeChargeEnd = false;
    }

    private void Update()
    {
        if (isRopeChargeEnd && !isRopeCharging && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isRopeCharging = true;
            isRopeChargeEnd = false;
            StartRopeCharge?.Invoke();
        }
        if(!isRopeChargeEnd && isRopeCharging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isRopeCharging = false;
            isRopeChargeEnd = true;
            EndRopeCharge?.Invoke();
        }
    }

}
