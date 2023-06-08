using UnityEngine;

namespace _Scripts.Powerups
{
    public class Speed : Powerup.Powerup
    {
        [SerializeField] private float speedMultiplier;
        private float _speedIncrease;
        
        protected override void Begin()
        {
            _speedIncrease = target.controller.Speed * speedMultiplier - target.controller.Speed;
            target.controller.Speed += _speedIncrease;
        }

        protected override void End()
        {
            target.controller.Speed -= _speedIncrease;
        }

        protected override void ValuesToCopyToOther(Powerup.Powerup pOther)
        {
            if (!(pOther is Speed)) return;
            Speed speed = (Speed)pOther;
            Debug.Log(speedMultiplier);
            Debug.Log(speed.speedMultiplier);
            speed.speedMultiplier = speedMultiplier;
        }

        protected override void DisplayEffect() {}
    }
}
