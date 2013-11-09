using Microsoft.Xna.Framework;

namespace Continuum.Utilities
{
    public class QuadraticBezierCurve
    {
        /// <summary>
        /// Punto di partenza di questa curva.
        /// </summary>
        public Vector2 startPoint { get; set; }

        /// <summary>
        /// Punto di arrivo di questa curva.
        /// </summary>
        public Vector2 targetPoint { get; set; }

        /// <summary>
        /// Punto di controllo di questa curva.
        /// </summary>
        public Vector2 controlPoint { get; set; }

        /// <summary>
        /// Costruttore vuoto (per serializzazione).
        /// Tutti i punti sono settati a zero.
        /// </summary>
        public QuadraticBezierCurve()
        {
            this.startPoint = Vector2.Zero;
            this.controlPoint = Vector2.Zero;
            this.targetPoint = Vector2.Zero;
        }

        /// <summary>
        /// Genera una nuova Curva di Bezier Quadratica in uno spazio bidimensionale.
        /// </summary>
        /// <param name="startPoint">Punto di partenza.</param>
        /// <param name="targetPoint">Punto di controllo.</param>
        /// <param name="controlPoint">Punto di arrivo.</param>
        public QuadraticBezierCurve(Vector2 startPoint, Vector2 targetPoint, Vector2 controlPoint)
        {
            this.startPoint = startPoint;
            this.targetPoint = targetPoint;
            this.controlPoint = controlPoint;
        }

        public Vector2 Evaluate(float t)
        {
            return Vector2.Lerp(Vector2.Lerp(startPoint, controlPoint, t), Vector2.Lerp(controlPoint, targetPoint, t), t);
        }

        public static Vector2 CalculateNewControlPointVector(Vector2 lastControlPoint, Vector2 lastTargetPoint, float module)
        {
            return ((Vector2.Normalize((lastTargetPoint-lastControlPoint))) * module) + lastTargetPoint;
        }
    }
}
