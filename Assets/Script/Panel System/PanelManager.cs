using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PanelType
{
    None,
    Pause,
    Confirmation, 
    Tutorial
}

public class PanelManager : MonoBehaviour
{
    private static PanelManager _instance;

    [SerializeField] private Transform panelContainer;
    [SerializeField] private List<PanelBase> panels = new();
    
    private Dictionary<PanelType, PanelBase> _panelLookup = new();
    private PanelBase _activePanel;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        panels = panelContainer.GetComponentsInChildren<PanelBase>().ToList();

        foreach (var panelBase in panels)
        {
            _panelLookup[panelBase.PanelType] = panelBase;
        }
    }

    private void OnEnable()
    {
        GameEvents.OnRequestOpenPanel.AddListener(HandelOpenPanel);
        GameEvents.OnRequestClosePanel.AddListener(HandelClosePanel);
    }

    private void OnDisable()
    {
        OnRemoveListeners();
    }

    private void OnDestroy()
    {
        OnRemoveListeners();
    }

    private void OnRemoveListeners()
    {
        GameEvents.OnRequestOpenPanel.RemoveListener(HandelOpenPanel);
        GameEvents.OnRequestClosePanel.RemoveListener(HandelClosePanel);
    }

    private void HandelOpenPanel(PanelType panelType)
    {
        if (_activePanel != null) _activePanel.ClosePanel();
        _panelLookup[panelType].OpenPanel();
        _activePanel = _panelLookup[panelType];
    }

    private void HandelClosePanel(PanelType panelType)
    {
        _panelLookup[panelType].ClosePanel();
        if (_activePanel == _panelLookup[panelType]) _activePanel = null;
    }
}
