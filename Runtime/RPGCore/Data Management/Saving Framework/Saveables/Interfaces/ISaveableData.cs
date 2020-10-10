using System;
using Newtonsoft.Json.Linq;
using Unity.Collections;

namespace RPGCore.FileManagement.SavingFramework
{
    public interface ISaveableData
    {
        string SurrogateName { get; }
        string Save();
        bool Load(JObject saveable);
    }
}