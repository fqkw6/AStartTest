﻿using UnityEngine;
using System.Collections;

namespace Lockstep
{
    public class SelectionRing : MonoBehaviour
    {
        [SerializeField]
        private Color _selectedColor = new Color (0f,.6f,0f);
        Color SelectedColor {get {return _selectedColor;}}

        [SerializeField]
        private Color _highlightedColor = new Color(0f,.7f,0f,.5f);
        Color HighlightedColor {get {return _highlightedColor;}}

        [SerializeField]
        private Color _noneColor = new Color(0,0,0,0);
        Color NoneColor {get {return _noneColor;}}

        Renderer cachedRenderer;

        public void Setup (float size) {
            this.OnSetup();
            this.SetSize(size);
            this.SetState(SelectionRingState.None);
        }
        protected virtual void OnSetup () {
            cachedRenderer = GetComponent<Renderer> ();
        }
        public void SetState (SelectionRingState state) {
            Color setColor = default(Color);
            switch (state) {
                case SelectionRingState.Selected:
                    setColor = SelectedColor;
                    break;
                case SelectionRingState.Highlighted:
                    setColor = HighlightedColor;
                    break;
                case SelectionRingState.None:
                    setColor = NoneColor;
                    break;
            }
            SetColor (setColor);
        }

        public virtual void SetColor (Color color) {
            cachedRenderer.material.color = color;

        }

        public virtual void SetSize (float size) {
            transform.localScale = Vector3.one * size;
        }


    }
}