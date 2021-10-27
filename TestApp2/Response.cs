using System.Collections.Generic;

namespace TestApp2
{
    //данный клас нужен чтобы коректно получить даные от сервера 
    class Response
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
    }
}
