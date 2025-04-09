using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable LocalVariableHidesMember

namespace Construction
{
    public class Builder : MonoBehaviour
    {
        private Camera _camera;
        private GameObject _cursorBlock;
        private Material _cachedMaterial;
        private GameObject _highlightedBlock;
        private int _selectedBlockIndex;

        public bool Deconstructing { get; set; }

        [field: SerializeField] private BlockCollection BlockCollection { get; set; }

        [field: SerializeField] private Material ConstructMaterial { get; set; }

        [field: SerializeField] private Material DeconstructMaterial { get; set; }

        private GameObject SelectedBlock { get; set; }

        private bool BuildModeEnabled { get; set; }

        public void Start()
        {
            _camera = Camera.main;
            SelectedBlock = BlockCollection.Blocks[_selectedBlockIndex];
        }

        public void Update()
        {
            if (!BuildModeEnabled)
                return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit))
                return;

            if (Deconstructing)
            {
                if (_cursorBlock)
                    _cursorBlock.SetActive(false);

                HighlightHoveredBlock(hit);
            }
            else
            {
                if (_highlightedBlock)
                    _highlightedBlock.GetComponent<MeshRenderer>().material = _cachedMaterial;

                DrawCursorBlock(hit);
            }
        }

        public void HighlightHoveredBlock(RaycastHit hit)
        {
            if (!hit.transform.TryGetComponent(out BuildingBlock buildingBlock) ||
                _highlightedBlock == buildingBlock.gameObject)
                return;

            if (_highlightedBlock)
                _highlightedBlock.GetComponent<MeshRenderer>().material = _cachedMaterial;

            var newBlockRenderer = buildingBlock.GetComponent<MeshRenderer>();
            _cachedMaterial = newBlockRenderer.material;
            newBlockRenderer.material = DeconstructMaterial;
            _highlightedBlock = buildingBlock.gameObject;
        }

        public void DrawCursorBlock(RaycastHit hit)
        {
            if (!_cursorBlock)
                InstantiateCursorBlock();

            _cursorBlock.SetActive(true);

            var type = _cursorBlock.GetComponent<BuildingBlock>().Type;
            var positionAndRotation = RayToGridPosition(hit, type);

            _cursorBlock.transform.position = positionAndRotation.Item1;
            _cursorBlock.transform.rotation = positionAndRotation.Item2;
        }

        public void InstantiateCursorBlock()
        {
            _cursorBlock = Place(SelectedBlock, at: Input.mousePosition);

            if (_cursorBlock.TryGetComponent(out Collider collider))
                collider.enabled = false;

            if (!_cursorBlock.TryGetComponent(out MeshRenderer meshRenderer))
                return;

            meshRenderer.material = ConstructMaterial;
        }

        public GameObject Place(Vector3 at)
        {
            return Place(SelectedBlock, at);
        }

        public GameObject Place(GameObject prefab, Vector3 at)
        {
            if (!BuildModeEnabled)
                return null;

            var ray = _camera.ScreenPointToRay(at);

            if (!Physics.Raycast(ray, out var hit))
                return null;

            if (!Deconstructing && prefab.TryGetComponent(out BuildingBlock blockToConstruct))
            {
                var positionAndRotation = RayToGridPosition(hit, blockToConstruct.Type);
                return Instantiate(prefab, positionAndRotation.Item1, positionAndRotation.Item2);
            }

            if (hit.transform.TryGetComponent(out BuildingBlock blockToRemove))
                Destroy(blockToRemove.gameObject);

            return null;
        }

        private static (Vector3, Quaternion) RayToGridPosition(RaycastHit hit, BlockType blockType)
        {
            return blockType switch {
                BlockType.Cube => RayToGridPositionCube(hit),
                BlockType.Floor => RayToGridPositionFloor(hit),
                BlockType.Wall => RayToGridPositionWall(hit),
                _ => (Vector3.zero, Quaternion.identity)
            };
        }

        private static (Vector3, Quaternion) RayToGridPositionCube(RaycastHit hit)
        {
            var worldPosPlusNormal = hit.point + hit.normal * 0.1f;

            var flooredPosition = new Vector3(
                Mathf.Floor(worldPosPlusNormal.x),
                Mathf.Floor(worldPosPlusNormal.y),
                Mathf.Floor(worldPosPlusNormal.z));

            var centeredPosition = flooredPosition + new Vector3(0.5f, 0.5f, 0.5f);

            return (centeredPosition, Quaternion.identity);
        }

        private static (Vector3, Quaternion) RayToGridPositionFloor(RaycastHit hit)
        {
            var worldPosPlusNormal = hit.point + hit.normal * 0.1f;

            var flooredPosition = new Vector3(
                Mathf.Floor(worldPosPlusNormal.x),
                Mathf.Floor(worldPosPlusNormal.y),
                Mathf.Floor(worldPosPlusNormal.z));

            var centeredPosition = flooredPosition + new Vector3(0.5f, 0.1f, 0.5f);

            return (centeredPosition, Quaternion.identity);
        }

        private static (Vector3, Quaternion) RayToGridPositionWall(RaycastHit hit)
        {
            var worldPosPlusNormal = hit.point + hit.normal * 0.1f;

            var flooredPosition = new Vector3(
                Mathf.Floor(worldPosPlusNormal.x),
                Mathf.Floor(worldPosPlusNormal.y + 1),
                Mathf.Floor(worldPosPlusNormal.z));

            var edgeCenters = new Vector3[4] {
                new(flooredPosition.x + 0.5f, flooredPosition.y, flooredPosition.z),
                new(flooredPosition.x + 1f, flooredPosition.y, flooredPosition.z + 0.5f),
                new(flooredPosition.x + 0.5f, flooredPosition.y, flooredPosition.z + 1f),
                new(flooredPosition.x, flooredPosition.y, flooredPosition.z + 0.5f)
            };

            var nearestEdgePos = edgeCenters.OrderBy(e => Vector3.Distance(hit.point, e)).First();
            var rotation = Quaternion.identity;

            if (Mathf.Approximately(nearestEdgePos.z, flooredPosition.z + 0.5f))
                rotation = Quaternion.Euler(0, 90, 0);

            return (nearestEdgePos, rotation);
        }

        public void ToggleBuildMode()
        {
            BuildModeEnabled = !BuildModeEnabled;

            if (BuildModeEnabled)
                return;

            if (_cursorBlock)
                Destroy(_cursorBlock);

            if (!_highlightedBlock)
                return;

            _highlightedBlock.GetComponent<MeshRenderer>().material = _cachedMaterial;
            _highlightedBlock = null;
        }

        public void CycleBlocks(int i)
        {
            if (!BuildModeEnabled)
                return;

            i = Math.Clamp(i, -1, 1);

            _selectedBlockIndex += i;

            if (_selectedBlockIndex > BlockCollection.Blocks.Count - 1)
                _selectedBlockIndex = 0;

            if (_selectedBlockIndex < 0)
                _selectedBlockIndex = BlockCollection.Blocks.Count - 1;

            SelectedBlock = BlockCollection.Blocks[_selectedBlockIndex];

            if (_cursorBlock)
                Destroy(_cursorBlock);

            InstantiateCursorBlock();
        }
    }
}