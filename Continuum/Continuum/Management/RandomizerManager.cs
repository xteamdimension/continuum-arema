using System;
using Microsoft.Xna.Framework;
using Continuum.Elements;
using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Management
{

    /// <summary>
    /// Component di gestione di tutti i randomizzatori presenti nel gioco.
    /// </summary>
    public class RandomizerManager
    {
        private GameState gs;

        public RandomizerManager(GameState gameState)
        {
            gs = gameState;
        }

        /// <summary>
        /// Aggiorna tutte le istanze della classe Randomizer presenti nella lista
        /// Ed elimina quelle che hanno terminato il loro compito
        /// </summary>
        public void Update()
        {
            if (!gs.pause)
            {
                Randomizer[] temp = new Randomizer[gs.randomizers.Count];
                int i = 0;

                //Aggiorna tutte le istanze della classe Randomizer presenti nella lista
                foreach (Randomizer x in gs.randomizers)
                {
                    x.Update();

                    if (x.lifeState == LifeState.DELETING)
                    {
                        temp[i] = x;
                        i++;
                    }
                }

                //Elimina le istanze della classe Randomizer che hanno terminato il loro ciclo di vita
                for (i = 0; i < temp.Length && temp[i] != null; i++)
                {
                    gs.randomizers.Remove(temp[i]);
                }
            }
        }
    }
}
