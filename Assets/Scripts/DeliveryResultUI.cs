using System;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Globalization;

public class DeliveryResultUI : MonoBehaviour
{
    public static DeliveryResultUI Instance;
    
    public float animationSpeed = 2f;
    public TextMeshProUGUI CostLabelText, CostValueText;
    public TextMeshProUGUI TipLabelText, TipValueText;
    public TextMeshProUGUI TotalLabelText, TotalValueText;
    public GameObject LineBreak;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void Display(float cost, float tip)
    {
        StopAllCoroutines();
        StartCoroutine(PlayAnimation(cost, tip));
    }

    private IEnumerator PlayAnimation(float cost, float tip)
    {
        CostLabelText.gameObject.SetActive(true);
        CostValueText.gameObject.SetActive(true);
        CostValueText.text = cost.ToString("C");
        
        yield return new WaitForSeconds(0.5f);
        
        TipLabelText.gameObject.SetActive(true);
        TipValueText.gameObject.SetActive(true);
        TipValueText.text = tip.ToString("C");
        
        yield return new WaitForSeconds(0.5f);
        
        LineBreak.gameObject.SetActive(true);
        TotalLabelText.gameObject.SetActive(true);
        TotalValueText.gameObject.SetActive(true);
        TotalValueText.text = (cost + tip).ToString("C");
        
        yield return new WaitForSeconds(0.5f);

        while (UIManager.Instance.CurrentUIPay < GameManager.Instance.Paid)
        {
            UIManager.Instance.CurrentUIPay += Time.deltaTime * animationSpeed;
            yield return null;
        }

        UIManager.Instance.CurrentUIPay = GameManager.Instance.Paid;
        
        yield return new WaitForSeconds(1.5f);
        
        CostLabelText.gameObject.SetActive(false);
        CostValueText.gameObject.SetActive(false);
        TipLabelText.gameObject.SetActive(false);
        TipValueText.gameObject.SetActive(false);
        TotalLabelText.gameObject.SetActive(false);
        TotalValueText.gameObject.SetActive(false);
        LineBreak.gameObject.SetActive(false);
    }
}
