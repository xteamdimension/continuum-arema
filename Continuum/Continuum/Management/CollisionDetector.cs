using Continuum.State;
using Continuum.Elements;
using Microsoft.Xna.Framework;
using Continuum.Utilities;
using System;
using Microsoft.Devices;

namespace Continuum.Management
{
    /// <summary>
    /// IL PENE
    /// </summary>
    public class CollisionDetector
    {
        GameState gs;

        public CollisionDetector(GameState gameState)
        {
            gs = gameState;
        }

        public void Update()
        {
            //Controlla le collisioni solo se non stiamo tornando indietro nel tempo
            if (gs.playerTime.continuum > 0)
            {
                int i = 0;
                int j = 0;
                gs.collisions.Sort();
                for (i = 0; (i < gs.collisions.AliveCount) && (gs.collisions.GetElementAt(i) != null); i++)
                {
                    if (TryPlayerCollision(gs.collisions.GetElementAt(i)))                      //controllo di collisioni sul player
                    {
                        //occhio alla sottrazione di vite negative
                        if (gs.collisions.GetElementAt(i) is Asteroid)                          //collisione tra player e asteroide
                        {
                            int temp = (int) gs.PlayerLife;
                            gs.PlayerHasCollided(((Asteroid)(gs.collisions.GetElementAt(i))).life, 1, 2);
                            gs.collisions.GetElementAt(i).HasCollided(temp, null);
                        }
                        else if (gs.collisions.GetElementAt(i) is Enemy)                        //collisione tra player e enemy
                        {
                            int temp = (int) gs.PlayerLife;
                            gs.PlayerHasCollided(((Enemy)(gs.collisions.GetElementAt(i))).life, 1, 1);
                            gs.collisions.GetElementAt(i).HasCollided(temp, null);
                        }
                        else if (gs.collisions.GetElementAt(i) is Bullet)                    //collisione tra player e bullet
                        {
                            if (!(((Bullet)(gs.collisions.GetElementAt(i))).isPlayerBullet)) //se il gunbullet non è del player causa un danno al player
                            {
                                gs.PlayerHasCollided(((Bullet)(gs.collisions.GetElementAt(i))).damage, 0, 1);
                                gs.collisions.GetElementAt(i).HasCollided(0, null);
                            }
                        }
                        else if (gs.collisions.GetElementAt(i) is Tachyon)                      //Collisione tra player e tachione
                        {
                            if (gs.timeTank <= Constants.MAX_TIME_TANK_VALUE)
                            {
                                gs.timeTank = MathHelper.Clamp(gs.timeTank += Constants.TACHYON_TIME_VALUE, 0, Constants.MAX_TIME_TANK_VALUE);
                                gs.collisions.GetElementAt(i).HasCollided(0, null);
                            }
                        }
                        else if (gs.collisions.GetElementAt(i) is PowerUp)                      //Collisione tra player e powerup
                        {
                            PowerUp p = (PowerUp)gs.collisions.GetElementAt(i);
                            switch(p.Type)
                            {
                                case PowerUpType.GUN:
                                    gs.playerGun.Upgrade(1);
                                    break;
                                case PowerUpType.ROCKET:
                                    gs.playerRocketLauncher.Upgrade(1);
                                    break;
                                case PowerUpType.GRANADE:
                                    gs.PlayerGranadeCount += 1;
                                    break;
                                default:
                                    throw new NotImplementedException("PowerUp non ancora implementato per il CollisionDetector");
                            }
                            p.HasCollided(0, null);
                        }
                    }
                    j = i + 1;
                    while (j < gs.collisions.AliveCount && gs.collisions.GetElementAt(j).Top <= gs.collisions.GetElementAt(i).Bottom)   //controllo collisioni tra Timetravelers
                    {
                        if (TryCollision(gs.collisions.GetElementAt(i), gs.collisions.GetElementAt(j)))
                        {
                            if (gs.collisions.GetElementAt(i) is Asteroid)                          //controllo di collisioni su asteroidi
                            {
                                /*
                                if (gs.collisions.GetElementAt(j) is Asteroid)                      //collisione tra asteroide e asteroide
                                {
                                    int temp = ((Asteroid)(gs.collisions.GetElementAt(i))).life;
                                    gs.collisions.GetElementAt(i).HasCollided(((Asteroid)(gs.collisions.GetElementAt(j))).life, null);
                                    gs.collisions.GetElementAt(j).HasCollided(temp, null);
                                }*/
                                if (gs.collisions.GetElementAt(j) is Enemy)                    //collisione tra asteroide e enemy
                                {
                                    int temp = ((Asteroid)(gs.collisions.GetElementAt(i))).life;
                                    gs.collisions.GetElementAt(i).HasCollided(((Enemy)(gs.collisions.GetElementAt(j))).life, null);
                                    gs.collisions.GetElementAt(j).HasCollided(temp, null);
                                }
                                else if (gs.collisions.GetElementAt(j) is Bullet)                //collisione tra asteroide e gunbullet
                                {
                                    gs.collisions.GetElementAt(i).HasCollided(((Bullet)(gs.collisions.GetElementAt(j))).damage, null);
                                    gs.collisions.GetElementAt(j).HasCollided(0, null);
                                }
                            }
                            else if (gs.collisions.GetElementAt(i) is Enemy)                        //controllo di collisioni con enemies
                            {
                                if (gs.collisions.GetElementAt(j) is Asteroid)                      //collisione tra enemy e asteroide
                                {
                                    int temp = ((Enemy)(gs.collisions.GetElementAt(i))).life;
                                    gs.collisions.GetElementAt(i).HasCollided(((Asteroid)(gs.collisions.GetElementAt(j))).life, null);
                                    gs.collisions.GetElementAt(j).HasCollided(temp, null);
                                }
                                else if (gs.collisions.GetElementAt(j) is Bullet)                //collisione tra enemy e bullet
                                {
                                    if (((Bullet)(gs.collisions.GetElementAt(j))).isPlayerBullet)//se il bullet è del player causa un danno all'enemy
                                    {
                                        gs.collisions.GetElementAt(i).HasCollided(((Bullet)(gs.collisions.GetElementAt(j))).damage, null);
                                        gs.collisions.GetElementAt(j).HasCollided(0, null);
                                    }
                                }
                            }
                            else if (gs.collisions.GetElementAt(i) is Bullet)                    //controllo di collisioni su gunbullets
                            {
                                if (gs.collisions.GetElementAt(j) is Asteroid)                      //collisione tra bullet e asteroide
                                {
                                    gs.collisions.GetElementAt(i).HasCollided(0, null);
                                    gs.collisions.GetElementAt(j).HasCollided(((Bullet)(gs.collisions.GetElementAt(i))).damage, null);
                                }
                                else if (gs.collisions.GetElementAt(j) is Enemy)                    //collisione tra gunbullet e enemy
                                {
                                    if (((Bullet)(gs.collisions.GetElementAt(i))).isPlayerBullet)//se il gunbullet è del player causa un danno all'enemy
                                    {
                                        gs.collisions.GetElementAt(i).HasCollided(0, null);
                                        gs.collisions.GetElementAt(j).HasCollided(((Bullet)(gs.collisions.GetElementAt(i))).damage, null);
                                    }
                                }
                                else if (gs.collisions.GetElementAt(j) is Bullet)                //collisione tra bullet e bullet
                                {
                                    if (((Bullet)(gs.collisions.GetElementAt(i))).isPlayerBullet != ((Bullet)(gs.collisions.GetElementAt(j))).isPlayerBullet) //se i gunbullets sono di due proprietari diversi si spaccano a vicenda
                                    {
                                        //gs.collisions.GetElementAt(i).HasCollided(0, gs.collisions.GetElementAt(j));
                                        //gs.collisions.GetElementAt(j).HasCollided(0, gs.collisions.GetElementAt(i));
                                    }
                                }
                            }
                        }
                        j++;
                    }
                }
            }
        }

        public bool TryCollision(TimeTraveler a, TimeTraveler b)
        {
            Rectangle A = Utility.newRectangleFromCenterPosition(a.CurrentPosition, a.Width, a.Height);
            Rectangle B = Utility.newRectangleFromCenterPosition(b.CurrentPosition, b.Width, b.Height);
            return A.Intersects(B);
        }

        public bool TryPlayerCollision(TimeTraveler b)
        {
            Rectangle A = Utility.newRectangleFromCenterPosition(gs.playerPosition, gs.PlayerWidth, gs.PlayerHeight);
            Rectangle B = Utility.newRectangleFromCenterPosition(b.CurrentPosition,b.Width ,b.Height);
            if (A.Intersects(B))
                return true;
            else
                return false;
        }

    }
}
