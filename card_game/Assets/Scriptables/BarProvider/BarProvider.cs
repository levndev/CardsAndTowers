using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarProvider : MonoBehaviour
{
    [SerializeField] private BarSO barVariable;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        barVariable.Text = text;
        barVariable.Slider = slider;
    }
}
