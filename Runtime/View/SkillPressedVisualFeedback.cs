using UnityEngine;
using UnityEngine.Tilemaps;
using Utils.Extensions;

namespace Match3.View
{
    public class SkillPressedVisualFeedback : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Tilemap _boardShapeTileMap;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _boardMask;
        [SerializeField] private RectTransform _buttonMask;

        private void Start()
        {
            Hide();
        }

        public void ShowFeedback(SkillView skillView)
        {
            // appear
            _canvas.gameObject.SetActive(true);
            
            // adjust button mask
            var rectTransform = skillView.GetComponent<RectTransform>();
            _buttonMask.position = rectTransform.position;
            _buttonMask.sizeDelta = rectTransform.sizeDelta;
            
            // adjust board mask
            var bounds = _boardShapeTileMap.WorldBounds();
            var viewportMin = _camera.WorldToViewportPoint(bounds.min);
            var viewportMax = _camera.WorldToViewportPoint(bounds.max);

            _boardMask.anchorMin = viewportMin;
            _boardMask.anchorMax = viewportMax;
            _boardMask.offsetMin = Vector2.zero;
            _boardMask.offsetMax = Vector2.zero;
        }

        public void Hide(SkillView skillView)
        {
            Hide();
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            _canvas.gameObject.SetActive(false);
        }
    }
}