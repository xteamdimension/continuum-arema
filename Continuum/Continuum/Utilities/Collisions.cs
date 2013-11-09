using Continuum.Elements;

namespace Continuum.Utilities
{
    public class Collisions
    {
        public TimeTraveler[] array;
        public int AliveCount { get; private set; }
        public int Count { get; private set; }
        public int Length
        {
            get
            {
                return array.Length;
            }
        }

        public Collisions()
        {
            array = new TimeTraveler[Constants.COLLISIONS_ARRAY_LENGTH];
            AliveCount = 0;
            Count = 0;
        }

        public TimeTraveler GetElementAt(int index){
            return array[index];
        }

        public void Insert(TimeTraveler c)
        {
            if (Count >= Length)
            {
                TimeTraveler[] temp = array;
                array = new TimeTraveler[Length * 2];
                for (int i = 0; i < temp.Length; i++)
                    array[i] = temp[i];
            }
            array[Count] = c;
            Count++;
        }

        public void Sort()
        {
            InsertionSort(Flag());
        }

        private int Flag()
        {
            int i = 0;
            int j = 0;
            int k = Count;
            while (k > j)
            {
                if (array[j] == null)
                {
                    array[j] = array[k - 1];
                    array[k - 1] = null;
                    k--;
                }
                else if (array[j].lifeState == LifeState.DELETING)
                {
                    array[j] = null;
                    Scambia(array, j, k - 1);
                    k--;
                }
                else if (array[j].lifeState == LifeState.DEAD)
                {
                    j++;
                }
                else
                {
                    Scambia(array, j, i);
                    i++;
                    j++;
                }
            }
            AliveCount = i;
            return i;
        }

        private void InsertionSort(int max)
        {
            for (int i = 1; i < max; i++)
            {
                TimeTraveler x = array[i];
                int j = i;
                while (j > 0 && x.Top < array[j - 1].Top)
                {
                    array[j] = array[j - 1];
                    j--;
                }
                array[j] = x;
            }
        }

        private void Scambia(TimeTraveler[] array, int a, int b)
        {
            TimeTraveler temp = array[a];
            array[a] = array[b];
            array[b] = temp;
        }
    }
}
