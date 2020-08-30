using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Finalmodule.Models;
namespace Finalmodule.Models
{
    public class algorithms
    {
        public string soundexalgorithm(string stringdata)
        {
            string x = "";
            x += stringdata[0];
            List<int> intgersss = new List<int>();
            for (int i = 1; i < stringdata.Length; i++)
            {
                x += changeletter(stringdata[i]);
                if (x[i - 1] == x[i])
                {
                    intgersss.Add(i);
                }
            }
            for (int y = intgersss.Count - 1; y >= 0; y--)
            {
                x = x.Remove(intgersss[y], 1);
            }
            x = x.Replace("0", "");
            x = x.Trim();
            while (x.Length < 4)
            {
                x += "0";
            }
            if (x.Length > 4)
            {
                x = x.Substring(0, 4);
            }
            return x;

        }
        public List<string> k_gram_algorithm(string data_str)
        {
            // ahmed ---> $ah,ahm,hme,med,ed$
            string y = "$" + data_str.Substring(0, 2);
            List<string> list1 = new List<string>();
            list1.Add(y);
            int j = 2;
            for (int i = 1; i < data_str.Length - 1; i++)
            {
                string x = list1[i - 1].Substring(1, 2);
                string x2 = x + data_str[j];
                j++;
                list1.Add(x2);
            }
            int last_two = data_str.Length - 2;
            string last = data_str.Substring(last_two, 2) + "$";
            list1.Add(last);
            return list1;
        }
        private char changeletter(char character)
        {
            if ("bfpv".Contains(character))
            {
                return '1';
            }
            if ("cgjkqsxz".Contains(character))
            {
                return '2';
            } 
            if ("dt".Contains(character))
            {
                return '3';
            } 
            if ('l' == character)
            {
                return '4';
            }
            if ("mn".Contains(character))
            {
                return '5';
            } 
            if ('r' == character)
            {
                return '6';
            } 

            return '0';
        }
    }
}