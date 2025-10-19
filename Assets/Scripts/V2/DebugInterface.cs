using TMPro;
using UnityEngine;

public class DebugInterface : MonoBehaviour
{
    [SerializeField] private TMP_Text  horizontalInput;
    [SerializeField] private TMP_Text  forwardsInput;
    [SerializeField] private TMP_Text  verticalInput;

    private void Start()
    {
        PlayerFlyingMovement.OnHorizontalInputChange += UpdateHorizontalMovementText;
        PlayerFlyingMovement.OnForwardsInputChange += UpdateForwardsMovementText;
        PlayerFlyingMovement.OnVerticalInputChange += UpdateVerticalMovementText;
    }

    private void UpdateHorizontalMovementText(float input)
    {
        horizontalInput.text = $"horz: {input:F2}";
    }
    
    private void UpdateForwardsMovementText(float input)
    {
        horizontalInput.text = $"forw: {input:F2}";
    }
    
    private void UpdateVerticalMovementText(float input)
    {
        horizontalInput.text = $"vert: {input:F2}";
    }
}
