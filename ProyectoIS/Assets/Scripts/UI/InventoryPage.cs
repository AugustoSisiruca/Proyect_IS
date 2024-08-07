using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItem itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private InventoryDescription itemDescription;

    [SerializeField]
    private MouseFollower mouseFollower;

    List<UIInventoryItem> listOfItems = new List<UIInventoryItem>();

    private int intcurrentlyDraggedIndex = -1;
    public event Action<int> OnDescriptionRequested,
                OnItemActionRequested,
                OnStartDragging;

    public event Action<int, int> OnSwapItems;
    [SerializeField]
    private ItemActionPanel actionPanel;

    public void UpdateData(int itemIndex,
            Sprite itemImage, int itemQuantity)
    {
        if (itemImage == null)
    {
        Debug.LogWarning("Item image is null at index " + itemIndex);
        return;
    }
        if (listOfItems.Count > itemIndex)
        {
            listOfItems[itemIndex].SetData(itemImage, itemQuantity);
        }
    }
    private void Awake()
    {

        itemDescription.ResetDescription();
        mouseFollower.Toggle(false);
    }

    public void InitilizeInventoryUi(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel, false);
            uiItem.transform.localScale = new Vector3(1, 1, 1);
            uiItem.transform.SetParent(contentPanel);
            listOfItems.Add(uiItem);
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleItemSelection;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            uiItem.OnItemClicked += HandleShowItemActions;
        }
    }

    private void HandleShowItemActions(UIInventoryItem InventoryItemUI)
    {
        int index = listOfItems.IndexOf(InventoryItemUI);
        if (index == -1)
            return;
        OnItemActionRequested?.Invoke(index);
    }
     public void AddAction(string actionName, Action performAction)
        {
            actionPanel.AddButton(actionName, performAction);
        }
    public void ShowItemAction(int itemIndex)
    {
        actionPanel.Toggle(true);
        actionPanel.transform.position = listOfItems[itemIndex].transform.position;
    }

    private void HandleEndDrag(UIInventoryItem InventoryItemUI)
    {
        mouseFollower.Toggle(false);
    }

    private void HandleSwap(UIInventoryItem InventoryItemUI)
    {
        int index = listOfItems.IndexOf(InventoryItemUI);
        if (index == -1)
        {
            return;
        }
        OnSwapItems?.Invoke(intcurrentlyDraggedIndex, index);
        HandleItemSelection(InventoryItemUI);

    }

    private void HandleBeginDrag(UIInventoryItem InventoryItemUI)
    {
        int index = listOfItems.IndexOf(InventoryItemUI);
        if (index == -1)
            return;
        intcurrentlyDraggedIndex = index;
        HandleItemSelection(InventoryItemUI);
        OnStartDragging?.Invoke(index);

        //mouseFollower.SetData(image, quantity);
    }


    private void HandleItemSelection(UIInventoryItem InventoryItemUI)
    {
        int index = listOfItems.IndexOf(InventoryItemUI);
        if (index == -1)
            return;
        OnDescriptionRequested?.Invoke(index);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        itemDescription.ResetDescription();
        ResetSelections();
    }

    public void ResetSelections()
    {
        itemDescription.ResetDescription();
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach (UIInventoryItem item in listOfItems)
        {
            item.Deselect();
        }
        actionPanel.Toggle(false);
    }

    internal void ResetAllItems()
    {
        foreach (var item in listOfItems)
        {
            item.ResetData();
            item.Deselect();
        }
    }
    internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
    {
        itemDescription.SetDescription(itemImage, name, description);
        DeselectAllItems();
        listOfItems[itemIndex].Select();
    }

    public void ResetSelection()
    {
        itemDescription.ResetDescription();
        DeselectAllItems();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ResetDraggedItem();
        actionPanel.Toggle(false);
    }

    public void CreateDraggedItem(Sprite sprite, int quantity)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }
    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        intcurrentlyDraggedIndex = -1;
    }
}
