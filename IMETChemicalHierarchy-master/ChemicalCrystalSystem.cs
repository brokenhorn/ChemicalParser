using System; 

namespace ChemicalHierarchy
{

    /// <summary>
    /// химическая модификация (добавляем к ChemicalSubstance тип кристаллической структуры)
    /// </summary>
    public class ChemicalCrystalSystem : ChemicalSubstance, IComparable<ChemicalCrystalSystem>, IComparable
    {
        /// <summary>
        /// описание кристаллической структуры
        /// </summary>
        public CrystalSystemType CrystalSystem { get; }

        /// <summary>
        /// формула вещества и модификация 
        /// </summary>
        public string ModificationName {
            get {
                return base.ToString() + " " + CrystalSystem;
            }
        }

        /// <summary>
        /// формула вещества и модификация в HTML
        /// </summary>
        public string ModificationNameHtml {
            get {
                return base.SubstanceNameHtml + " " + CrystalSystem;
            }
        }

        /// <summary>
        /// конструктор модификации
        /// </summary>
        /// <param name="substance">вещество</param>
        /// <param name="cs"></param>
        public ChemicalCrystalSystem(ChemicalSubstance substance, CrystalSystemType cs) : base(substance)
        {
            CrystalSystem = cs;
        }

        /// <summary>
        /// конструктор копирования модификации
        /// </summary>
        /// <param name="crystalSystem">химическая модификация</param>
        public ChemicalCrystalSystem(ChemicalCrystalSystem crystalSystem) : base(crystalSystem)
        {
            CrystalSystem = crystalSystem.CrystalSystem;
        }

        public override string ToString()
        {
            return ModificationName;
        }


        #region реализация интерфейсов IComparable<ChemicalCrystalSystem> и IComparable
        public int CompareTo(ChemicalCrystalSystem other)
        {
            if (other == null) return 1;
            int retVal = ((ChemicalSubstance)this).CompareTo(other as ChemicalSubstance);
            if (retVal != 0)
                return retVal;
            return CrystalSystem.CompareTo(other.CrystalSystem);
        }
        public new int CompareTo(object obj)
        {
            return ((IComparable<ChemicalCrystalSystem>)this).CompareTo(obj as ChemicalCrystalSystem);
        }
        #endregion
    }
}
