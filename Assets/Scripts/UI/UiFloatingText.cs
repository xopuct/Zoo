using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Pool;

namespace Zoo
{
    public class UiFloatingText : MonoBehaviour
    {
        public CanvasGroup Prefab;
        public Transform Root;
        public float TimeToShow = 2f;
        public AnimationCurve CurveAlpha = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public AnimationCurve CurveScale = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Inject]
        private GameService gameService;
        [Inject]
        private CameraService cameraService;

        private ObjectPool<CanvasGroup> pool;
        private readonly Dictionary<CanvasGroup, float> kills = new();

        private void Awake()
        {
            pool = new(
                createFunc: () => Instantiate(Prefab, Root),
                actionOnGet: text => text.gameObject.SetActive(true),
                actionOnRelease: text => text.gameObject.SetActive(false),
                actionOnDestroy: text => Destroy(text.gameObject),
                defaultCapacity: 2,
                maxSize: 64);
            gameService.OnKillEvent += OnKill;
        }

        private void OnDestroy()
        {
            gameService.OnKillEvent -= OnKill;
            pool.Dispose();
        }


        private void OnKill(Unit victim, Unit killer)
        {
            Show(killer);
        }

        private void Show(Unit killer)
        {
            var floatingText = pool.Get();
            Vector3 screenPosition =
                cameraService.Camera.WorldToScreenPoint(
                    killer.transform.position + Vector3.back * killer.Collider.bounds.extents.z);

            floatingText.transform.position = screenPosition;
            kills.Add(floatingText, Time.time + TimeToShow);
        }

        private void LateUpdate()
        {
            using var _ = ListPool<CanvasGroup>.Get(out var toRemove);
            foreach (var (text, time) in kills)
            {
                if (Time.time > time)
                {
                    toRemove.Add(text);
                }
                else
                {
                    var timeLeft = time - Time.time;
                    var normalizedTime = 1 - Mathf.Clamp01(timeLeft / TimeToShow);
                    text.alpha = CurveAlpha.Evaluate(normalizedTime);
                    text.transform.localScale =
                        Vector3.one * CurveScale.Evaluate(normalizedTime);
                }
            }

            foreach (var expired in toRemove)
            {
                pool.Release(expired);
                kills.Remove(expired);
            }
        }
    }
}
