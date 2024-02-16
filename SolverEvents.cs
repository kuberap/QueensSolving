
using System;

namespace Queens
{

    /// <summary>
    /// Trida pro predavani informaci o konfiguraci sachovnice
    /// </summary>
    public class QueensConfigFoundEventArgs : EventArgs //dedim od bazove tridy pro predavani info
    {
        /// <summary>
        /// Konstruktor tridy
        /// </summary>
        /// <param name="config">Pole s konfiguraci sachovnice</param>
        public QueensConfigFoundEventArgs(int[] config)
        {
            ///<summary>
            ///Protoze predavam pole, tak ho kopiruji - abych pak nepracoval s odkazem na pole uvnitr resice
            ///</summary>
            QueensConfiguration = new int[config.Length];
            for (int i = 0; i < config.Length; i++)
                QueensConfiguration[i] = config[i];//kopiruj
        }
        public int[] QueensConfiguration { get; }
    }
 
 
    /// <summary>
    /// Trosku umela trida pro predavani udalosti o skonceni vypoctu
    /// </summary>
    public class QueensFinalizedEventArgs : EventArgs
    {
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="cnt"></param>
        public QueensFinalizedEventArgs(int cnt)
        {
            SolutionFound=cnt;
        }

        public int SolutionFound {get;}=0;

    }
 
    public class NQueensSolver
    { 
        /// <summary>
        /// Resic pro umisteni N dam na sachovnici. 
        /// Snazim se i ukazat udalostmi rizene programovani=>kazda nalezena konfigurace vyvola udalost
        /// </summary>


        public delegate void QueensConfigFoundHandler(object sender, QueensConfigFoundEventArgs e);
        public event QueensConfigFoundHandler QueensConfigFound;

        public delegate void QueensFinalizedHandler(object sender, QueensFinalizedEventArgs e);
        public event QueensFinalizedHandler QueensFinalized; 

        private bool QueensThreat(int qx1, int qy1, int qx2, int qy2)
        {
            int dx = Math.Abs(qx1 - qx2); //dx>0 ==> neni v samem sloupecku
            int dy = Math.Abs(qy1 - qy2); //dy>0 ==> neni ve stejnem radku

            if (dx == 0 || dy == 0 || dx == dy) //ohrozeni je bud ve stejnem radku, nebo sloupci, nebo na stejne diagonale
                return true;
            return false;
        }
        public void FindNQueens(int n)
        {
            int[] queens = new int[n]; //vytvoreno pole 0, tedy prvni dama je umistena na pozici 0
            int count = 1;//prvni dama je jiz umistena inicializaci pole
            int ypos = 1; //dalsi pozice, kam budu umistovat
            bool danger; //slouzi pro kontrolu, zda umistovana dama ohroyuje jiz umistene damy
            bool finished = false;// ridici promenna cyklu pro urceni, zda se ma koncit
            int configuration_counter=0;//pocet nalezenych konfiguraci

            while (!finished) //nevim kolik konfiguraci existuje
            {
                danger = false; //predpokladej uspech, tj. vloyenim neni ohrozena zadna z dam na sachovnici
                for (int i = 0; i < count; i++) //pres vsechny umistene damy
                    if (danger = QueensThreat(i, queens[i], count, ypos)) //testuj, zda jde pridat damu, tak aby neohrozovala jiz vlozene
                        break;//kdyz nejde, tak konec pokusu o vlozeni

                if (danger)//damu neslo umistit
                {
                    if (ypos < n - 1)//kdyz jeji pozice neni na konci sloupce
                        ypos++;//posun ji o jednu vyse
                    else
                    {
                        do
                        {
                            count--;//posun se na ose x, tj. vem predchozi
                            if (count < 0)//kdyz byla vyjmuta prvni, tak uz nic vlozit nejde
                            {
                                finished = true; //nastav kontrolni promennou cyklu na STOP
                                break;
                            }

                            ypos = queens[count] + 1;//vyndej posledni vlozenou damu
                        } while (ypos == n);//kdyz pozice damy je na konci sloupce, tak se posun o policko doleva a opakuj
                    }
                }
                else
                {
                    queens[count] = ypos;//umisti damu na danou pozici
                    count++;//posun se doprava
                    ypos = 0;//zacni od zacatku

                    if (count == n) //kdyz je umisteno n dam
                    {   configuration_counter++;//zvys pocet konfiguraci
                        QueensConfigFound?.Invoke(this, new QueensConfigFoundEventArgs(queens));//volej udalost, pokud je na ni neco poveseno
                        ///<remark>
                        ///pouzil jsem ?, abych nemusel psat podminku, zda je na handleru neco poveseno, tj. zda neni null
                        ///</remark>
                    }
                }
            }
            QueensFinalized?.Invoke(this, new QueensFinalizedEventArgs(configuration_counter));



        }


    }

    class MainClass
    {

        /// <summary>
        /// Funkce volana pri udalosti naleyeni konfigurace
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public static void PrintQueens(object o, QueensConfigFoundEventArgs e)
        {
            Console.Write($"Thread ID>{Environment.CurrentManagedThreadId}\tUmisteno>|");
            for (int i = 0; i < e.QueensConfiguration.Length; i++)
                Console.Write($"{e.QueensConfiguration[i]}|");
            Console.WriteLine();
        }
       /// <summary>
       /// Funkce volana pri dobehu vypoctu
       /// </summary>
       /// <param name="o"></param>
       /// <param name="e"></param>
                      
       public static void PrintEnd(object o, QueensFinalizedEventArgs e)
       {
        Console.WriteLine($"Thread ID>{Environment.CurrentManagedThreadId}\tHotovo>{e.SolutionFound} konfiguraci");
       }

    
       public static void Main(string[] args) //od Csharp 7 by melo jit i Task Main(...), ale pri testu na linuxu ne
        {
           
           
            NQueensSolver qs = new NQueensSolver();
            qs.QueensConfigFound += PrintQueens;//registrace obsluzne funkce na udalost
            qs.QueensFinalized += PrintEnd; //registrace obsluzne funkce na udalost
            qs.FindNQueens(5);
            Console.WriteLine($"Thread ID>{Environment.CurrentManagedThreadId}\tPO VOLANI METODY");
           


           /* NQueensSolver qs = new NQueensSolver();
            qs.QueensConfigFound += PrintQueens;
            qs.QueensFinalized += PrintEnd; 
            Task t= Task.Run(async()=>qs.FindNQueens(5));
            Console.WriteLine($"Thread ID>{Environment.CurrentManagedThreadId}\tPO VOLANI METODY");
            t.Wait();
            */
            
        }


       

    }


}
