using Continuum.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Elements
{
    /// <summary>
    /// Un oggetto Time Traveler renderizzato con una texture animata
    /// </summary>
    public class Animation : TimeTraveler
    {
        public int Length;
        public int Rows;
        public int Index;
        public int Cols;
        public int SequenceWidth;
        public int SequenceHeight;
        public int Row;
        public int Col;
        public float StretchRatio;

        /// <summary>
        /// Ridefinisce DestinationRectangle di Time Traveler per permettere anche alle animazioni di essere renderizzate Stretchate.
        /// È definito new per esplicitare al compilatore l'intenzione di effettuare hiding.
        /// </summary>
        public override Rectangle DestinationRectangle
        {
            get
            {
                return new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, (int)(StretchRatio * this.Width), (int)(StretchRatio * this.Height));
            }
        }

        /// <summary>
        /// Inizializza una nuova istanza della classe Animation
        /// </summary>
        /// <param name="Position">La posizione della texture</param>
        /// <param name="SequenceTexture">Il nome della texture con la sequenza di frame</param>
        /// <param name="NumberOfFrames">Il numero totale di frame della sequenza</param>
        /// <param name="NumberOfRows">Il numero di righe della matrice di frame</param>
        /// <param name="NumberOfCols">Il numero di colonne della matrice di frame</param>
        /// <param name="FramePerSecond">Il numero di frame al secondo</param>
        /// <param name="RotationStartRadiant">Posizione di partenza della rotazione in radianti</param>
        /// <param name="ScalingPercentage">Percentuale di scaling</param>
        /// <param name="gameState">Il Game State</param>
        public Animation(Vector2 Position, string SequenceTexture, int NumberOfFrames, int NumberOfRows, int NumberOfCols, int FramePerSecond, float RotationStartRadiant, float RotationSpeed, GameState gameState)
        {
            this.Rows = NumberOfRows;
            this.Length = NumberOfFrames;
            this.Cols = NumberOfCols;
            this.Index = 0;
            this.Row = 0;
            this.Col = 0;
            this.StretchRatio = 1;
            InitializeTimeTraveler(Position, FramePerSecond, SequenceTexture, gameState, RotationStartRadiant, RotationSpeed);
            this.SequenceWidth = gs.textures[TextureIndex].Width;
            this.SequenceHeight = gs.textures[TextureIndex].Height;
            Width = SequenceWidth / NumberOfCols;
            Height = SequenceHeight / NumberOfRows;
            SourceRectangle = new Rectangle(0, 0, Width, Height);
            Origin = new Vector2(SourceRectangle.Value.Center.X, SourceRectangle.Value.Center.Y);
        }

        public Animation() { }

        public override Vector2 EvaluatePosition(float Delta)
        {
            if (gs.levelTime.continuum != 0)
            {
                Index = (int)Delta;
                Row = Index / Cols;
                Col = Index % Cols;
                SourceRectangle = new Rectangle(Col * Width, Row * Height, Width, Height);
                if (Index < 0)
                    lifeState = LifeState.DELETING;
                if (Index > Length - 1)
                    lifeState = LifeState.DEAD;
            }
            return startPosition;
        }

        public override string ToString()
        {
            return "\n\nLength: " + Length + "\nRows: " + Rows + "\nCols: " + Cols + "\nFrameWidth: " + Width + "\nFrameHeight: " + Height + "\nWidth: " + SequenceWidth + "\nHeight: " + SequenceHeight + "\nIndex: " + Index + "\nRow: " + Row + "\nCol: " + Col + "\nDelta: " + delta + "\nLifeState: " + lifeState;
        }

        public override void HasCollided(int Value, object arg)
        {
            throw new System.NotImplementedException("Non deve poter collidere!!!");
        }
    }
}
