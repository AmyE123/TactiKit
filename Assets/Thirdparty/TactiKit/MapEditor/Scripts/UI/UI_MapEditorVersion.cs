namespace TactiKit.MapEditor
{
    using TMPro;
    using UnityEngine;

    /// <summary>
    /// A class for handling titles & version numbers in the Map Editor.
    /// </summary>
    public class UI_MapEditorVersion : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TextAsset _versionFile;

        private void Start()
        {
            _titleText.text = _versionFile.text;
        }
    }
}