namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Window
{
    public interface IViewable
    {
        ViewableState State { get; }
        
        void Show();
        void Hide();
    }
    
    public enum ViewableState
    {
        Shown,
        Hidden,
    }
}