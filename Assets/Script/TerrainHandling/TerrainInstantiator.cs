using UnityEngine;

namespace TerrainHandling
{
    // Instantiates terrain data on awake so that runtime changes aren't saved to the terrain asset instance.
    [RequireComponent(typeof(UnityEngine.Terrain))]
    public class TerrainInstantiator : MonoBehaviour
    {
        private UnityEngine.Terrain _terrain;

        public void Awake()
        {
            _terrain = GetComponent<Terrain>();
            _terrain.terrainData = Instantiate(_terrain.terrainData);
        }
    }
}