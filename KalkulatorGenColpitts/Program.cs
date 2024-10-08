namespace KalkulatorGenColpitts
{
    using ConsoleApp2;
    using System.Globalization;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Transactions;







    internal class Program
    {
        private static void Main(string[] args)
        {
            uint Q = 40;
            uint Rl = 1 + Q * Q;
            uint R = 1000;
            uint C = 22;
            uint Cs;
            uint Lmin = 0;
            uint Lmax = 0;



            Complex gammaL =
                Complex.Zero;

            Complex gammaG =
            Complex.Zero;

            Complex Zg =
             Complex.Zero;

            Czwornik s = new Czwornik(50, 100);

            Console.WriteLine("Odczyt parametrów S");

            string file = "";
            try
            {
                StreamReader sr = new StreamReader("matrix.s2p");
                file = sr.ReadToEnd();
            }
            catch
            {
                    Environment.Exit(0);
            }

            

            string[] subs = file.Split('\n');


            double[] freqs = new double[subs.Length - 2];
            Czwornik[] matrixes = new Czwornik[subs.Length - 2];


            string[] subs2, subs2_;
            subs2 = new string[9];
            subs2_ = new string[9];
            uint freq_iter = 0;
            uint k = 1;
            Complex re = 0, imag = 0;
            for (uint i = 2; i < subs.Length; i++)
            {
                subs2 = subs[i].Split(" ");

                if (subs2.Length == 1) continue;
                freqs[i - 2] = Double.Parse(subs2[0], CultureInfo.InvariantCulture);
                freq_iter++;
                k = 1;
                for (uint j = 1; j < subs2.Length; j++)
                {
                    if (subs2[j] != "")
                    {
                        switch (k)
                        {
                            case 1:

                                re = new Complex(Double.Parse(subs2[j], CultureInfo.InvariantCulture), 0);
                                break;

                            case 2:
                                imag = new Complex(0, Double.Parse(subs2[j], CultureInfo.InvariantCulture));
                                matrixes[i - 2].s11 = Complex.FromPolarCoordinates(re.Real, ConvertToRadians(imag.Imaginary));
                                break;

                            case 3:
                                re = new Complex(Double.Parse(subs2[j], CultureInfo.InvariantCulture), 0);
                                break;

                            case 4:
                                imag = new Complex(0, Double.Parse(subs2[j], CultureInfo.InvariantCulture));
                                matrixes[i - 2].s21 = Complex.FromPolarCoordinates(re.Real, ConvertToRadians(imag.Imaginary));
                                break;

                            case 5:
                                re = new Complex(Double.Parse(subs2[j], CultureInfo.InvariantCulture), 0);
                                break;

                            case 6:
                                imag = new Complex(0, Double.Parse(subs2[j], CultureInfo.InvariantCulture));
                                matrixes[i - 2].s12 = Complex.FromPolarCoordinates(re.Real, ConvertToRadians(imag.Imaginary));
                                break;

                            case 7:
                                re = new Complex(Double.Parse(subs2[j], CultureInfo.InvariantCulture), 0);
                                break;

                            case 8:
                                imag = new Complex(0, Double.Parse(subs2[j], CultureInfo.InvariantCulture));
                                matrixes[i - 2].s22 = Complex.FromPolarCoordinates(re.Real, ConvertToRadians(imag.Imaginary));
                                break;

                            default: break;

                        }


                        k++;
                    }
                }


            }
            freq_iter--;
            double freq_step = (freqs[freq_iter] - freqs[0]) / freq_iter;
            Console.WriteLine("Odczytano czestotliwosci z zakresu {0}  -  {1} [MHz] ", freqs[0], freqs[freq_iter]);
            Console.WriteLine("Krok czestotliwosci = {0} MHz", freq_step);
            string mess;
            uint flag = 1;
            while (true)
            {
                switch (flag)
                {


                    case 1:


                        Console.WriteLine("Podaj czestotliwosc[MHz] :");

                        mess = Console.ReadLine();

                        try
                        {
                           s.freq = double.Parse(mess);
                        }
                        catch
                        {
                            Environment.Exit(0);
                        }
                       


                        if (s.freq < freqs[0]) s.freq = freqs[0];
                        if (s.freq > freqs[freq_iter])  s.freq = freqs[freq_iter];

                        uint index = 0;

                      

                        index = (uint)((s.freq - freqs[0]) / freq_step);

                        if (freqs[index] < s.freq) index++;

                        Console.WriteLine("Najbliższa wybranej czestotliwosc z pliku = {0} MHz", freqs[index]);

                        s.s11 = matrixes[index].s11;
                        s.s12 = matrixes[index].s12;
                        s.s21 = matrixes[index].s21;
                        s.s22 = matrixes[index].s22;
                        s.CalculateYparamsFromS();
                        s.RecalculateRolletFactor();

                        Console.WriteLine("s11 = {0}<{1}", s.s11.Magnitude, ConvertToDegrees(s.s11.Phase));
                        Console.WriteLine("s12 = {0}<{1}", s.s12.Magnitude, ConvertToDegrees(s.s12.Phase));
                        Console.WriteLine("s21 = {0}<{1}", s.s21.Magnitude, ConvertToDegrees(s.s21.Phase));
                        Console.WriteLine("s22 = {0}<{1}", s.s22.Magnitude, ConvertToDegrees(s.s22.Phase));

                        Console.WriteLine("Wsp Rolleta k = {0} ", s.Rollet_factor);


                        Console.WriteLine("Podaj minimalna wartosc induckyjnosci[nH] :");

                        mess = Console.ReadLine();
                        

                        try
                        {
                            Lmin = uint.Parse(mess);
                        }
                        catch
                        {
                            Environment.Exit(0);
                        }

                        Console.WriteLine("Podaj maksymalna wartosc induckyjnosci[nH] :");

                        mess = Console.ReadLine();
                       

                        try
                        {
                             Lmax = uint.Parse(mess);
                        }
                        catch
                        {
                            Environment.Exit(0);
                        }


                        Console.WriteLine("Podaj dobroc cewki :");

                        mess = Console.ReadLine();
                        

                        try
                        {
                           Q = uint.Parse(mess);
                        }
                        catch
                        {
                            Environment.Exit(0);
                        }

                        Console.WriteLine("Podaj wartosc pojemnosci obwodu rezonansowego[pF] :");

                        mess = Console.ReadLine();
                       

                        try
                        {
                             C = uint.Parse(mess);
                        }
                        catch
                        {
                            Environment.Exit(0);
                        }

                        Console.WriteLine("Podaj wartosc pojemnosci sprzezenia zwrotnego[pF] :");

                        mess = Console.ReadLine();

                        Cs = 0;
                        try
                        {
                            Cs = uint.Parse(mess);
                        }
                        catch
                        {
                            Environment.Exit(0);
                        }

                        Console.WriteLine("Podaj wartosci rezystancji obciazenia[Ohm] :");

                        mess = Console.ReadLine();
                        
                        try
                        {
                            R = uint.Parse(mess);
                        }
                        catch
                        {
                            Environment.Exit(0);
                        }

                        Rl = (1 + Q) * (1 + Q);
                        s.AddCouplingCap(Cs);

                        s.CalculateSparams();
                        s.RecalculateRolletFactor();

                        Console.WriteLine("s11 = {0}<{1}", s.s11.Magnitude, ConvertToDegrees(s.s11.Phase));
                        Console.WriteLine("s12 = {0}<{1}", s.s12.Magnitude, ConvertToDegrees(s.s12.Phase));
                        Console.WriteLine("s21 = {0}<{1}", s.s21.Magnitude, ConvertToDegrees(s.s21.Phase));
                        Console.WriteLine("s22 = {0}<{1}", s.s22.Magnitude, ConvertToDegrees(s.s22.Phase));
                        Console.WriteLine("Wsp Rolleta k = {0} \n\n", s.Rollet_factor);


                        double inductance = Lmin, inductance_temp = 0;
                        while (true/*s.s11_prim.Magnitude < 1*/)
                        {

                            gammaL = GetGammaL(inductance, s);

                            s.CalculateS11prim(gammaL);

                            inductance_temp = ChangeIncuctance(inductance, C, s);
                            if (inductance_temp >= Lmax) break;



                            Console.WriteLine("Indukcyjnosc [nH] = {0} ", inductance);
                            Console.WriteLine("S11 prim = {0}", s.s11_prim.Magnitude);

                            Console.WriteLine("gammaL = {0}<{1}\n ", gammaL.Magnitude, ConvertToDegrees(gammaL.Phase));
                            if (s.s11_prim.Magnitude >= 1) break;
                            if (inductance >= Lmax) break;
                            inductance += 1;


                        }

                        if ((inductance_temp < Lmax) && (inductance < Lmax))
                        {
                            Console.WriteLine("Niestabilnosc dla  induckyjnosci[nH] = {0} ", inductance_temp);
                            // Console.WriteLine("s11 prim = {0} ", s.s11_prim);
                            Console.WriteLine("s11 prim = {0}<{1}", s.s11_prim.Magnitude, ConvertToDegrees(s.s11_prim.Phase));

                            gammaG = 1 / s.s11_prim;
                            Console.WriteLine("gammaG ={0}", gammaG);
                            Console.WriteLine("gammaG = {0}<{1}", gammaG.Magnitude, ConvertToDegrees(gammaG.Phase));
                            Zg = s.z0_impedance * ((1 + gammaG) / (1 - gammaG));

                            Console.WriteLine("|Zg| = {0} ", Zg.Magnitude);
                            Console.WriteLine("Zg< = {0} ", ConvertToDegrees(Zg.Phase));
                            Console.WriteLine("Zg = {0} ", Zg);


                            if (ConvertToDegrees(s.s11_prim.Phase) < 0 && ConvertToDegrees(s.s11_prim.Phase) > -180)
                            {
                                Console.WriteLine("Pojedynczy kondensator nie wywola oscylacji");
                            }
                            else //if (ConvertToDegrees(Zg.Phase) > -180 && ConvertToDegrees(Zg.Phase) < 0)
                            {
                                Console.WriteLine("ESR[ohm] = {0} ", Zg.Real);
                                Console.WriteLine("C2[pF] = {0} ", -1000000 / (Math.PI * 2 * s.freq * Zg.Imaginary));
                            }


                        }
                        else
                        {
                            Console.WriteLine("Dla podanych parametrow generator nie zadziala");
                        }


                        flag = 2;
                        break;

                    case 2:
                        Console.WriteLine("Powtorzyc? t/n");
                        mess = Console.ReadLine();
                        if (String.Compare(mess, "t") == 0) flag = 1;
                        if (String.Compare(mess, "n") == 0) flag = 3;

                        break;

                    default: break;




                }


            }

            double ConvertToRadians(double angle)
            {
                return Math.PI / 180 * angle;
            }


            double ConvertToDegrees(double rad)
            {
                return 180 / Math.PI * rad;
            }

            double ChangeIncuctance(double L, double C, Czwornik t)
            {
                double Yc = Math.PI * 2 * t.freq * 0.000001 * C;
                double Yl = 1 / (Math.PI * 2 * t.freq * L * 0.001);

                double Yind = Yc + Yl;
                Yind = 1 / Yind;

                return ((Yind * 1000) / (Math.PI * 2 * t.freq));
            }

            Complex GetGammaL(double L, Czwornik t)
            {

                double ESR = (R * (uint)(2 * Math.PI * t.freq * L * 0.001 * Q)) / (R + (uint)(2 * Math.PI * t.freq * L * 0.001 * Q));
                
                Complex Z = new Complex(0, Math.PI * 2 * t.freq * L * 0.001);
                Z = (Z * ESR) / (Z + ESR);
               
                return (Z - t.z0_impedance) / (Z + t.z0_impedance);
               
            }
        }
    }

    namespace ConsoleApp2
    {
        public struct Czwornik
        {
            public Complex s11;
            public Complex s12;
            public Complex s21;
            public Complex s22;

            public Complex s11_prim;
            public Complex s22_prim;

            public Complex y11;
            public Complex y12;
            public Complex y21;
            public Complex y22;

            Complex z11;
            Complex z12;
            Complex z21;
            Complex z22;

            public double Rollet_factor;

            public double z0_impedance;
            public double freq; //MHz

            public Czwornik(uint system_impedance, double frequency)
            {
                z0_impedance = system_impedance;
                s11_prim = Complex.Zero;
                s22_prim = Complex.Zero;
                freq = frequency;

            }

            public void ConvertCEtoCB_yparams()
            {
                y11 = y11 + y12 + y21 + y22;
                y12 = -(y12 + y22);
                y21 = -(y21 + y22);
                //y22 = y22;
            }

            public void ConvertCBtoCE_yparams()
            {

                y21 = y22 - y21;
                y12 = y22 - y12;
                y11 = y11 - (y12 + y21 + y22);
            }


            public void CalculateYparamsFromS()
            {
                Complex x = s12 * s21;
                Complex denominator = ((1 + s22) * (1 + s11)) - x;

                y11 = (Complex)(((double)(1 / z0_impedance)) * (((1 + s22) * (1 - s11)) + x) / denominator);

                y12 = (Complex)(((double)(1 / z0_impedance)) * (-2 * s12) / denominator);

                y21 = (Complex)(((double)(1 / z0_impedance)) * (-2 * s21) / denominator);

                y22 = (Complex)(((double)(1 / z0_impedance)) * (((1 - s22) * (1 + s11)) + x) / denominator);

                CalculateZparamsFromY();


            }

            public void AddCouplingCap(uint C)
            {
                double admitance = Math.PI * 2 * this.freq * 1000000 * C * 0.000000000001;
                y11 += new Complex(0, admitance);
                y12 -= new Complex(0, admitance);
                y21 -= new Complex(0, admitance);
                y22 += new Complex(0, admitance);
            }

            public void AddCouplingCap(Czwornik s)
            {

                y11 += s.y11;
                y12 += s.y12;
                y21 += s.y21;
                y22 += s.y22;
            }

            public void AddSerialInductance(uint L)
            {
                double impedance = Math.PI * 2 * this.freq * 1000000 * L * 0.000000001;
                z11 += new Complex(0, impedance);
                z12 += new Complex(0, impedance);
                z21 += new Complex(0, impedance);
                z22 += new Complex(0, impedance);

                CalculateYparamsFromZ();
                CalculateSparams();

            }

            void CalculateZparamsFromY()
            {
                Complex y_det = (y11 * y22) - (y12 * y21);

                z11 = y22 / y_det;

                z12 = -y12 / y_det;

                z21 = -y21 / y_det;


                z22 = y11 / y_det;
            }

            void CalculateYparamsFromZ()
            {
                Complex z_det = (z11 * z22) - (z12 * z21);

                y11 = z22 / z_det;

                y12 = -z12 / z_det;

                y21 = -z21 / z_det;


                y22 = z11 / z_det;
            }

            public void CalculateSparams()
            {
                Complex y11_temp = z0_impedance * y11;
                Complex y12_temp = z0_impedance * y12;
                Complex y21_temp = z0_impedance * y21;
                Complex y22_temp = z0_impedance * y22;

                s11 = (Complex)((((1 + y22_temp) * (1 - y11_temp)) + (y12_temp * y21_temp)) / (((1 + y22_temp) * (1 + y11_temp)) - (y12_temp * y21_temp)));

                s12 = (Complex)((-2 * y12_temp) / (((1 + y22_temp) * (1 + y11_temp)) - (y12_temp * y21_temp)));

                s21 = (Complex)((-2 * y21_temp) / (((1 + y22_temp) * (1 + y11_temp)) - (y12_temp * y21_temp)));

                s22 = (Complex)((((1 - y22_temp) * (1 + y11_temp)) + (y12_temp * y21_temp)) / (((1 + y22_temp) * (1 + y11_temp)) - (y12_temp * y21_temp)));
            }

            public void CalculateS11prim(Complex gammaL)
            {
                s11_prim = s11 + ((s12 * s21 * gammaL) / (1 - (s22 * gammaL)));
            }

            Complex GetDet()
            {
                Complex a = s11 * s22;
                Complex b = s12 * s21;
                //return ( (s11 * s22) - (s21 * s12) );
                return (a - b);
            }
            public void RecalculateRolletFactor()
            {
                double s11_2 = (s11.Magnitude * s11.Magnitude);
                double s22_2 = (s22.Magnitude * s22.Magnitude);
                Complex det = GetDet();
                double mianownik = 2 * Complex.Abs(s21 * s12);
                double det_2 = det.Magnitude * det.Magnitude;

               

                Rollet_factor = (1 - s11_2 - s22_2 + det_2) / mianownik;


            }


        }
    }

}
