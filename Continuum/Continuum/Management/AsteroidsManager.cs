using System;
using Microsoft.Xna.Framework;
using Continuum.Elements;
using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Management
{

    /// <summary>
    /// Component di gestione di tutte le istanze delle armi sparate durante la partita.
    /// </summary>
    public class AsteroidManager
    {
        private GameState gs;

        public AsteroidManager(GameState gameState)
        {
            gs = gameState;
        }

        /// <summary>
        /// Aggiorna tutte le istanze della classe Asteroid presenti nella lista
        /// Ed elimina quelle che hanno terminato il loro compito
        /// </summary>
        public void Update()
        {
            if (!gs.pause)
            {
                Asteroid[] temp = new Asteroid[gs.asteroids.Count];
                int i = 0;

                //Aggiorna tutte le istanze della classe Asteroid presenti nella lista
                foreach (Asteroid x in gs.asteroids)
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

                //Elimina le istanze della classe Asteroid che hanno terminato il loro ciclo di vita
                for (i = 0; i < temp.Length && temp[i] != null; i++)
                {
                    gs.asteroids.Remove(temp[i]);
                }
            }
        }
    }
}
