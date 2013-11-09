namespace Continuum.State
{
    public struct PlayerState
    {
        public float life;
        public float positionX;
        public float positionY;
        public bool toggleGun;
        public float timeStamp;
        public int gunLevel;
        public int rocketLauncherLevel;

        public PlayerState(float positionX, float positionY, float life, bool toggleGun, float timeStamp, int gunLevel, int rocketLauncherLevel)
        {
            this.life = life;
            this.positionX = positionX;
            this.positionY = positionY;
            this.toggleGun = toggleGun;
            this.timeStamp = timeStamp;
            this.gunLevel = gunLevel;
            this.rocketLauncherLevel = rocketLauncherLevel;
        }
    }
}
