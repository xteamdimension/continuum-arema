using System;
using Microsoft.Xna.Framework;
using Continuum.Elements;
using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Management
{

    /// <summary>
    /// Component di gestione di tutte le istanze dei PowerUp presenti nel gioco.
    /// </summary>
    public class PowerUpManager
    {
        private GameState gs;

        public PowerUpManager(GameState gameState)
        {
            gs = gameState;
        }

        /// <summary>
        /// Aggiorna tutte le istanze della classe PowerUp presenti nella lista
        /// Ed elimina quelle che hanno terminato il loro compito
        /// </summary>
        public void Update()
        {
            if (!gs.pause)
            {
                PowerUp[] temp = new PowerUp[gs.powerUps.Count];
                int i = 0;

                //Aggiorna tutte le istanze della classe PowerUp presenti nella lista
                foreach (PowerUp x in gs.powerUps)
                {
                    x.Update();
                    if (!Utility.IsInScreenSpace(Utility.newRectangleFromCenterPosition(x.CurrentPosition, 30, 30)))
                    {
                        x.lifeState = LifeState.DEAD;
                    }

                    if (x.lifeState == LifeState.DELETING)
                    {
                        temp[i] = x;
                        i++;
                    }
                }

                //Elimina le istanze della classe PowerUp che hanno terminato il loro ciclo di vita
                for (i = 0; i < temp.Length && temp[i] != null; i++)
                {
                    gs.powerUps.Remove(temp[i]);
                }
            }
        }
    }
}
