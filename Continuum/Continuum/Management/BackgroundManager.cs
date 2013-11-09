using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Continuum.Elements;
using Continuum.State;
using Continuum.Utilities;
using System.Collections.Generic;

namespace Continuum.Management
{
    /// <summary>
    /// Component di gestione degli sfondi.
    /// </summary>
    public class BackgroundManager
    {
        private GameState gs;
        public BackgroundTexture[] BackgroundLevels { get; private set; }
        private LinkedList<ElementRecord> elementRecords;

        public BackgroundManager(GameState gameState, int numberOfLevels)
        {
            gs = gameState;
            BackgroundLevels = new BackgroundTexture[numberOfLevels];
            elementRecords = new LinkedList<ElementRecord>();
        }

        /// <summary>
        /// Aggiorna e monitora tutti i livelli di background.
        /// </summary>
        public void Update()
        {
            if (!gs.pause)
            {
                BackgroundTexture[] temp = new BackgroundTexture[gs.backgrounds.Count];
                int i = 0;

                //Aggiorna tutte le istanze della classe BackgroundTexture presenti nella lista
                foreach (BackgroundTexture x in gs.backgrounds)
                {
                    x.Update();

                    if (gs.levelTime.continuum > 0)
                    {
                        BackgroundTexture targetLevel = BackgroundLevels[x.Level];
                        bool hasTransition = (x.TransitionTextureIndex != null);

                        if (targetLevel != x)
                        {
                            if (targetLevel != null && targetLevel.lifeState != LifeState.DEAD)
                            {
                                if (targetLevel.lifeState != LifeState.BEINGREPLACED && x.StartTime >= targetLevel.StartTime)
                                {
                                    if (hasTransition)
                                        targetLevel.Replacing(x.TransitionTextureIndex.Value);
                                    else
                                        targetLevel.Replacing(x.TextureIndex);
                                }
                            }
                            else
                            {
                                AddElementRecord(Value => BackgroundLevels[x.Level] = (BackgroundTexture)Value, BackgroundLevels[x.Level]);
                                BackgroundLevels[x.Level] = x;
                                x.Start();
                                x.Update();
                            }
                        }
                    }

                    if (x.lifeState == LifeState.DELETING)
                    {
                        temp[i] = x;
                        i++;
                    }
                }

                //Elimina le istanze della classe BackgroundTexture che hanno terminato il loro ciclo di vita
                for (i = 0; i < temp.Length && temp[i] != null; i++)
                {
                    gs.backgrounds.Remove(temp[i]);
                }

                if (elementRecords.Last != null && gs.levelTime.time - elementRecords.Last.Value.Time > gs.timeTank)
                    elementRecords.RemoveLast();

                while (elementRecords.First != null && elementRecords.First.Value.Time > gs.levelTime.time)
                {
                    elementRecords.First.Value.Rewind();
                    elementRecords.RemoveFirst();
                }
            }
        }

        public void AddElementRecord(RewindMethod Rewind, object Value)
        {
            elementRecords.AddFirst(new ElementRecord(gs.levelTime.time, Rewind, Value));
        }
    }
}
