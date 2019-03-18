using Assets.Scripts.CombinedTerrainGeneration;
using Assets.Scripts.CombinedTerrainGeneration.GenerationMethods;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Assets.Scripts.CombinedTerrainGeneration.Configuration;
using UnityEngine;

public class GeneratorConfigManager {

    public Dictionary<Type, List<ConfigurableField>> TypeConfigLists = new Dictionary<Type, List<ConfigurableField>>();

    private Type _configType = typeof(ConfigurableAttribute);

    public List<ConfigurableField> GetConfigList(Type type)
    {
        if (!TypeConfigLists.ContainsKey(type)) TypeConfigLists.Add(type, GetConfigurables(type));
        return TypeConfigLists[type];
    }

    private List<ConfigurableField> GetConfigurables(Type type)
    {
        List<ConfigurableField> Configurables = new List<ConfigurableField>();
        FieldInfo[] fields = type.GetFields();
        List<FieldInfo> configurableFields = GetConfigurableFields(type);
        object instance = Activator.CreateInstance(type);
        if (configurableFields != null)
        {
            foreach (FieldInfo field in configurableFields)
            {
                object value = field.GetValue(instance);
                ConfigurableField newC = new ConfigurableField(field.Name, value.ToString(), GetTypeFromValue(value));
                if (value is bool) newC.BoolValue = (bool)value;
                Configurables.Add(newC);
            } 
        }
        return Configurables;
    }

    public void ApplyConfigurableValuesTo(Method method, List<ConfigurableField> values)
    {
        foreach(ConfigurableField field in values)
        {
            try
            {
                method.GetType().GetField(field.Name).SetValue(method, ConvertToType(field));
            }
            catch(ArgumentException e)
            {
                Debug.LogError("Field value assign failed for: " + method.GetType().Name + "." + field.Name);
            }
        }
    }

    private List<FieldInfo> GetConfigurableFields(Type type)
    {
        FieldInfo[] fields = type.GetFields();
        if (fields.Length == 0) return null;
        return fields.Where(x => x.CustomAttributes.Count() > 0 && x.CustomAttributes.Where(y => y.AttributeType == _configType).First() != null).ToList();
    }

    private object ConvertToType(ConfigurableField type)
    {
        switch (type.ValueType)
        {
            case ConfigurableField.Type.FLOAT:
                return Convert.ToSingle(type.StringValue);
            case ConfigurableField.Type.INT:
                return Convert.ToInt32(type.StringValue);
            case ConfigurableField.Type.BOOLEAN:
                return type.BoolValue;
            case ConfigurableField.Type.STRING:
            default:
                return type.StringValue;
        }
    }

    private ConfigurableField.Type GetTypeFromValue(object o)
    {
        if (o is int) return ConfigurableField.Type.INT;
        else if (o is float) return ConfigurableField.Type.FLOAT;
        else if (o is bool) return ConfigurableField.Type.BOOLEAN;
        return ConfigurableField.Type.STRING;
    }
}

public struct ConfigurableField
{
    public enum Type { INT, FLOAT, STRING, BOOLEAN };

    public string Name;
    public string StringValue;
    public bool BoolValue;
    public Type ValueType;

    public ConfigurableField(string name, string strVal, Type type)
    {
        Name = name;
        StringValue = strVal;
        ValueType = type;
        BoolValue = false;
    }
}
