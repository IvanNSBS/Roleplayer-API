using System;
using Unity.Collections;

namespace RPGCore.FileManagement.SavingFramework
{
    public interface ISaveableData
    {
        string SurrogateName { get; }
        string Save();
        bool Load(string componentJsonString);
    }
}