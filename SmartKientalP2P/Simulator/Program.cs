using System;
using System.IO;

namespace Simulator
{
    /// <summary>
    /// Tool um LSB Testdaten direkt auf den FTP zu schreiben. Konfiguriert ist ftpintra Test/Q.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            int sleepTime = 60;

            if (args.Length == 1)
            {
                int.TryParse(args[0], out sleepTime);
            }


            // read template file
            string[] templateLines = File.ReadAllLines("template.csv", System.Text.Encoding.Default);
            int d = 0;

            while (true)
            {
                // generate filename
                DateTime newDate = DateTime.Now;
                string filename = string.Format("E_MW_01MIN_MOM_{0}_DBR4_key", newDate.Ticks);

                // write to stream
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(memStream, System.Text.Encoding.Default))
                    {
                        // replace dates
                        for (int i = 0; i < templateLines.Length; i++)
                        {
                            // template line
                            //1; 8156; "P WEH WHR Erlenbach                 ORG Stollen                   MWV    Ws-Stand"; 2016; 08; 31; 23; 59; 00; s; 141.247559; cm; D

                            // set current date
                            string line = templateLines[i].Replace("2017;05;15;15;58;00;", newDate.ToString("yyyy;MM;dd;HH;mm;00;"));

                            // add sin curve to the initial value (+/- 10%)
                            string[] parts = line.Split(new char[] { ';' }, StringSplitOptions.None);
                            double value = double.Parse(parts[10]);
                            value = value + value * 0.1 * Math.Sin(d * Math.PI / 180);
                            parts[10] = Math.Round(value, 4).ToString();

                            // write to stream
                            sw.WriteLine(string.Join(";", parts));
                        }

                        d++;
                        sw.Flush();

                        // reset stream position
                        memStream.Seek(0, SeekOrigin.Begin);

                        // upload file

                    
                        /*
                        // reset stream position
                        memStream.Seek(0, SeekOrigin.Begin);
                        if (!System.IO.Directory.Exists("files"))
                            System.IO.Directory.CreateDirectory("files");

                        // write to disk
                        using (FileStream fs = new FileStream("files/" + filename, FileMode.CreateNew))
                        {
                            memStream.CopyTo(fs);
                            fs.Flush();
                        }
                        */
                    }
                }

                // sleep for one minute
                System.Threading.Thread.Sleep(sleepTime * 1000);
            }
        }
    }
}
