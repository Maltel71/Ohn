using System.Collections.Generic;
using UnityEngine;

namespace Construction
{
    [CreateAssetMenu(menuName = "BlockCollection")]
    public class BlockCollection : ScriptableObject
    {
        [field: SerializeField] public List<GameObject> Blocks { get; set; }
    }
}
