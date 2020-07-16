using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Component
{
    public abstract class UiListItem<TM> : UIBehaviour
    {
        #region Properties

        private TM _model;

        public TM Model
        {
            get => _model;
            set
            {
                _model = value;

                OnModelChanged();
                FireModelChanged();
            }
        }

        #endregion

        #region Events

        public event EventHandler ModelChanged;

        #endregion

        protected virtual void FireModelChanged()
        {
            ModelChanged?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void OnModelChanged();
    }

    public abstract class UiSelectableListItem<TM> : UiListItem<TM>
    {
        #region Properties

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;

                OnSelectionChanged();
                FireSelectionChanged();
            }
        }

        #endregion

        #region Events

        public event EventHandler SelectionChanged;

        #endregion

        protected virtual void FireSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void OnSelectionChanged();
    }
}