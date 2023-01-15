using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Bar")]
public class BarSO : ScriptableObject
{
    public TextMeshProUGUI Text;
    public Slider Slider;
}
