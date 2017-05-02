using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ExercThread
{
    public class Action
    {

        private string mensagem;
        private object locker = new object();

        #region Métodos

        public string GerarMensagem()
        {
            var chars = "abcdefghijklmnopqrstuvxyz";
            var random = new Random();
            mensagem = new string(
                Enumerable.Repeat(chars, 80)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return mensagem;
        }

        private bool VerificaTodaStringMaiusculo()
        {

            if (mensagem.ToUpper() == mensagem)
            {
                return true;
            }

            return false;
            
        }

        private void AlterarLetra()
        {
            

            try
            {
                //Encontrar a primeira letra minúscula da string

                while (!this.VerificaTodaStringMaiusculo())
                {
                    //Inicio região crítica
                    //Console.WriteLine(Thread.CurrentThread.Name + " " + Thread.CurrentThread.ThreadState);
                    lock (locker)
                    {
                        for (var i = 0; i < mensagem.Length; i++)
                        {
                            if (mensagem[i] >= 97 && mensagem[i] <= 122)
                            {
                                string caracter = mensagem[i].ToString();

                                caracter = caracter.ToUpper();

                                mensagem = mensagem.Remove(i, 1);
                                mensagem = mensagem.Insert(i, caracter);

                                break;
                            }
                        }
                        //Thread.Sleep(1000);
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        //Método principal
        public bool InstaciarProblema()
        {
            try
            {
                //Gerar a string contendo 80 caracteres
                Console.WriteLine("Mensagem Inicial: " + this.GerarMensagem());

                //Instacia 30 Threads
                List<Thread> threads = new List<Thread>();

                for(var i = 0; i < 30; i++)
                {
                    //inicializa a thread atribuindo um nome somente para controle
                    threads.Add(new Thread(AlterarLetra));
                    threads[i].Name = "Thread " + i;
                    threads[i].Start(); 
                }

                if (this.VerificaTodaStringMaiusculo())
                {
                    Console.WriteLine("Mensagem Resultante: " + mensagem + " | " + mensagem.Length);
                    Console.WriteLine();
                    Console.WriteLine("A execução finalizou com sucesso!!!");
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;


        }

        #endregion Métodos





    }
}
