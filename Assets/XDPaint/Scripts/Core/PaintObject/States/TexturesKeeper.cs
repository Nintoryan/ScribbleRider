using System;
using System.Collections.Generic;
using UnityEngine;
using XDPaint.Tools;

namespace XDPaint.Core.PaintObject.States
{
    public class TexturesKeeper
    {
        public Action OnChangeState;
        public Action OnResetState;
        public Action<RenderTexture> OnMouseUp;
        public Action OnReDraw;
        public Action OnUndo;
        public Action OnRedo;

        private List<RenderTexture> _textures = new List<RenderTexture>();
        private Action<RenderTexture> _extraDraw;
        private int _currentStateIndex;
        private bool _isEnabled;
        private bool _lockOnFirstTexture;

        public void Init(Action<RenderTexture> extraDraw, bool enabled)
        {
            _isEnabled = enabled;
            _extraDraw = extraDraw;
            OnMouseUp = texture =>
            {
                if (!_isEnabled)
                    return;
                
                if (_textures.Count > 0)
                {
                    for (var i = _textures.Count - 1; i >= _currentStateIndex; i--)
                    {
                        var element = _textures[i];
                        RenderTexture.ReleaseTemporary(element);
                        _textures.RemoveAt(i);
                    }

                    _lockOnFirstTexture = _currentStateIndex + 1 > Settings.Instance.UndoRedoMaxActionsCount;
                    if (_lockOnFirstTexture && _textures.Count >= Settings.Instance.UndoRedoMaxActionsCount + 1)
                    {
                        RenderTexture.ReleaseTemporary(_textures[0]);
                        _textures.RemoveAt(0);
                    }
                }
                _textures.Add(texture);
                _currentStateIndex = _textures.Count;
            };
        }

        /// <summary>
        /// Undo
        /// </summary>
        public void Undo()
        {
            if (!_isEnabled)
                return;

            var newIndex = _currentStateIndex - 2;
            var lockedTextureIndex = _lockOnFirstTexture ? 0 : 1;
            if (newIndex + lockedTextureIndex >= 0)
            {
                OnResetState();
            }
            OnChangeState();
            OnReDraw = () =>
            {
                RenderTexture texture = null;
                if (newIndex >= 0)
                {
                    texture = _textures[newIndex];
                }
                _extraDraw(texture);
            };
            _currentStateIndex--;
            if (_currentStateIndex < 0)
            {
                _currentStateIndex = 0;
            }

            if (OnUndo != null)
            {
                OnUndo();
            }
        }

        /// <summary>
        /// Redo
        /// </summary>
        public void Redo()
        {
            if (!_isEnabled)
                return;
            
            OnChangeState();
            _currentStateIndex++;
            var newIndex = _currentStateIndex - 1;
            if (newIndex >= 0)
            {
                OnReDraw = () => _extraDraw(_textures[newIndex]);
            }
            if (_currentStateIndex > _textures.Count)
            {
                _currentStateIndex = _textures.Count;
            }
            
            if (OnRedo != null)
            {
                OnRedo();
            }
        }

        /// <summary>
        /// Removes all saved Textures from TexturesKeeper
        public void Reset()
        {
            if (!_isEnabled)
                return;
            
            _currentStateIndex = 0;
            foreach (var state in _textures)
            {
                RenderTexture.ReleaseTemporary(state);
                state.DiscardContents();
            }
            _textures.Clear();
        }

        /// <summary>
        /// Returns if can Undo
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            var minimalIndex = _lockOnFirstTexture ? 1 : 0;
            return _textures.Count > 0 && _currentStateIndex > minimalIndex;
        }

        /// <summary>
        /// Returns if can Redo
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return _textures.Count > 0 && _currentStateIndex < _textures.Count;
        }
    }
}