
using Microsoft.Xna.Framework;

namespace Continuum.Utilities
{
    /// <summary>
    /// Definisce una traiettoria
    /// </summary>
    public class BezierPath
    {
        public Rectangle positionBounds;
        public Vector2 startPosition;
        public Vector2 endPosition;
        public int numberOfCurves;
        //private int bufferSize;
        public BezierPathTrajectory trajectoryType;
        public QuadraticBezierCurve[] trajectory;
        public int CurveIndex { get; set; }
        public bool isFinished;

        /// <summary>
        /// Costruttore della traiettoria finita
        /// </summary>
        /// <param name="duration">durata della traiettoria</param>
        /// <param name="trajectoryType">tipo di traiettoria che sarà eseguita</param>
        /// <param name="numberOfCurves">numero di curve quadratiche di Bezier che compone la traiettoria ovvero il numero di cambi di direzione</param>
        /// <param name="positionBounds">area rettangolare su cui può insistere la traiettoria</param>
        /// <param name="startPosition">posizione di partenza dell'oggetto</param>
        /// <param name="endPosition">posizione finale dell'oggetto</param>
        public BezierPath(BezierPathTrajectory trajectoryType, int numberOfCurves, Rectangle positionBounds, Vector2 startPosition, Vector2 endPosition)
        {
            //ricalcola i margini affinche i nemici evitino di uscire dallo schermo: il ricalcolo va fatto con una differenza tra i bounds sul rettangolo e il MAX_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION controllare anche il MIN
            this.trajectoryType = trajectoryType;
            this.numberOfCurves = numberOfCurves;
            this.positionBounds = positionBounds;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.CurveIndex = 0;
            this.isFinished = false;
            switch(trajectoryType)
            {
                case BezierPathTrajectory.RANDOM:
                    trajectory = new QuadraticBezierCurve[numberOfCurves];
                    Vector2 sp = startPosition;
                    Vector2 cp = (Vector2.Normalize(new Vector2(Utility.NextRandom(positionBounds.Left, positionBounds.Right), Utility.NextRandom(positionBounds.Top, positionBounds.Bottom)) - sp) * Utility.NextRandom(Constants.MIN_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION, Constants.MAX_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION)) + sp;
                    Vector2 tp = (Vector2.Normalize(new Vector2(Utility.NextRandom(positionBounds.Left, positionBounds.Right), Utility.NextRandom(positionBounds.Top, positionBounds.Bottom)) - cp) * Utility.NextRandom(Constants.MIN_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION, Constants.MAX_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION)) + cp;
                    trajectory[0] = new QuadraticBezierCurve(sp, tp, cp);
                    for (int i = 1; i<trajectory.Length-1; i++)
                    {
                        sp = trajectory[i - 1].targetPoint;
                        cp = (QuadraticBezierCurve.CalculateNewControlPointVector(trajectory[i - 1].controlPoint, trajectory[i - 1].targetPoint, Utility.NextRandom(Constants.MIN_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION, Constants.MAX_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION)));
                        tp = (Vector2.Normalize(new Vector2(Utility.NextRandom(positionBounds.Left, positionBounds.Right), Utility.NextRandom(positionBounds.Top, positionBounds.Bottom)) - sp) * Utility.NextRandom(Constants.MIN_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION, Constants.MAX_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION)) + sp;
                        trajectory[i] = new QuadraticBezierCurve(sp, tp, cp);
                    }
                    sp = trajectory[trajectory.Length - 2].targetPoint;
                    cp = (QuadraticBezierCurve.CalculateNewControlPointVector(trajectory[trajectory.Length - 2].controlPoint, trajectory[trajectory.Length - 2].targetPoint, Utility.NextRandom(Constants.MIN_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION, Constants.MAX_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION)));
                    tp = endPosition;
                    trajectory[trajectory.Length-1] = new QuadraticBezierCurve(sp,tp,cp);
                break;
                case BezierPathTrajectory.TRAIETTORIA1:
                break;
                case BezierPathTrajectory.TRAIETTORIA2:
                break;

            }

        }

        public BezierPath() { }

        /// <summary>
        /// Indica se la traiettoria è stata completata
        /// </summary>
        public bool IsFinished
        {
            get { 
                    isFinished = CurveIndex >= trajectory.Length;
                    return isFinished;
                }
        }

        public Vector2 NextPosition(float delta)
        {
            return trajectory[CurveIndex].Evaluate(delta - CurveIndex);
        }
    }
}
