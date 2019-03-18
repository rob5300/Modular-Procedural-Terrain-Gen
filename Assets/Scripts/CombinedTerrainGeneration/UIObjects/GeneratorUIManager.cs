using Assets.Scripts.CombinedTerrainGeneration;
using Assets.Scripts.CombinedTerrainGeneration.GenerationMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUIManager : MonoBehaviour {

    public GenerationInstance GenerationInst;

    [Header("Method Objects")]
    public Transform MethodContentArea;
    public MethodBox MethodDataBoxInstance;
    public Dropdown MethodsDropdown;
    public RectTransform AddNewBox;
    [Header("Conversion Method Objects")]
    public Transform ConvertMethodContentArea;
    public Dropdown ConversionMethodsDropdown;
    public MethodBox ConversionMethodBoxInstance;

    private List<List<MethodDataPair>> _dataPairs = new List<List<MethodDataPair>>();
    private List<MethodDataPair> _converterDataPairs = new List<MethodDataPair>();
    private int _currentIndex = 0;

    private int _currentConvertMethodSelected = -1;
    private GameObject _currentConvertMethodBox;

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

        //Populate the dropdown for normal methods
        List<Dropdown.OptionData> methodDropdownData = new List<Dropdown.OptionData>();
        foreach(Type type in Methods)
        {
            methodDropdownData.Add(new Dropdown.OptionData(type.Name));
        }
        MethodsDropdown.AddOptions(methodDropdownData);


        //Populate the dropdown for conversion methods
        List<Dropdown.OptionData> cmethodDropdownData = new List<Dropdown.OptionData>();
        foreach (Type type in ConversionMethods)
        {
            cmethodDropdownData.Add(new Dropdown.OptionData(type.Name));
        }
        ConversionMethodsDropdown.AddOptions(cmethodDropdownData);

        //Set the first conversion method as the current conversion method by default
        ConvertMethodChange(0);
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

    public void ConvertMethodChange(int index)
    {
        if(index != _currentConvertMethodSelected)
        {
            GenerationInst.AddMethod(Activator.CreateInstance(ConversionMethods[index]) as ConversionMethod);
            UpdateConvertMethodUI(index);
            _currentConvertMethodSelected = index;
        }
    }

    private void UpdateConvertMethodUI(int index)
    {
        if (_currentConvertMethodBox != null) Destroy(_currentConvertMethodBox);
        //Make a new ui box for this method and populate it with the correct data.
        GameObject newBox = Instantiate(ConversionMethodBoxInstance.gameObject, ConvertMethodContentArea);
        MethodBox box = newBox.GetComponent<MethodBox>();
        box.Title.text = ConversionMethods[index].Name;
        box.Index = index;
        newBox.SetActive(true);
        _currentConvertMethodBox = box.gameObject;
        MakeDataPairs(ConversionMethods[index], box, true);
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

    private void MakeDataPairs(Type method, MethodBox box, bool conversionMethod = false)
    {
        //Get a list of the configurable values
        List<ConfigurableField> values = GenerationInst.GetConfigList(method);

        //If we have any values to display.
        if (values != null)
        {
            List<MethodDataPair> pairs;
            if (!conversionMethod)
            {
                _dataPairs.Add(new List<MethodDataPair>());
                pairs = _dataPairs[_currentIndex];
            }
            else
            {
                _converterDataPairs.Clear();
                pairs = _converterDataPairs;
            }
            foreach (ConfigurableField field in values)
            {
                GameObject newVal = Instantiate(box.DataPairObject.gameObject, box.DataPanel);
                MethodDataPair dataPair = newVal.GetComponent<MethodDataPair>();
                dataPair.Name.text = field.Name + ":";
                dataPair.FieldData = field;
                if(field.ValueType == ConfigurableField.Type.BOOLEAN)
                {
                    //Field is a boolean.
                    dataPair.EnableToggle();
                    dataPair.Toggle.isOn = field.BoolValue;
                }
                else
                {
                    //field is any other type.
                    dataPair.Input.text = field.StringValue;
                    SetTextInputToType(dataPair.Input, field.ValueType);
                }
                pairs.Add(dataPair);
                dataPair.gameObject.SetActive(true);
            } 
        }
        box.FitContent();
    }

    public List<ConfigurableField> GetDataFromPairs(int index)
    {
        List<ConfigurableField> dataPairs = new List<ConfigurableField>();
        foreach(MethodDataPair dPair in _dataPairs[index])
        {
            //Update the string or boolean value using the input fields value.
            if (dPair.FieldData.ValueType == ConfigurableField.Type.BOOLEAN)
            {
                dPair.FieldData.BoolValue = dPair.Toggle.isOn;
            }
            else
            {
                dPair.FieldData.StringValue = dPair.Input.text;
            }
            dataPairs.Add(dPair.FieldData);
        }
        return dataPairs;
    }

    public List<ConfigurableField> GetDataFromPairsOnConversionMethod()
    {
        List<ConfigurableField> dataPairs = new List<ConfigurableField>();
        foreach (MethodDataPair dPair in _converterDataPairs)
        {
            dataPairs.Add(dPair.FieldData);
        }
        return dataPairs;
    }

    private void SetTextInputToType(InputField inputField, ConfigurableField.Type type)
    {
        switch (type)
        {
            case ConfigurableField.Type.FLOAT:
                inputField.contentType = InputField.ContentType.DecimalNumber;
                return;
            case ConfigurableField.Type.INT:
                inputField.contentType = InputField.ContentType.IntegerNumber;
                return;
            case ConfigurableField.Type.STRING:
            default:
                inputField.contentType = InputField.ContentType.Standard;
                return;
        }
    }
}
