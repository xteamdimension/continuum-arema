namespace Continuum.Utilities
{
    /// <summary>
    /// Definisce un singolo attributo
    /// </summary>
    public class LevelElementAttribute
    {
        /// <summary>
        /// Il prossimo attributo
        /// </summary>
        public LevelElementAttribute Next { get; set; }

        /// <summary>
        /// Il valore dell'attributo
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Il nome dell'attributo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Costruttore del nodo
        /// </summary>
        /// <param name="Value">Il valore dell'attributo</param>
        /// <param name="Name">Il nome dell'attributo</param>
        public LevelElementAttribute(string Name, string Value)
        {
            this.Value = Value;
            this.Name = Name;
        }

        /// <summary>
        /// Cerca un attributo per restituirne il valore
        /// </summary>
        /// <param name="Name">Il nome</param>
        /// <returns>Il valore</returns>
        internal string Attribute(string Name)
        {
            if (this.Name == Name)
            {
                return Value;
            }
            else
            {
                if (Next == null)
                {
                    return null;
                }
                else
                {
                    return Next.Attribute(Name);
                }
            }
        }

        /// <summary>
        /// Cerca un attributo per restistuirne il valore
        /// </summary>
        /// <param name="i">L'indice dell'attributo</param>
        /// <param name="level">Il livello di profondità corrente</param>
        /// <returns></returns>
        internal LevelElementAttribute Attribute(int i, int level)
        {
            if (i == level)
            {
                return this;
            }
            else
            {
                if (Next == null)
                {
                    return null;
                }
                else
                {
                    return Next.Attribute(i, level + 1);
                }
            }
        }
    }
}
