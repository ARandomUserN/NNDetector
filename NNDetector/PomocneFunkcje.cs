using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace NNDetector
{
    static class PomocneFunkcje
    {
        static bool czy_ma_adres_IPv4(String URL)
        {
            /*bool werdykt = false;
            String wzorzec = "([0-9]{1,3}\\.){3}[0-9]{1,3}";
            if (Regex.IsMatch(URL, wzorzec))
            {
                werdykt = true;
            }
            return werdykt; */
            return Regex.IsMatch(URL, "([0-9]{1,3}\\.){3}[0-9]{1,3}");
        }

        static bool czy_ma_numer_portu(String URL)
        {
            return Regex.IsMatch(URL, "\\:[0-9]{1,5}");
        }

        public static double[] opisz_adres_liczbami(String adresURL)
        {
            double[] opis = new double[11] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            //kolumna 1 - Having_IPhaving_IP_Address
            if (czy_ma_adres_IPv4(adresURL))
            {
                opis[0] = 0;
            }
            //kolumna 2 - URLURL_Length
            if (adresURL.Length > 60)
            {
                opis[1] = 0;
            }
            //kolumna 3 - Shortining_Service
            if (adresURL.Contains("bit.ly"))
            {
                opis[2] = 0;
            }
            //kolumna 4 - Having_At_Symbol
            if (adresURL.Contains('@'))
            {
                opis[3] = -1;
            }
            //kolumna 5 - Double_slash_redirecting
            int ostatni_znak_http = -1; //potrzebne pozniej
            if (adresURL.Substring(0, 4).Equals("http"))
            {
                ostatni_znak_http = 3;
                if (adresURL[4] == 's')
                {
                    ostatni_znak_http++;
                }
                if (adresURL.Substring(ostatni_znak_http + 1, 3).Equals("://"))
                {
                    if (adresURL.Substring(ostatni_znak_http + 4).Contains("//"))
                    {
                        opis[4] = -1;
                    }
                }
                else
                {
                    opis[4] = 0;
                    if (adresURL.Substring(ostatni_znak_http + 1).Contains("//"))
                    {
                        opis[4] = -1;
                    }
                }
            }
            //kolumna 6 - Prefix_Suffix
            int poczatek_bez_http = 0;
            if (ostatni_znak_http > 2)
            {
                if (adresURL.Substring(ostatni_znak_http + 1, 3).Equals("://"))
                {
                    poczatek_bez_http = ostatni_znak_http + 4;
                    String adres_bez_http = adresURL.Substring(poczatek_bez_http);
                    int gdzie_poczatki_podstron = adres_bez_http.IndexOf('/');
                    if (gdzie_poczatki_podstron == -1)
                    {
                        if (adres_bez_http.Contains('-'))
                        {
                            opis[5] = 0;
                        }
                    }
                    else
                    {
                        if (adres_bez_http.Substring(0, gdzie_poczatki_podstron).Contains('-'))
                        {
                            opis[5] = 0;
                        }
                    }

                }
            }
            //kolumna 7 - Having_Sub_Domain
            //kolumna 8 - SSLfinal_State
            if (!adresURL.StartsWith("https://"))
            {
                opis[7] = 0;
            }
            //kolumna 9 - Port
            if (czy_ma_numer_portu(adresURL))
            {
                opis[8] = -1;
            }
            //kolumna 10 - HTTPS_token
            if (adresURL.Substring(poczatek_bez_http).Contains("https"))
            {
                opis[9] = 0;
            }
            //kolumna 11 - SFH
            //kolumna 12 - Submitting_to_email
            if (adresURL.Contains("mailto:"))
            {
                opis[10] = 0;
            }
            //kolumna 13 - Abnormal_URL
            return opis;
        }

    }
}
