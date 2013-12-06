using System;
using Microsoft.Xna.Framework;
using Continuum.State;
using Continuum.Utilities;
using Continuum.Management;

namespace Continuum.Weapons
{
    public class Gun : IWeapons
    {
        public float lastShotTime;
        public bool player; //Indica se l'istanza di quest'arma è del giocatore o di un nemico.
        public int level;
        public float startTime;
        public float levelStartTime; //Indica il momento in cui l'arma è giunta al livello corrente
        public GameState gs;
        public TimeMachine ownerTime;

        /// <summary>
        /// Costruttore di un'arma GUN.
        /// </summary>
        /// <param name="player">Indica se l'arma appena creata è del giocatore o di un nemico</param>
        public Gun(bool player, GameState gameState)
        {
            gs = gameState;
            lastShotTime = startTime;
            this.player = player;
            if (player)
            {
                this.level = 1;
                this.ownerTime = gs.playerTime;
            }
            else
            {
                this.level = -1;
                this.ownerTime = gs.levelTime;
            }
            startTime = ownerTime.time;
            levelStartTime = ownerTime.time;
        }

        public Gun() { }

        /// <summary>
        /// Questa specifica arma lancia un bullet ogni mezzo secondo circa.
        /// </summary>
        /// <param name="position">Posizione del proprietario di quest'arma</param>
        public void Update(Vector2 position)
        {
            if (!gs.pause && ownerTime.continuum > 0)
            {
                if (ownerTime.time - levelStartTime > GetLevelDuration(Level))
                {
                    Upgrade(-1);
                }
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
                        case 3:
                            gs.newGunBullet(position, new Vector2(0, y), this);
                            break;
                        case 4:
                        case 5:
                        case 6:
                            Vector2 dir = new Vector2(-Constants.GUN_BULLET_X_DIRECTION, y);
                            dir.Normalize();
                            gs.newGunBullet(position, dir, this);
                            dir = new Vector2(0, y);
                            gs.newGunBullet(position, dir, this);
                            dir = new Vector2(Constants.GUN_BULLET_X_DIRECTION, y);
                            dir.Normalize();
                            gs.newGunBullet(position, dir, this);
                            break;
                        default:
                            throw new Exception("Impossibile che ci sia il livello di potenziamento " + Level + " in Gun");
                    }
                    lastShotTime = ownerTime.time;
                }
            }
            if(!gs.pause && ownerTime.continuum < 0)
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
                    return 1;
                case 1:
                    return 1;
                case 2:
                    return 1;
                case 3:
                    return 2;
                case 4:
                    return 2;
                case 5:
                    return 2;
                case 6:
                    return 3;
                default:
                    throw new Exception("Il numero di livello specificato (" + level + ") è inesistente per l'arma Gun");
            }
        }

        public int GetSpeed(int level)
        {
            switch (level)
            {
                case -1:
                    return 500;
                case 1:
                    return 500;
                case 2:
                    return 550;
                case 3:
                    return 600;
                case 4:
                    return 650;
                case 5:
                    return 700;
                case 6:
                    return 750;
                default:
                    throw new Exception("Il numero di livello specificato (" + level + ") è inesistente per l'arma Gun");
            }
        }

        public float GetTimeForShooting(int level)
        {
            switch (level)
            {
                case -1:
                    return 1.5f;
                case 1:
                    return 0.4f;
                case 2:
                    return 0.3f;
                case 3:
                    return 0.25f;
                case 4:
                    return 0.2f;
                case 5:
                    return 0.18f;
                case 6:
                    return 0.15f;
                default:
                    throw new Exception("Il numero di livello specificato (" + level + ") è inesistente per l'arma Gun");
            }
        }

        public float GetLevelDuration(int level)
        {
            switch (level)
            {
                case -1:
                    return float.MaxValue;
                case 1:
                    return float.MaxValue;
                case 2:
                    return 30;
                case 3:
                    return 25;
                case 4:
                    return 20;
                case 5:
                    return 15;
                case 6:
                    return 10;
                default:
                    throw new Exception("Il numero di livello specificato (" + level + ") è inesistente per l'arma Gun");
            }
        }

        public bool Upgrade(int numOfLevels)
        {
            level = level + numOfLevels;
            levelStartTime = ownerTime.time;
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
            get { return 1; }
        }
    }
}
