using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject _popupPanel;
    [SerializeField] private float _popupDuration = 1f;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _cancelButton;

    private Func<string, UniTask> _requestAction;

    public void SetRequestAction(Func<string, UniTask> requestAction)
    {
        _requestAction = requestAction;
    }
    
    public void ShowPopup()
    {
        _popupPanel.SetActive(true);
        _popupPanel.transform.DOScale(Vector3.one, _popupDuration).From(Vector3.zero);
    }
    public void HidePopup()
    {
        _inputField.text = string.Empty;
        _popupPanel.transform.DOScale(Vector3.zero, _popupDuration).From(_popupPanel.transform.localScale).
            OnComplete(() => _popupPanel.SetActive(false));
    }
    
    private void OnApply()
    {
        HidePopup();
        _requestAction.Invoke(_inputField.text);
    }

    private void InputValidation(string value)
    {
        if (!int.TryParse(value, out int result))
        {
            _inputField.text = "";
        }
    }
    
    private void Start()
    {
        _inputField.onValueChanged.AddListener(InputValidation);

        _applyButton.onClick.AddListener(OnApply);
        _cancelButton.onClick.AddListener(HidePopup);
    }

    private void OnDestroy()
    {
        _inputField.onValueChanged.RemoveListener(InputValidation);

        _applyButton.onClick.RemoveListener(OnApply);
        _cancelButton.onClick.RemoveListener(HidePopup);
    }
}
