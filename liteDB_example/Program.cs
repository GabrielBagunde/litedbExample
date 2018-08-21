using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace teste_minimongo
{
    class Program
    {
        static void Main(string[] args)
        {
            bool sair = false;
            ConsoleKeyInfo opcao;
            Console.WriteLine("procurando consumidores");

            ExibeOpcoes();
            do
            {
                opcao = Console.ReadKey();

                if (opcao.Key.ToString() == "C")
                {
                    sair = true;
                }
                else if (opcao.Key.ToString() == "A")
                {
                    InsereUmConsumidor();
                    ExibeOpcoes();
                }
                else if (opcao.Key.ToString() == "B")
                {
                    ApagaTodos();
                    ExibeOpcoes();
                }
                else if (opcao.Key.ToString() == "F")
                {
                    Console.WriteLine("digite o nome para busca");
                    string nome = Console.ReadLine();
                    if (opcao.Key.ToString() == "")
                    {
                        Console.WriteLine("Nenhum caracter digitado");
                    }
                    else
                    {
                        BuscaoPorNome(nome);
                        ExibeOpcoes();
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Apertado - {0} - Opção não tem funcionalidade", opcao.Key.ToString()));
                }
            } while (!sair);
        }
        static void LimparLinha()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
        private static void ExibeOpcoes()
        {
            Console.WriteLine("***********");
            Console.WriteLine("OPCOES");
            Console.WriteLine("A - Adicionar 1000 consumidores");
            Console.WriteLine("B - Apagar todos");
            Console.WriteLine("F - Pesquisa por nome");
            Console.WriteLine("C - Sair");
        }
        private static void ExibeTodosOsConsumidores()
        {
            var consumidores = new CustomerRepository().GetAll();
            if (consumidores != null)
            {
                foreach (var item in consumidores)
                {
                    Console.WriteLine(item.Name);
                }
            }
            else
            {
                Console.WriteLine("Nenhum consumidor cadastrado");
            }
        }
        private static void BuscaoPorNome(string nome)
        {
            var costumer = new CustomerRepository();
            costumer.SearchByName(nome);
            if (costumer.Quantidade > 0)
            {
                Console.WriteLine("**** Exibindo ******");
                foreach (var item in costumer.BuscaNomes)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("Encontrados - " + costumer.Quantidade);
            }
            else
            {
                Console.WriteLine("Nenhum consumidor encontrado para os parametros de busca");
            }
        }
        private static void ApagaTodos()
        {
            new CustomerRepository().DeleteAll();
            Console.WriteLine("Todos os consumidores excluidos");
        }
        private static void InsereUmConsumidor()
        {
            for (int i = 0; i < 1000; i++)
            {
                var customer = new Customer
                {
                    Name = i + "John Doe" + DateTime.Now.Second.ToString(),
                    Phones = new string[] { "8000-0000", "9000-0000" },
                    IsActive = true
                };
                new CustomerRepository().InsertOne(customer);
            }
        }
    }

    public class CustomerRepository
    {
        public CustomerRepository()
        {

        }

        public int Quantidade { get; set; }
        public List<string> BuscaNomes { get; set; }
        public IEnumerable<Customer> GetAll()
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                var customers = db.GetCollection<Customer>("customers");
                return customers.FindAll();
            }
        }
        public void InsertOne(Customer entity)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                var customers = db.GetCollection<Customer>("customers");
                customers.Insert(entity);
            }
        }
        public void SearchByName(string name)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                var customers = db.GetCollection<Customer>("customers");
                BuscaNomes = customers.Find(x => x.Name.Contains(name)).Select(y => y.Name).ToList();
                Quantidade = BuscaNomes.Count;
            }
        }
        public void DeleteAll()
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                var customers = db.GetCollection<Customer>("customers");
                customers.Delete(x => x.Id != 0);
            }
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }
}
