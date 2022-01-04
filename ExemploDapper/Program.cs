using Dapper;
using ExemploDapper.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExemploDapper
{
    internal class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            string _con = Configuration["ConnectionStrings:DefaultConnection"];

            Console.WriteLine("\tc - Consultar");
            Console.WriteLine("\ti - Incluir");
            Console.WriteLine("\ta - Atualizar");
            Console.WriteLine("\td - Deletar");
            Console.Write("Sua opção (c,i,d,a) ? ");

            while (Console.ReadLine() != "exit")
            {
                Console.WriteLine("\tc - Consultar");
                Console.WriteLine("\ti - Incluir");
                Console.WriteLine("\ta - Atualizar");
                Console.WriteLine("\td - Deletar");
                Console.Write("Sua opção (c,i,d,a) ? ");

                switch (Console.ReadLine())
                {
                    case "c":
                        Consultar(_con);
                        break;

                    case "i":
                        Incluir(_con);
                        break;

                    case "a":
                        Console.WriteLine("Informe o ID do cliente a ser atualizado:");
                        var idAtualizar = Console.ReadLine();

                        Atualizar(_con, Guid.Parse(idAtualizar));
                        break;

                    case "d":
                        Console.WriteLine("Informe o ID do cliente a ser removido:");
                        var idExcluir = Console.ReadLine();

                        Excluir(_con, Guid.Parse(idExcluir));
                        break;
                }
               
            }

        }

        static void Consultar(string conexao)
        {
            var oracleConnection = new OracleConnection(conexao);
            oracleConnection.Open();

            var query = "SELECT * from clientes";

            using (var conn = new OracleConnection(conexao))
            {
                var result = conn.Query<Cliente>(query).ToList();

                foreach (var cliente in result)
                {
                    Console.WriteLine(new string('*', 20));
                    Console.WriteLine("\nID: " + cliente.ClienteId.ToString());
                    Console.WriteLine("Nome : " + cliente.Nome);
                    Console.WriteLine("Idade: " + cliente.Idade.ToString());
                    Console.WriteLine(new string('*', 20));
                }

            }

        }

        static async void Incluir(string conexao)
        {
            Cliente model = new Cliente();
            model.ClienteId = Guid.NewGuid().ToString();
            model.Nome = "teste";
            model.Idade = 99;
            model.Email = "email@teste.com";

            using (var conn = new OracleConnection(conexao))
            {
                await conn.OpenAsync();

                var query = @"INSERT INTO APPACADEMY.clientes (ClienteId, Nome, Idade, Email) VALUES (:ClienteId,:Nome,:Idade,:Email)";

                try
                {
                    await conn.ExecuteAsync(query, model);

                    Console.WriteLine($"Cliente {model.Nome} incluido com sucesso");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }

            }

        }

        static async void Atualizar(string conexao, Guid id)
        {
            Random rnd = new Random();

            Cliente model = new Cliente();
            model.ClienteId = id.ToString();
            model.Nome = "teste" + rnd.Next(100);
            model.Idade = 99;
            model.Email = "email@teste.com";

            using (var conn = new OracleConnection(conexao))
            {
                await conn.OpenAsync();

                var query = @"UPDATE APPACADEMY.clientes 
                                 SET Nome = :Nome,
                                     Idade = :Idade,
                                     Email = :Email 
                               WHERE ClienteId = :ClienteId";

                try
                {
                    await conn.ExecuteAsync(query, model);

                    Console.WriteLine($"Cliente {model.Nome} atualizado com sucesso");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }

            }

        }

        static async void Excluir(string conexao, Guid id)
        {
            Random rnd = new Random();

            using (var conn = new OracleConnection(conexao))
            {
                await conn.OpenAsync();

                var query = @"DELETE APPACADEMY.clientes                                  
                               WHERE ClienteId = :ClienteId";

                try
                {
                    await conn.ExecuteAsync(query, new { ClienteId = id.ToString()});

                    Console.WriteLine($"Cliente removido com sucesso");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }

            }

        }
        
    }
}
