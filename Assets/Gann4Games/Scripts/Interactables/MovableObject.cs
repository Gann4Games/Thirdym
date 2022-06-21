using UnityEngine;
using DG.Tweening;
using System;

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

            public KeyFrame(Vector3 position, Vector3 rotation, Vector3 scale, Ease easing, float time) {
                Position = position;
                Rotation = rotation;
                Scale = scale;
                Easing = easing;
                Time = time;
            }
        }

        [SerializeField] KeyFrame[] keyframes;
        [SerializeField] AudioClip moveStartSFX, moveEndSFX;

        AudioSource _audioSource;
        Rigidbody _rigidbody;
        int _currentFrame;
        Vector3 _startPos;

        public bool IsMoving { get; private set; }
        public KeyFrame CurrentFrame {
            get
            {
                KeyFrame cframe = keyframes [_currentFrame % keyframes.Length];
                return new KeyFrame(_startPos + cframe.Position, cframe.Rotation, cframe.Scale, cframe.Easing, cframe.Time);
            }
        }

        private void Awake()
        {
            _startPos = transform.position;
            if (TryGetComponent(out Rigidbody rb)) _rigidbody = rb;
            _audioSource = GetComponent<AudioSource>();
        }
        public override void InteractableSwitch_Signal(object sender, EventArgs e) => Move();
        public void Move()
        {
            if (IsMoving) return;
            _currentFrame++;
            OnMoveStart();
            _rigidbody.DOMove(CurrentFrame.Position, CurrentFrame.Time).SetEase(CurrentFrame.Easing).OnComplete(() => OnMoveEnd());
            _rigidbody.DORotate(CurrentFrame.Rotation, CurrentFrame.Time).SetEase(CurrentFrame.Easing);
            transform.DOScale(CurrentFrame.Scale, CurrentFrame.Time).SetEase(CurrentFrame.Easing);
        }

        void OnMoveStart()
        {
            SetMoveState(true);
            _audioSource.Stop();
            _audioSource.PlayOneShot(moveStartSFX);
        }

        void OnMoveEnd()
        {
            SetMoveState(false);
            _audioSource.Stop();
            _audioSource.PlayOneShot(moveEndSFX);
        }

        void SetMoveState(bool value) => IsMoving = value;

        private void OnDrawGizmosSelected()
        {
            foreach(KeyFrame frame in keyframes)
            {
                Gizmos.color = new Color(0, 1, 0, 0.25f);
                Gizmos.DrawSphere(_startPos + frame.Position, 0.1f);
            }
        }

    }
}