using System;
using System.Collections.Generic;
using System.Text;

namespace Elmax
{
    public class HyperElement
    {
        public delegate bool DoubleElementPrediate(Elmax.Element elem1, Elmax.Element elem2);

        public static List< KeyValuePair<Elmax.Element, Elmax.Element> >
	        JoinOneToOne(
	            List<Elmax.Element> vecElem1,
	            string attrName1,
	            List<Elmax.Element> vecElem2,
	            string attrName2,
	            bool bCaseSensitive)
        {
            string str1, str2;
            List<KeyValuePair<Elmax.Element, Elmax.Element>> vecResults = new List<KeyValuePair<Elmax.Element, Elmax.Element>>();
            for (int i = 0; i < vecElem1.Count; ++i)
            {
                if (attrName1 == "")
                    str1 = vecElem1[i].GetString("");
                else
                    str1 = vecElem1[i].Attribute(attrName1).GetString("");

                for (int j = 0; j < vecElem2.Count; ++j)
                {
                    if (attrName2 == "")
                        str2 = vecElem2[j].GetString("");
                    else
                        str2 = vecElem2[j].Attribute(attrName2).GetString("");

                    if (bCaseSensitive)
                    {
                        if (str1 == str2)
                        {
                            vecResults.Add(new KeyValuePair<Elmax.Element, Elmax.Element>(vecElem1[i], vecElem2[j]));
                            break;
                        }
                    }
                    else
                    {
                        if (str1.ToLower() == str2.ToLower())
                        {
                            vecResults.Add(new KeyValuePair<Elmax.Element, Elmax.Element>(vecElem1[i], vecElem2[j]));
                            break;
                        }
                    }
                }

            }

            return vecResults;
        }

        public static List<KeyValuePair<Elmax.Element, List<Elmax.Element>>>
            JoinOneToMany(
            List<Elmax.Element> vecElem1,
            string attrName1,
            List<Elmax.Element> vecElem2,
            string attrName2,
            bool bCaseSensitive)
        {
            string str1, str2;
            List<KeyValuePair<Elmax.Element, List<Elmax.Element>>> vecResults = new List<KeyValuePair<Elmax.Element, List<Elmax.Element>>>();
            bool makepair = false;
            for (int i = 0; i < vecElem1.Count; ++i)
            {
                makepair = false;
                if (attrName1 == "")
                    str1 = vecElem1[i].GetString("");
                else
                    str1 = vecElem1[i].Attribute(attrName1).GetString("");

                for (int j = 0; j < vecElem2.Count; ++j)
                {
                    if (attrName2 == "")
                        str2 = vecElem2[j].GetString("");
                    else
                        str2 = vecElem2[j].Attribute(attrName2).GetString("");

                    if (bCaseSensitive)
                    {
                        if (str1 == str2)
                        {
                            if (makepair == false)
                            {
                                List<Elmax.Element> vecChild = new List<Elmax.Element>();
                                vecResults.Add(new KeyValuePair<Elmax.Element, List<Elmax.Element>>(vecElem1[i], vecChild));
                                makepair = true;
                            }
                            vecResults[vecResults.Count - 1].Value.Add(vecElem2[j]);
                        }
                    }
                    else
                    {
                        if (str1.ToLower() == str2.ToLower())
                        {
                            if (makepair == false)
                            {
                                List<Elmax.Element> vecChild = new List<Elmax.Element>();
                                vecResults.Add(new KeyValuePair<Elmax.Element, List<Elmax.Element>>(vecElem1[i], vecChild));
                                makepair = true;
                            }
                            vecResults[vecResults.Count - 1].Value.Add(vecElem2[j]);
                        }
                    }
                }

            }

            return vecResults;
        }
        public static List<KeyValuePair<Elmax.Element, Elmax.Element>>
            JoinOneToOne(
                List<Elmax.Element> vecElem1,
                List<Elmax.Element> vecElem2,
                DoubleElementPrediate pred)
        {
            List<KeyValuePair<Elmax.Element, Elmax.Element>> vecResults = new List<KeyValuePair<Elmax.Element, Elmax.Element>>();
            for (int i = 0; i < vecElem1.Count; ++i)
            {
                for (int j = 0; j < vecElem2.Count; ++j)
                {
                    if (pred(vecElem1[i], vecElem2[j]))
                    {
                        vecResults.Add(new KeyValuePair<Elmax.Element, Elmax.Element>(vecElem1[i], vecElem2[j]));
                        break;
                    }
                }

            }

            return vecResults;
        }

        public static List<KeyValuePair<Elmax.Element, List<Elmax.Element>>>
            JoinOneToMany(
            List<Elmax.Element> vecElem1,
            List<Elmax.Element> vecElem2,
            DoubleElementPrediate pred)
        {
            List<KeyValuePair<Elmax.Element, List<Elmax.Element>>> vecResults = new List<KeyValuePair<Elmax.Element, List<Elmax.Element>>>();
            bool makepair = false;
            for (int i = 0; i < vecElem1.Count; ++i)
            {
                makepair = false;

                for (int j = 0; j < vecElem2.Count; ++j)
                {
                    if (pred(vecElem1[i], vecElem2[j]))
                    {
                        if (makepair == false)
                        {
                            List<Elmax.Element> vecChild = new List<Elmax.Element>();
                            vecResults.Add(new KeyValuePair<Elmax.Element, List<Elmax.Element>>(vecElem1[i], vecChild));
                            makepair = true;
                        }
                        vecResults[vecResults.Count - 1].Value.Add(vecElem2[j]);
                    }
                }

            }

            return vecResults;
        }

    }
}
