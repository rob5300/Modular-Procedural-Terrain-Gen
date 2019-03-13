using System;
using UnityEngine.UI;
using System.Collections.Concurrent;
using UnityEngine;

namespace Assets.Scripts.CombinedTerrainGeneration
{
    public class UnityHelper : MonoBehaviour
    {
        public ConcurrentQueue<Action> UnityTasks = new ConcurrentQueue<Action>();

        public void Update()
        {
            Action action;
            if (UnityTasks.Count > 0 && !UnityTasks.TryDequeue(out action))
            {
                action.Invoke();
            }

        }
    }
}
