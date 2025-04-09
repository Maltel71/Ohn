using UnityEngine;

namespace Construction
{
    public class BuildingBlock : MonoBehaviour
    {
        [field: SerializeField] public BlockType Type { get; private set; }
    }
}