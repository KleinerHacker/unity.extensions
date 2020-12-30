using PcSoft.ExtendedAnimation._90_Scripts._00_Runtime.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PcSoft.UnityGameTooling._90_Scripts._00_Runtime.Components.Cameras
{
    [AddComponentMenu(UnityGameToolingConstants.Menu.Component.CameraMenu + "/Flying Panning Zoom Camera Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class FlyingPanningZoomCameraController : MonoBehaviour
    {
        #region Inspector Data

        [Header("Behavior")]
        [FormerlySerializedAs("maxHigh")]
        [SerializeField]
        private float relativeMaxHigh = 10f;

        [FormerlySerializedAs("minHigh")]
        [SerializeField]
        private float relativeMinHigh = 1f;

        [SerializeField]
        [Tooltip("If this field is TRUE camera uses raycast to find underlying colliders to place camera over it (relative high). If it is set to FALSE " +
                 "the given high is absolute. This option should be used on terrains")]
        private bool useHighDetection = true;

        [SerializeField]
        [Range(0f, 90f)]
        private float maxHighRotation = 45f;

        [SerializeField]
        [Range(0f, 90f)]
        private float minHighRotation = 10f;

        [Space]
        [FormerlySerializedAs("levelCount")]
        [SerializeField]
        private byte highLevelCount = 5;

        [FormerlySerializedAs("startLevel")]
        [SerializeField]
        private byte startHighLevel = 4;

        [Space]
        [Tooltip("Allow or forbid horizontal movement and rotation")]
        [SerializeField]
        private bool allowFreeMoving = true;

        [Space]
        [SerializeField]
        private CameraBorder border;

        [Space]
        [SerializeField]
        private LayerMask colliderLayerMask;

        [Header("Animation")]
        [SerializeField]
        private AnimationCurve highChangeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        [Range(0.01f, 1f)]
        private float highChangeSpeed = 0.1f;

        [Header("Input")]
        [SerializeField]
        private InputActionReference movementInputRef;

        [SerializeField]
        private InputActionReference scrollingInputRef;

        #endregion

        private InputAction _movementInput;
        private InputAction _scrollingInput;

        private byte _currentHighLevel;

        #region Builtin Methods

        private void Awake()
        {
            _movementInput = movementInputRef.ToInputAction();
            _scrollingInput = scrollingInputRef.ToInputAction();

            _currentHighLevel = startHighLevel;
            UpdateCameraHeight(true);
        }

        private void OnEnable()
        {
            _movementInput.performed += MovementInputPerformed;
            _movementInput.Enable();

            _scrollingInput.performed += ScrollingInputPerformed;
            _scrollingInput.Enable();
        }

        private void OnDisable()
        {
            _movementInput.Disable();
            _movementInput.performed -= MovementInputPerformed;

            _scrollingInput.Disable();
            _scrollingInput.performed -= ScrollingInputPerformed;
        }

        #endregion

        private void MovementInputPerformed(InputAction.CallbackContext e)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            var delta = -e.ReadValue<Vector2>();

            if (Equals(e.control.device, Mouse.current))
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    DoMovement(delta.x, delta.y);
                    return;
                }
                else if (Mouse.current.rightButton.isPressed)
                {
                    DoRotation(delta.x, delta.y);
                    return;
                }

                return;
            }

            DoMovement(delta.x, delta.y);
        }

        private void DoMovement(float deltaX, float deltaY)
        {
            var t = transform;
            var newPos = t.rotation * new Vector3(allowFreeMoving ? deltaX : 0f, 0f, deltaY) + t.position;
            if (border != null && !border.InBox(newPos))
                return;
            transform.position = new Vector3(newPos.x, CalculateCameraHeight(newPos) ?? newPos.y, newPos.z);
        }

        private void DoRotation(float deltaX, float deltaY)
        {
            if (!allowFreeMoving)
                return;
            
            var rot = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rot.x, rot.y - deltaX, rot.z);
        }

        private void ScrollingInputPerformed(InputAction.CallbackContext e)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            var value = e.ReadValue<float>();
            if (value < 0f && _currentHighLevel > 0)
            {
                _currentHighLevel--;
            }
            else if (value > 0f && _currentHighLevel < highLevelCount - 1)
            {
                _currentHighLevel++;
            }

            UpdateCameraHeight();
        }

        private void UpdateCameraHeight(bool immediately = false)
        {
            var rotationDif = maxHighRotation - minHighRotation;
            var rotation = minHighRotation + rotationDif / highLevelCount * _currentHighLevel;

            var t = transform;

            var startPosition = t.position;
            var targetPosition = new Vector3(startPosition.x, CalculateCameraHeight(startPosition) ?? startPosition.y, startPosition.z);

            var startRotation = t.rotation;
            var targetRotation = Quaternion.Euler(rotation, startRotation.eulerAngles.y, startRotation.eulerAngles.z);

            if (immediately)
            {
                t.position = targetPosition;
                t.rotation = targetRotation;

                return;
            }

            StopAllCoroutines();
            StartCoroutine(AnimationUtils.RunAnimation(highChangeCurve, highChangeSpeed,
                v =>
                {
                    transform.position = Vector3.Lerp(startPosition, targetPosition, v);
                    transform.rotation = Quaternion.Lerp(startRotation, targetRotation, v);
                }));
        }

        private float? RaycastCollisionHeight(Vector3 pos)
        {
            if (!useHighDetection)
                return 0f;
            
            if (Physics.Raycast(new Vector3(pos.x, 1000f, pos.z), Vector3.down, out var hit, float.MaxValue, colliderLayerMask) && hit.collider.CompareTag("Terrain"))
                return hit.point.y;

            return null;
        }

        private float? CalculateCameraHeight(Vector3 pos)
        {
            var highDif = relativeMaxHigh - relativeMinHigh;
            var high = relativeMinHigh + highDif / highLevelCount * _currentHighLevel;

            return RaycastCollisionHeight(pos) + high;
        }
    }
}