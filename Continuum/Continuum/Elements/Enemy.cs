using System;
using Continuum.State;
using Continuum.Utilities;
using Continuum.Weapons;
using Microsoft.Xna.Framework;

namespace Continuum.Elements
{
    /// <summary>
    /// Definisce un nuovo Nemico
    /// </summary>
    public class Enemy : TimeTraveler
    {

        public IWeapons Weapon;
        public BezierPath trajectory;
        public int life;
        public int BoundDamaged;
        public float smokeWait = Constants.SMOKE_DELAY;
        public float smokeTime;
        public PowerUpType powerUpType;

        /// <summary>
        /// Inizializza una nuova istanza per un nuovo nemico
        /// </summary>
        /// <param name="StartPosition">posizione di partenza</param>
        /// <param name="Speed">velocità</param>
        /// <param name="Texture">il nome della texture</param>
        /// <param name="Weapon">arma a disposizione</param>
        /// <param name="Life">quantità di vita</param>
        /// <param name="gameState">gameState del gioco</param>
        public Enemy(Vector2 StartPosition, float Speed, string Texture, IWeapons Weapon, int Life, GameState gameState, PowerUpType PowerUpType) 
        {
            this.Weapon = Weapon;
            this.life = Life;
            this.trajectory = new BezierPath(BezierPathTrajectory.RANDOM, 25, new Rectangle(0, 0, Constants.SCREEN_WIDTH, 500), StartPosition, new Vector2(0, 0));//traiettoria random da cambiare poi impostandola dall'xml e passandola al costruttore
            InitializeTimeTraveler(StartPosition, Speed, Texture, gameState);
            this.BoundDamaged = (int) (this.life * Constants.CRITICAL_BOUND_DAMAGE);
            this.powerUpType = PowerUpType;
        }

        public Enemy() { }

        public void Damaging(int amount)
        {
            AddElementRecord(Value => life = (int)Value, life);
            life -= amount;
            if (life <= 0)
            {
                lifeState = LifeState.DEAD;
                if (powerUpType != PowerUpType.NONE)
                    gs.newPowerUp(CurrentPosition, powerUpType);
            }
            else if (life <= BoundDamaged)
                lifeState = LifeState.DAMAGED;
        }

        public override Vector2 EvaluatePosition(float Delta)
        {
            Vector2 NextPosition = Vector2.Zero;
            trajectory.CurveIndex = (int)Delta;

            if(trajectory.IsFinished)
            {
                lifeState = LifeState.DEAD;
            }
            if (lifeState != LifeState.DEAD)
            {
                if(!trajectory.IsFinished)
                    NextPosition = trajectory.NextPosition(Delta);
                Weapon.Update(NextPosition);

                if (lifeState == LifeState.DAMAGED)
                {
                    if (gs.levelTime.continuum > 0)
                    {
                        if (smokeWait >= Constants.SMOKE_DELAY)
                        {
                            smokeTime = gs.levelTime.time;
                            gs.newAnimation(NextPosition,TextureConstant.ANIMATION_DAMAGESMOKE, 117, 11, 10, 50, (float)Utility.NextRandom(0f, (float)Math.PI * 2), 1); 
                            smokeWait = 0;
                        }
                        else
                            smokeWait += gs.levelTime.time - smokeTime;
                    }
                }
            }
            return NextPosition;            
        }

        public override string ToString()
        {
            return "Enemy - " + lifeState.ToString();
        }

        public override void HasCollided(int Value, object arg)
        {
            this.Damaging(Value);
        } 
    }
}
