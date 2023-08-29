using System.ComponentModel.DataAnnotations;

namespace DotNetCoreLearning.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public string CreatedBy { get; set; }
    }   
    
    
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}