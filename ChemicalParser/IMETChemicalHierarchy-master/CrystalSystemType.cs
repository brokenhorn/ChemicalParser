namespace ChemicalHierarchy
{
    /// <summary>
    /// сингония
    /// </summary>
    public enum CrystalSystemType : byte
    {
        // Hexagonal = Trigonal | Rhombohedral
        // Trigonal == Rhombohedral
        Trigonal = 1, Rhombohedral = 1, Hexagonal = 1,   // одно и то же на уровне сингоний!!!
        Triclinic = 2, Monoclinic = 3, Orthorhombic = 4, Tetragonal = 5, Cubic= 6
    }
}
