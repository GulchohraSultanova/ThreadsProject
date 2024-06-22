using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.PostDto
{
    public  class CreatePostDto
    {
        public string Content { get; set; }
        public List<string> ? Images { get; set; } = new List<string>();
        public List<int> TagIds { get; set; }
    }
    public class CreatePostDtoValidation:AbstractValidator<CreatePostDto>
    { 
        public CreatePostDtoValidation() 
        {
            RuleFor(x => x.Content).NotEmpty()
            .WithMessage("The Content cannot be empty!").MaximumLength(500)
                .WithMessage("The length cannot be more than 500!");
        }
    }
}
