using System;
using Continuum.Management;
using Continuum.State;
using Continuum.Utilities;
using Continuum.Weapons;
using Microsoft.Xna.Framework;


namespace Continuum.Elements
{
    public class Player
    {
        public GameState gs;
        public Vector2 interpolationA;
        public Vector2 interpolationB;
        public float timeinterpolation;
        public float deathTime;

        //public float smokeWait = Constants.SMOKE_DELAY;

        public int TextureIndex { get; set; }

        public Player(GameState gs)
        {
            this.gs = gs;
            interpolationA = new Vector2();
            interpolationB = new Vector2();
            deathTime = 0;
        }
        public Player(){}

        public void Update(InputManager input)
        {
            UpdatePlayerState();

            if (gs.playerTime.continuum >= 0)
            {

                if (gs.playerLifeState == LifeState.DEAD)
                {
                    if (deathTime > gs.timeTank)
                        gs.playerLifeState = LifeState.DELETING;    //INDICA CHE IL PLAYER È MORTO DA TROPPO TEMPO E IL GIOCO DEVE FINIRE
                    else
                    {
                        deathTime += gs.playerTime.elapsedContinuumTime;
                    }
                }
                else
                {
                    deathTime = 0;
                    if (gs.damageTimer >= 0)
                        gs.damageTimer -= gs.playerTime.elapsedContinuumTime;
                    else
                    {
                        gs.PlayerLife += gs.playerTime.elapsedContinuumTime * Constants.PLAYER_LIFE_RISING_INCREMENT;
                    }

                    //UPDATE SMOKE
                    // E' commentata perchè con la grafica il fumo non esiste più
                    //
                    //if (gs.playerLifeState == LifeState.DAMAGED)
                    //{
                    //    if (smokeWait >= Constants.SMOKE_DELAY)
                    //    {
                    //        gs.newAnimation(gs.playerPosition, TextureConstant.ANIMATION_DAMAGESMOKE, 117, 11, 10, 50, Utility.NextRandom(0, 2 * MathHelper.Pi), 1);
                    //        smokeWait = 0;
                    //    }
                    //    else
                    //        smokeWait += gs.playerTime.elapsedContinuumTime;
                    //}

                    //UPDATE POSIZIONE
                    gs.playerPosition.X = MathHelper.Clamp(gs.playerPosition.X + (input.AccelerometerReadingCorrected.X * 800 * gs.playerTime.elapsedContinuumTime), 0, Constants.SCREEN_WIDTH);
                    gs.playerPosition.Y = MathHelper.Clamp(gs.playerPosition.Y - (input.AccelerometerReadingCorrected.Y * 800 * gs.playerTime.elapsedContinuumTime), 0, Constants.SCREEN_HEIGHT);

                    //UPDATE ARMI
                    if (gs.toggleGun)
                    {
                        gs.playerGun.Update(gs.playerPosition);
                        gs.playerRocketLauncher.Update(gs.playerPosition);
                    }
                }

                

                //Uno state nuovo ogni 33 millisecondi DELLA TIMEMACHINE
                if (gs.playerStates.Count == 0 || (gs.playerTime.time * 1000) - (gs.playerStates.First.Value.timeStamp * 1000) >= 33) 
                    gs.playerStates.AddFirst(new PlayerState(gs.playerPosition.X, gs.playerPosition.Y, gs.PlayerLife, gs.toggleGun, gs.playerTime.time, gs.playerGun.Level, gs.playerRocketLauncher.Level, gs.PlayerGranadeCount));
                
                //Cancella l'ultimo stato se sono passati più di 4 secondi da quando è stato creato.
                if (gs.playerTime.time - gs.playerStates.Last.Value.timeStamp > gs.timeTank)
                    gs.playerStates.RemoveLast();

                interpolationA = gs.playerPosition;
                if (gs.playerStates.First != null)
                {
                    interpolationB.X = gs.playerStates.First.Value.positionX;
                    interpolationB.Y = gs.playerStates.First.Value.positionY;
                    timeinterpolation = (gs.playerTime.time - gs.playerStates.First.Value.timeStamp) * 1000;
                }
            }
            else
            {
                if (gs.timeTank > 0)
                {
                    // QUESTE DUE RIGHE SERVONO PER EVITARE IL PROBLEMA CHE NON FACEVA SPARARE IL PLAYER PER QUALCHE SECONDO DOPO CHE ERA TORNATO INDIETRO NEL TEMPO.
                    gs.playerGun.Update(gs.playerPosition);
                    gs.playerRocketLauncher.Update(gs.playerPosition);

                    //REWIND SMOKE
                    //if (smokeWait <= 0)
                    //    smokeWait = Constants.SMOKE_DELAY;
                    //else
                    //    if (gs.playerLifeState != LifeState.DAMAGED)
                    //        smokeWait = Constants.SMOKE_DELAY;
                    //    else
                    //        smokeWait -= gs.playerTime.elapsedContinuumTime;

                    if (gs.playerStates.Count > 0)
                    {
                        //Consumo lo stato al momento opportuno
                        if (gs.playerTime.time <= gs.playerStates.First.Value.timeStamp)
                        {
                            if(gs.playerStates.First.Next != null)
                                timeinterpolation = - (gs.playerStates.First.Next.Value.timeStamp - gs.playerStates.First.Value.timeStamp) * 1000;

                            gs.playerPosition.X = gs.playerStates.First.Value.positionX;
                            gs.playerPosition.Y = gs.playerStates.First.Value.positionY;
                            gs.PlayerLife = gs.playerStates.First.Value.life;
                            gs.playerGun.Level = gs.playerStates.First.Value.gunLevel;
                            gs.playerRocketLauncher.Level = gs.playerStates.First.Value.rocketLauncherLevel;
                            gs.toggleGun = gs.playerStates.First.Value.toggleGun;
                            gs.PlayerGranadeCount = gs.playerStates.First.Value.granades;
                            //gs.playerWeapons = gs.playerStates.First.Value.weaponsStack;

                            gs.playerStates.RemoveFirst();

                            if (gs.playerStates.First != null)
                            {
                                interpolationA = gs.playerPosition;
                                interpolationB.X = gs.playerStates.First.Value.positionX;
                                interpolationB.Y = gs.playerStates.First.Value.positionY;
                            }
                        }
                        else
                        {
                            if (gs.playerStates.First != null)
                            {
                                float amount = (gs.playerTime.time * 1000 - gs.playerStates.First.Value.timeStamp * 1000) / timeinterpolation;
                                gs.playerPosition = Vector2.Lerp(interpolationB, interpolationA, amount);
                            }
                        }
                    }
                }
            }
        }

        private void UpdatePlayerState()
        {
            if (gs.playerLifeState != LifeState.DELETING)
            {
                if (gs.PlayerLife > Constants.PLAYER_LIFE_CRITICAL_VALUE)
                    gs.playerLifeState = LifeState.NORMAL;
                else if (gs.PlayerLife > 0 && gs.PlayerLife <= Constants.PLAYER_LIFE_CRITICAL_VALUE)
                    gs.playerLifeState = LifeState.DAMAGED;
                else if (gs.PlayerLife == 0 && gs.playerLifeState != LifeState.DEAD && gs.playerLifeState != LifeState.DELETING)
                {
                    gs.newExplosion(gs.playerPosition);
                    gs.playerLifeState = LifeState.DEAD;
                    gs.timeState = TimeState.FORWARD;
                }
            }
        }
    }
}
