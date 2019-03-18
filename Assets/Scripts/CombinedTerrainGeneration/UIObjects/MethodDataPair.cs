using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MethodDataPair : MonoBehaviour {

    [System.NonSerialized]
    public ConfigurableField FieldData;
    public InputField Input;
    public Toggle Toggle;
    public Text Name;

    public void Awake()
    {
        Toggle.gameObject.SetActive(false);
        Input.gameObject.SetActive(true);
    }

    public void EnableToggle()
    {
        Toggle.gameObject.SetActive(true);
        Input.gameObject.SetActive(false);
    }
}
