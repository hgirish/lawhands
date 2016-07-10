using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace lawhands.Models.AccountViewModels
{
    public class RoleViewModel
    {
        [HiddenInput]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}