using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataService
{
    bool SaveData<T>(string relativePath, T data);

    T LoadData<T>(string relativePath);

    public void LoadDataAsync<T>(string relativePath, Action<T> callback);

    public T LoadDataFullPath<T>(string fullpath);

    public void LoadDataAsyncFullPath<T>(string fullpath, Action<T> callback);

}
