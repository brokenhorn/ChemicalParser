using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IMETlib.Chemistry
{
    public class REFACTOR_ChemicalUtils
    {

        /// <summary>
        /// Старье, но используется в ChemicalUtils
        /// </summary>
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


        /// <summary>
        /// функция для построения строки и XML для БД по описанию системы
        /// </summary>
        /// <param name="Mode">Mode = 0 - получаем описание системы по XML (ChemicalSystemNode - <ChemicalSystem>)
        // Mode = 1 - получаем описание системы по XML (ChemicalSubstanceNode - <ChemicalSubstance>)
        // Mode = 2 - получаем описание системы по HTML-строке из mySystemXML</param>
        /// <param name="InXmlNode"></param>
        /// <param name="NumElements"></param>
        /// <param name="mySystemXML"></param>
        /// <param name="mySystemString"></param>
        /// <param name="Response"></param>
        /// <returns></returns>
        public static int GetSystemStringByDescription(int Mode, ref XmlNode InXmlNode, ref int NumElements, ref string mySystemXML, ref string mySystemString, ref string Response)
        {
            int GetSystemStringByDescription = 0;
            Response = "";
            string st, HTML;
            string[] el;
            int i=0;
            XmlNodeList nodeList = null;

            HTML = mySystemXML;
            NumElements = 0;
            mySystemString = "";
            mySystemXML = "";
            switch (Mode)
            {
                case 0:
                case 1:
                    if (Mode == 0)
                        nodeList = InXmlNode.SelectNodes("Element");
                    else
                        nodeList = InXmlNode.SelectNodes(".//Index/Element");
                    if (nodeList.Count > 0)
                    {
                        el = new string[nodeList.Count]; // массив химических элементов
                        i = el.GetLowerBound(0);
                        foreach (XmlNode node in nodeList) // заполним массив 
                        {
                            el[i] = node.InnerText;
                            i += 1;
                        }
                        Array.Sort(el); // отсортируем массив
                        st = "undefined";
                        for (i = el.GetLowerBound(0); i <= el.GetUpperBound(0); i++)
                        {
                            if (el[i] != st)
                            {
                                st = el[i];
                                // debug("i = " & i.ToString() & ", st = '" & st & "'")
                                mySystemString += st + "-";
                                mySystemXML += "<Element>" + st + "</Element>";
                                NumElements += 1;
                            }
                        }
                        mySystemString = "-" + mySystemString;
                        mySystemXML = "<ChemicalSystem>" + mySystemXML + "</ChemicalSystem>";
                    }
                    else
                    {
                        GetSystemStringByDescription = 1;
                        Response = "GetSystemStringByDescription: Не найдено элементов Element в описании химической системы";
                    }
                    break;
                case 2:      // Mode = 2 - получаем описание системы по HTML-строке из mySystemXML
                    List<string> ElList = new List<string>(); // список хим. элементов
                    string Element;
                    i = 0;
                    int i_start;
                    while (i < HTML.Length)
                    {
                        if (HTML[i] == '<')
                        {
                            while (i < HTML.Length)
                            {
                                if (HTML[i] != '>')
                                    i += 1;
                                else
                                    break;
                            }
                            // здесь продолжаем двигаться игнорируя все в рамках тега
                            if (i < HTML.Length)
                                i += 1; // уйдем с символа >
                            while (i < HTML.Length)
                            {
                                if (HTML[i] != '>')
                                    i += 1;
                                else
                                    break;
                            }
                        }
                        else if (char.IsUpper(HTML, i))
                        {
                            i_start = i;
                            if (i < HTML.Length)
                                i += 1;
                            while (i < HTML.Length)
                            {
                                if (char.IsLower(HTML, i))
                                    i += 1;
                                else
                                    break;
                            }
                            // встали на символ следующий после окончания элемента
                            // i_end = i
                            Element = HTML.Substring(i_start, i - i_start);
                            ElList.Add(Element);
                            continue;
                        }
                        if (i < HTML.Length)
                            i += 1;
                    }
                    // Удалим на всякий случай все элементы "X", т.к. это не химические элементы
                    for (i = 0; i <= ElList.Count - 1; i++)
                    {
                        if (ElList[i] == "X")
                        {
                            ElList.RemoveAt(i);
                            i -= 1;
                        }
                    }
                    // создадим и заполним массив химических элементов
                    el = new string[ElList.Count]; // массив химических элементов
                    for (i = 0; i <= ElList.Count - 1; i++)
                        el[i] = ElList[i];
                    ElList.Clear();
                    ElList = null/* TODO Change to default(_) if this is not a reference type */;
                    Array.Sort(el); // отсортируем массив
                    st = "undefined";
                    for (i = el.GetLowerBound(0); i <= el.GetUpperBound(0); i++)
                    {
                        if (el[i] != st)
                        {
                            st = el[i];
                            // debug("i = " & i.ToString() & ", st = '" & st & "'")
                            mySystemString += st + "-";
                            mySystemXML += "<Element>" + st + "</Element>";
                            NumElements += 1;
                        }
                    }
                    mySystemString = "-" + mySystemString;
                    mySystemXML = "<ChemicalSystem>" + mySystemXML + "</ChemicalSystem>";
                    break;
                default:
                    // неизвестный режим команды
                    GetSystemStringByDescription = 10;
                    Response = "GetSystemStringByDescription: Неизвестный режим Mode";
                    break;
            }

            return GetSystemStringByDescription;
        }



        // по переданному XML-узлу Index (с дочерним узлом Element) подсчитывает его количественное вхождение
        // в состав вещества двигаясь вверх по иерархии до XML-узла ChemicalSubstance
        public static int GetCompositionForElementNode(XmlNode IndexNode, ref double valueMin, ref double valueMax, ref string Response)
        {
            int retVal = 0;
            Response = "";
            XmlNode valueNode, valueMinNode, valueMaxNode;

            if (IndexNode==null)
            {
                retVal = 1;
                Response = "GetCompositionForElementNode: IndexNode = NULL";
            }
            else if (IndexNode.Name == "Index")
            {
                valueNode = IndexNode.Attributes.GetNamedItem("value");
                if (valueNode==null)
                {
                    valueMinNode = IndexNode.Attributes.GetNamedItem("valueMin");
                    valueMaxNode = IndexNode.Attributes.GetNamedItem("valueMax");
                    if (valueMinNode == null || valueMaxNode==null)
                    {
                        retVal = 2;
                        Response = "GetCompositionForElementNode: Отсутствует атрибут value и не встречены атрибуты valueMin и valueMax";
                    }
                    else
                    {
                        double vMin, vMax;
                        bool result;
                        result = double.TryParse(valueMinNode.InnerText.Replace(",", "."), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out vMin);
                        if (!result)
                            vMin = double.NaN;
                        result = double.TryParse(valueMaxNode.InnerText.Replace(",", "."), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out vMax);
                        if (!result)
                            vMax = double.NaN;
                        valueMin *= vMin;
                        valueMax *= vMax;
                        IndexNode = IndexNode.ParentNode;
                        retVal = GetCompositionForElementNode(IndexNode, ref valueMin, ref valueMax, ref Response);
                    }
                }
                else
                {
                    double v;
                    bool result;
                    result = double.TryParse(valueNode.InnerText.Replace(",", "."), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out v);
                    if (!result)
                        v = double.NaN;
                    valueMin *= v;
                    valueMax *= v;
                    IndexNode = IndexNode.ParentNode;
                    retVal = GetCompositionForElementNode(IndexNode, ref valueMin, ref valueMax, ref Response);
                }
            }
            else if (IndexNode.Name != "ChemicalSubstance")
            {
                retVal = 10;
                Response = "GetCompositionForElementNode: Встречен недопустимый элемент " + IndexNode.Name;
            }

            return retVal;
        }



        /// <summary>
        /// получаем строковое описание вещества
        /// </summary>
        /// <param name="html">формула вещества</param>
        /// <param name="substanceXML">дополнительное возвращается XML с описание вещества</param>
        /// <returns>строка - описание вещества</returns>
        public static string GetSubstanceStringByHtml(string html, out string substanceXML) {
            XmlDocument doc = null;
            XmlNode OutChemicalSubstanceNode = null;
            string resp = "";

            REFACTOR_ChemicalUtils.ChemicalSubstanceXMLByHTML(ref doc, ref OutChemicalSubstanceNode, html, ref resp);

            int NumElements = 0;
            string mySystemXML = "";
            string mySystemString = "";
            substanceXML = "";
            string mySubstanceString = "";
            resp = "";
            REFACTOR_ChemicalUtils.GetSubstanceStringByDescription(ref OutChemicalSubstanceNode, ref NumElements, ref mySystemXML, ref mySystemString, ref substanceXML, ref mySubstanceString, ref resp);

            // string html = "LiNbO<sub>3</sub>";
            //Assert.AreEqual(mySystemXML, "<ChemicalSystem><Element>Li</Element><Element>Nb</Element><Element>O</Element></ChemicalSystem>");
            //Assert.AreEqual(mySystemString, "-Li-Nb-O-");

            //Assert.AreEqual(mySubstanceXML, "<ChemicalSubstanceComposition><Item Element=\"Li\" value=\"1\" /><Item Element=\"Nb\" value=\"1\" /><Item Element=\"O\" value=\"3\" /></ChemicalSubstanceComposition>");
            //Assert.AreEqual(mySubstanceString, "Li(1)Nb(1)O(3)");
            return mySubstanceString;
        }



        /// <summary>
        /// функция для построения строки и XML для БД по описанию вещества/смеси
        /// </summary>
        /// <param name="InXmlNode"></param>
        /// <param name="NumElements"></param>
        /// <param name="mySystemXML"></param>
        /// <param name="mySystemString"></param>
        /// <param name="mySubstanceXML"></param>
        /// <param name="mySubstanceString"></param>
        /// <param name="Response"></param>
        /// <returns></returns>
        public static void GetSubstanceStringByDescription(ref XmlNode InXmlNode, ref int NumElements, ref string mySystemXML, ref string mySystemString, ref string mySubstanceXML, ref string mySubstanceString, ref string Response)
        {
            Response = "";
            string st, HTML, stMin, stMax;
            XmlNodeList nodeList = null/* TODO Change to default(_) if this is not a reference type */;
            XmlNode ElNode = null/* TODO Change to default(_) if this is not a reference type */;
            double ElMin, ElMax;

            HTML = mySubstanceXML;
            NumElements = 0;
            mySystemString = "";
            mySystemXML = "";
            mySubstanceString = "";
            mySubstanceXML = "";
                    nodeList = InXmlNode.SelectNodes(".//Index/Element");
                    if (nodeList.Count > 0)
                    {
                        ChemicalElementQuantity[] ElQuan = new ChemicalElementQuantity[nodeList.Count];

                        string[] el = new string[nodeList.Count]; // массив химических элементов
                        double[] valueMin = new double[nodeList.Count]; // нижняя граница содержания элемента - valueMin
                        double[] valueMax = new double[nodeList.Count]; // верхняя граница содержания элемента - valueMax
                                                                                // x будем считать как NaN
                        int i = 0;
                        int res = 0;
                        foreach (XmlNode node in nodeList) // заполним массив 
                        {
                            ElMin = 1;
                            ElMax = 1;
                            st = "";
                            res = GetCompositionForElementNode(node.ParentNode, ref ElMin, ref ElMax, ref st);
                            ElQuan[i] = new ChemicalElementQuantity(node.InnerText, ElMin, ElMax);

                            i += 1;
                        }
                        Array.Sort(ElQuan); // отсортируем массив
                        st = "undefined";
                        List<ChemicalElementQuantity> AL = new List<ChemicalElementQuantity>();
                        int last_i=0;
                        AL.Clear();
                        for (i = ElQuan.GetLowerBound(0); i <= ElQuan.GetUpperBound(0); i++)
                        {
                            if (ElQuan[i].Element != st)
                            {
                                st = ElQuan[i].Element;
                                last_i = i;
                                AL.Add(ElQuan[i]);
                            }
                            else
                            {
                                ElQuan[last_i].Min += ElQuan[i].Min;
                                ElQuan[last_i].Max += ElQuan[i].Max;
                            }
                        }
                        // теперь пройдемся по сформированному списку и создадим строку и XML
                        NumElements = AL.Count;
                        if (AL.Count > 0)
                        {
                            foreach (ChemicalElementQuantity item in AL) {
                                mySystemString += item.Element + "-";
                                mySystemXML += "<Element>" + item.Element + "</Element>";
                                ElMin = item.Min;
                                ElMax = item.Max;
                                if (double.IsNaN(ElMin))
                                    stMin = "x";
                                else
                                    stMin = ElMin.ToString("g", NumberFormatInfo.InvariantInfo);
                                if (double.IsNaN(ElMax))
                                    stMax = "x";
                                else
                                    stMax = ElMax.ToString("g", NumberFormatInfo.InvariantInfo);
                                mySubstanceString += item.Element + "(" + stMin;
                                mySubstanceXML += "<Item Element=\"" + item.Element + "\" ";
                                if (stMin != stMax)
                                {
                                    mySubstanceString += "-" + stMax;
                                    mySubstanceXML += "valueMin=\"" + stMin + "\" valueMax=\"" + stMax + "\"";
                                }
                                else
                                    mySubstanceXML += "value=\"" + stMin + "\"";
                                mySubstanceString += ")";
                                mySubstanceXML += " />";
                            }
                        }
                        AL.Clear();
                        AL = null/* TODO Change to default(_) if this is not a reference type */;
                        mySystemString = "-" + mySystemString;
                        mySystemXML = "<ChemicalSystem>" + mySystemXML + "</ChemicalSystem>";
                        mySubstanceXML = "<ChemicalSubstanceComposition>" + mySubstanceXML + "</ChemicalSubstanceComposition>";
                    }
                    else
                    {
                        throw new ArgumentException("Не найдено элементов Element в описании химического вещества");
                    }
        }

        protected static int parseIndexes(ref double minIndex, ref double maxIndex, ref string Str, string Response)
        {
            int retVal = 0;
            Response = "";
            int i;
            bool result;

            minIndex = 0;
            maxIndex = 0;
            i = Str.IndexOf('-');
            if (i > -1)
            {
                // разберемся с минимальным индексом
                result = double.TryParse(Str.Substring(0, i), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out minIndex);
                if (!result)
                    minIndex = double.NaN;
                // разберемся с максимальным индексом
                result = double.TryParse(Str.Substring(i + 1, Str.Length - i - 1), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out maxIndex);
                if (!result)
                    maxIndex = double.NaN;
            }
            else
            {
                result = double.TryParse(Str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out minIndex);
                if (!result)
                    minIndex = double.NaN;
                maxIndex = minIndex;
            }

            return retVal;
        }


        protected static void CreateIndexNode(ref XmlDocument doc, ref XmlNode RootIndexNode, ref string Element, ref string NestedFormula, ref double minIndex, ref double maxIndex, ref string Response)
        {
            XmlNode tmpNode = null/* TODO Change to default(_) if this is not a reference type */;
            XmlAttribute tmpAttr = null/* TODO Change to default(_) if this is not a reference type */;
            string stMin, stMax;

            if (double.IsNaN(minIndex))
                stMin = "x";
            else
                stMin = minIndex.ToString("g", NumberFormatInfo.InvariantInfo);
            if (double.IsNaN(maxIndex))
                stMax = "x";
            else
                stMax = maxIndex.ToString("g", NumberFormatInfo.InvariantInfo);
            tmpNode = doc.CreateNode(XmlNodeType.Element, "Index", null/* TODO Change to default(_) if this is not a reference type */);
            if (stMin == stMax)
            {
                // атрибут value
                tmpAttr = doc.CreateAttribute("value");
                tmpAttr.Value = stMin;
                tmpNode.Attributes.SetNamedItem(tmpAttr);
            }
            else
            {
                // атрибут valueMin
                tmpAttr = doc.CreateAttribute("valueMin");
                tmpAttr.Value = stMin;
                tmpNode.Attributes.SetNamedItem(tmpAttr);
                // атрибут valueMax
                tmpAttr = doc.CreateAttribute("valueMax");
                tmpAttr.Value = stMax;
                tmpNode.Attributes.SetNamedItem(tmpAttr);
            }
            RootIndexNode.AppendChild(tmpNode);

            if (Element != "")
            {
                tmpNode.InnerXml = "<Element>" + Element + "</Element>";
                Element = "";
            }
            else if (NestedFormula != "")
            {
                // РЕКУРСИЯ
                ChemicalSubstanceXMLByHTMLRecursive(ref doc, ref tmpNode, NestedFormula, ref Response);
                NestedFormula = "";
            }
            // сбросим индексы
            minIndex = 1;
            maxIndex = 1;
        }

        // получаем XML-описание вещества/смеси по HTML-строке из HTML
        protected static void ChemicalSubstanceXMLByHTMLRecursive(ref XmlDocument doc, ref XmlNode RootIndexNode, string HTML, ref string Response)
        {
            double minIndex, maxIndex;
            string Element = "";
            string NestedFormula = "";
            minIndex = 1;
            maxIndex = 1;
            int i = 0;
            int i_start=0, i_end=0, flag=0;
            while (i < HTML.Length)
            {
                // ищем индекс (он должен быть в тегах <sub>...</sub>)
                if (HTML[i] == '<')
                {
                    i_start = i;
                    flag = 0;
                    while (i < HTML.Length)
                    {
                        if (HTML[i] != '>')
                            i += 1;
                        else
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 1)
                    {
                        if (HTML.IndexOf('/', i_start, i - i_start) > -1)
                        {
                            throw new ArgumentException("Встречен закрывающий тег " + HTML.Substring(i_start, i - i_start + 1) + ", тогда как ожидался открывающий");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Встречен признак открытия тега (<), которому не соответствует знак < (" + HTML.Substring(i_start, i - i_start + 1) + ")");
                    }
                    if (i < HTML.Length)
                        i += 1;
                    i_start = i;     // указывает на начало индекса
                    flag = 0;
                    while (i < HTML.Length)
                    {
                        if (HTML[i] != '<')
                            i += 1;
                        else
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 1)
                    {
                        i_end = i;   // указывает на "<" 
                        flag = 0;
                        while (i < HTML.Length)
                        {
                            if (HTML[i] != '>')
                                i += 1;
                            else
                            {
                                flag = 1;
                                break;
                            }
                        }
                        if (flag == 1)
                        {
                            if (HTML.IndexOf('/', i_start, i - i_start) == -1)
                            {
                                throw new ArgumentException("Встречен открывающий тег " + HTML.Substring(i_start, i - i_start) + ", тогда как ожидался закрывающий");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Встречен признак открытия тега '<', которому не соответствует признак закрытия тега '>' (" + HTML.Substring(i_start, i - i_start) + ")");
                        }
                        string s = HTML.Substring(i_start, i_end - i_start);
                        flag = parseIndexes(ref minIndex, ref maxIndex, ref s, Response);
                    }
                    else
                    {
                        throw new ArgumentException("Не обнаружен закрывающий тег");
                    }
                    if (i < HTML.Length)
                        i += 1; // уйдем с символа >
                    continue;
                }
                else if (char.IsUpper(HTML, i))
                {
                    i_start = i;
                    if (i < HTML.Length)
                        i += 1;
                    while (i < HTML.Length)
                    {
                        if (char.IsLower(HTML, i))
                            i += 1;
                        else
                            break;
                    }
                    if (Element != "" | NestedFormula != "")
                        CreateIndexNode(ref doc, ref RootIndexNode, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, ref Response);
                    // встали на символ следующий после окончания элемента
                    Element = HTML.Substring(i_start, i - i_start);
                    continue;
                }
                else if (HTML[i] == '(')
                {
                    i_start = i; // встали на (
                    flag = 0;
                    if (i < HTML.Length)
                        i += 1; // уйдем с символа (
                    while (i < HTML.Length)   // ищем )
                    {
                        if (HTML[i] == ')')
                        {
                            if (flag == 0)
                            {
                                i_end = i;
                                flag = -1;
                                break;
                            }
                            else
                                flag -= 1;
                        }
                        else if (HTML[i] == '(')
                            flag += 1;
                        if (i < HTML.Length)
                            i += 1;
                    }
                    if (flag == -1)
                    {
                        if (Element != "" | NestedFormula != "")
                            CreateIndexNode(ref doc, ref RootIndexNode, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, ref Response);
                        NestedFormula = HTML.Substring(i_start + 1, i_end - i_start - 1);
                    }
                    else
                    {
                        throw new ArgumentException("Не найден символ ')' парный к '('");
                    }
                    continue;
                }
                if (i < HTML.Length)
                    i += 1;
            }
            if (Element != "" | NestedFormula != "")
                CreateIndexNode(ref doc, ref RootIndexNode, ref Element, ref NestedFormula, ref minIndex, ref maxIndex, ref Response);

        }


        // получаем XML-описание вещества/смеси по HTML-строке из HTML
        public static void ChemicalSubstanceXMLByHTML(ref XmlDocument doc, ref XmlNode OutChemicalSubstanceNode, string HTML, ref string Response)
        {
            Response = "";
            XmlNode myXmlNode = null/* TODO Change to default(_) if this is not a reference type */;
            XmlNode myRootIndexNode = null/* TODO Change to default(_) if this is not a reference type */;
            HTML = HTML.Replace(",", ".");
            HTML = HTML.Replace(" ", "");
            HTML = HTML.Replace("-", "");
            HTML = HTML.Replace("[", "(");
            HTML = HTML.Replace("]", ")");
            // начинаем построение ответа для клиента
            if (doc==null)
                doc = new XmlDocument();
            if (OutChemicalSubstanceNode == null)
            {
                doc.LoadXml("<?xml version=\"1.0\" encoding=\"windows-1251\"?><ChemicalSubstance><Index value=\"1\" /></ChemicalSubstance>");
                OutChemicalSubstanceNode = doc.DocumentElement;
            }
            myRootIndexNode = OutChemicalSubstanceNode.SelectSingleNode("Index");
            ChemicalSubstanceXMLByHTMLRecursive(ref doc, ref myRootIndexNode, HTML, ref Response);
        }
    }
}
