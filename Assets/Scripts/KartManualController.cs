
using UnityEngine;

public class KartManualController : MonoBehaviour
{
    [Header("Player Controller")]
    public string m_HorizontalAxisName = "Horizontal";
    public string m_VerticalAxisName = "Vertical";
    public string m_BrakeButtonName = "Jump";

    private CarKinematics m_Controller;

    private void Awake()
    {
        m_Controller = GetComponent<CarKinematics>();
    }

    private void Update()
    {
        m_Controller.HorizontalInput = Input.GetAxis(m_HorizontalAxisName);
        m_Controller.VerticalInput = Input.GetAxis(m_VerticalAxisName);
        m_Controller.BrakeInput = Input.GetButton(m_BrakeButtonName);
    }
}