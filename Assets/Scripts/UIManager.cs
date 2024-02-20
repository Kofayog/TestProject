using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Vector3 = UnityEngine.Vector3;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button _create;
    [SerializeField] private Button _delete;
    [SerializeField] private Button _update;
    [SerializeField] private Button _refresh;

    [SerializeField] private ButtonController _button;
    [SerializeField] private Transform _buttonsParent;
    [SerializeField] private PopupController _popupController;
    
    private IWebRequestController _webRequestController;

    private Dictionary<string, ButtonController> _buttonsCollection = new Dictionary<string, ButtonController>();
    private ButtonController _selectedButton;

    private void OnUpdate(ButtonData data)
    {
        if (!_buttonsCollection.TryGetValue(data.id.ToString(), out var buttonController))
        {
            return;
        }

        buttonController.transform.DOShakeRotation(1f);
    }


    private void OnDelete(string id)
    {
        if (!_buttonsCollection.TryGetValue(id, out var buttonController))
        {
            return;
        }

        _buttonsCollection.Remove(id);
        
        buttonController.transform.
            DOJump(buttonController.transform.position + Vector3.up * 0.3f, 4f, 3, 2f).
            OnComplete(() => Destroy(buttonController.gameObject));
    }

    private void OnRefresh(ButtonData data)
    {
        var id = data.id.ToString();
        
        if (!_buttonsCollection.TryGetValue(id, out var buttonController))
        {
            buttonController = Instantiate(_button, _buttonsParent);
            _buttonsCollection.Add(id, buttonController);
        }
        
        buttonController.SetupData(data);
        buttonController.transform.DOScale(Vector3.one, 0.3f).From(Vector3.zero);
    }

    private void OnCreate(ButtonData data)
    {
        var id = data.id.ToString();
        if (_buttonsCollection.ContainsKey(id))
        {
            return;
        }
        
        var instance = Instantiate(_button, _buttonsParent);
        instance.SetupData(data);
        
        _buttonsCollection.Add(id, instance);
    }

    private void CreateRequest()
    {
        _webRequestController.Create().Forget();
    }
    private void DeleteRequest()
    {
        _popupController.ShowPopup();
        _popupController.SetRequestAction(_webRequestController.Delete);
    }
    private void UpdateRequest()
    {
        _popupController.ShowPopup();
        _popupController.SetRequestAction(_webRequestController.Update);
    }
    private void RefreshRequest()
    {
        _popupController.ShowPopup();
        _popupController.SetRequestAction(_webRequestController.Refresh);
    }

    private void Start()
    {
        _webRequestController = new MockapiWebController();
        _webRequestController.OnCreateSuccessful += OnCreate;
        _webRequestController.OnRefreshSuccessful += OnRefresh;
        _webRequestController.OnDeleteSuccessful += OnDelete;
        _webRequestController.OnUpdateSuccessful += OnUpdate;
        
        _create.onClick.AddListener(CreateRequest);
        _delete.onClick.AddListener(DeleteRequest);
        _update.onClick.AddListener(UpdateRequest);
        _refresh.onClick.AddListener(RefreshRequest);
    }

    private void OnDestroy()
    {
        _webRequestController.OnCreateSuccessful -= OnCreate;
        _webRequestController.OnRefreshSuccessful -= OnRefresh;
        _webRequestController.OnDeleteSuccessful -= OnDelete;
        _webRequestController.OnUpdateSuccessful -= OnUpdate;
        
        _create.onClick.RemoveListener(CreateRequest);
        _delete.onClick.RemoveListener(DeleteRequest);
        _update.onClick.RemoveListener(UpdateRequest);
        _refresh.onClick.RemoveListener(RefreshRequest);
    }
}
