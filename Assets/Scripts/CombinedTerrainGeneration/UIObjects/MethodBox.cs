using Assets.Scripts.CombinedTerrainGeneration;
using UnityEngine;
using UnityEngine.UI;

public class MethodBox : MonoBehaviour {

    [System.NonSerialized]
    public Method method;
    [System.NonSerialized]
    public GenerationInstance GenInstance;
    public Text Title;
    public Transform DataPanel;
    public MethodDataPair DataPairObject;
    public ContentSizeFitter Fitter;

    private bool _fixContentFit = false;

    public void Start()
    {
        DataPairObject.gameObject.SetActive(false);
        Fitter.enabled = false;
    }

    public void Update()
    {
        //This is used to enable the content size fitter for one frame then disable it again to force it to fit the content.
        if (_fixContentFit)
        {
            if (Fitter.enabled)
            {
                Fitter.enabled = false;
                _fixContentFit = false;
            }
            else Fitter.enabled = true;
        }
    }

    public void RemoveButton()
    {
        GenInstance.RemoveMethod(method);
        Destroy(gameObject);
    }

    public void FitContent()
    {
        _fixContentFit = true;
    }
}
