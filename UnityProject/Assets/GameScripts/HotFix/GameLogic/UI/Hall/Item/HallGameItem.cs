using HsCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HallGameItem : MonoBehaviour
{
    public TMP_Text txtName;   
    public HsButton btnLogo;
    public Image imgCollect;
    
    private int mItemDataIndex;
    private System.Action<HallGameItem> mOnClickItemCallBack;
   
    public void Init(System.Action<HallGameItem> OnClickItemCallBack = null)
    {
        mOnClickItemCallBack = OnClickItemCallBack;
        btnLogo.onClick.AddListener(OnButtonClick);           
    } 
  

    private void OnButtonClick()
    {
        if (mOnClickItemCallBack != null)
        {
            mOnClickItemCallBack(this);
        }
    }

  
    public void SetItemData(object itemData, int itemIndex)
    {
        mItemDataIndex = itemIndex;
    }
}
