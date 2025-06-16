using System;
using System.Collections.Generic;
using System.Globalization;

namespace Questao1
{
    /*
    Questão 1

    Uma instituição financeira solicita que para cadastrar uma conta bancária, é necessário informar:
    o número da conta, 
    o nome do titular da conta, 
    e o valor de depósito inicial que o titular depositou ao abrir a conta.

    O valor de depósito inicial, é opcional, ou seja: 
    se o titular não tiver dinheiro a depositar no momento de abrir sua conta, o depósito inicial não será feito e o saldo inicial da conta será, naturalmente, zero.

    Importante: Após a conta ser aberta, o número da conta nunca poderá ser alterado. Já o nome do titular pode ser alterado (pois uma pessoa pode mudar de nome quando contrai matrimônio por exemplo).

    O saldo da conta não pode ser alterado livremente. É preciso haver um mecanismo para proteger isso. 

    O saldo só aumenta por meio de depósitos, e só diminui por meio de saques. 

    Para cada saque realizado, a instituição cobra uma taxa de $ 3.50. 

    Observação: a conta pode ficar com saldo negativo se o saldo não for suficiente para realizar o saque e/ou pagar a taxa.

    Disponibilizamos um programa que solicita os dados de cadastro da conta, dando opção para que seja ou não
    informado o valor de depósito inicial. Em seguida, realizar um depósito e depois um saque, sempre
    mostrando os dados da conta após cada operação.

    Você deve implementar a classe “ContaBancaria” para que o programa funcione conforme dois cenários de teste abaixo:

    Exemplo 1:

    Entre o número da conta: 5447
    Entre o titular da conta: Milton Gonçalves
    Haverá depósito inicial (s/n)? s
    Entre o valor de depósito inicial: 350.00

    Dados da conta:
    Conta 5447, Titular: Milton Gonçalves, Saldo: $ 350.00

    Entre um valor para depósito: 200
    Dados da conta atualizados:
    Conta 5447, Titular: Milton Gonçalves, Saldo: $ 550.00

    Entre um valor para saque: 199
    Dados da conta atualizados:
    Conta 5447, Titular: Milton Gonçalves, Saldo: $ 347.50

    Exemplo 2:
    Entre o número da conta: 5139
    Entre o titular da conta: Elza Soares
    Haverá depósito inicial (s/n)? n

    Dados da conta:
    Conta 5139, Titular: Elza Soares, Saldo: $ 0.00

    Entre um valor para depósito: 300.00
    Dados da conta atualizados:
    Conta 5139, Titular: Elza Soares, Saldo: $ 300.00

    Entre um valor para saque: 298.00
    Dados da conta atualizados:
    Conta 5139, Titular: Elza Soares, Saldo: $ -1.50
    
    */

    public class MockContas
    {
        public static List<IContaBancaria> Contas = new();
    }

    public interface IContaBancaria
    {
        /// <summary>
        /// Número da conta.
        /// </summary>
        int Numero { get; }
        /// <summary>
        /// Nome do titular.
        /// </summary>
        string Titular { get; }
        /// <summary>
        /// Saldo atual da conta.
        /// </summary>
        double Saldo { get; }
        /// <summary>
        /// Efetua um depósito.
        /// </summary>
        /// <param name="quantia">Quantia a ser depositada.</param>
        /// <returns>
        /// Saldo atual após o depósito.
        /// </returns>
        double Deposito(double quantia);
        /// <summary>
        /// Efetua um saque.
        /// </summary>
        /// <param name="quantia">Quantia a ser sacada.</param>
        /// <returns>
        /// Saldo atual após o saque.
        /// </returns>
        double Saque(double quantia);        
        /// <summary>
        /// Taxa aplicada a cada operação de saque.
        /// </summary>
        const double TaxaSaque = 3.50;
    }
    class ContaBancaria : IContaBancaria
    {
        private readonly int _numero;
        private string _titular;
        private double _saldo;
        public int Numero { get => _numero; }
        public string Titular
        {
            get => _titular;
            private set => _titular = value;
        }
        public double Saldo
        {
            get => _saldo;
            private set => _saldo = value;
        }
        public ContaBancaria(int numero, string titular, double depositoInicial = 0)
        {
            _numero = numero;
            _titular = titular;
            _saldo = depositoInicial;
        }
        public double Deposito(double quantia)
        {
            _saldo += quantia;
            return _saldo;
        }
        public double Saque(double quantia)
        {
            _saldo -= IContaBancaria.TaxaSaque;
            _saldo -= quantia;
            return _saldo;
        }
    }
}
