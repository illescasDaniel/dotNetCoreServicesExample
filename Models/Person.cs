using System;

namespace myMicroservice.Models
{
    public struct Person
    {
        /// <summary>Unique identifier</summary>
        /// <example>10</example>
        public int Id { get; set; }

        /// <example>"Pepe"</example>
        public string Name { get; set; }

        /// <example>50</example>
        public UInt16? Age { get; set; }

        // This needs to be added IF this was a class, or else: "Deserialization of reference types without parameterless constructor is not supported"
        //public Person()
        //{
        //    Id = 0;
        //    Name = "";
        //    Age = null;
        //}

        /// <summary>Constructor</summary>
        public Person(int id, string name, UInt16? age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
    }
}
