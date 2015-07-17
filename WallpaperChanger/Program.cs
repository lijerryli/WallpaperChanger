/*
 * Name: Jerry Li
 * Description: Program which fetches an image off reddit and sets it as your wallpaper (imgur links only)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;

namespace WallpaperChanger
{
    class Program
    {
        //code for setting wallpaper from http://alanbondo.wordpress.com/2008/06/21/changing-the-desktop-wallpaper-with-c/
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(
            UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        private static void SetWallpaper(String path)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }


        static void Main(string[] args)
        {
            //create webclient for accessing html and pic
            WebClient client = new WebClient();

            //which subreddit to get the image off of
            string subReddit = "earthporn";

            //load in full html code
            string html = client.DownloadString("http://www.reddit.com/r/" + subReddit);

            //declare variables
            bool found = false; //if the image has been found
            int post = 0; //location of post
            string newHtml; //html chunk
            string imgLink = ""; //the image link

            try
            {
                //loop until a valid image link is found
                while (!found)
                {
                    //find the first post
                    post = html.IndexOf("thing id-t3", post + 1);

                    //make sure it links to imgur
                    if (html.Substring(post, 1000).Contains("http://imgur.com"))
                    {
                        //get a chunk of the html code which includes the link
                        newHtml = html.Substring(post, 1000);

                        //find the actual image link
                        imgLink = newHtml.Substring(newHtml.IndexOf("http://imgur.com"), 24);

                        //make sure it isn't a part of a gallery
                        if (!imgLink.Contains("gall") && !imgLink.Contains("/a/"))
                        {
                            found = true;
                        }
                    }

                    //check for i.imgur url as well
                    else if (html.Substring(post, 1000).Contains("http://i.imgur.com"))
                    {
                        newHtml = html.Substring(post, 1000);

                        imgLink = newHtml.Substring(newHtml.IndexOf("http://i.imgur.com"), 26);

                        if (!imgLink.Contains("gall") && !imgLink.Contains("/a/"))
                        {
                            found = true;
                        }
                    }
                }


                if (found)
                {
                    //dl the pic
                    client.DownloadFile(imgLink + ".jpg", "background.jpg");

                    //set as background
                    SetWallpaper(Directory.GetCurrentDirectory() + "\\background.jpg");
                }

                //if it can't be found
                else
                {
                    Console.WriteLine("No Image Found");
                    Console.ReadKey();
                }
            }

            catch
            {
                Console.WriteLine("No Image Found");
                Console.ReadKey();
            }

            client.Dispose();
        }
    }
}
