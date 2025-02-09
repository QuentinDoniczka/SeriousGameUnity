using System.Collections.Generic;
using System.Linq;

namespace Project.Api.Models.Validators
{
    public class ValidationResult
    {
        public List<string> Errors { get; set; } = new List<string>();
        public bool IsValid => !Errors.Any();
    }
}