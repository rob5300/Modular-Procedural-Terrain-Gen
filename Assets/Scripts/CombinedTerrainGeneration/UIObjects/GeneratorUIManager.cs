using Assets.Scripts.CombinedTerrainGeneration;
using Assets.Scripts.CombinedTerrainGeneration.GenerationMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUIManager : MonoBehaviour {

    public GenerationInstance GenerationInst;

    public Transform MethodContentArea;
    public MethodBox MethodDataBoxInstance;
    public Dropdown MethodsDropdown;
    public Dropdown ConversionMethodsDropdown;
    public RectTransform AddNewBox;

    private List<List<MethodDataPair>> DataPairs = new List<List<MethodDataPair>>();
    private int _currentIndex = 0;

    private Type[] Methods =
    {
        typeof(PerlinNoiseGeneration)
    };

    private Type[] ConversionMethods =
    {
        typeof(MarchingCubesConversion)
    };

    public void Start()
    {
        MethodDataBoxInstance.gameObject.SetActive(false);

        if(GenerationInst == null)
        {
            GenerationInst = FindObjectOfType<GenerationInstance>();
            if(GenerationInst == null)
            {
                Debug.LogError("No Generation Instance found.");
            }
        }
    }

    public void AddNewMethod()
    {
        AddMethodFromIndex(MethodsDropdown.value);
    }

    private void AddMethodFromIndex(int index)
    {
        GenerationInst.AddMethod(Activator.CreateInstance(Methods[index]) as Method);
        AddMethodToUI(Methods[index]);
        AddNewBox.SetAsLastSibling();
    }

    private void AddMethodToUI(Type method)
    {
        //Add new method box
        GameObject newBox = Instantiate(MethodDataBoxInstance.gameObject, MethodContentArea);
        MethodBox box = newBox.GetComponent<MethodBox>();
        box.Title.text = method.Name;
        box.Index = _currentIndex;
        newBox.SetActive(true);
        MakeDataPairs(method, box);
        _currentIndex++;
    }

    private void MakeDataPairs(Type method, MethodBox box)
    {
        //Get a list of the configurable values
        List<string> values = GenerationInst.GetConfigList(method);
        DataPairs.Add(new List<MethodDataPair>());
        List<MethodDataPair> pairs = DataPairs[_currentIndex];
        foreach (string str in values)
        {
            GameObject newVal = Instantiate<GameObject>(box.DataPairObject.gameObject, box.DataPanel);
            MethodDataPair dataPair = newVal.GetComponent<MethodDataPair>();
            dataPair.Name.text = str + ":";
            dataPair.key = str;
            pairs.Add(dataPair);
            dataPair.gameObject.SetActive(true);
        }
        box.FitContent();
    }

    public Dictionary<string, string> GetDataFromPairs(int index)
    {
        Dictionary<string, string> dataPairs = new Dictionary<string, string>();
        foreach(MethodDataPair dPair in DataPairs[index])
        {
            dataPairs.Add(dPair.key, dPair.Input.text);
        }
        return dataPairs;
    }
}
