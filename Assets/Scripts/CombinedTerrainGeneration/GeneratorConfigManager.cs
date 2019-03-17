using Assets.Scripts.CombinedTerrainGeneration;
using Assets.Scripts.CombinedTerrainGeneration.GenerationMethods;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Assets.Scripts.CombinedTerrainGeneration.Configuration;
using UnityEngine;

public class GeneratorConfigManager {

    public Dictionary<Type, List<string>> TypeConfigLists = new Dictionary<Type, List<string>>();

    private Type _configType = typeof(ConfigurableAttribute);

    public List<string> GetConfigList(Type type)
    {
        if (!TypeConfigLists.ContainsKey(type)) TypeConfigLists.Add(type, GetConfigurables(type));
        return TypeConfigLists[type];
    }

    private List<string> GetConfigurables(Type type)
    {
        List<string> Configurables = new List<string>();
        FieldInfo[] fields = type.GetFields();
        List<FieldInfo> configurableFields = GetConfigurableFields(type);
        if (configurableFields != null)
        {
            foreach (FieldInfo field in configurableFields)
            {
                Configurables.Add(field.Name);
            } 
        }
        return Configurables;
    }

    public void ApplyConfigurableValuesTo(Method method, Dictionary<string, string> values)
    {
        foreach(KeyValuePair<string, string> pair in values)
        {
            try
            {
                method.GetType().GetField(pair.Key).SetValue(method, pair.Value);
            }
            catch(ArgumentException e)
            {
                Debug.LogError("Field value assign failed for: " + pair.Key);
            }
        }
    }

    private List<FieldInfo> GetConfigurableFields(Type type)
    {
        FieldInfo[] fields = type.GetFields();
        if (fields.Length == 0) return null;
        return fields.Where(x => x.CustomAttributes.Where(y => y.AttributeType == _configType).First() != null).ToList();
    }
}
