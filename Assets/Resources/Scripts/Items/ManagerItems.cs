﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Resources.Scripts.Items
{
    public class ManagerItems : MonoBehaviour
    {
        [Header("Components")] [SerializeField]
        private ListView listViewItems;

        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private TextMeshProUGUI countItems;
        [SerializeField] private TextMeshProUGUI buttonSellText;
        private List<IItem> _listElements;

        private static int sumSelectedItems;

        private void Start()
        {
            _listElements = new List<IItem>();
            sumSelectedItems = 0;
        }

        public void FieldGeneration()
        {
            DeleteItems();
            listViewItems.SetDefaultSizeContent();
            _listElements = User.GetItems();
            countItems.text = "Предметов: " + _listElements.Count;
            foreach (var item in _listElements)
            {
                GameObject element = listViewItems.Add(itemPrefab);
                ListElement elementMeta = element.GetComponent<ListElement>();
                elementMeta.SetTitle(item.GetName());
                elementMeta.SetMainImage(item.GetMainImage());
                elementMeta.SetPriceImage(item.GetTypePriceImage());
                elementMeta.SetBackgroundImage(item.GetBackgroundImage());
                elementMeta.SetPrice(item.GetTypePrice(), item.GetPrice());
                Button actionButton = elementMeta.GetActionButton();
                actionButton.onClick.AddListener(() =>
                {
                    if (item.GetState())
                    {
                        item.SetState(false);
                        actionButton.image.color = new Color(178f, 184f, 195f, 0.2f);
                        sumSelectedItems += item.GetPrice();
                    }
                    else
                    {
                        item.SetState(true);
                        actionButton.image.color = new Color(178f, 184f, 195f, 0f);
                        sumSelectedItems -= item.GetPrice();
                    }

                    buttonSellText.text = "Продать " + sumSelectedItems;
                });
            }
        }

        public void OpenOtherScreen()
        {
            foreach (var item in _listElements)
            {
                item.SetState(true);
            }

            sumSelectedItems = 0;
        }

        private void DeleteItems()
        {
            _listElements = new List<IItem>(User.GetItems());
            foreach (var item in _listElements.Where(item => !item.GetState()))
            {
                User.RemoveItem(item);
            }

            _listElements.Clear();
            listViewItems.DestroyObjects();
        }

        public void ClickOnButton()
        {
            buttonSellText.text = "Продать";
            User.AddMoney(sumSelectedItems);
            ManagerEvent.ActivateChangeMoney();
            sumSelectedItems = 0;
            FieldGeneration();
        }
    }
}