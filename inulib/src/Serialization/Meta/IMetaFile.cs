﻿namespace INUlib.Serialization.Meta
{
    public interface IBaseMetaFile
    {
        bool Load();
    }

    public interface IMetaFile<T> : IBaseMetaFile where T : class
    {
        T Data { get; }
    }
}