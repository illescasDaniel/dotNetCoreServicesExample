namespace myMicroservice.Models
{
    public class Person
    {
        /// <summary>Unique identifier</summary>
        /// <example>10</example>
        public int Id { get; set; }

        /// <example>"Pepe"</example>
        public string Name { get; set; }

        /// <example>50</example>
        public int? Age { get; set; }

        // This needs to be added or else: "Deserialization of reference types without parameterless constructor is not supported"
        public Person()
        {
            Id = 0;
            Name = "";
            Age = null;
        }

        /// <summary>Constructor</summary>
        public Person(int id, string name, int? age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
    }
}
