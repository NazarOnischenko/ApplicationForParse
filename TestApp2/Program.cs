using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestApp2
{
    class Program
    {
        //глобальные поля для подключения и работы с http запросами
        private static readonly HttpClient client = new HttpClient();
        private static string stringOfConnect;
        private static string stringOfTypeRequest;
        static async Task Main(string[] args)
        {
            //получаем адрес и тип запроса  
            Console.Write("Enter url:");
            stringOfConnect = Console.ReadLine();
            Console.Write("Enter type request:");
            stringOfTypeRequest = Console.ReadLine();
            //вызываем метод для подключения
            await Connection();
            
        }
        private static async Task Connection()
        {
            //подключаемся через заголовки http
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36");

            //проверяем какой тип запроса отправить
            if (stringOfTypeRequest.ToLower() == "get") {
                
                GetMethod(await client.GetStringAsync(stringOfConnect));

            }
            else if (stringOfTypeRequest.ToLower() == "post")
            {
                PostMethod(client);
            }
        }
        //метод для get запроса
        public static void GetMethod(string data)
        {
            //парсим данные которые получили при запросе
            var parsed = JsonConvert.DeserializeObject<Response>(data);
            //переменные для вывода
            string nameProd = "Product name";
            string nameCategory = "Category name";
            //наибольшие строки с списка продуктов и списка категорий, так же нужно для оформления вывода
            int MaxLengthProduct = nameProd.Length;
            int MaxLengthCategory = nameCategory.Length;
            //находим самую длинную строку наименования продукта
            foreach (var prod in parsed.Products)
            {
                if (MaxLengthProduct < prod.Name.Length)
                {
                    MaxLengthProduct = prod.Name.Length;
                }
            }
            //находим самую длинную строку наименования категории
            foreach (var categor in parsed.Categories)
            {
                if (MaxLengthCategory < categor.Name.Length)
                {
                    MaxLengthCategory = categor.Name.Length;
                }
            }
            //оформляем вывод в виде таблицы
            for (int i = 0; i < MaxLengthCategory + MaxLengthProduct + 3; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();

            //получаем разницу символов между самыми большими строками и строками для именования столбцов 
            var difProd = MaxLengthProduct - nameProd.Length;
            var difCategor = MaxLengthCategory - nameCategory.Length;
            //выводим наименования столбцов таблици
            Console.Write($"|{nameProd}");
            for (int j = 0; j < difProd; j++)
            {
                Console.Write(" ");
            }
            Console.Write($"|{nameCategory}");
            for (int j = 0; j < difCategor; j++)
            {
                Console.Write(" ");
            }
            Console.WriteLine("|");
            //выводим продукты и категории
            foreach (var category in parsed.Categories)
            {
                foreach (var product in parsed.Products)
                {
                    if (category.Id == product.CategoryId)
                    {

                        difProd = MaxLengthProduct - product.Name.Length;
                        difCategor = MaxLengthCategory - category.Name.Length;
                        Console.Write("|");
                        Console.Write($"{product.Name}");
                        for (int j = 0; j < difProd; j++)
                        {
                            Console.Write(" ");
                        }
                        Console.Write($"|{category.Name}");
                        for (int j = 0; j < difCategor; j++)
                        {
                            Console.Write(" ");
                        }
                        Console.WriteLine("|");
                    }
                }
            }
            //заканчиваем таблицу
            for (int i = 0; i < MaxLengthCategory + MaxLengthProduct + 3; i++)
            {
                Console.Write("-");
            }
        }
        public static async Task PostMethod(HttpClient client)
        {
            //получаем новый продукт и его категорию
            Console.Write("Enter your product name:");
            var nameProd = Console.ReadLine();
            Console.Write("Enter your category:");
            var nameCategory = Console.ReadLine();

            //получаем данные с удаленного сервера чтобы сравнить сущестующие категории с категорией, что ввел пользователь
            var data = await client.GetStringAsync(stringOfConnect);
            var parsed = JsonConvert.DeserializeObject<Response>(data);

            //переменные для оредиления id продукта и категории
            int idProd = 0;
            int idCategor = 0;
            bool indCategory = false;//идентификатор для категории в случае если категория уже есть на удаленном сервер
            //ищем есть ли данная категория на удаленном сервере 
            foreach (var categor in parsed.Categories)
            {
                if (nameCategory == categor.Name)
                {
                    idCategor = categor.Id;
                    indCategory = true;
                }
            }
            //определяем id для продукта
            foreach (var prod in parsed.Products)
            {
                if (idProd < prod.Id)
                {
                    idProd = prod.Id;
                }
            }
            //если категории нету на сервер, то определяем ей id, создаем ее и отправляем post запрос
            if (!indCategory)
            {
                foreach (var categor in parsed.Categories)
                {
                    if (idCategor < categor.Id)
                    {
                        idCategor = categor.Id;
                    }
                }
                var category = new Category(idCategor, nameCategory);
                var jsonCategory = JsonConvert.SerializeObject(category);
                var dataCategory = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
                var responseCategory = await client.PostAsync(stringOfConnect, dataCategory);
                var stringMsgCategory = responseCategory.Content.ReadAsStringAsync().Result;
                Console.WriteLine(stringMsgCategory);
            }
            //отправляем post запрос с даными о продукте
            var product = new Product(idProd, nameProd, idCategor);
            var jsonProd = JsonConvert.SerializeObject(product);
            var dataProd = new StringContent(jsonProd, Encoding.UTF8, "application/json");
            var responseProd = await client.PostAsync(stringOfConnect, dataProd);
            var stringMsgProd = responseProd.Content.ReadAsStringAsync().Result;
            Console.WriteLine(stringMsgProd);
        }
    }
    
}
