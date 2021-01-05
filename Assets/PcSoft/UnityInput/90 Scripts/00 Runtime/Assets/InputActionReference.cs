using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Extra;
using UnityEngine;

namespace PcSoft.UnityInput._90_Scripts._00_Runtime.Assets
{
    [CreateAssetMenu(menuName = UnityInputConstants.Root + "/Input Action Reference", fileName = "InputAction")]
    public sealed class InputActionReference : ScriptableObject
    {
        #region Inspector Data

        [SerializeField]
        [ReadOnly]
        private string guid = System.Guid.NewGuid().ToString();

        #endregion

        #region Properties

        public string Guid => guid;

        #endregion

        private bool Equals(InputActionReference other)
        {
            return base.Equals(other) && guid == other.guid;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is InputActionReference other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (guid != null ? guid.GetHashCode() : 0);
            }
        }
    }
}