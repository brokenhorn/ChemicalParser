using System;

namespace ChemicalHierarchy
{
    /// <summary>
    /// описание кристаллической структуры
    /// </summary>
    public class ChemicalCrystalStructure : ChemicalCrystalSystem, IComparable<ChemicalCrystalStructure>, IComparable
    {
        /// <summary>
        /// пространственная группа
        /// </summary>
        public string SpaceGroup { get; }

        /// <summary>
        /// Число формульных единиц в элементарной ячейки (осмысленное значение всегда строго больше нуля)
        /// </summary>
        public double NumberOfFormulaUnits { get; }

        public ChemicalCrystalStructure(ChemicalCrystalSystem crystalSystem, string spaceGroup,
            double numberOfFormulaUnits) : base(crystalSystem)
        {
            if (string.IsNullOrEmpty(spaceGroup))
                throw new ApplicationException("ChemicalCrystalStructure: spaceGroup is empty");
            if (numberOfFormulaUnits <= 0)
                throw new ApplicationException("ChemicalCrystalStructure: numberOfFormulaUnits<=0");
            SpaceGroup = spaceGroup;
            NumberOfFormulaUnits = numberOfFormulaUnits;
        }


        public override string ToString()
        {
            return base.ToString() + $" SpaceGroup: {SpaceGroup}" + (NumberOfFormulaUnits>0 ? " NumberOfFormulaUnits: " + NumberOfFormulaUnits : "");
        }

        #region реализация интерфейсов IComparable<ChemicalCrystalSystem> и IComparable
        public int CompareTo(ChemicalCrystalStructure other)
        {
            if (other == null) return 1;
            int retVal = ((ChemicalCrystalSystem)this).CompareTo(other as ChemicalCrystalSystem);
            if (retVal != 0)
                return retVal;
            retVal = string.Compare(SpaceGroup, other.SpaceGroup, StringComparison.InvariantCultureIgnoreCase);
            if (retVal != 0)
                return retVal;
            return NumberOfFormulaUnits.CompareTo(other.NumberOfFormulaUnits);
        }
        public new int CompareTo(object obj)
        {
            return ((IComparable<ChemicalCrystalStructure>)this).CompareTo(obj as ChemicalCrystalStructure);
        }
        #endregion

    }

}
