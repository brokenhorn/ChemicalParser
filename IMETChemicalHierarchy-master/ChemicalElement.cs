using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace ChemicalHierarchy
{
    /// <summary>
    /// химический элемент
    /// </summary>
    public sealed class ChemicalElement : IComparable<ChemicalElement>, IComparable
    {
        /// <summary>
        /// файл со списком допустимых элементов
        /// </summary>
        const string ElementsFileName = "elements.txt";

        /// <summary>
        /// список всех химических элементов Периодической системы (из БД или ресурса), упорядоченный по возрастанию атомного номера
        /// _elements = {"H", "He", ...};
        /// </summary>
        private readonly static string[] _elements;
        /// <summary>
        /// Статический конструктор (инициализация списка допустимых химичеких элементов)
        /// В папке с решением должен быть файл elements.txt
        /// </summary>
        static ChemicalElement()
        {
            _elements = @"H
He
Li
Be
B
С
N
O
F
Ne
Na
Mg
Al
Si
P
S
Cl
Ar
К
Ca
Sc
Ti
V
Cr
Mn
Fe
Со
Ni
Cu
Zn
Ga
Ge
As
Se
Br
Kr
Rb
Sr
Y
Zr
Nb
Mo
Tc
Ru
Rh
Pd
Ag
Cd
In
Sn
Sb
Те
I
Xe
Cs
Ba
La
Ce
Pr
Nd
Pm
Sm
Eu
Gd
Tb
Dy
Ho
Er
Tm
Yb
Lu
Hf
Ta
W
Re
Os
Ir
Pt
Au
Hg
Tl
Pb
Bi
Po
At
Rn
Fr
Ra
Ac
Th
Pa
U
Np
Pu
Am
Cm
Bk
Cf
Es
Fm
Md
No
Lr
Rf
Db
Sg
Bh
Hs
Mt
Ds
Rg
Cn
Uut
Uuq
Uup
Uuh
Uus
Uuo
Uuе
Ubn
Ubu
Ubb
Ubt
Ubq
Ubp
Ubn".Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Версия с файлом
            //if (!File.Exists(ElementsFileName))
            //    throw new ApplicationException("Не найден файл со списком допустимых элементов: " + ElementsFileName);
            //_elements = File.ReadAllLines(ElementsFileName);

            // Версия с встроенным в сборку ресурсом
            //var assembly = Assembly.GetExecutingAssembly();
            //using (Stream stream = assembly.GetManifestResourceStream(ElementsFileName))
            //using (StreamReader reader = new StreamReader(stream))
            //{
            //    string result = reader.ReadToEnd();
            //    _elements = result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            //}
        }
        /// <summary>
        /// обозначение элемента (H, He, ...)
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Атомный номер элемента
        /// </summary>
        public byte AtomicNumber { get; }
        /// <summary>
        /// свойства химического элемента (из БД)
        /// </summary>
        public NameValueCollection Properties { get; set; }
        /// <summary>
        /// создание элемента по имени (обозначению)
        /// </summary>
        /// <param name="name">имя элемента, например: "H", "He" и т.п.</param>
        public ChemicalElement(string name)
        {
            int num = Array.IndexOf(_elements, name); // StringComparison.InvariantCulture
            if (num < 1 || num > 255)
                throw new ApplicationException("Не найден атомный номер по названию элемента: " + name);
            AtomicNumber = (byte)(num + 1);
            Name = name;
        }
        /// <summary>
        /// создание элемента по атомному номеру
        /// </summary>
        /// <param name="atomicNumber">Атомный номер элемента</param>
        public ChemicalElement(int atomicNumber)
        {
            Name = _elements[AtomicNumber = (byte)(atomicNumber - 1)]; // если атомный номер неправильный => IndexOutOfRangeException
        }

        public override string ToString()
        {
            return Name;
        }

        #region реализация интерфейсов IComparable<ChemicalElement> и IComparable
        public int CompareTo(ChemicalElement other)
        {
            if (other == null) return 1;
            return AtomicNumber.CompareTo(other.AtomicNumber);
        }

        public int CompareTo(object obj)
        {
            return (this as IComparable<ChemicalElement>).CompareTo(obj as ChemicalElement);
        }
        #endregion
    }
}
