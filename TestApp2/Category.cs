namespace TestApp2
{
    class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category(int id,string name)
        {
            Name = name;
            Id = id;
        }
    }
}
