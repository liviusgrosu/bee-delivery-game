using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private OrderData _orderData;

    [SerializeField]
    private TextMeshProUGUI _dropOffText;
    [SerializeField]
    private TextMeshProUGUI _pickUpText;
    [SerializeField]
    private TextMeshProUGUI _payText;
    [SerializeField]
    private TextMeshProUGUI _weightText;
    
    public Button StartButton;
    public TextMeshProUGUI StartButtonText;
    
    public RectTransform DropOffSymbol;
    public RectTransform PickUpSymbol;
    
    private bool _inProgress = false;
    private bool _completed = false;

    private void Start()
    {
        GameManager.JobInProgress += ToggleJob;
    }

    public void Init()
    {
        _dropOffText.text = $"Drop Off: {_orderData.Dropoff}";
        _pickUpText.text = $"Pick Up: {_orderData.Pickup}";
        _payText.text = $"Pay: ${_orderData.Pay}";
        _weightText.text = $"Weight: {_orderData.Weight} Kg";
        
        var dropOffPos = UIManager.Instance.PoiList[_orderData.Dropoff];
        var pickUpPos = UIManager.Instance.PoiList[_orderData.Pickup];
        
        DropOffSymbol.position = dropOffPos.position;
        PickUpSymbol.position = pickUpPos.position;
        
        DropOffSymbol.gameObject.SetActive(false);
        PickUpSymbol.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_inProgress)
        {
            return;
        }
        
        DropOffSymbol.gameObject.SetActive(true);
        PickUpSymbol.gameObject.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_inProgress)
        {
            return;
        }
        
        DropOffSymbol.gameObject.SetActive(false);
        PickUpSymbol.gameObject.SetActive(false);
    }

    public void TakeJob()
    {
        _inProgress = true;
        
        DropOffSymbol.gameObject.SetActive(true);
        PickUpSymbol.gameObject.SetActive(true);
        
        StartButton.interactable = false;
        StartButtonText.text = "Delivering";
        
        GameManager.Instance.AssignJob(
            _orderData.Pickup, 
            _orderData.Dropoff, 
            _orderData.Weight, 
            _orderData.Pay,
            FinishedJob);
    }

    public void FinishedJob()
    {
        StartButtonText.text = "Completed";
        _inProgress = false;
        _completed = true;
    }

    public void ToggleJob(bool state)
    {
        if (_completed || _inProgress)
        {
            return;
        }
        
        StartButton.interactable = state;
        StartButtonText.text = state ? "Start" : "Unavailable";
    }
}
