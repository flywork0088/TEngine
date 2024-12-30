using System;
using GameLogic;
using SuperScrollView;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainView : MonoBehaviour
{
    public LoopGridView mLoopGridView;
    public Button btnSetting;

    private void Start()
    {
        mLoopGridView.InitGridView(15, OnGetItemByRowColumn);
        btnSetting.onClick.AddListener(OnSettingClick);
    }

    public void OnSettingClick()
    {
         GameModule.UI.ShowUIAsync<UISettingWindow>();
    }
    
    private LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index, int row, int column)
    {
        LoopGridViewItem item = gridView.NewListViewItem("ItemPrefab");
        HallGameItem itemScript = item.GetComponent<HallGameItem>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            itemScript.Init(OnItemClick); 
        }
          
        return item;
    }

    private void OnItemClick(HallGameItem gameItem)
    {
        
    }
}

