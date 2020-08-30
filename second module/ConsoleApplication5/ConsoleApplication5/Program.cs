using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;
using mshtml;
using System.Text.RegularExpressions;
using System.Security.Authentication;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Resources;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using NetSpell;
using NetSpell.SpellChecker;
using NetSpell.SpellChecker.Dictionary;
using NetSpell.SpellChecker.Forms;

namespace ConsoleApplication26
{
    class Program
    {
       
        static void Main(string[] args)
        {
            // start connection with database
            SqlConnection sqlConnection = new SqlConnection("Data Source=AHMEDFATHY-PC;Initial Catalog=newDB;Integrated Security=True; MultipleActiveResultSets=true");
            sqlConnection.Open(); 
            // select statment to retrieve everything from database
            string queryString = "SELECT * FROM crawler_Table";
            SqlCommand cmd = new SqlCommand(queryString, sqlConnection);
            // declare variable from reader to read from database (all the content from  database)
            SqlDataReader rdr = cmd.ExecuteReader();

            int counterofopages = 0;   // counter for number of pages that i read it from database (at least 1500)

            // datastructure to save term and doc_id and frequency and list of positions for this term 
            List<KeyValuePair<string, KeyValuePair<int[], List<int>>>> indexmap = new List<KeyValuePair<string, KeyValuePair<int[], List<int>>>>();
           
            // while loop to read row by row from the reader
            while (rdr.Read()) {
                // this condition to break from loop when take at least 1500 page
                if (counterofopages == 1600)
                {
                    break;
                } 
                // try and catch to throw any exceptions out if it retreive null from innertext or something else 
                
                int boolll = 0; // boolean to check if the inner text has exception change boolean = 1 and skip tha link
                try
                {
                    //===================================================//
                    // retreive from each row docid , url (link) , content of the page (html page)
                    int doc_id = (int)rdr["doc_id"];
                    string url = (string)rdr["URL"];
                    string content = (string)rdr["Page_Content"];
                    //===================================================//

                    // pasre html page from database and get the inner text  (step 1)
                    IHTMLDocument2 myDoc = new HTMLDocumentClass();
                    myDoc.write(content);
                    string elements = myDoc.body.innerText;
                    //===================================================//
                                                                                //(it will be)
                    /// split in (step 2) (to take tokens and save it in array of strings named (tokens)
                    string[] tokens = elements.Split(',', ' ', '.', ':', '\t', '\n', '\r');

                    int i = 0; // counter to calculate the position for every term  

                    // check if any string it will be null or empty 
                    tokens = tokens.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    //===================================================//

                    /// saves every term and its list (positions) (s in dictionary named (termsandpos) before removing stop words 
                    Dictionary<string, List<int>> termsandpos = new Dictionary<string, List<int>>();
                    foreach (var words in tokens)
                    {
                        List<int> listofpos = new List<int>();
                        i++;
                        // using regex to remove punctuation characters from every word   (step 3) -> req 1
                        string word = Regex.Replace(words, @"[^\w\d\s]", "");
                        word = Regex.Replace(word, @"\d", "");
                        // if the word is empty after removing punctuation characters continues and don't save it
                        if (word == "")
                        {
                            continue;
                        }
                        // using spelling class from netspell reference and create object from it and using it to check if this word is real word in english or not.
                        Spelling ss = new Spelling();
                        // when the object from spelling class is used , the dialog window will opened and has many feature and i will closed by using next line to continue my run it's not used for my code.
                        ss.ShowDialog =false;
                        // check if this word is not found in dictionary in the spell library , continue ( go to the next word).
                        // esle continue the rest of the code (that is mean the word is found in the dictionary).
                        if (ss.SpellCheck(word))
                        {
                           continue;
                        }

                        word = word.ToLower(); //case folding in  (step 3) -> req 2

                        //If the word  is already existed ,add the new position in the list of this word 
                        if (termsandpos.ContainsKey(word))
                        {
                            listofpos = termsandpos[word];
                            listofpos.Add(i);
                            termsandpos[word] = listofpos;
                        }
                        // else, add the word and the first position 
                        else
                        {
                            listofpos.Add(i);
                            termsandpos.Add(word, listofpos);
                        }
                    }
                    //===================================================//

                    /////  stop words removing in (step 3) -> req 3
                    /// list of stop words 
                    /// create anthor dictinary to copy all terms without stop words
                    Dictionary<string, List<int>> temp = new Dictionary<string, List<int>>();
                    List<string> stopwords = new List<string>() { "a", "about", "above", "above", "across", "after", "afterwards", "again", "against", "all", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "amoungst", "amount", "an", "and", "another", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "are", "around", "as", "at", "back", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides", "between", "beyond", "bill", "both", "bottom", "but", "by", "call", "can", "cannot", "cant", "co", "con", "could", "couldnt", "cry", "de", "describe", "detail", "do", "done", "down", "due", "during", "each", "eg", "eight", "either", "eleven", "else", "elsewhere", "empty", "enough", "etc", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "few", "fifteen", "fify", "fill", "find", "fire", "first", "five", "for", "former", "formerly", "forty", "found", "four", "from", "front", "full", "further", "get", "give", "go", "had", "has", "hasnt", "have", "he", "hence", "her", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "him", "himself", "his", "how", "however", "hundred", "ie", "if", "in", "inc", "indeed", "interest", "into", "is", "it", "its", "itself", "keep", "last", "latter", "latterly", "least", "less", "ltd", "made", "many", "may", "me", "meanwhile", "might", "mill", "mine", "more", "moreover", "most", "mostly", "move", "much", "must", "my", "myself", "name", "namely", "neither", "never", "nevertheless", "next", "nine", "no", "nobody", "none", "noone", "nor", "not", "nothing", "now", "nowhere", "of", "off", "often", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise", "our", "ours", "ourselves", "out", "over", "own", "part", "per", "perhaps", "please", "put", "rather", "re", "same", "see", "seem", "seemed", "seeming", "seems", "serious", "several", "she", "should", "show", "side", "since", "sincere", "six", "sixty", "so", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "still", "such", "system", "take", "ten", "than", "that", "the", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thick", "thin", "third", "this", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too", "top", "toward", "towards", "twelve", "twenty", "two", "un", "under", "until", "up", "upon", "us", "very", "via", "was", "we", "well", "were", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "will", "with", "within", "without", "would", "yet", "you", "your", "yours", "yourself", "yourselves", "the" };
                    for (int f = 0; f < termsandpos.Count; f++)
                    {
                       // if the term is already existed in the stopwords list or the term is a single character like ii or i , continue (and go to the next term).
                        if (stopwords.Contains(termsandpos.Keys.ElementAt(f)) || termsandpos.Keys.ElementAt(f).Length <= 2)
                        {
                            continue;
                        }
                            // else ,that's mean the term is not a stop word then add it and its positions in the temp dictionary.
                        else
                        {
                            List<int> copyofpositions = new List<int>();
                            copyofpositions = termsandpos[termsandpos.Keys.ElementAt(f)];
                            temp.Add(termsandpos.Keys.ElementAt(f), copyofpositions);
                            
                        }

                    }
                    //===================================================//

                    ////  al stemming algorithm            (step 3) --> req 4
                    var stemmer = new PorterStemmer();    // declare object from claas of porterstemmer algorithm
                    Dictionary<string, List<int>> finalterm = new Dictionary<string, List<int>>();
                    foreach (KeyValuePair<string, List<int>> iter1 in temp)
                    {
                        //===================================================//

                        // add every term and its docid in table called (TermsBStemming_Table) in db before stemming (the note in step 3 -->req 4)
                        string insertString3 = "INSERT INTO TermsBStemming_Table (termBstemming,docID) VALUES (@termBstemming,@docID)";
                        SqlCommand cmd3 = new SqlCommand(insertString3, sqlConnection);
                        SqlParameter par1 = new SqlParameter("@termBstemming", iter1.Key);
                        SqlParameter par2 = new SqlParameter("@docID", doc_id);
                        cmd3.Parameters.Add(par1);
                        cmd3.Parameters.Add(par2);
                        cmd3.ExecuteNonQuery();
                        //===================================================//

                        List<int> listofpositions = new List<int>();
                        // called function (StemWord) and send the term and return term after stemming
                        string stem = stemmer.StemWord(iter1.Key);  
                        // check if this stem is already existed in finalterm dictionary (the new datastructure to save the term and its list after stemmnig)
                        if (finalterm.ContainsKey(stem))
                        {
                            List<int> tempforsimlir = new List<int>();
                            tempforsimlir = finalterm[stem]; // take the list of positions for this term (old positions added before for this term)
                            listofpositions = temp[iter1.Key]; // take the list of new positions for this term
                            /// added the new positions and old position in one list
                            for (int j = 0; j < listofpositions.Count; j++)
                            {
                                tempforsimlir.Add(listofpositions[j]);
                            }
                            // and save it again for the term
                            finalterm[stem] = tempforsimlir;

                        }
                        // addd the term ans its list to finalterm dictionary
                        else
                        {
                            listofpositions = temp[iter1.Key]; 
                            finalterm.Add(stem, listofpositions);
                        }
                    }

                    //===================================================//

                    ////  inverted index (step 4)
                   
                    foreach (KeyValuePair<string, List<int>> iter in finalterm)
                    {
                       
                        int freq = iter.Value.Count; // calculate freq through count number of positions
                        int[] arr = new int[2]; // save in this array doc id and the frequency
                        arr[0] = doc_id;
                        arr[1] = freq;
                        // convert list of the positions for every term to string 
                        var resultofpositions = string.Join(", ", iter.Value);
                        //===================================================//

                        // save term and docid ans=d frequency and (list of positions as string ) in table called Inverted_Index in db.
                        string insertString2 = "INSERT INTO Inverted_Index (Term,DocID,Frequency,position) VALUES (@Term,@DocID,@Frequency,@position)";
                        SqlCommand cmd2 = new SqlCommand(insertString2, sqlConnection);
                        SqlParameter paramter1 = new SqlParameter("@Term", iter.Key);
                        SqlParameter paramter2 = new SqlParameter("@DocID", doc_id);
                        SqlParameter paramter3 = new SqlParameter("@Frequency", freq);
                        SqlParameter paramter4 = new SqlParameter("@position", resultofpositions);
                        cmd2.Parameters.Add(paramter1);
                        cmd2.Parameters.Add(paramter2);
                        cmd2.Parameters.Add(paramter3);
                        cmd2.Parameters.Add(paramter4);
                        cmd2.ExecuteNonQuery();
                        //===================================================//
                        /// store in index list term and arrof ints (arr[0]=docid,arr[1] = freqs of every term) and list of all positions of this term (if i needed in ranks or something else).
                        indexmap.Add(new KeyValuePair<string, KeyValuePair<int[], List<int>>>(iter.Key, new KeyValuePair<int[], List<int>>(arr, iter.Value)));
                    }

                    //===================================================//

                }
                //===================================================//
                    //catch any type of exception and change the boolean that i decalred equal zero
                catch (NullReferenceException ex)
                {     
                    boolll = 1;
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentOutOfRangeException exx)
                {
                    boolll = 1;
                    Console.WriteLine(exx.Message);
                }
                // if the boolean became equal 1 , then leave this link and go to anthor link
                if (boolll == 1)
                {
                    continue;
                }

                //===================================================// 
                 /// to count number of pages (at least 1500 page)                                                                                          
                counterofopages++;
                //===================================================//
            }
            //===================================================//
            // close the reader from database
            rdr.Close();
            /// close the connection
            sqlConnection.Close();
            //===================================================//
        }
    }

}
/// <summary>
///  class of stemming algorithm (porter stemmer) /// open source
/// </summary>
public class PorterStemmer
{
    // The passed in word turned into a char array. 
    // Quicker to use to rebuilding strings each time a change is made.
    private char[] wordArray;

    // Current index to the end of the word in the character array. This will
    // change as the end of the string gets modified.
    private int endIndex;

    // Index of the (potential) end of the stem word in the char array.
    private int stemIndex;


    /// <summary>
    /// Stem the passed in word.
    /// </summary>
    /// <param name="word">Word to evaluate</param>
    /// <returns></returns>
    public string StemWord(string word)
    {

        // Do nothing for empty strings or short words.
        if (string.IsNullOrWhiteSpace(word) || word.Length <= 2) return word;

        wordArray = word.ToCharArray();

        stemIndex = 0;
        endIndex = word.Length - 1;
        Step1();
        Step2();
        Step3();
        Step4();
        Step5();
        Step6();

        var length = endIndex + 1;
        return new String(wordArray, 0, length);
    }


    // Step1() gets rid of plurals and -ed or -ing.
    /* Examples:
           caresses  ->  caress
           ponies    ->  poni
           ties      ->  ti
           caress    ->  caress
           cats      ->  cat

           feed      ->  feed
           agreed    ->  agree
           disabled  ->  disable

           matting   ->  mat
           mating    ->  mate
           meeting   ->  meet
           milling   ->  mill
           messing   ->  mess

           meetings  ->  meet  		*/
    private void Step1()
    {
        // If the word ends with s take that off
        if (wordArray[endIndex] == 's')
        {
            if (EndsWith("sses"))
            {
                endIndex -= 2;
            }
            else if (EndsWith("ies"))
            {
                SetEnd("i");
            }
            else if (wordArray[endIndex - 1] != 's')
            {
                endIndex--;
            }
        }
        if (EndsWith("eed"))
        {
            if (MeasureConsontantSequence() > 0)
                endIndex--;
        }
        else if ((EndsWith("ed") || EndsWith("ing")) && VowelInStem())
        {
            endIndex = stemIndex;
            if (EndsWith("at"))
                SetEnd("ate");
            else if (EndsWith("bl"))
                SetEnd("ble");
            else if (EndsWith("iz"))
                SetEnd("ize");
            else if (IsDoubleConsontant(endIndex))
            {
                endIndex--;
                int ch = wordArray[endIndex];
                if (ch == 'l' || ch == 's' || ch == 'z')
                    endIndex++;
            }
            else if (MeasureConsontantSequence() == 1 && IsCVC(endIndex)) SetEnd("e");
        }
    }

    // Step2() turns terminal y to i when there is another vowel in the stem.
    private void Step2()
    {
        if (EndsWith("y") && VowelInStem())
            wordArray[endIndex] = 'i';
    }

    // Step3() maps double suffices to single ones. so -ization ( = -ize plus
    // -ation) maps to -ize etc. note that the string before the suffix must give m() > 0. 
    private void Step3()
    {
        if (endIndex == 0) return;

        /* For Bug 1 */
        switch (wordArray[endIndex - 1])
        {
            case 'a':
                if (EndsWith("ational")) { ReplaceEnd("ate"); break; }
                if (EndsWith("tional")) { ReplaceEnd("tion"); }
                break;
            case 'c':
                if (EndsWith("enci")) { ReplaceEnd("ence"); break; }
                if (EndsWith("anci")) { ReplaceEnd("ance"); }
                break;
            case 'e':
                if (EndsWith("izer")) { ReplaceEnd("ize"); }
                break;
            case 'l':
                if (EndsWith("bli")) { ReplaceEnd("ble"); break; }
                if (EndsWith("alli")) { ReplaceEnd("al"); break; }
                if (EndsWith("entli")) { ReplaceEnd("ent"); break; }
                if (EndsWith("eli")) { ReplaceEnd("e"); break; }
                if (EndsWith("ousli")) { ReplaceEnd("ous"); }
                break;
            case 'o':
                if (EndsWith("ization")) { ReplaceEnd("ize"); break; }
                if (EndsWith("ation")) { ReplaceEnd("ate"); break; }
                if (EndsWith("ator")) { ReplaceEnd("ate"); }
                break;
            case 's':
                if (EndsWith("alism")) { ReplaceEnd("al"); break; }
                if (EndsWith("iveness")) { ReplaceEnd("ive"); break; }
                if (EndsWith("fulness")) { ReplaceEnd("ful"); break; }
                if (EndsWith("ousness")) { ReplaceEnd("ous"); }
                break;
            case 't':
                if (EndsWith("aliti")) { ReplaceEnd("al"); break; }
                if (EndsWith("iviti")) { ReplaceEnd("ive"); break; }
                if (EndsWith("biliti")) { ReplaceEnd("ble"); }
                break;
            case 'g':
                if (EndsWith("logi"))
                {
                    ReplaceEnd("log");
                }
                break;
        }
    }

    /* step4() deals with -ic-, -full, -ness etc. similar strategy to step3. */
    private void Step4()
    {
        switch (wordArray[endIndex])
        {
            case 'e':
                if (EndsWith("icate")) { ReplaceEnd("ic"); break; }
                if (EndsWith("ative")) { ReplaceEnd(""); break; }
                if (EndsWith("alize")) { ReplaceEnd("al"); }
                break;
            case 'i':
                if (EndsWith("iciti")) { ReplaceEnd("ic"); }
                break;
            case 'l':
                if (EndsWith("ical")) { ReplaceEnd("ic"); break; }
                if (EndsWith("ful")) { ReplaceEnd(""); }
                break;
            case 's':
                if (EndsWith("ness")) { ReplaceEnd(""); }
                break;
        }
    }

    /* step5() takes off -ant, -ence etc., in context <c>vcvc<v>. */
    private void Step5()
    {
        if (endIndex == 0) return;

        switch (wordArray[endIndex - 1])
        {
            case 'a':
                if (EndsWith("al")) break; return;
            case 'c':
                if (EndsWith("ance")) break;
                if (EndsWith("ence")) break; return;
            case 'e':
                if (EndsWith("er")) break; return;
            case 'i':
                if (EndsWith("ic")) break; return;
            case 'l':
                if (EndsWith("able")) break;
                if (EndsWith("ible")) break; return;
            case 'n':
                if (EndsWith("ant")) break;
                if (EndsWith("ement")) break;
                if (EndsWith("ment")) break;
                /* element etc. not stripped before the m */
                if (EndsWith("ent")) break; return;
            case 'o':
                if (EndsWith("ion") && stemIndex >= 0 && (wordArray[stemIndex] == 's' || wordArray[stemIndex] == 't')) break;
                /* j >= 0 fixes Bug 2 */
                if (EndsWith("ou")) break; return;
            /* takes care of -ous */
            case 's':
                if (EndsWith("ism")) break; return;
            case 't':
                if (EndsWith("ate")) break;
                if (EndsWith("iti")) break; return;
            case 'u':
                if (EndsWith("ous")) break; return;
            case 'v':
                if (EndsWith("ive")) break; return;
            case 'z':
                if (EndsWith("ize")) break; return;
            default:
                return;
        }
        if (MeasureConsontantSequence() > 1)
            endIndex = stemIndex;
    }

    /* step6() removes a final -e if m() > 1. */
    private void Step6()
    {
        stemIndex = endIndex;

        if (wordArray[endIndex] == 'e')
        {
            var a = MeasureConsontantSequence();
            if (a > 1 || a == 1 && !IsCVC(endIndex - 1))
                endIndex--;
        }
        if (wordArray[endIndex] == 'l' && IsDoubleConsontant(endIndex) && MeasureConsontantSequence() > 1)
            endIndex--;
    }

    // Returns true if the character at the specified index is a consonant.
    // With special handling for 'y'.
    private bool IsConsonant(int index)
    {
        var c = wordArray[index];
        if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u') return false;
        return c != 'y' || (index == 0 || !IsConsonant(index - 1));
    }

    /* m() measures the number of consonant sequences between 0 and j. if c is
       a consonant sequence and v a vowel sequence, and <..> indicates arbitrary
       presence,

          <c><v>       gives 0
          <c>vc<v>     gives 1
          <c>vcvc<v>   gives 2
          <c>vcvcvc<v> gives 3
          ....		*/
    private int MeasureConsontantSequence()
    {
        var n = 0;
        var index = 0;
        while (true)
        {
            if (index > stemIndex) return n;
            if (!IsConsonant(index)) break; index++;
        }
        index++;
        while (true)
        {
            while (true)
            {
                if (index > stemIndex) return n;
                if (IsConsonant(index)) break;
                index++;
            }
            index++;
            n++;
            while (true)
            {
                if (index > stemIndex) return n;
                if (!IsConsonant(index)) break;
                index++;
            }
            index++;
        }
    }

    // Return true if there is a vowel in the current stem (0 ... stemIndex)
    private bool VowelInStem()
    {
        int i;
        for (i = 0; i <= stemIndex; i++)
        {
            if (!IsConsonant(i)) return true;
        }
        return false;
    }

    // Returns true if the char at the specified index and the one preceeding it are the same consonants.
    private bool IsDoubleConsontant(int index)
    {
        if (index < 1) return false;
        return wordArray[index] == wordArray[index - 1] && IsConsonant(index);
    }

    /* cvc(i) is true <=> i-2,i-1,i has the form consonant - vowel - consonant
       and also if the second c is not w,x or y. this is used when trying to
       restore an e at the end of a short word. e.g.

          cav(e), lov(e), hop(e), crim(e), but
          snow, box, tray.		*/
    private bool IsCVC(int index)
    {
        if (index < 2 || !IsConsonant(index) || IsConsonant(index - 1) || !IsConsonant(index - 2)) return false;
        var c = wordArray[index];
        return c != 'w' && c != 'x' && c != 'y';
    }

    // Does the current word array end with the specified string.
    private bool EndsWith(string s)
    {
        var length = s.Length;
        var index = endIndex - length + 1;
        if (index < 0) return false;

        for (var i = 0; i < length; i++)
        {
            if (wordArray[index + i] != s[i]) return false;
        }
        stemIndex = endIndex - length;
        return true;
    }

    // Set the end of the word to s.
    // Starting at the current stem pointer and readjusting the end pointer.
    private void SetEnd(string s)
    {
        var length = s.Length;
        var index = stemIndex + 1;
        for (var i = 0; i < length; i++)
        {
            wordArray[index + i] = s[i];
        }
        // Set the end pointer to the new end of the word.
        endIndex = stemIndex + length;
    }

    // Conditionally replace the end of the word
    private void ReplaceEnd(string s)
    {
        if (MeasureConsontantSequence() > 0) SetEnd(s);
    }
    
}