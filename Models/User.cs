using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace myMicroservice.Models
{
    public struct User
    {

        public int Id { get; set; }
        [MaxLength(40)]
        public string Name { get; set; }
        public int Age { get; set; }

        public string? Token { get; set; }

        public User(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
            Token = null;
        }
    }
}
