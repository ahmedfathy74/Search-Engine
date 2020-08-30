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

namespace ConsoleApplication26
{
    class Program
    {
        static List<string> lis = new List<string>(); //Container carrying URL'S
        static int p = 20;
        static string URL = "https://www.bbc.com";
        static int counter = 1;
        static SqlConnection sqlConnection = new SqlConnection("Data Source=AHMEDFATHY-PC;Initial Catalog=crawlerdatabase;Integrated Security=True"); //Define sql connection
        static void Main(string[] args)
        {

            // Create a new 'WebRequest' object to the mentioned URL
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            myWebRequest.KeepAlive = false;
            myWebRequest.ProtocolVersion = HttpVersion.Version10;
            myWebRequest.ServicePoint.ConnectionLimit = 1;
            // The response object of 'WebRequest' is assigned to aWebResponse' variable.
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream streamResponse = myWebResponse.GetResponseStream();
            StreamReader sReader = new StreamReader(streamResponse);
            string rString = sReader.ReadToEnd();
            streamResponse.Close();
            sReader.Close();
            myWebResponse.Close();
            //===================================================//

            // at first add reference to mshtml from solution explorer
            IHTMLDocument2 myDoc = new HTMLDocumentClass();
            myDoc.write(rString);
            IHTMLElementCollection elements = myDoc.links;
            //===================================================//



            /*To handle the about urls , we needed to get the site name (ex:bbc.com)
             *  to complete the about url : (ex : about:/word) "
             * so result be (//bbc.com/word)
             */
            string empty = "/";
            int indx = URL.IndexOf('w');
            int length = URL.Length - indx;
            empty += '/';
            empty += URL.Substring(indx, length).Trim();
            //==================================================//
            //    sqlConnection.Open(); //Start connection                   
            foreach (IHTMLElement el in elements)
            {
                p = 20;

                //To check the list size till reaching 3000 links.
                if (lis.Count() > 3000)
                {
                    break;
                }
                //Get link
                string link = (string)el.getAttribute("href");
                //Check to remove the last slash from the link to avoid duplication in list.
                if (link[link.Length - 1] == '/')
                {
                    link = link.Remove(link.Length - 1);
                }
                //To handle about links :
                //Start by getting index of : then Check if the link size >7
                //as there might be ambigous links like (About:/)(blank:/)
                //Then removes each about about with http 
                //check if the link contains 2 slashes (//) or only (1) after the " : " 
                int indxxxx = link.IndexOf(':');
                if (link.Length > 7)
                {
                    if (link[0] == 'a' && link[4] == 't' && link[6] == '/' && link[7] == '/')
                    {
                        link = link.Replace("about", "http");
                    }
                }
                //Same as the previous but handle the only 1 / 

                if (link.Length > 7)
                {

                    if (link[0] == 'a' && link[4] == 't' && link[7] != '/')
                    {
                        link = link.Replace("about", "http");
                        //insert a / after the index of ":"
                        //so final link would be instead of : about:/world => http://www.bbc.com/world ////
                        link = link.Insert(indxxxx, empty);
                    }
                }

                //Before adding the link to the valid list we must filter it accoirding to 3 functions
                //1 : If the link is not already visited before 
                //2 : If the link is replying (valid) no errors..
                //3 : If the link is English supported 
                if (!lis.Contains(link) && CheckURLValid(link) && GetPage(link) && checkEnglish(rString))
                {
                    lis.Add(link);
                }

                if (CheckURLValid(link) && GetPage(link) && checkEnglish(rString))
                {
                    //Write the link and it's content into the db.
                      writeindatabase(link, rString);
                    //Go to recursion function with the link 
                    Console.WriteLine("URL Number " + counter + " : " + link);
                    counter++;
                    recursion(link);

                }


            }
            sqlConnection.Close();
        }
        static void recursion(string l)
        {

            string empty = "/";

            //From the bbc we have 285 links some of them is not working probably due to not found or not valid 
            //So we constantly gave the p initial value to get from each link of the 285, 20 links till we finsh them all.

            if (p == 0)
            {

                return;
            }
            int indx = URL.IndexOf('w');
            int length = URL.Length - indx;
            empty += '/';
            empty += URL.Substring(indx, length).Trim();

            // Create a new 'WebRequest' object to the mentioned URL
            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(l);
            myWebRequest.KeepAlive = false;
            myWebRequest.ProtocolVersion = HttpVersion.Version10;
            myWebRequest.ServicePoint.ConnectionLimit = 1;
            // The response object of 'WebRequest' is assigned to aWebResponse' variable.
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream streamResponse = myWebResponse.GetResponseStream();
            StreamReader sReader = new StreamReader(streamResponse);
            string rString = sReader.ReadToEnd();
            streamResponse.Close();
            sReader.Close();
            myWebResponse.Close();

            // at first add reference to mshtml from solution explorer
            IHTMLDocument2 myDoc = new HTMLDocumentClass();
            myDoc.write(rString);
            IHTMLElementCollection elements = myDoc.links;
            foreach (IHTMLElement el in elements)
            {

                if (p == 0)
                {
                    break;
                }
                string link = (string)el.getAttribute("href", 0);
                //Same handling as in main
                if (link[link.Length - 1] == '/')
                {
                    link = link.Remove(link.Length - 1);
                }
                int indxxxx = link.IndexOf(':');
                if (link.Length > 7)
                {
                    if (link[0] == 'a' && link[4] == 't' && link[6] == '/' && link[7] == '/')
                    {
                        link = link.Replace("about", "http");
                    }

                }
                if (link.Length > 7)
                {
                    if (link[0] == 'a' && link[4] == 't' && link[7] != '/')
                    {
                        link = link.Replace("about", "http");
                        link = link.Insert(indxxxx, empty);
                    }
                }
                //If the link is already visited continues.
                if (lis.Contains(link))
                {
                    continue;
                }
                else if (CheckURLValid(link) && GetPage(link) && checkEnglish(rString))
                {
                    Console.WriteLine("URL Number " + counter + " : " + link);
                    counter++;
                    p--;
                    lis.Add(link);


                    writeindatabase(link, rString);
                    //Start recursion till stopping condition is met.
                    recursion(link);
                }
            }

        }
        //In this function we take the rstring of the link and search for the lang or the hreflang . 
        //If they contains en that means they support english language  then returns true 
        //Else returns false
        public static bool checkEnglish(string rstring)
        {
            if (rstring.Contains("lang=\"en") || (rstring.Contains("hreflang =\"en")))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        //Function that takes the url or link and check if it is in the format of http or https..
        public static bool CheckURLValid(string strURL)
        {
            Uri uriResult;
            return Uri.TryCreate(strURL, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        //Chech if the page is valid and retrieve a reply 
        //if there is an error or notfound return false
        public static bool GetPage(String url)
        {
            bool flag = false;
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                    flag = true;
                // Releases the resources of the response.
                myHttpWebResponse.Close();
            }

            catch (Exception e)
            {

            }
            return flag;
        }
       // Function that writes in the database that gets the link and rstring then add it to the database.
            static void writeindatabase(String url, String Content)
            {
               /* sqlConnection.Open();
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "insert into crawlerdata values( '" + URL + "','" + PageContent + "')";
                command.ExecuteNonQuery();*/
           //SqlConnection con = new SqlConnection("Data Source=AHMEDFATHY-PC;Initial Catalog=crawlerdatabase;Integrated Security=True");
            sqlConnection.Open();
            string insertString = "INSERT INTO crawlerdata (URL,PageContent) VALUES (@URL,@PageContent)";
            SqlCommand cmd = new SqlCommand(insertString, sqlConnection);
            SqlParameter p1 = new SqlParameter("@URL", url);
            SqlParameter p2 = new SqlParameter("@PageContent", Content);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.ExecuteNonQuery();
            }
    }


}


