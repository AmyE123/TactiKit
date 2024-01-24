namespace CT6GAMAI
{
    using TMPro;
    using UnityEngine;

    public class UI_TileInfoManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _terrainValueText;
        [SerializeField] private TMP_Text _defenseValueText;
        [SerializeField] private TMP_Text _movementValueText;
        [SerializeField] private TMP_Text _healthBuffValueText;

        private NodeData _previousNodeData;
        public NodeData ActiveNodeData;        

        private void RefreshTileInfoUI()
        {
            TerrainData activeTerrainData = ActiveNodeData.TerrainType;

            if(_previousNodeData != ActiveNodeData)
            {
                _previousNodeData = ActiveNodeData;
                //_terrainValueText.text = ActiveTerrainData.TerrainType.ToString();

                _terrainValueText.text = ActiveNodeData.name.ToString();

                if (activeTerrainData.TerrainType == Constants.Terrain.Unwalkable)
                {                    
                    _defenseValueText.text = "X";
                    _movementValueText.text = "X";
                    _healthBuffValueText.text = "X";
                }
                else
                {
                    _defenseValueText.text = activeTerrainData.DefenseBoost.ToString();
                    _movementValueText.text = activeTerrainData.MovementCost.ToString();
                    _healthBuffValueText.text = activeTerrainData.HealPercentageBoost.ToString() + "%";
                }
            }
        }

        public void SetTerrainType(NodeData data)
        {
            ActiveNodeData = data;   
            RefreshTileInfoUI();
        }
    }
}