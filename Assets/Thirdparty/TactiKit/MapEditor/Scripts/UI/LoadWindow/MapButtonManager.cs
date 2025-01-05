namespace TactiKit.MapEditor
{
    using TMPro;
    using UnityEngine;

    /// <summary>
    /// A manager class for the map buttons that populate the load UI.
    /// This class is used to set the data up on each map button.
    /// </summary>
    public class MapButtonManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _mapTitle;
        [SerializeField] private TMP_Text _mapDate;
        [SerializeField] private string _mapFilePath;

        /// <summary>
        /// Set the map data for the button
        /// </summary>
        /// <param name="title">The title of the map.</param>
        /// <param name="date">The date that the map was last accessed.</param>
        public void SetMapData(string title, string date, string filepath)
        {
            _mapTitle.text = title;
            _mapDate.text = date;
            _mapFilePath = filepath;
        }
    }
}