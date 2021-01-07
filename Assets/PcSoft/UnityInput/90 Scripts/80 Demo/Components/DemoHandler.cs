using PcSoft.UnityInput._90_Scripts._00_Runtime;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Types;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions;
using UnityEngine;

namespace PcSoft.UnityInput._90_Scripts._80_Demo.Components
{
    [AddComponentMenu(UnityInputConstants.Root + "/Demo Handler")]
    [DisallowMultipleComponent]
    public class DemoHandler : MonoBehaviour
    {
        #region Inspector Data

        [SerializeField]
        private InputActionReference mouseClickRreference;

        [SerializeField]
        private InputActionReference mouseMoveReference;

        [SerializeField]
        private InputActionReference cheatKeyReference;

        #endregion

        private InputAction _mouseClick;
        private InputAction _mouseMove;
        private InputAction _cheatKey;

        #region Builtin Methods

        private void Awake()
        {
            _mouseClick = mouseClickRreference.ToInputAction();
            _mouseMove = mouseMoveReference.ToInputAction();
            _cheatKey = cheatKeyReference.ToInputAction();
        }

        private void OnEnable()
        {
            _mouseMove.Performed += MouseMoveOnPerformed;
            _mouseClick.Performed += MouseClickOnPerformed;
            _cheatKey.Performed += CheatKeyOnPerformed;
        }

        private void OnDisable()
        {
            _mouseMove.Performed -= MouseMoveOnPerformed;
            _mouseClick.Performed -= MouseClickOnPerformed;
            _cheatKey.Performed -= CheatKeyOnPerformed;
        }

        #endregion

        private void MouseMoveOnPerformed(InputActionContext obj)
        {
            Debug.Log("Mouse Move: " + obj.ReadValue<Vector2>());
        }

        private void MouseClickOnPerformed(InputActionContext obj)
        {
            Debug.Log("Mouse Click: " + obj.ReadValue<bool>());
        }

        private void CheatKeyOnPerformed(InputActionContext obj)
        {
            Debug.Log("Cheat Key pressed");
        }
    }
}
