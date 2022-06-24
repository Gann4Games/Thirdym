using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Gann4Games.Thirdym.Interactables
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class MovableObject : SwitchListener
    {
        [Serializable]
        public class KeyFrame
        {
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale = Vector3.one;
            public Ease Easing = Ease.Linear;
            public float Time = 1;
            public UnityEvent OnStart;

            public KeyFrame(Vector3 position, Vector3 rotation, Vector3 scale, Ease easing, float time) {
                Position = position;
                Rotation = rotation;
                Scale = scale;
                Easing = easing;
                Time = time;
            }
        }

        [SerializeField] List<KeyFrame> keyframes;
        [SerializeField] AudioClip moveStartSFX, moveEndSFX;

        AudioSource _audioSource;
        Rigidbody _rigidbody;
        int _currentFrame;

        public bool IsMoving { get; private set; }

        KeyFrame CurrentFrame => keyframes[_currentFrame % keyframes.Count];

        private void Awake()
        {
            if (TryGetComponent(out Rigidbody rb)) _rigidbody = rb;
            _audioSource = GetComponent<AudioSource>();
        }
        public override void InteractableSwitch_Signal(object sender, EventArgs e) => Move();
        public void Move()
        {
            if (IsMoving) return;
            OnMoveStart();
            _rigidbody.DOMove(CurrentFrame.Position, CurrentFrame.Time).SetEase(CurrentFrame.Easing).OnComplete(() => OnMoveEnd());
            _rigidbody.DORotate(CurrentFrame.Rotation, CurrentFrame.Time).SetEase(CurrentFrame.Easing);
            transform.DOScale(CurrentFrame.Scale, CurrentFrame.Time).SetEase(CurrentFrame.Easing);
        }

        void OnMoveStart()
        {
            SetMoveState(true);
            CurrentFrame.OnStart.Invoke();
            _audioSource.Stop();
            _audioSource.PlayOneShot(moveStartSFX);
            _currentFrame++;
        }

        void OnMoveEnd()
        {
            SetMoveState(false);
            _audioSource.Stop();
            _audioSource.PlayOneShot(moveEndSFX);
        }

        void SetMoveState(bool value) => IsMoving = value;

        [ContextMenu("Generate keyframe")]
        public void GenerateKeyframe()
        {
            KeyFrame newKeyframe = new KeyFrame(transform.position, transform.eulerAngles, transform.localScale, Ease.Linear, 5);
            keyframes.Add(newKeyframe);
        }
        
        [ContextMenu("Reset transform")]
        public void ResetPosition()
        {
            KeyFrame firstFrame = keyframes[0];
            transform.position = firstFrame.Position;
            transform.eulerAngles = firstFrame.Rotation;
            transform.localScale = firstFrame.Scale;
        }

        private void OnDrawGizmosSelected()
        {
            foreach(KeyFrame frame in keyframes)
            {
                Gizmos.color = new Color(0, 1, 0, 0.25f);
                Gizmos.DrawSphere(frame.Position, 0.1f);
            }
        }

    }
}