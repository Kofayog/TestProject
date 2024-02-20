using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IWebRequestController
{
    public event Action<ButtonData> OnCreateSuccessful;
    public event Action<string> OnDeleteSuccessful;
    public event Action<ButtonData> OnUpdateSuccessful;
    public event Action<ButtonData> OnRefreshSuccessful;
    
    UniTask Create();
    UniTask Delete(string id);
    UniTask Update(string id);
    UniTask Refresh(string id);
}
