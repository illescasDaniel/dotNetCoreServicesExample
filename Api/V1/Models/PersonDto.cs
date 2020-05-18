using System;
using System.ComponentModel.DataAnnotations;

namespace myMicroservice.Api.V1.Models
{
    public struct PersonDto
    {
        /// <summary>Unique identifier</summary>
        /// <example>10</example>
        [Required]
        public int Id { get; set; }

        /// <example>Pepe</example>
        [Required]
        public string Name { get; set; }

        /// <example>50</example>
        [Required]
        public UInt16? Age { get; set; }

        // This needs to be added IF this was a class, or else: "Deserialization of reference types without parameterless constructor is not supported"
        //public Person()
        //{
        //    Id = 0;
        //    Name = "";
        //    Age = null;
        //}

        /// <summary>Constructor</summary>
        public PersonDto(int id, string name, UInt16? age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
    }
}
