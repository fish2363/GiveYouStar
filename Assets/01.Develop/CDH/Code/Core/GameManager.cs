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
    private bool isCatchStar;

    private void Awake()
    {
        isRopeCharging = false;
        isRopeChargeEnd = false;
        isRopeChargeTurn = true;
        isCatchStar = false;
    }

    private void Update()
    {
        if (isRopeChargeTurn && !isCatchStar)
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
                isRopeChargeTurn = false;
                EndRopeCharge?.Invoke();
            }
        }
    }

    public void SetRopeChargeTurn() => isRopeChargeTurn = true;
    public void SetCatchStar() => isCatchStar = true;
    public void EndCatchStar() => isCatchStar = false;

}
