using Continuum.State;
using Continuum.Utilities;
using Microsoft.Xna.Framework;

namespace Continuum.Elements
{
    /// <summary>
    /// Lancia nemici in maniera casuale
    /// </summary>
    public class EnemyRandomizer : Randomizer
    {
        public DynamicNormalRandomVariable speedRV;    //La velocità degli asteroidi lanciati
        public DynamicNormalRandomVariable lifeRV;   //Il danno recato dagli asteroidi lanciati
        public float powerUpProbability;
        public float rocketProbability;
        public float granadeProbability;
        public string weapon;

        /// <summary>
        /// Crea un nuovo EnemyRandomizer
        /// </summary>
        /// <param name="probability">La probabilità che avvenga un lancio per ogni secondo</param>
        /// <param name="probabilityIncrementPerMinute">L'aumento della probabilità per ogni minuto</param>
        /// <param name="probabilityMax">Il massimo valore che può assumere la probabilità</param>
        /// <param name="powerUpProbabilityPerLaunch">La probabilità che un nemico lanciato possa avere un PowerUp</param>
        /// <param name="rocketPowerUpProbability">La probabilità che un nemico lanciato con PowerUp possegga un PowerUp di tipo Rocket</param>
        /// <param name="granadePowerUpProbability">La probabilità che un nemico lanciato con PowerUp possegga un PowerUp di tipo granade</param>
        /// <param name="lifeRandomVariable">Una variabile aleatoria per la scelta della vita dei nemici</param>
        /// <param name="speedRandomVariable">Una variabile aleatoria per la scelta della velocità dei nemici</param>
        /// <param name="texture">La texture dei nemici lanciati</param>
        public EnemyRandomizer(float probability, float? probabilityIncrementPerMinute, float? probabilityMax, float? powerUpProbabilityPerLaunch, float? rocketPowerUpProbability, float? granadePowerUpProbability, DynamicNormalRandomVariable speedRandomVariable, DynamicNormalRandomVariable lifeRandomVariable, TimeDependentVar maxSimultaneousEnemies, TimeDependentVar maxSecondsWithoutEnemies, string weapon, string texture, GameState gameState)
        {
            InitializeRandomizer(probability, probabilityIncrementPerMinute, probabilityMax, maxSimultaneousEnemies, maxSecondsWithoutEnemies, texture, gameState);
            powerUpProbability = powerUpProbabilityPerLaunch == null ? 0 : powerUpProbabilityPerLaunch.Value;
            rocketProbability = rocketPowerUpProbability == null ? 0 : rocketPowerUpProbability.Value;
            granadeProbability = granadePowerUpProbability == null ? 0 : granadePowerUpProbability.Value;
            this.weapon = weapon;
            speedRV = speedRandomVariable;
            lifeRV = lifeRandomVariable;
        }

        public EnemyRandomizer() { }

        public override Vector2 EvaluatePosition(float Delta)
        {
            // Aggiorna le random variables
            speedRV.Update(Delta);
            lifeRV.Update(Delta);

            return base.EvaluatePosition(Delta);
        }

        protected override void Launch()
        {
            double r = Utility.NextRandom(0d, 1d);
            PowerUpType t = PowerUpType.NONE;
            if (r < powerUpProbability)
            {
                r = Utility.NextRandom(0d, 1d);
                if (r < rocketProbability)
                    t = PowerUpType.ROCKET;
                else if (r < granadeProbability + rocketProbability)
                    t = PowerUpType.GRANADE;
                else
                    t = PowerUpType.GUN;
            }
            gs.newEnemy(new Vector2(Utility.NextRandom(-30, Constants.SCREEN_WIDTH + 30), -50), speedRV.Next(), texture, weapon, (int)lifeRV.Next(), t);
        }


        protected override int GetAliveElementsCount()
        {
            int count = 0;
            foreach (Enemy e in gs.enemies)
            {
                if (e.lifeState == LifeState.NORMAL || e.lifeState == LifeState.DAMAGED)
                    count++;
            }
            return count;
        }

        public override void HasCollided(int Value, object arg)
        {

        }
    }
}
