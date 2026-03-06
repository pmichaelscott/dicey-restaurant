using UnityEngine;

[RequireComponent(typeof(Dice))]
public class DiceRoller : MonoBehaviour
{
    [Header("Roll Forces")]
    [SerializeField] private float impulseMin = 3.5f;
    [SerializeField] private float impulseMax = 6.5f;
    [SerializeField] private float torqueMin = 8f;
    [SerializeField] private float torqueMax = 16f;

    [Header("Settle Detection")]
    [SerializeField] private float linearThreshold = 0.05f;
    [SerializeField] private float angularThreshold = 0.2f;
    [SerializeField] private float settleTime = 0.5f;
    [SerializeField] private float maxRollTime = 6f;

    private Dice dice;
    private float belowThresholdTimer = 0f;
    private float rollTimer = 0f;
    private DiceValue diceValue;

    private void Awake()
    {
        dice = GetComponent<Dice>();
        diceValue = GetComponent<DiceValue>();
    }

    public void Roll(Vector3 rollDirection)
    {
        // Reset timers
        belowThresholdTimer = 0f;
        rollTimer = 0f;

        dice.SetState(DiceState.Rolling);

        // Ensure physics is active
        dice.rb.isKinematic = false;
        dice.rb.WakeUp();

        // Clear old motion
        dice.rb.linearVelocity = Vector3.zero;
        dice.rb.angularVelocity = Vector3.zero;

        // Apply random impulse + torque
        float impulse = Random.Range(impulseMin, impulseMax);
        dice.rb.AddForce(rollDirection.normalized * impulse, ForceMode.Impulse);

        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        float torque = Random.Range(torqueMin, torqueMax);
        dice.rb.AddTorque(randomTorque * torque, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (dice.state != DiceState.Rolling)
            return;

        rollTimer += Time.fixedDeltaTime;

        float lv = dice.rb.linearVelocity.magnitude;
        float av = dice.rb.angularVelocity.magnitude;

        bool below = lv < linearThreshold && av < angularThreshold;

        if (below)
            belowThresholdTimer += Time.fixedDeltaTime;
        else
            belowThresholdTimer = 0f;

        
        if (belowThresholdTimer >= settleTime)
        {
            FinishRoll();
            return;
        }

        // If it never settles, damp and accept
        if (rollTimer >= maxRollTime)
        {
            dice.rb.linearVelocity *= 0.2f;
            dice.rb.angularVelocity *= 0.2f;

            // Give it a short window to settle after damping
            belowThresholdTimer = settleTime;
            FinishRoll();
        }
    }

    private void FinishRoll()
    {

        dice.rb.linearVelocity = Vector3.zero;
        dice.rb.angularVelocity = Vector3.zero;

        dice.SetState(DiceState.Idle);
        var diceValue = GetComponent<DiceValue>();
        
        if (diceValue != null)
            {
                int result = diceValue.ComputeAndStoreValue();
                Debug.Log($"Die rolled: {result}");
            }
    }
}
