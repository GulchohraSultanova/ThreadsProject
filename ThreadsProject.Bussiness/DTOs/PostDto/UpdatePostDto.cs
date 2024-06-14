using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.PostDto
{
    public class UpdatePostDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
    public class UpdatePostDtoValidation : AbstractValidator<UpdatePostDto>
    {
        public UpdatePostDtoValidation()
        {
            RuleFor(x => x.Content).NotEmpty()
            .WithMessage("The Content cannot be empty!").MaximumLength(500)
                .WithMessage("The length cannot be more than 500!");
        }
    }
}
