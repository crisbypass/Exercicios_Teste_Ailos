using System;
using System.Globalization;

namespace Questao1
{
    class Program
    {
        static void Main(string[] args)
        {
            GerenciarContas();
        }
        static void GerenciarContas()
        {
            Console.Clear();

            IContaBancaria conta;

            Console.Write("Entre o número da conta: ");
            int numero = int.Parse(Console.ReadLine());
            Console.Write("Entre o titular da conta: ");
            string titular = Console.ReadLine();
            Console.Write("Haverá depósito inicial (s/n)? ");
            char resp = char.Parse(Console.ReadLine());
            if (resp == 's' || resp == 'S')
            {
                Console.Write("Entre o valor de depósito inicial: ");
                double depositoInicial = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                conta = new ContaBancaria(numero, titular, depositoInicial);
            }
            else
            {
                conta = new ContaBancaria(numero, titular);
            }

            Console.WriteLine();
            Console.WriteLine("Dados da conta:");
            BuscarContaInfo(conta);
            
            Console.Write("Entre um valor para depósito: ");
            double quantia = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            conta.Deposito(quantia);
            Console.WriteLine("Dados da conta atualizados:");
            BuscarContaInfo(conta);
            
            Console.Write("Entre um valor para saque: ");
            quantia = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            conta.Saque(quantia);
            Console.WriteLine("Dados da conta atualizados:");
            BuscarContaInfo(conta);            

            Console.Write("Deseja efetuar outras operações (s/n)? ");
            char continuar = char.Parse(Console.ReadLine());
            if (continuar == 's' || continuar == 'S')
            {
                GerenciarContas();
            }

            Console.WriteLine("A operação foi finalizada.\n");            
        }
        static void BuscarContaInfo(IContaBancaria conta, string formatoSaldo = "N2") =>
            Console.WriteLine(string.Format("Conta: {0}, Titular: {1}, Saldo: $ {2}\n",
                conta.Numero, conta.Titular, conta.Saldo.ToString(formatoSaldo)));
    }
}
