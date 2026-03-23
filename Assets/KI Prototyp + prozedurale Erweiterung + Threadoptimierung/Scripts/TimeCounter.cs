using TMPro;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    private int count = 0;

    private void Awake()
    {
        count = 0;
    }

    private void FixedUpdate()
    {
        count = (int)Time.timeSinceLevelLoad;
        text.text = count.ToString();
    }
}
