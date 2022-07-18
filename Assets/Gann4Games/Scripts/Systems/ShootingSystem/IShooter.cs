using System.Collections;

namespace Gann4Games.Thirdym.ShootSystem
{
    public interface IShooter
    {
        public void BeginFire();
        public IEnumerator FireCoroutine();
        public void StopFire();
    }
}
