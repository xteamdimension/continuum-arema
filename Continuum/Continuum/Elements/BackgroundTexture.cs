using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Elements
{
    /// <summary>
    /// Texture di sfondo.
    /// </summary>
    public class BackgroundTexture : TimeTraveler
    {
        /// <summary>
        /// La seconda posizione in cui deve essere renderizzata la BackgroundTexture, per garantire la continuità dello sfondo.
        /// </summary>
        public Vector2 CurrentPosition2;

        public Rectangle DestinationRectangle2
        {
            get
            {
                return new Rectangle((int)CurrentPosition2.X, (int)CurrentPosition2.Y, Width, Height);
            }
        }

        /// <summary>
        /// Il momento in cui la texture inizia effettivamente ad aggiornare la sua posizione.
        /// </summary>
        public float StartTime2;

        /// <summary>
        /// La texture che sostituirà la corrente.
        /// </summary>
        public int ReplacingTextureIndex { get; private set; }

        /// <summary>
        /// La texture di transizione.
        /// </summary>
        public int? TransitionTextureIndex { get; private set; }

        /// <summary>
        /// Conta il numero di volte che la texture ha percorso lo schermo.
        /// </summary>
        public int loops;

        /// <summary>
        /// Il livello su cui verrà renderizzata la BackgroundTexture
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Costruttore della classe BackgroundTexture.
        /// </summary>
        /// <param name="Speed">La velocità dello sfondo.</param>
        /// <param name="TransitionTexture">La texture di transizione. Inserire null se non si vuole una texture di transizione.</param>
        /// <param name="Texture">Il nome della texture caricata nella textureList in gameState</param>
        /// <param name="gameState">il gameState</param>
        public BackgroundTexture(int Level, int Speed, string Texture, string TransitionTexture, GameState gameState)
        {
            int textureIndex = gameState.textureIndices.GetTextureIndex(Texture);
            if (TransitionTexture == null)
            {
                this.TransitionTextureIndex = null;
            }
            else
            {
                TransitionTextureIndex = gameState.textureIndices.GetTextureIndex(TransitionTexture);
            }
            loops = 0;
            this.Level = Level;
            InitializeTimeTraveler(Vector2.Zero, Speed, Texture, gameState);
            float stretchRatio = (float)480 / this.Width;
            Width = (int)(stretchRatio * Width);
            Height = (int)(stretchRatio * Height);
            startPosition = new Vector2(0, 800 - Height);
            currentPosition = startPosition;
            CurrentPosition2 = startPosition - new Vector2(0, gameState.textures[textureIndex].Height);
            StartTime2 = 0;
            blockUpdate = true;
        }

        public BackgroundTexture() { }

        protected override void EvaluateDelta()
        {
            elapsedTime = gs.levelTime.time - StartTime2;
            delta = elapsedTime * speed;
            if (elapsedTime < 0)
                elapsedTime = 0;
            if (delta < 0)
                delta = 0;
        }

        public override Vector2 EvaluatePosition(float Delta)
        {
            Vector2 evaluate = Vector2.Zero;
            switch (lifeState)
            {
                case LifeState.NORMAL:
                    evaluate.Y = startPosition.Y + (Delta) - Height * loops;
                    CurrentPosition2.Y = evaluate.Y - Height;
                    break;
                case LifeState.TRANSITIONING:
                    evaluate.Y = startPosition.Y + (Delta) - Height * loops;
                    CurrentPosition2.Y = evaluate.Y - Height;
                    break;
                case LifeState.BEINGREPLACED:
                    evaluate.Y = startPosition.Y + (Delta) - Height * loops;
                    CurrentPosition2.Y = evaluate.Y - Height;
                    break;
            }

            if (gs.levelTime.continuum > 0 && evaluate.Y > 800)
            {
                AddElementRecord(Value => loops = (int)Value, loops);
                loops++;
                if (lifeState == LifeState.TRANSITIONING)
                {
                    lifeState = LifeState.NORMAL;
                }
                if (lifeState == LifeState.BEINGREPLACED)
                {
                    lifeState = LifeState.DEAD;
                }
            }

            return evaluate;
        }

        /// <summary>
        /// Informa la BackgroundTexture che sta per essere rimpiazzata da un'altra BackgroundTexture.
        /// </summary>
        /// <param name="newTexture"></param>
        public void Replacing(int newTexture)
        {
            lifeState = LifeState.BEINGREPLACED;
            ReplacingTextureIndex = newTexture;
        }

        /// <summary>
        /// Informa la BackgroundTexture che è stata appena inserita nel livello di destinazione
        /// </summary>
        public void Start()
        {
            AddElementRecord(Value => StartTime2 = (float)Value, StartTime2);
            StartTime2 = gs.levelTime.time;
            loops = 0;
            if (TransitionTextureIndex != null)
                lifeState = LifeState.TRANSITIONING;
            AddElementRecord(Value => blockUpdate = (bool)Value, blockUpdate);
            blockUpdate = false;
        }

        public override string ToString()
        {
            string str = "Texture = " + TextureIndex + "\n   lifeState = " + lifeState + "\n   loops = " + loops;
            str += "\n   elapsedTime = " + elapsedTime + "\n   delta = " + delta;
            foreach (ElementRecord x in ElementRecords)
            {
                str += "\n   record: " + x.Time + "  " + x.Value;
            }
            return str;
        }

        public override void HasCollided(int Value, object arg)
        {
            throw new System.NotImplementedException("Non deve poter collidere!!!");
        }
    }
}
