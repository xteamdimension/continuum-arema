using Continuum.State;
using Continuum.Utilities;
using Microsoft.Xna.Framework;

namespace Continuum.Elements
{
    /// <summary>
    /// Lancia TachyonStream in maniera casuale
    /// </summary>
    public class TachyonStreamRandomizer : Randomizer
    {
        public DynamicNormalRandomVariable durationRV;    //La durata dei TachyonStream lanciati

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
        public TachyonStreamRandomizer(float probability, float? probabilityIncrementPerMinute, float? probabilityMax, DynamicNormalRandomVariable durationRandomVariable, TimeDependentVar maxSecondsWithoutTachyonStream, string texture, GameState gameState)
        {
            InitializeRandomizer(probability, probabilityIncrementPerMinute, probabilityMax, new TimeDependentVar(1, null, null, null, null), maxSecondsWithoutTachyonStream, texture, gameState);

            durationRV = durationRandomVariable;
        }

        public TachyonStreamRandomizer() { }

        public override Vector2 EvaluatePosition(float Delta)
        {
            durationRV.Update(Delta);
            return base.EvaluatePosition(Delta);
        }

        protected override int GetAliveElementsCount()
        {
            return (gs.tachyonStream != null && (gs.tachyonStream.lifeState == LifeState.NORMAL || gs.tachyonStream.lifeState == LifeState.DAMAGED)) ? 1 : 0;
        }

        protected override void Launch()
        {
            if(gs.tachyonStream == null || gs.tachyonStream.lifeState == LifeState.DEAD || gs.tachyonStream.lifeState == LifeState.DELETING)
                gs.newTachyonStream(Utility.NextRandom(Constants.TACHYON_STREAM_WIDTH / 2, Constants.SCREEN_WIDTH - Constants.TACHYON_STREAM_WIDTH / 2), durationRV.Next(), texture);
        }

        public override void HasCollided(int Value, object arg)
        {

        }
    }
}
