using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public UnityEvent StartRopeCharge;
    public UnityEvent EndRopeCharge;

    private bool isRopeCharging;
    private bool isRopeChargeEnd;
    private bool isRopeChargeTurn;

    private void Awake()
    {
        isRopeCharging = false;
        isRopeChargeEnd = false;
        isRopeChargeTurn = true;
    }

    private void Update()
    {
        if (isRopeChargeTurn)
        {
            if (!isRopeCharging && Mouse.current.leftButton.wasPressedThisFrame)
            {
                isRopeCharging = true;
                isRopeChargeEnd = false;
                StartRopeCharge?.Invoke();
            }
            if (isRopeCharging && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isRopeCharging = false;
                isRopeChargeEnd = true;
                EndRopeCharge?.Invoke();
                isRopeChargeTurn = false;
            }
        }
    }

    public void SetRopeChargeTurn() => isRopeChargeTurn = true;

}
