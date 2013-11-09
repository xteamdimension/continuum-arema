using Microsoft.Xna.Framework;
using Continuum.Weapons;
using Continuum.State;
using Continuum.Utilities;
using System;

namespace Continuum.Elements
{
    /// <summary>
    /// Proiettile dell'arma "RocketLauncher"
    /// </summary>
    public class FollowingRocket : Bullet
    {
        /// <summary>
        /// Il tempo (in secondi) che passa prima di iniziare a seguire.
        /// </summary>
        public const float FOLLOW_TIME_OUT = 0.3f;

        /// <summary>
        /// Costruttore della classe FollowingRocket
        /// </summary>
        /// <param name="startPosition">La posizione da cui viene sparato il proiettile</param>
        /// <param name="direction">La direzione del proiettile</param>
        /// <param name="rocketLauncher">L'istanza dell'arma che ha sparato il proiettile</param>
        /// <param name="gameState">Il GameState</param>
        public FollowingRocket(Vector2 StartPosition, Vector2 Direction, RocketLauncher rocketLauncher, GameState gameState)
        {
            InitializeBullet(StartPosition, Direction, TextureConstant.FOLLOWINGROCKET, rocketLauncher, gameState);
        }

        public FollowingRocket() { }

        /// <summary>
        /// Valuta la posizione del FollowingRocket
        /// </summary>
        /// <returns>La posizione del FollowingRocket</returns>
        public override Vector2 EvaluatePosition(float Delta)
        {
            if (elapsedTime > FOLLOW_TIME_OUT)
            {
                if (gs.levelTime.continuum > 0)
                {
                    Vector2 target = NearestEnemy();
                    if (target != new Vector2(-1, -1))
                    {
                        TravelToTarget(target, Delta);
                    }

                    Vector2 evaluate = new Vector2();
                    evaluate.X = CurrentPosition.X + (direction.X * gs.levelTime.elapsedContinuumTime * speed);
                    evaluate.Y = CurrentPosition.Y + (direction.Y * gs.levelTime.elapsedContinuumTime * speed);
                    AddElementRecord(new RewindMethod(Value => currentPosition = (Vector2)Value), evaluate);
                    return evaluate;
                }
                else return CurrentPosition;
            }
            else
            {
                return base.EvaluatePosition(Delta);
            }
        }

        /// <summary>
        /// Modifica la direzione del rocket in modo che segua il bersaglio.
        /// </summary>
        /// <param name="Target">Il bersaglio da seguire</param>
        private void TravelToTarget(Vector2 Target, float Delta)
        {
            Vector2 targetDirection = (Target - CurrentPosition);
            targetDirection.Normalize();

            float angle = Math.Min(Constants.FOLLOWING_ROCKET_RADIANS_STEP * Delta, Utility.CalculateAngleBetweenVectors(direction, targetDirection));

            if (gs.levelTime.continuum >= 0 && Utility.TowardsClockwise(direction, targetDirection) || gs.levelTime.continuum < 0 && !Utility.TowardsClockwise(direction, targetDirection))
                angle = -angle;

            Matrix rotate = new Matrix(1, 0, 0, CurrentPosition.X, 0, 1, 0, CurrentPosition.Y, 0, 0, 1, 0, 0, 0, 0, 1) * new Matrix((float)Math.Cos(angle), -(float)Math.Sin(angle), 0, 1, (float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0, 0, 0, 1, 0, 0, 0, 0, 1) * new Matrix(1, 0, 0, -CurrentPosition.X, 0, 1, 0, -CurrentPosition.Y, 0, 0, 1, 0, 0, 0, 0, 1);

            Vector2 newDirection = Vector2.Transform(direction, rotate);
            AddElementRecord(new RewindMethod(Value => direction = (Vector2)Value), newDirection);
            direction = newDirection;
            float newRotation = Utility.CalculateXAngleFromVector(direction) + (float)Math.PI / 2;
            AddElementRecord(new RewindMethod(Value => Rotation = (float)Value), newRotation);
            Rotation = newRotation;
        }

        protected override float EvaluateRotation(float RotationDelta)
        {
            if (elapsedTime <= FOLLOW_TIME_OUT)
                return Utility.CalculateXAngleFromVector(direction) + (float)Math.PI / 2;
            else
                return Rotation;
        }

        /// <summary>
        /// Ritorna la posizione corrente del nemico più vicino.
        /// Se il Rocket è di un nemico, ritorna la posizione del player.
        /// </summary>
        /// <returns></returns>
        private Vector2 NearestEnemy()
        {
            if (isPlayerBullet)
            {
                float min = float.MaxValue;
                Vector2 nearest = new Vector2(-1, -1);
                foreach (Enemy x in gs.enemies)
                {
                    if (x.lifeState != LifeState.DEAD)
                    {

                        float distance = Vector2.Distance(x.CurrentPosition, currentPosition);
                        if (distance < min)
                        {
                            min = distance;
                            nearest = x.CurrentPosition;
                        }
                    }
                }
                return nearest;
            }
            else return gs.playerPosition;
        }

        public override void HasCollided(int Value, object arg)
        {
            if (!(arg is GunBullet))
            {
                lifeState = LifeState.DEAD;
            }
        }
    }
}

