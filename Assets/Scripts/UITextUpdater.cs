using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Text))]
public class UITextUpdater : MonoBehaviour {

    private Text text;

    public void Awake()
    {
        text = GetComponent<Text>();
    }

    public void SetTextValue(float val)
    {
        text.text = val.ToString();
    }

    public void SetTextValue(int val)
    {
        text.text = val.ToString();
    }

    public void SetTextValue(string val)
    {
        text.text = val;
    }
}
