using System;
using UnityEngine.EventSystems;

namespace PcSoft.ExtendedUI._90_Scripts._00_Runtime.Components.UI.Component
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
                if (_model != null && isActiveAndEnabled)
                {
                    OnDetachModel(_model);
                }

                _model = value;

                if (_model != null && isActiveAndEnabled)
                {
                    OnAttachModel(_model);
                }

                FireModelChanged();
            }
        }

        #endregion

        #region Events

        public event EventHandler ModelChanged;

        #endregion

        #region Builtin Methods

        protected override void OnEnable()
        {
            if (Model != null)
            {
                OnAttachModel(Model);
            }
        }

        protected override void OnDisable()
        {
            if (Model != null)
            {
                OnDetachModel(Model);
            }
        }

        #endregion

        protected virtual void FireModelChanged()
        {
            ModelChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAttachModel(TM model)
        {
            //Empty
        }

        protected virtual void OnDetachModel(TM model)
        {
            //Empty
        }
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