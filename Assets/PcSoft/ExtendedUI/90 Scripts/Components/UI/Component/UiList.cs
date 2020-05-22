using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Component
{
    public abstract class UiList<TI, TM> : UIBehaviour where TI : UiListItem<TM>
    {
        #region Inspector Data

        [Header("Behavior")]
        private ListUpdatePolicy updatePolicy = ListUpdatePolicy.FillAndClear;

        [Header("References")]
        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private TI itemPrefab;

        #endregion

        #region Properties

        protected abstract TM[] ContentData { get; }

        #endregion

        private readonly IList<TI> _listItems = new List<TI>();

        #region Builtin Methods

        protected override void Awake()
        {
            switch (updatePolicy)
            {
                case ListUpdatePolicy.FillAndClear:
                case ListUpdatePolicy.Refresh:
                    break;
                case ListUpdatePolicy.OnlyStartup:
                    FillList();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void OnEnable()
        {
            switch (updatePolicy)
            {
                case ListUpdatePolicy.FillAndClear:
                    FillList();
                    break;
                case ListUpdatePolicy.Refresh:
                    Refresh();
                    break;
                case ListUpdatePolicy.OnlyStartup:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void OnDisable()
        {
            switch (updatePolicy)
            {
                case ListUpdatePolicy.FillAndClear:
                    ClearList();
                    break;
                case ListUpdatePolicy.Refresh:
                case ListUpdatePolicy.OnlyStartup:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        public void Refresh()
        {
            ClearList();
            FillList();
        }

        private void ClearList()
        {
            foreach (var listItem in _listItems)
            {
                Destroy(listItem.gameObject);
            }

            _listItems.Clear();
        }

        private void FillList()
        {
            foreach (var data in ContentData)
            {
                var go = Instantiate(itemPrefab.gameObject, Vector3.zero, Quaternion.identity);
                go.transform.parent = content;

                var listItem = go.GetComponent<TI>();
                listItem.Model = data;

                _listItems.Add(listItem);
            }
        }
    }

    public enum ListUpdatePolicy
    {
        FillAndClear,
        Refresh,
        OnlyStartup
    }
}