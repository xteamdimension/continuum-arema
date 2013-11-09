using System.Collections.Generic;
using Continuum.State;
using Continuum.Elements;
using Continuum.Utilities;

namespace Continuum.Management
{

    /// <summary>
    /// Component di gestione di tutti i proiettili sparati durante la partita
    /// </summary>
    public class BulletsManager
    {
        GameState gs;

        public BulletsManager(GameState gameState)
        {
            gs = gameState;
        }
        
        /// <summary>
        /// Aggiornamento dello stato di tutti i proiettili presenti.
        /// </summary>
        public void Update()
        {
            if (!gs.pause)
            {
                UpdateBullets(gs.bullets);
            }
        }

        /// <summary>
        /// Aggiorna lo stato di tutti i Bullets
        /// </summary>
        /// <param name="list"></param>
        public void UpdateBullets(LinkedList<Bullet> list)
        {
            //Inserisce nella lista di bullet tutti i bullet che sono stati appena creati
            foreach (Bullet x in gs.newBullets)
            {
                list.AddLast(x);
            }

            Bullet[] temp = new Bullet[list.Count];
            int i = 0;

            //Aggiorna tutte le istanze della classe Bullet presenti nella lista
            foreach (Bullet x in list)
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

            //Elimina le istanze della classe Bullet che hanno terminato il loro ciclo di vita
            for (i = 0; i < temp.Length && temp[i] != null; i++)
            {
                list.Remove(temp[i]);
            }
        }
    }
}
