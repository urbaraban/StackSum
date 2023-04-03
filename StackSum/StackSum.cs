using System.Collections.ObjectModel;

namespace StackSumLib
{
    public class StackSum : Collection<StackItem>
    {
        public float CommonSum
        {
            get
            {
                float result = 0.0f;
                for (int i = 0; i < this.Count; i += 1)
                {
                    result += this[i].Value;
                }
                return result;
            }
        }
        
    }

    public struct StackItem
    {
        public string Display { get; set; }
        public float Value { get; set; }
        public int Multiplier { get; set; }

        public StackItem(string display, float value, int multiplier = 1)
        {
            this.Display = display;
            this.Value = value;
            this.Multiplier = multiplier;
        }
    }
}