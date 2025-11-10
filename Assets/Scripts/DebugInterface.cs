using TMPro;
using UnityEngine;

public class DebugInterface : MonoBehaviour
{
    [SerializeField] private TMP_Text  velocityInput;

    private void Start()
    {
        PlayerFlyingMovement.RigidBodyVelocityChange += UpdateVelocityText;
    }

    private void UpdateVelocityText(Vector3 input)
    {
        velocityInput.text = $"X: {input.x:F1}, Y: {input.y:F1}, Z: {input.z:F1}";
    }
}
