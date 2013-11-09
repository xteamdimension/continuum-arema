using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Elements
{
    /// <summary>
    /// Lancia asteroidi in maniera casuale
    /// </summary>
    public class AsteroidRandomizer : Randomizer
    {
        public DynamicNormalRandomVariable speedRV;    //La velocità degli asteroidi lanciati
        public DynamicNormalRandomVariable lifeRV;   //Il danno recato dagli asteroidi lanciati

        /// <summary>
        /// Crea un nuovo AsteroidRandomizer
        /// </summary>
        /// <param name="probability">La probabilità che avvenga un lancio per ogni secondo</param>
        /// <param name="probabilityIncrementPerMinute">L'aumento della probabilità per ogni minuto. Per il valore predefinito inserire null.</param>
        /// <param name="probabilityMax">Il massimo valore che può assumere la probabilità. Per il valore predefinito inserire null.</param>
        /// <param name="lifeRandomVariable">Una variabile aleatoria per la scelta della vita degli asteroidi</param>
        /// <param name="speedRandomVariable">Una variabile aleatoria per la scelta della velocità degli asteroidi</param>
        /// <param name="texture">La texture degli asteroidi lanciati</param>
        /// <param name="gameState">Il Game State</param>
        public AsteroidRandomizer (float probability, float? probabilityIncrementPerMinute, float? probabilityMax, DynamicNormalRandomVariable speedRandomVariable, DynamicNormalRandomVariable lifeRandomVariable, string texture, GameState gameState)
        {
            InitializeRandomizer(probability, probabilityIncrementPerMinute, probabilityMax, texture, gameState);

            speedRV = speedRandomVariable;
            lifeRV = lifeRandomVariable;
        }

        public AsteroidRandomizer() { }

        public override Microsoft.Xna.Framework.Vector2 EvaluatePosition(float Delta)
        {
            speedRV.Update(Delta);
            lifeRV.Update(Delta);
            return base.EvaluatePosition(Delta);
        }

        protected override void Launch()
        {
            gs.newAsteroid(Utility.NextRandom(0, Constants.SCREEN_WIDTH), (int)speedRV.Next(), (int)lifeRV.Next(), texture);
        }

        public override void HasCollided(int Value, object arg)
        {

        }
    }
}
