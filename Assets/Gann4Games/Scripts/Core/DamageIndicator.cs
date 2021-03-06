using UnityEngine;
using TMPro;
using Gann4Games.Thirdym.Utility;

namespace Gann4Games.Thirdym.Core
{
    public class DamageIndicator : MonoBehaviour
    {
        [Tooltip("The seconds it takes for the object to fully disappear.")]
        [SerializeField] float visualizationTime = 2;
        [Tooltip("Units to move per second.")]
        [SerializeField] Vector3 travelPerSecond = Vector3.up * .1f;
        [SerializeField] TextMeshPro textObject;

        Color _startColor;
        readonly TimerTool _timer = new TimerTool();

        private void Awake()
        {
            _startColor = textObject.color;
            _timer.SetTimeOut(visualizationTime);
        }
        private void Update()
        {
            float alpha = visualizationTime - (_timer.currentTime);
            textObject.color = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);
            _timer.CountTime();

            transform.Translate(travelPerSecond * Time.deltaTime);

            if (textObject.color.a <= 0) Destroy(gameObject);
        }
        public void Display(string value, Color color)
        {
            textObject.text = value;
            _startColor = color;
        }
    }
}
