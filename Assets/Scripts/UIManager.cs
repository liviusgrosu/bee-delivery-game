using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public TextMeshProUGUI PickUpText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI DeliveredPackagesText;

    public Transform PoiParent;
    public Dictionary<string, RectTransform> PoiList;

    public Transform JobsParent;

    public enum MenuState
    {
        InGame,
        JobList
    }
    
    public MenuState CurrentMenu = MenuState.InGame;

    [SerializeField] private GameObject _inGameMenu;
    [SerializeField] private GameObject _jobListMenu;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        PoiList = new Dictionary<string, RectTransform>();
    }

    private void Start()
    {
        foreach (var poi in PoiParent.GetComponentsInChildren<RectTransform>())
        {
            PoiList.Add(poi.name, poi);
        }

        foreach (var JobUI in JobsParent.GetComponentsInChildren<OrderUI>())
        {
            JobUI.Init();
        }
        
        _inGameMenu.SetActive(true);
        _jobListMenu.SetActive(false);
        CurrentMenu = MenuState.InGame;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (CurrentMenu)
            {
                case  MenuState.InGame:
                    _inGameMenu.SetActive(false); 
                    _jobListMenu.SetActive(true);
                    CurrentMenu = MenuState.JobList;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case  MenuState.JobList:
                    _inGameMenu.SetActive(true);
                    _jobListMenu.SetActive(false);
                    CurrentMenu = MenuState.InGame;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
            }
        }
    }
}
