using System;
using Continuum.State;
using Continuum.Management;
using Microsoft.Xna.Framework;
using Continuum.Utilities;

namespace Continuum.Weapons
{
    public class RocketLauncher : IWeapons
    {
        public float elapsedTime;
        public float lastShotTime;
        public bool player; //Indica se l'istanza di quest'arma è del giocatore o di un nemico.
        public int level;
        public float startTime;
        public GameState gs;
        public TimeMachine ownerTime;

        /// <summary>
        /// Costruttore di un'arma RocketLauncher.
        /// </summary>
        /// <param name="player">Indica se l'arma appena creata è del giocatore o di un nemico</param>
        public RocketLauncher(bool player, GameState gameState)
        {
            elapsedTime = 0;
            gs = gameState;
            lastShotTime = startTime;
            this.player = player;
            if (player)
            {
                this.level = MinLevel;
                this.ownerTime = gs.playerTime;
            }
            else
            {
                this.level = -1;
                this.ownerTime = gs.levelTime;
            }
            startTime = ownerTime.time;
        }

        public RocketLauncher() { }

        /// <summary>
        /// Questa specifica arma lancia un bullet ogni mezzo secondo circa.
        /// </summary>
        /// <param name="position">Posizione del proprietario di quest'arma</param>
        public void Update(Vector2 position)
        {
            elapsedTime = (ownerTime.time - startTime);
            if (!gs.pause && ownerTime.continuum > 0 && level != 0)
            {
                if (ownerTime.time - lastShotTime >= GetTimeForShooting(Level))
                {
                    int y;
                    if (isPlayerWeapon)
                        y = -1;
                    else
                        y = 1;

                    switch (Level)
                    {
                        case -1:
                        case 1:
                        case 2:
                            gs.newRocket(position, new Vector2(0, y), this);
                            break;
                        case 3:
                            {
                                Vector2 dir = new Vector2(-Constants.ROCKET_X_DIRECTION, y);
                                dir.Normalize();
                                gs.newRocket(position, dir, this);
                                dir = new Vector2(0, y);
                                gs.newRocket(position, dir, this);
                                dir = new Vector2(Constants.ROCKET_X_DIRECTION, y);
                                dir.Normalize();
                                gs.newRocket(position, dir, this);
                            }
                            break;
                        case 4:
                        case 5:
                            gs.newFollowingRocket(position, new Vector2(0, y), this);
                            break;
                        case 6:
                            {
                                Vector2 dir = new Vector2(-Constants.ROCKET_X_DIRECTION, y);
                                dir.Normalize();
                                gs.newFollowingRocket(position, dir, this);
                                dir = new Vector2(0, y);
                                gs.newFollowingRocket(position, dir, this);
                                dir = new Vector2(Constants.ROCKET_X_DIRECTION, y);
                                dir.Normalize();
                                gs.newFollowingRocket(position, dir, this);
                            }
                            break;
                        default:
                            throw new Exception("Impossibile che ci sia il livello di potenziamento " + Level + " in RocketLauncher");
                    }
                    lastShotTime = ownerTime.time;
                }
            }
            if (!gs.pause && ownerTime.continuum < 0)
            {
                if (ownerTime.time - lastShotTime < 0)
                {
                    lastShotTime = lastShotTime - GetTimeForShooting(Level);
                }
            }
        }

        public int GetDamage(int level)
        {
            switch (level)
            {
                case -1:
                    return 4;
                case 1:
                    return 4;
                case 2:
                    return 5;
                case 3:
                    return 5;
                case 4:
                    return 6;
                case 5:
                    return 7;
                case 6:
                    return 7;
                default:
                    throw new Exception("Il numero di livello specificato (" + level + ") è inesistente per l'arma RocketLauncher");
            }
        }

        public int GetSpeed(int level)
        {
            switch (level)
            {
                case -1:
                    return 300;
                case 1:
                    return 300;
                case 2:
                    return 400;
                case 3:
                    return 400;
                case 4:
                    return 500;
                case 5:
                    return 600;
                case 6:
                    return 600;
                default:
                    throw new Exception("Il numero di livello specificato (" + level + ") è inesistente per l'arma RocketLauncher");
            }
        }

        public float GetTimeForShooting(int level)
        {
            switch (level)
            {
                case -1:
                    return 3.0f;
                case 1:
                    return 2.0f;
                case 2:
                    return 1.6f;
                case 3:
                    return 1.6f;
                case 4:
                    return 1.6f;
                case 5:
                    return 1.3f;
                case 6:
                    return 1.3f;
                case 0:
                    return 0;
                default:
                    throw new Exception("Il numero di livello specificato (" + level + ") è inesistente per l'arma RocketLauncher");
            }
        }

        public bool Upgrade(int numOfLevels)
        {
            level = level + numOfLevels;
            if (level > MaxLevel)
                level = MaxLevel;
            if (level < MinLevel)
                level = MinLevel;
            return true;
        }

        /// <summary>
        /// Specifica se il bullet è stato sparato dal player
        /// </summary>
        public bool isPlayerWeapon
        {
            get
            {
                if (this.level >= 0)
                    return true;
                else
                    return false;
            }
        }

        public int Level
        {
            get { return level; }
            set
            {
                if (value <= MaxLevel)
                    level = value;
            }
        }

        public int MaxLevel
        {
            get { return 6; }
        }

        public int MinLevel
        {
            get { return 0; }
        }
    }
}
