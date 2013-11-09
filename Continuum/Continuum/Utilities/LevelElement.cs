using System;

namespace Continuum.Utilities
{
    public class LevelElement
    {
        public string Name;
        public string XmlCode;

        /// <summary>
        /// Costruisce una nuova istanza della classe LevelElement
        /// </summary>
        /// <param name="name">Il nome dell'elemento</param>
        public LevelElement(string name, string XmlCode)
        {
            Name = name;
        }

        /// <summary>
        /// Il numero di attributi presenti
        /// </summary>
        public int AttributeCount;

        /// <summary>
        /// Il primo elemento della lista
        /// </summary>
        private LevelElementAttribute First { get; set; }

        /// <summary>
        /// Aggiunge un attributo all'elemento
        /// </summary>
        /// <param name="Name">Il nome dell'attributo</param>
        /// <param name="Value">Il valore dell'attributo</param>
        public void Add(string Name, string Value)
        {
            LevelElementAttribute node = new LevelElementAttribute(Name, Value);
            node.Next = First;
            First = node;
            AttributeCount++;
        }

        /// <summary>
        /// Restituisce il valore dell'attributo con nome Name
        /// </summary>
        /// <param name="Name">Il nome dell'attributo</param>
        /// <returns>Il valore dell'attributo</returns>
        public string Attribute(string Name)
        {
            if (First != null)
            {
                return First.Attribute(Name);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Restituisce il valore dell'attributo in posizione i
        /// </summary>
        /// <param name="Name">L'indice dell'attributo</param>
        /// <returns>Il valore dell'attributo</returns>
        public LevelElementAttribute Attribute(int i)
        {
            i = AttributeCount - i - 1;
            if (First != null && i<AttributeCount && i>=0)
            {
                return First.Attribute(i, 0);
            }
            else
            {
                return null;
            }
        }
    }
}
