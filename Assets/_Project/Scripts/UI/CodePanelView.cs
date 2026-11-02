using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Codebound.UI
{
    // One text object per code line so a single line can be highlighted.
    public class CodePanelView : MonoBehaviour
    {
        static readonly Color Off = new Color(1f, 1f, 1f, 0f);
        static readonly Color On = new Color(1f, 0.8f, 0.4f, 0.35f);

        [SerializeField] GameObject[] _rows;
        [SerializeField] TMP_Text[] _texts;
        [SerializeField] Image[] _highlights;

        public int RowCount => _rows == null ? 0 : _rows.Length;

        public void SetCode(IReadOnlyList<string> lines)
        {
            for (var i = 0; i < _rows.Length; i++)
            {
                var used = i < lines.Count;
                _rows[i].SetActive(used);
                if (used) _texts[i].text = $"{i + 1}  {lines[i]}";
                _highlights[i].color = Off;
            }
        }

        // line is 1-based; 0 clears the highlight.
        public void Highlight(int line)
        {
            for (var i = 0; i < _highlights.Length; i++)
                _highlights[i].color = i == line - 1 ? On : Off;
        }
    }
}
