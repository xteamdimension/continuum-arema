using System;
using Continuum.Utilities;
using Microsoft.Xna.Framework;
using Continuum.State;

namespace Continuum.Elements
{
    /// <summary>
    /// Danno ad area
    /// </summary>
    public class AreaDamage : Bullet
    {
        public float Duration;
        public float StretchRatio;

        /// <summary>
        /// Ridefinisce DestinationRectangle di Time Traveler per permettere all'AreaDamage di Assumere le dimensioni desiderate.
        /// È definito new per esplicitare al compilatore l'intenzione di effettuare hiding.
        /// </summary>
        public override Rectangle DestinationRectangle
        {
            get
            {
                return new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, (int)(StretchRatio * this.Width), (int)(StretchRatio * this.Height));
            }
        }

        public AreaDamage() { }

        public AreaDamage(Vector2 StartPosition, string SequenceTexture, int NumberOfFrames, int NumberOfRows, int NumberOfCols, float DamageDuration, float Range, int Damage, GameState gameState)
        {
            InitializeTimeTraveler(StartPosition, 1, TextureConstant.VOID_TEXTURE, gameState);
            StretchRatio = Range / 32;
            gs.newAnimation(StartPosition, SequenceTexture, NumberOfFrames, NumberOfRows, NumberOfCols, (int)(NumberOfFrames/DamageDuration), 0, 0);
            damage = Damage;
            isPlayerBullet = true;
            this.Duration = DamageDuration;
        }

        /// <summary>
        /// Valuta la posizione della granata
        /// </summary>
        /// <returns>La posizione della granata</returns>
        public override Vector2 EvaluatePosition(float Delta)
        {
            if (Delta >= Duration)
                lifeState = LifeState.DEAD;
            return startPosition;
        }

        public override void HasCollided(int Value, object arg)
        {
            
        }

        public override string ToString()
        {
            return "PlasmaGranade - " + lifeState.ToString();
        }
    }
}
