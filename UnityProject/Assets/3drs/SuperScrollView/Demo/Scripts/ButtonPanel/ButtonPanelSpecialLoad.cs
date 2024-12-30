﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ButtonPanelSpecialLoad
    {
        public LoopListView2 mLoopListView;
        public DataSourceMgr<ItemData> mDataSourceMgr;
        public int mItemCountPerRow = 3;
        public int mExtraHeaderItemCount = 0;
        public int mExtraFooterItemCount = 0;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mAddButton;
        InputField mAddInput;
        Button mBackButton;        

        public void Start()
        {
            mSetCountButton = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountButton").GetComponent<Button>();
            mSetCountInput = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountInputField").GetComponent<InputField>();
            mSetCountButton.onClick.AddListener(OnSetCountButtonClicked);
            
            mScrollToButton = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnScrollToButtonClicked);

            mAddButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddButton.onClick.AddListener(OnAddButtonClicked);
            mAddInput = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddInputField").GetComponent<InputField>();

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }      

        void OnSetCountButtonClicked()
        {
            int count = 0;
            if (int.TryParse(mSetCountInput.text, out count) == false)
            {
                return;
            }
            if (count < 0)
            {
                return;
            }

            int tmpRow = GetRow(count);                 
            mDataSourceMgr.SetDataTotalCount(count); 
            int extraCount = mExtraHeaderItemCount + mExtraFooterItemCount;    
            mLoopListView.SetListItemCount(tmpRow+extraCount, false);
            mLoopListView.RefreshAllShownItem();            
        }  

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if((itemIndex < 0) || (itemIndex >= mDataSourceMgr.TotalItemCount))
            {
                return;
            }
            int tmpRowIndex = GetRowIndex(itemIndex+1);
            mLoopListView.MovePanelToItemIndex(tmpRowIndex+mExtraHeaderItemCount, 0);
        }        

        void OnAddButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mAddInput.text, out itemIndex) == false)
            {
                return;
            }
            if((itemIndex < 0) || (itemIndex > mDataSourceMgr.TotalItemCount))
            {
                return;
            }             
            ItemData newData = mDataSourceMgr.InsertData(itemIndex);  
            newData.mDesc = newData.mDesc +" [New]";
            int tmpRow = GetRow(mDataSourceMgr.TotalItemCount);
            int extraCount = mExtraHeaderItemCount + mExtraFooterItemCount;
            mLoopListView.SetListItemCount(tmpRow+extraCount, false);
            mLoopListView.RefreshAllShownItem();               
        }     

        int GetRow(int itemCount)
        {
            int tmpRow = itemCount / mItemCountPerRow;
            if (itemCount % mItemCountPerRow > 0)
            {
                tmpRow++;
            }
            return tmpRow;
        }

        int GetRowIndex(int itemCount)
        {
            int tmpRow = GetRow(itemCount);
            if (tmpRow > 0)
            {
                tmpRow--;
            }
            return tmpRow;
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }         
    }
}