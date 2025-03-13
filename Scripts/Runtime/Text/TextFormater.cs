using System;
using TMPro;
using UnityEngine;

namespace DSystemUtils.Text
{
    [AddComponentMenu("Utils/Text Formater")]
    [RequireComponent(typeof(TMP_Text))]
    public partial class TextFormater : MonoBehaviour
    {
        [field: SerializeField] protected string Format { get; private set; } = "{0}";
        
        private TMP_Text _tmpText;

        protected virtual void Awake()
        {
            if (_tmpText == null)
                _tmpText = GetComponent<TMP_Text>();
        }

        public virtual void SetFormat(string format)
        {
            Format = format;
        }

        public virtual void UpdateText(IFormattable formattable)
        {
            if (_tmpText == null)
                _tmpText = GetComponent<TMP_Text>();
            
            string text = string.Format(Format, formattable);
            
            _tmpText.text = text;
        }

        public virtual void UpdateText(params IFormattable[] formattables)
        {
            if (_tmpText == null)
                _tmpText = GetComponent<TMP_Text>();
            
            string text = string.Format(Format, formattables);
            
            _tmpText.text = text;
        }

        public virtual void UpdateText(string text)
        {
            _tmpText.text = text;
        }
    }
}