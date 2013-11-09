using Continuum.State;
using Continuum.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Continuum.Elements;

namespace Continuum.Management
{
    public class EnemyManager
    {
        GameState gs;

        public EnemyManager(GameState gameState)
        {
            gs = gameState;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (!gs.pause)
            {
                Enemy[] temp = new Enemy[gs.enemies.Count];
                int i = 0;

                foreach (Enemy x in gs.enemies)
                {
                    x.Update();
                    if (x.lifeState == LifeState.DELETING)
                    {
                        temp[i] = x;
                        i++;
                    }
                }

                //Elimina le istanze della classe Enemy che hanno terminato il loro ciclo di vita
                for (i = 0; i < temp.Length && temp[i] != null; i++)
                    gs.enemies.Remove(temp[i]);
            }
        }
    }
}
