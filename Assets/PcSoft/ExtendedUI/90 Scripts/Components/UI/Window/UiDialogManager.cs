using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Window
{
    [AddComponentMenu(ExtendedUIConstants.Menus.Components.Ui.WindowMenu + "/Dialog Manager")]
    [DisallowMultipleComponent]
    public sealed class UiDialogManager : UIBehaviour
    {
        #region Static Area

        public static UiDialogManager Singleton => Resources.FindObjectsOfTypeAll<UiDialogManager>()[0];

        #endregion
        
        #region Builtin Methods

        private void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            #else
            if (Input.GetButtonDown("Cancel"))
            #endif
            {
                Debug.Log("Escape action");
                
                foreach (var uiDialog in Resources.FindObjectsOfTypeAll<UiDialog>())
                {
                    switch (uiDialog.EscapeAction)
                    {
                        case DialogEscapeAction.None:
                            break;
                        case DialogEscapeAction.Toggle:
                            Debug.Log("Toggle dialog (escape)", uiDialog);
                            if (uiDialog.State == DialogState.Shown)
                                uiDialog.Hide();
                            else 
                                uiDialog.Show();
                            break;
                        case DialogEscapeAction.HideOnly:
                            if (uiDialog.State == DialogState.Shown)
                            {
                                Debug.Log("Hide dialog (escape)", uiDialog);
                                uiDialog.Hide();
                            }

                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion
    }
}