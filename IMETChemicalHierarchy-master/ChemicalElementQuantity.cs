using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMETlib.Chemistry
{
    /// <summary>
    /// Старье, но используется в ChemicalUtils
    /// </summary>
    [Obsolete("Это старье и не должно использоваться", false)]
    public class ChemicalElementQuantity : IComparable<ChemicalElementQuantity>
    {
        public string Element { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }

        public ChemicalElementQuantity(string ElementName, double valueMin, double valueMax)
        {
            Element = ElementName;
            Min = valueMin;
            Max = valueMax;
        }

        public int CompareTo(ChemicalElementQuantity other)
        {
            int i = Element.CompareTo(other.Element);
            if (i != 0)
                return i;
            i = Min.CompareTo(other.Min);
            if (i != 0)
                return i;
            return Max.CompareTo(other.Max);
        }

        public override string ToString() =>
            (Min == Max) ? $"{Element}({Min})" : $"{Element}({Min}-{Max})";
    }
}
