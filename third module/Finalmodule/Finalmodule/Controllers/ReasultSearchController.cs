using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Resources;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using Finalmodule.Models;

namespace Finalmodule.Controllers
{
    public class ReasultSearchController : Controller
    {
        // GET: ReasultSearch
        public ActionResult Index()
        {
            var ll = new Finallinks() { boolval = null,searchname="" };
            return View(ll);
        }
        // k graaaaaaaaaaaaaaaaaaaaaaaaam
        //////
        ///
        [Route("ReasultSearch/Trigram/{Search_String2}")]
        public ActionResult Trigram(string Search_String2)
        // doing all index steps
        {
            if (Search_String2 == "emptystring")
            {
                var lll = new Finallinks() { boolval = null, searchname = "" };
                return View(lll);
            }
            string exact_search = "";
            if (Search_String2.Contains('"'))
            {
                var reg = new Regex("\".*?\"");
                var matches = reg.Matches(Search_String2);
                foreach (var item in matches)
                {
                    exact_search = item.ToString().Trim('"');
                }
            }
            else
            {
                exact_search = Search_String2;
            }
            string[] tokens2 = exact_search.Split(',', ' ', '.', ':', '\t', '\n', '\r');
            List<string> liss2 = new List<string>();
            foreach (var words2 in tokens2)
            {
                // using regex to remove punctuation characters from every word   (step 3) -> req 1
                string word2 = Regex.Replace(words2, @"[^\w\d\s]", "");
                word2 = Regex.Replace(word2, @"\d", "");
                // if the word is empty after removing punctuation characters continues and don't save it
                if (word2 == "")
                {
                    continue;
                }
                word2 = word2.TrimEnd();
                word2 = word2.ToLower(); //case folding in  (step 3) -> req 2
                liss2.Add(word2);
            }
            string[] qw4 = liss2.ToArray();
            var stemmer = new PorterStemmer();
            List<string> stopwords2 = new List<string>() { "a", "about", "above", "above", "across", "after", "afterwards", "again", "against", "all", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "amoungst", "amount", "an", "and", "another", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "are", "around", "as", "at", "back", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides", "between", "beyond", "bill", "both", "bottom", "but", "by", "call", "can", "cannot", "cant", "co", "con", "could", "couldnt", "cry", "de", "describe", "detail", "do", "done", "down", "due", "during", "each", "eg", "eight", "either", "eleven", "else", "elsewhere", "empty", "enough", "etc", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "few", "fifteen", "fify", "fill", "find", "fire", "first", "five", "for", "former", "formerly", "forty", "found", "four", "from", "front", "full", "further", "get", "give", "go", "had", "has", "hasnt", "have", "he", "hence", "her", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "him", "himself", "his", "how", "however", "hundred", "ie", "if", "in", "inc", "indeed", "interest", "into", "is", "it", "its", "itself", "keep", "last", "latter", "latterly", "least", "less", "ltd", "made", "many", "may", "me", "meanwhile", "might", "mill", "mine", "more", "moreover", "most", "mostly", "move", "much", "must", "my", "myself", "name", "namely", "neither", "never", "nevertheless", "next", "nine", "no", "nobody", "none", "noone", "nor", "not", "nothing", "now", "nowhere", "of", "off", "often", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise", "our", "ours", "ourselves", "out", "over", "own", "part", "per", "perhaps", "please", "put", "rather", "re", "same", "see", "seem", "seemed", "seeming", "seems", "serious", "several", "she", "should", "show", "side", "since", "sincere", "six", "sixty", "so", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "still", "such", "system", "take", "ten", "than", "that", "the", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thick", "thin", "third", "this", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too", "top", "toward", "towards", "twelve", "twenty", "two", "un", "under", "until", "up", "upon", "us", "very", "via", "was", "we", "well", "were", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "will", "with", "within", "without", "would", "yet", "you", "your", "yours", "yourself", "yourselves", "the" };
            List<string> List_tokens2 = new List<string>(qw4);
            List<string> List_tokenss2 = new List<string>();
            /// list of items for soundex///////////////////
            List<string> List_word_gram = new List<string>();
            /// 
            foreach (var item22 in List_tokens2)
            {
                if (stopwords2.Contains(item22))
                {
                    //List_tokens2.Remove(item22);
                    continue;
                }
                else
                {
                    List_word_gram.Add(item22);
                }

            }
            List<List<string>> LIST_grams_eword = new List<List<string>>();
            //List<string> final_res_soundex = new List<string>();
            // perform for all query words the kgram algorithms
            var kgramindex = new algorithms();
            foreach (var itemsou in List_word_gram)
            {
                string str1 = itemsou;
                List<string> saved_grames = new List<string>();
                saved_grames = kgramindex.k_gram_algorithm(str1);
                LIST_grams_eword.Add(saved_grames);
            }

            SqlConnection con1 = new SqlConnection();
            //ahmedfathy-pc
            int check2 = 0;
            Dictionary<string, string> kgramsave = new Dictionary<string, string>();
            List<string> final_cal = new List<string>();
            con1.ConnectionString = "Data Source=.\\MYSQLSERVER2014;Initial Catalog=Crawler;Integrated Security=True; MultipleActiveResultSets=true";
            con1.Open();
            using (con1)
            {
                string searchQueryone1 = "Select * from Kgram_Index";
                SqlCommand cmd121 = new SqlCommand(searchQueryone1, con1);
                SqlDataReader rdr21 = cmd121.ExecuteReader();
                while (check2 == 0)
                {
                    if (rdr21.HasRows)
                    {
                        // search direct 
                        check2 = 1;
                        /// return the first word that have the same code for all query words 
                        /// 
                        int y = 0;
                        foreach (var item2 in LIST_grams_eword)
                        {
                            List<string> list2 = new List<string>();
                            list2 = item2;
                            List<string> list3 = new List<string>();
                            for (int i = 0; i < list2.Count; i++)
                            {
                                string searchQuerytwo2 = "Select * from Kgram_Index where gram = @zip22two";
                                SqlCommand cmdd = new SqlCommand(searchQuerytwo2, con1);
                                cmdd.Parameters.AddWithValue("@zip22two", list2[i]);
                                SqlDataReader rdrrr = cmdd.ExecuteReader();
                                while (rdrrr.Read())
                                {
                                    string listofterms = (string)rdrrr["terms"];
                                    list3.Add(listofterms);
                                    break;
                                }
                            }
                            // jaccard coeffient
                            Dictionary<string,string> diclist = new Dictionary<string,string>();
                            int t = 0;
                            string str1 = list3[t];
                            t++;
                            string[] arr = str1.Split(',');
                            for (int i = 0; i < arr.Length; i++)
                            {
                              
                                for (int j = t; j <list3.Count; j++)
                                {
                                    string[] arr2 = list3[j].Split(',');
                                    if (arr2.Contains(arr[i]))
                                    {
                                        if (diclist.ContainsKey(arr[i]))
                                        {
                                            string pos = diclist[arr[i]];  
                                            pos = pos + "," + j.ToString();
                                            diclist[arr[i]] = pos;
                                        }
                                        else
                                        {
                                            diclist.Add(arr[i],"0");
                                            string pos = diclist[arr[i]];
                                            pos = pos + "," + j.ToString();
                                            diclist[arr[i]] = pos;
                                        }
                                    }
                                    
                                }
                                
                            }
                            Dictionary<string, double> jaccard_cal = new Dictionary<string, double>();
                            foreach (KeyValuePair<string, string> iter2 in diclist)
                            {
                                string x = iter2.Value;
                                string[] arrt = x.Split(',');
                                int index = Int32.Parse(arrt[0]);
                                string[] temp = list3[index].Split(',');
                                List<string> lis = new List<string>(temp);
                                for (int i = 1; i < arrt.Length; i++)
                                {
                                    int index2 = Int32.Parse(arrt[i]);
                                    string[] temp2 = list3[index2].Split(',');
                                    for (int j = 0; j < temp2.Length; j++)
                                    {
                                        
                                        if (lis.Contains(temp2[j]))
                                        {
                                            continue;
                                        }
                                        //else
                                        //{
                                        //    lis.Add(temp2[j]);
                                        //}
                                    }
                                }
                                double arrsize = arrt.Length;
                                double lissize = lis.Count;
                                var  jaccard = 0.0;
                                jaccard = (arrsize / lissize);
                                if (jaccard >= 0.05)
                                {
                                    jaccard_cal.Add(iter2.Key, jaccard);
                                }
                             
                            }

                            foreach (KeyValuePair<string, double> itemtwo in jaccard_cal.OrderByDescending(key => key.Value))
                            {
                                final_cal.Add(itemtwo.Key);
                                break;
                            }

                        }
            


                    }
                    ////implement k_gram for all terms in table termsbefore stemming
                    else
                    {
                        // for all words in the termbefore stemming table
                        // excute kgram algorithhm
                        string searchQuerythreee1 = "Select * from TermsBStemming_Table";
                        SqlCommand commendd1 = new SqlCommand(searchQuerythreee1, con1);
                        SqlDataReader reader41 = commendd1.ExecuteReader();
                        while (reader41.Read())
                        {
                            string term = (string)reader41["termBstemming"];
                            List<string> saved_grames = new List<string>();
                            saved_grames = kgramindex.k_gram_algorithm(term);
                            foreach (var item in saved_grames)
                            {
                                if (kgramsave.ContainsKey(item))
                                {
                                    string res = kgramsave[item];
                                    string[] ss = res.Split(',');
                                    if (ss.Contains(term))
                                    {
                                        continue;
                                    }
                                    res = res + "," + term;
                                    kgramsave[item] = res;
                                }
                                else
                                {
                                    kgramsave.Add(item, term);
                                }
                            }
                            

                        }
                        // save in the k_gram_index table
                        reader41.Close();
                        foreach (KeyValuePair<string, string> iter1 in kgramsave)
                        {
                            string insertString34 = "INSERT INTO Kgram_Index (gram,terms) VALUES (@gram,@terms)";
                            SqlCommand cmd31 = new SqlCommand(insertString34, con1);
                            SqlParameter par11 = new SqlParameter("@gram", iter1.Key);
                            SqlParameter par21 = new SqlParameter("@terms", iter1.Value);
                            cmd31.Parameters.Add(par11);
                            cmd31.Parameters.Add(par21);
                            cmd31.ExecuteNonQuery();
                        }
                    }
                }
                //rdr2.Close();
                      
                        

            }
            con1.Close();
            //return the words that will be smailir
            string final_word_query = "";
            foreach (var x in final_cal)
            {
                string c = (string)x;
                final_word_query = final_word_query + "" + c;
            }
            var wordsss = new List<Links>();
            var word_query = new Links() { Link_Name = final_word_query };
            wordsss.Add(word_query);
            var listlinkss2 = new Finallinks
            {
                Listoflinks = wordsss,
                boolval = "Kgramcheck",
                searchname = Search_String2
            };
            return View(listlinkss2);
        }


        [Route("ReasultSearch/Soundex/{Search_String2}")]
        public ActionResult Soundex(string Search_String2)
            // doing all index steps
        {
            if (Search_String2 == "emptystring")
            {
                var lll = new Finallinks() { boolval = null, searchname = "" };
                return View(lll);
            }
            string exact_search = "";
            if (Search_String2.Contains('"'))
            {
                var reg = new Regex("\".*?\"");
                var matches = reg.Matches(Search_String2);
                foreach (var item in matches)
                {
                    exact_search = item.ToString().Trim('"');
                }
            }
            else
            {
                exact_search = Search_String2;
            }
                string[] tokens2 = exact_search.Split(',', ' ', '.', ':', '\t', '\n', '\r');
                List<string> liss2 = new List<string>();
                foreach (var words2 in tokens2)
                {
                    // using regex to remove punctuation characters from every word   (step 3) -> req 1
                    string word2 = Regex.Replace(words2, @"[^\w\d\s]", "");
                    word2 = Regex.Replace(word2, @"\d", "");
                    // if the word is empty after removing punctuation characters continues and don't save it
                    if (word2 == "")
                    {
                        continue;
                    }
                    word2 = word2.TrimEnd();
                    word2 = word2.ToLower(); //case folding in  (step 3) -> req 2
                    liss2.Add(word2);
                }
                string[] qw3 = liss2.ToArray();
                List<string> stopwords2 = new List<string>() { "a", "about", "above", "above", "across", "after", "afterwards", "again", "against", "all", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "amoungst", "amount", "an", "and", "another", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "are", "around", "as", "at", "back", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides", "between", "beyond", "bill", "both", "bottom", "but", "by", "call", "can", "cannot", "cant", "co", "con", "could", "couldnt", "cry", "de", "describe", "detail", "do", "done", "down", "due", "during", "each", "eg", "eight", "either", "eleven", "else", "elsewhere", "empty", "enough", "etc", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "few", "fifteen", "fify", "fill", "find", "fire", "first", "five", "for", "former", "formerly", "forty", "found", "four", "from", "front", "full", "further", "get", "give", "go", "had", "has", "hasnt", "have", "he", "hence", "her", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "him", "himself", "his", "how", "however", "hundred", "ie", "if", "in", "inc", "indeed", "interest", "into", "is", "it", "its", "itself", "keep", "last", "latter", "latterly", "least", "less", "ltd", "made", "many", "may", "me", "meanwhile", "might", "mill", "mine", "more", "moreover", "most", "mostly", "move", "much", "must", "my", "myself", "name", "namely", "neither", "never", "nevertheless", "next", "nine", "no", "nobody", "none", "noone", "nor", "not", "nothing", "now", "nowhere", "of", "off", "often", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise", "our", "ours", "ourselves", "out", "over", "own", "part", "per", "perhaps", "please", "put", "rather", "re", "same", "see", "seem", "seemed", "seeming", "seems", "serious", "several", "she", "should", "show", "side", "since", "sincere", "six", "sixty", "so", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "still", "such", "system", "take", "ten", "than", "that", "the", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thick", "thin", "third", "this", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too", "top", "toward", "towards", "twelve", "twenty", "two", "un", "under", "until", "up", "upon", "us", "very", "via", "was", "we", "well", "were", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "will", "with", "within", "without", "would", "yet", "you", "your", "yours", "yourself", "yourselves", "the" };
                List<string> List_tokens2 = new List<string>(qw3);
                List<string> List_tokenss2 = new List<string>();
                /// list of items for soundex///////////////////
                List<string> List_Bsoundex = new List<string>();
                /// 
                foreach (var item22 in List_tokens2)
                {
                    if (stopwords2.Contains(item22))
                    {
                        //List_tokens2.Remove(item22);
                        continue;
                    }
                    else
                    {
                        List_Bsoundex.Add(item22);
                    }

                }
                List<string> List_asoundex = new List<string>();
                List<string> final_res_soundex = new List<string>();
            // perorm for all query words the soundex algorithms
                var soundex_var = new algorithms();
                foreach(var itemsou in List_Bsoundex){
                    string str1 = itemsou;  
                    string soundexalgo = soundex_var.soundexalgorithm(str1);
                    List_asoundex.Add(soundexalgo);
                }

                SqlConnection con1 = new SqlConnection();
                int check = 0;
                 Dictionary<string, string> soundexsave = new Dictionary<string, string>();
                //ahmedfathy-pc
                con1.ConnectionString = "Data Source=.\\MYSQLSERVER2014;Initial Catalog=Crawler;Integrated Security=True; MultipleActiveResultSets=true";
                con1.Open();
                 using (con1)
                {

                        string searchQueryone = "Select * from soundex_table";
                        SqlCommand cmd12 = new SqlCommand(searchQueryone, con1);
                        SqlDataReader rdr2 = cmd12.ExecuteReader();
                        while(check==0){
                             if(rdr2.HasRows){
                            // search direct 
                                 check = 1;
                                 /// return the first word that have the same code for all query words 
                                 for(int i = 0; i < List_asoundex.Count; i++)
                                 {
                                     string searchQuerytwo2 = "Select * from soundex_table where soundex_code = @zip2two";
                                     SqlCommand cmdd = new SqlCommand(searchQuerytwo2, con1);
                                     cmdd.Parameters.AddWithValue("@zip2two", List_asoundex[i]);
                                     SqlDataReader rdrrr = cmdd.ExecuteReader();
                                     while (rdrrr.Read())
                                     {
                                         string s_code = (string)rdrrr["soundex_code"];
                                         if (List_asoundex[i] == s_code)
                                         {
                                             string terms_ret = (string)rdrrr["Terms"];
                                             string[] list_terms = terms_ret.Split(',');
                                             final_res_soundex.Add(list_terms[0]);
                                             break;
                                         }
                                     }
                                 }
                                 
                            
                        }
                            ////implement soundex for all terms in table termsbefore stemming
                        else{
                                 // for all words in the termbefore stemming table
                                 // excute siundex algorithhm
                            string searchQuerythreee = "Select * from TermsBStemming_Table";
                            SqlCommand commendd = new SqlCommand(searchQuerythreee, con1);
                            SqlDataReader reader4 = commendd.ExecuteReader();
                            while(reader4.Read()){
                               string term = (string)reader4["termBstemming"];
                               string soundex = soundex_var.soundexalgorithm(term);
                               if (soundexsave.ContainsKey(soundex))
                                {
                                    string res = soundexsave[soundex];
                                    res = res + "," +term;
                                    soundexsave[soundex] = res;
                                } 
                                else
                                {
                                    soundexsave.Add(soundex, term);
                                }

                            }
                                 // save in the soundex table
                                   reader4.Close();
                                   foreach (KeyValuePair<string,string> iter1 in soundexsave)
                                   {
                                   string insertString3 = "INSERT INTO soundex_table (soundex_code,Terms) VALUES (@soundex_code,@Terms)";
                                    SqlCommand cmd3 = new SqlCommand(insertString3, con1);
                                    SqlParameter par1 = new SqlParameter("@soundex_code",iter1.Key);
                                    SqlParameter par2 = new SqlParameter("@Terms", iter1.Value);
                                    cmd3.Parameters.Add(par1);
                                    cmd3.Parameters.Add(par2);
                                    cmd3.ExecuteNonQuery();
                                  }
                             }
                        }
                        //rdr2.Close();
                      
                        
                    
                }
                con1.Close();
            //return the words that will be smailir
            string final_word_query = "";
            foreach (var x in final_res_soundex)
            {
                string c = (string)x;
                final_word_query = final_word_query + "" + c;
            }
            var wordsss = new List<Links>();
            var word_query = new Links() { Link_Name = final_word_query };
            wordsss.Add(word_query);
            var listlinkss2 = new Finallinks
            {
                Listoflinks = wordsss,
                boolval = "soundexcheck",
                searchname = Search_String2
            };
            return View(listlinkss2);
        }
        [Route("ReasultSearch/Search/{Search_String}")]
        public ActionResult Index(string Search_String)
        {
          // check if the search string is empty , return a null value
            if (Search_String == "emptystring")
            {
                var lll = new Finallinks() { boolval = null, searchname = "" };
                return View(lll);
            }
           ///////////////////////////////////////////////////////////////////
          //////                                 exact search key word                      ////////////


            //////////////////////////////////////////////////////////////////////////////////////////////
            /// if the word between double quotes
            if (Search_String.Contains('"'))
            {
                // exract the word from ' '
                var reg = new Regex("\".*?\"");
                var matches = reg.Matches(Search_String);
                string exact_search = "";
                foreach (var item in matches)
                {
                    exact_search = item.ToString().Trim('"');
                }
                // using index steps
                // 1 step split using some chracters
                string[] tokens2 = exact_search.Split(',', ' ', '.', ':', '\t', '\n', '\r');
                List<string> liss2 = new List<string>();
               
                foreach (var words2 in tokens2)
                {
                    // using regex to remove punctuation characters from every word   (step 3) -> req 1
                    string word2 = Regex.Replace(words2, @"[^\w\d\s]", "");
                    word2 = Regex.Replace(word2, @"\d", "");
                    // if the word is empty after removing punctuation characters continues and don't save it
                    if (word2 == "")
                    {
                        continue;
                    }
                    word2 = word2.TrimEnd();  
                    word2 = word2.ToLower(); //case folding in  (step 3) -> req 2
                    liss2.Add(word2);
                }
                string[] qw2 = liss2.ToArray();
                var stemmer = new PorterStemmer(); 
                List<string> stopwords2 = new List<string>() { "a", "about", "above", "above", "across", "after", "afterwards", "again", "against", "all", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "amoungst", "amount", "an", "and", "another", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "are", "around", "as", "at", "back", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides", "between", "beyond", "bill", "both", "bottom", "but", "by", "call", "can", "cannot", "cant", "co", "con", "could", "couldnt", "cry", "de", "describe", "detail", "do", "done", "down", "due", "during", "each", "eg", "eight", "either", "eleven", "else", "elsewhere", "empty", "enough", "etc", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "few", "fifteen", "fify", "fill", "find", "fire", "first", "five", "for", "former", "formerly", "forty", "found", "four", "from", "front", "full", "further", "get", "give", "go", "had", "has", "hasnt", "have", "he", "hence", "her", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "him", "himself", "his", "how", "however", "hundred", "ie", "if", "in", "inc", "indeed", "interest", "into", "is", "it", "its", "itself", "keep", "last", "latter", "latterly", "least", "less", "ltd", "made", "many", "may", "me", "meanwhile", "might", "mill", "mine", "more", "moreover", "most", "mostly", "move", "much", "must", "my", "myself", "name", "namely", "neither", "never", "nevertheless", "next", "nine", "no", "nobody", "none", "noone", "nor", "not", "nothing", "now", "nowhere", "of", "off", "often", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise", "our", "ours", "ourselves", "out", "over", "own", "part", "per", "perhaps", "please", "put", "rather", "re", "same", "see", "seem", "seemed", "seeming", "seems", "serious", "several", "she", "should", "show", "side", "since", "sincere", "six", "sixty", "so", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "still", "such", "system", "take", "ten", "than", "that", "the", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thick", "thin", "third", "this", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too", "top", "toward", "towards", "twelve", "twenty", "two", "un", "under", "until", "up", "upon", "us", "very", "via", "was", "we", "well", "were", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "will", "with", "within", "without", "would", "yet", "you", "your", "yours", "yourself", "yourselves", "the" };
                List<string> List_tokens2 = new List<string>(qw2);
                List<string> List_tokenss2 = new List<string>();
                // fremove stop words from the query
                foreach (var item22 in List_tokens2)
                {
                    if (stopwords2.Contains(item22))
                    {
                        continue;
                    }
                    else
                    {
                        /// perform stemming algorithm for all query words
                        string stem = stemmer.StemWord(item22);
                        List_tokenss2.Add(stem);
                    }

                }
                ///////////
                SqlConnection con2 = new SqlConnection();
                //ahmedfathy-pc
                var linkss2 = new List<Links>();
                con2.ConnectionString = "Data Source=.\\MYSQLSERVER2014;Initial Catalog=Crawler;Integrated Security=True; MultipleActiveResultSets=true";
                con2.Open();
                Dictionary<int, int> documents_ranked_freq2 = new Dictionary<int, int>();
                Dictionary<int, int> avarage_dic2 = new Dictionary<int, int>();
                List<string> links_from_db2 = new List<string>();

                using (con2)
                {
                    // loop for all query words and extract every word
                   
                    for (int i = 0; i < List_tokenss2.Count; i++)
                    {
                        // extract all words from inverted index that have the same wwords in the query
                        string searchQuerytwo = "Select * from Inverted_Index where Term = @ziptwo";
                        SqlCommand cmd12 = new SqlCommand(searchQuerytwo, con2);
                        cmd12.Parameters.AddWithValue("@ziptwo", List_tokenss2[i]);
                        SqlDataReader rdr2 = cmd12.ExecuteReader();
                        Dictionary<int, KeyValuePair<int, string>> equal_docid2 = new Dictionary<int, KeyValuePair<int, string>>();
                        while (rdr2.Read())
                        {
                            // get all atrributes for each record
                            int docid12 = (int)rdr2["DocID"];
                            int frequency12 = (int)rdr2["Frequency"];
                            string Positions12 = (string)rdr2["position"];
                            // if i have one word search i will usiing search ranked
                            if (List_tokenss2.Count == 1)
                            {
                                // if this word in the list add the freq
                                // else adda new word
                                if (documents_ranked_freq2.ContainsKey(docid12))
                                {
                                    int freqtemp = documents_ranked_freq2[docid12];
                                    freqtemp += frequency12;
                                    documents_ranked_freq2[docid12] = freqtemp;
                                }
                                else
                                {
                                    documents_ranked_freq2.Add(docid12, frequency12);
                                }
                            }
                                // i will use poistion in the search in eaxact search
                            else
                            {
                                // if this word in this word in the dic i will add the frequency and positions
                                // else add a new record in the dictinoary
                                if (equal_docid2.ContainsKey(docid12))
                                {
                                    int freq = equal_docid2[docid12].Key;
                                    freq += frequency12;
                                    string temp = equal_docid2[docid12].Value;
                                    temp += Positions12;
                                    equal_docid2[docid12] = (new KeyValuePair<int, string>(freq, temp));
                                }
                                else
                                {
                                    equal_docid2.Add(docid12, new KeyValuePair<int, string>(frequency12, Positions12));
                                }
                            }

                        }
                        rdr2.Close();
                        // this loop for take a first word with a second to search exact llike
                        // "home link" first loop  will take home and second will take link and start search between them
                        for (int j = i + 1; j < List_tokenss2.Count; j++)
                        {

                            string searchQuery21two = "Select * from Inverted_Index where Term = @zziiptwo";
                            SqlCommand cmd2two = new SqlCommand(searchQuery21two, con2);
                            cmd2two.Parameters.AddWithValue("@zziiptwo", List_tokenss2[j]);
                            SqlDataReader rdrr2 = cmd2two.ExecuteReader();
                            Dictionary<int, KeyValuePair<int, string>> equal_docid_firstloop = new Dictionary<int, KeyValuePair<int, string>>();

                            while (rdrr2.Read())
                            {
                                int docid22 = (int)rdrr2["DocID"];
                                int frequency22 = (int)rdrr2["Frequency"];
                                string Positions22 = (string)rdrr2["position"];
                                // collect for each word number of frequncy and list of positions for this worrd
                                // if the word exist add the new positions and add a new frequency
                                if (equal_docid_firstloop.ContainsKey(docid22))
                                {
                                    int freq2 = equal_docid_firstloop[docid22].Key;
                                    freq2 += frequency22;
                                    string temp2 = equal_docid_firstloop[docid22].Value;
                                    temp2 += Positions22;
                                    equal_docid_firstloop[docid22] = (new KeyValuePair<int, string>(freq2, temp2));
                                }
                                // else add a new record ini the DC for this word
                                else
                                {
                                    equal_docid_firstloop.Add(docid22, new KeyValuePair<int, string>(frequency22, Positions22));
                                }

                            }
                            rdrr2.Close();
                            /// for each item in the dictionary
                            /// 
                            // calcuate the the difrrence between the positions
                            foreach (var iter_item2 in equal_docid_firstloop)
                            {
                                if (equal_docid2.ContainsKey(iter_item2.Key))
                                {
                                    string posot2_temp2 = equal_docid_firstloop[iter_item2.Key].Value.Substring(0, equal_docid_firstloop[iter_item2.Key].Value.Length - 1);
                                    string[] arr22 = posot2_temp2.Split(',');
                                    var numbers22 = arr22.Select(x => Int32.Parse(x)).ToList();
                                    int totalfreq2 = 0, freq1 = 0;
                                    int temp22 = 0;
                                    string poss1_temp2 = equal_docid2[iter_item2.Key].Value.Substring(0, equal_docid2[iter_item2.Key].Value.Length - 1);
                                    string[] arr32 = poss1_temp2.Split(',');
                                    var numbers12 = arr32.Select(x => Int32.Parse(x)).ToList();
                                    // calcullate the diffrenece between words the diffrence between them qual 1
                                    foreach (var pos12 in numbers12)
                                    {
                                        int min2 = 0;
                                        foreach (var pos22 in numbers22)
                                        {

                                            min2 = pos22 - pos12;
                                            if (min2 < 0)
                                            {
                                                continue;
                                            }
                                            if (min2 == 1)
                                            {
                                                freq1 = equal_docid2[iter_item2.Key].Key;
                                                temp22 = 1;
                                                break;
                                            }

                                        }
                                        // ranking using frequncy if the exact search is coorect 
                                        if (temp22 == 1)
                                        {
                                            totalfreq2 = freq1 + equal_docid_firstloop[iter_item2.Key].Key;
                                            break;
                                        }

                                    }
                                        avarage_dic2.Add(iter_item2.Key, totalfreq2);
                                }
                            }

                            //nhayt second loop
                        }
                        
                    }
                    /// get from the database all links from this dictionsry using positions
                    foreach (KeyValuePair<int, int> itemtwo in avarage_dic2.OrderByDescending(key => key.Value))
                    {
                        string searchQuerytw2 = "Select URL from crawler_Table where doc_id=" + itemtwo.Key;
                        SqlCommand cmd3two = new SqlCommand(searchQuerytw2, con2);
                        SqlDataReader finalrdr2 = cmd3two.ExecuteReader();
                        while (finalrdr2.Read())
                        {
                            links_from_db2.Add(finalrdr2[0].ToString());
                        }
                    }
                    // get all links from tha database using frequency
                    foreach (var item2two in documents_ranked_freq2.OrderByDescending(key2 => key2.Value))
                    {
                        string searchQuery4two = "Select URL from crawler_Table where doc_id=" + item2two.Key;
                        SqlCommand cmd4 = new SqlCommand(searchQuery4two, con2);
                        SqlDataReader finalrdrr2 = cmd4.ExecuteReader();
                        while (finalrdrr2.Read())
                        {
                            links_from_db2.Add(finalrdrr2[0].ToString());
                        }
                    }

                }
                con2.Close();
                // add this links to the front web page
                foreach (var linkktwo in links_from_db2)
                {
                    var Linktwo = new Links() { Link_Name = linkktwo };
                    linkss2.Add(Linktwo);
                }
                var listlinkss2 = new Finallinks
                {
                    Listoflinks = linkss2,
                    boolval = "test",
                    searchname = Search_String
                };
                return View(listlinkss2);

            }
            ////////////////////////////////////////////////////
            ///////////////////////////////////////////////////
            //                                                      multi search keyword//


            /////////////////////////////////////////////////////////////////////////////////////
            //// the same steps for multi but  use the postions not excat ex any words in the page will be rertived in the search
            string[] tokens = Search_String.Split(',', ' ', '.', ':', '\t', '\n', '\r');
            List<string> liss = new List<string>();
            
            foreach (var words in tokens)
            {
                
                // using regex to remove punctuation characters from every word   (step 3) -> req 1
                string word = Regex.Replace(words, @"[^\w\d\s]", "");
                word = Regex.Replace(word, @"\d", "");
                // if the word is empty after removing punctuation characters continues and don't save it
                if (word == "")
                {
                    continue;
                }
                word = word.TrimEnd();
                word = word.ToLower(); //case folding in  (step 3) -> req 2
                liss.Add(word);
                
            }
            string []qw = liss.ToArray();
            var stemmer2 = new PorterStemmer(); 
            List<string> stopwords = new List<string>() { "a", "about", "above", "above", "across", "after", "afterwards", "again", "against", "all", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "amoungst", "amount", "an", "and", "another", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "are", "around", "as", "at", "back", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides", "between", "beyond", "bill", "both", "bottom", "but", "by", "call", "can", "cannot", "cant", "co", "con", "could", "couldnt", "cry", "de", "describe", "detail", "do", "done", "down", "due", "during", "each", "eg", "eight", "either", "eleven", "else", "elsewhere", "empty", "enough", "etc", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "few", "fifteen", "fify", "fill", "find", "fire", "first", "five", "for", "former", "formerly", "forty", "found", "four", "from", "front", "full", "further", "get", "give", "go", "had", "has", "hasnt", "have", "he", "hence", "her", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "him", "himself", "his", "how", "however", "hundred", "ie", "if", "in", "inc", "indeed", "interest", "into", "is", "it", "its", "itself", "keep", "last", "latter", "latterly", "least", "less", "ltd", "made", "many", "may", "me", "meanwhile", "might", "mill", "mine", "more", "moreover", "most", "mostly", "move", "much", "must", "my", "myself", "name", "namely", "neither", "never", "nevertheless", "next", "nine", "no", "nobody", "none", "noone", "nor", "not", "nothing", "now", "nowhere", "of", "off", "often", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise", "our", "ours", "ourselves", "out", "over", "own", "part", "per", "perhaps", "please", "put", "rather", "re", "same", "see", "seem", "seemed", "seeming", "seems", "serious", "several", "she", "should", "show", "side", "since", "sincere", "six", "sixty", "so", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "still", "such", "system", "take", "ten", "than", "that", "the", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thick", "thin", "third", "this", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too", "top", "toward", "towards", "twelve", "twenty", "two", "un", "under", "until", "up", "upon", "us", "very", "via", "was", "we", "well", "were", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "will", "with", "within", "without", "would", "yet", "you", "your", "yours", "yourself", "yourselves", "the" };
            List<string> List_tokens = new List<string>(qw);
            List<string> List_tokenss = new List<string>();
            foreach (var item2 in List_tokens)
            {
                if (stopwords.Contains(item2))
                {
                    //List_tokens.Remove(item2);
                    continue;
                }
                else
                {
                    string stem2 = stemmer2.StemWord(item2);
                    List_tokenss.Add(stem2);
                }
                
            }
            ////////////////////////////////////////////////////////////

            SqlConnection con = new SqlConnection();
            //ahmedfathy-pc
            var linkss = new List<Links>();
            con.ConnectionString = "Data Source=.\\MYSQLSERVER2014;Initial Catalog=Crawler;Integrated Security=True; MultipleActiveResultSets=true";
            con.Open();
            Dictionary<int, int> documents_ranked_freq = new Dictionary<int, int>();
            Dictionary<int, double> avarage_dic = new Dictionary<int, double>();
            List<string> links_from_db = new List<string>();
       
            using (con)
            {  
                for (int i = 0; i < List_tokenss.Count; i++)
                {
                    
                    string searchQuery = "Select * from Inverted_Index where Term = @zip";
                    SqlCommand cmd = new SqlCommand(searchQuery, con);
                    cmd.Parameters.AddWithValue("@zip", List_tokenss[i]);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    Dictionary<int, KeyValuePair<int, string>> equal_docid = new Dictionary<int, KeyValuePair<int, string>>();
                    while (rdr.Read())
                    {
                        int docid = (int)rdr["DocID"];
                        int frequency = (int)rdr["Frequency"];
                        string Positions = (string)rdr["position"];
                        if (List_tokenss.Count == 1)
                        {
                            if (documents_ranked_freq.ContainsKey(docid))
                            {
                                int freqtemp = documents_ranked_freq[docid];
                                freqtemp += frequency;
                                documents_ranked_freq[docid] = freqtemp;
                            }
                            else
                            {
                                documents_ranked_freq.Add(docid,frequency);
                            }
                        }
                        else {
                            if (equal_docid.ContainsKey(docid))
                            {
                                int freq = equal_docid[docid].Key;
                                freq += frequency;
                                string temp = equal_docid[docid].Value;
                                temp += Positions;
                                equal_docid[docid] = (new KeyValuePair<int, string>(freq, temp));
                            }
                            else
                            {
                                equal_docid.Add(docid, new KeyValuePair<int, string>(frequency, Positions));
                            }
                        }
                        
                    }
                    rdr.Close();
                    for (int j = i + 1; j < List_tokenss.Count; j++)
                    {
                       
                        string searchQuery2= "Select * from Inverted_Index where Term = @zziip";
                        SqlCommand cmd2 = new SqlCommand(searchQuery2, con);
                        cmd2.Parameters.AddWithValue("@zziip", List_tokenss[j]);
                        SqlDataReader rdrr = cmd2.ExecuteReader();
                        Dictionary<int, KeyValuePair<int, string>> equal_docid_2loop = new Dictionary<int, KeyValuePair<int, string>>();
        
                        while (rdrr.Read())
                        {
                            int docid2 = (int)rdrr["DocID"];
                            int frequency2 = (int)rdrr["Frequency"];
                            string Positions2 = (string)rdrr["position"];
                            if (equal_docid_2loop.ContainsKey(docid2))
                            {
                                int freq = equal_docid_2loop[docid2].Key;
                                freq += frequency2;
                                string temp = equal_docid_2loop[docid2].Value;
                                temp += Positions2;
                                equal_docid_2loop[docid2] = (new KeyValuePair<int, string>(freq, temp));
                            }
                            else
                            {
                                equal_docid_2loop.Add(docid2, new KeyValuePair<int, string>(frequency2, Positions2));
                            }
                            
                        }
                        rdrr.Close();
                        foreach (var iter_item in equal_docid_2loop)
                        {
                            if (equal_docid.ContainsKey(iter_item.Key))
                            {
                                string posot2_temp = equal_docid_2loop[iter_item.Key].Value.Substring(0, equal_docid_2loop[iter_item.Key].Value.Length - 1);
                                string[] arr2 = posot2_temp.Split(',');
                                var numbers2 = arr2.Select(x => Int32.Parse(x)).ToList();
                                ////////////
                                double average = 0.0;
                                int reault = 0;
                                string poss1_temp = equal_docid[iter_item.Key].Value.Substring(0, equal_docid[iter_item.Key].Value.Length - 1);
                                string[] arr3 = poss1_temp.Split(',');
                                var numbers1 = arr3.Select(x => Int32.Parse(x)).ToList();
                                int counter = 0, counter2 = 0;
                                // calculation using l proximty by calcualting avarage for 3 min distance 
                                foreach (var pos1 in numbers1)
                                {
                                    int min = 0, temp = 1000000;
                                    foreach (var pos2 in numbers2)
                                    {

                                        min = pos2 - pos1;
                                        if (min < 0)
                                        {
                                            //min = min * (-1);
                                            counter++;
                                            continue;
                                        }
                                        if (min < temp)
                                        {
                                         // mkan counter2          
                                            temp = min;
                                        }

                                    }
                                    // temp result is changed 
                                    //if (temp != 1000000)
                                    //{
                                    //    
                                    //}
                                    counter2++;
                                    reault += temp;
                                }
                                // if the page donot have proximty , i will used freq to order pages
                                if (counter == (numbers1.Count * numbers2.Count))
                                {
                                    documents_ranked_freq.Add(iter_item.Key, equal_docid_2loop[iter_item.Key].Key);
                                    continue;
                                }
                                //else using proximty
                                average = (reault / counter2); 
                                avarage_dic.Add(iter_item.Key, average);
                            }
                                // if the doc id doeanot match with any one than i will add to freq list(for second word)
                            else
                            {
                                documents_ranked_freq.Add(iter_item.Key, equal_docid_2loop[iter_item.Key].Key);
                            }
                        }
                        // add the doc id using freq (if this doc id is not in dic) (firt word)
                        foreach (var item_sec in equal_docid)
                        {
                            if (avarage_dic.ContainsKey(item_sec.Key))
                            {
                                continue;
                            }
                            if (documents_ranked_freq.ContainsKey(item_sec.Key))
                            {
                                int freqtemp2 = documents_ranked_freq[item_sec.Key];
                                freqtemp2 += equal_docid[item_sec.Key].Key;
                                documents_ranked_freq[item_sec.Key] = freqtemp2;
                                continue;
                            }
                            documents_ranked_freq.Add(item_sec.Key, equal_docid[item_sec.Key].Key);
                        }
                        //nhyt l second loop
                    }
                }
                // get the links from db using the positions
                foreach (KeyValuePair<int, double> item in avarage_dic.OrderByDescending(key => key.Value))
                {
                    string searchQuery3 = "Select URL from crawler_Table where doc_id=" + item.Key;
                    SqlCommand cmd3 = new SqlCommand(searchQuery3, con);
                    SqlDataReader finalrdr = cmd3.ExecuteReader();
                    while (finalrdr.Read())
                    {
                        links_from_db.Add(finalrdr[0].ToString());
                    }
                }
                // get all links from db using frequency
                foreach (var item2 in documents_ranked_freq.OrderByDescending(key2 => key2.Value))
                {
                    string searchQuery4 = "Select URL from crawler_Table where doc_id=" + item2.Key;
                    SqlCommand cmd4 = new SqlCommand(searchQuery4, con);
                    SqlDataReader finalrdrr = cmd4.ExecuteReader();
                    while (finalrdrr.Read())
                    {
                        links_from_db.Add(finalrdrr[0].ToString());
                    }
                }    
                
            }
            con.Close();
            foreach (var linkk in links_from_db)
            {
                var Link = new Links() { Link_Name = linkk };
                linkss.Add(Link);
            }
            var listlinkss = new Finallinks
            {
                Listoflinks=linkss,
                boolval="test",
                searchname=Search_String
            };
            return View(listlinkss);
        }
    }
}