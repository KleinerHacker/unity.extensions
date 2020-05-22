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
            }
        }

        #endregion

        protected abstract void OnModelChanged();
    }
}