using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Continuum.Utilities
{

    public class LevelReader
    {
        private LevelElement[] elements;
        private int count;
        public LevelElement Current { private set; get; }
        public LevelElement Next { private set; get; }
        public LevelElement Previous { private set; get; }
        
        public LevelReader(string url)
        {
            XmlReader xr = XmlReader.Create(url);
            List<LevelElement> nodelist = new List<LevelElement>();
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    LevelElement le = new LevelElement(xr.Name, xr.NamespaceURI);
                    for (int i = 0; i < xr.AttributeCount; i++)
                    {
                        xr.MoveToNextAttribute();
                        le.Add(xr.Name, xr.Value);
                    }
                    nodelist.Add(le);
                }
            }
            elements = nodelist.ToArray();
            count = 0;
            RefreshElements();
            xr.Dispose();
        }

        public void MoveNext()
        {
            if(count < elements.Length - 1)
                count++;
            RefreshElements();
        }

        public void MovePrevious()
        {
            if(count > 0)
                count--;
            RefreshElements();
        }

        private void RefreshElements()
        {
            Current = elements[count];
            if (count + 1 < elements.Length)
                Next = elements[count + 1];
            else
                Next = null;
            if (count - 1 >= 0)
                Previous = elements[count - 1];
            else
                Previous = null;
        }
    }
}
