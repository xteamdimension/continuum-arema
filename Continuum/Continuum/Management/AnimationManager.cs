using Continuum.State;
using Continuum.Elements;
using Continuum.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Continuum.Management
{
    /// <summary>
    /// Component di gestione di tutte le animazioni presenti nel gioco
    /// </summary>
    public class AnimationManager
    {
        GameState gs;

        /// <summary>
        /// Inizializza una nuova istanza della classe AnimationManager
        /// </summary>
        /// <param name="gameState">Il GameState del gioco</param>
        public AnimationManager(GameState gameState)
        {
            gs = gameState;
        }

        public void Update()
        {
            if (!gs.pause)
            {
                Animation[] temp = new Animation[gs.animations.Count];
                int i = 0;

                //Aggiorna tutte le istanze della classe Animation presenti nella lista
                foreach (Animation x in gs.animations)
                {
                    x.Update();
                    if (x.lifeState == LifeState.DELETING)
                        temp[i++] = x;
                }

                //Elimina le istanze della classe Animation che hanno terminato il loro ciclo di vita
                for (i = 0; i < temp.Length && temp[i] != null; i++)
                    gs.animations.Remove(temp[i]);
            }
        }
    }
}
