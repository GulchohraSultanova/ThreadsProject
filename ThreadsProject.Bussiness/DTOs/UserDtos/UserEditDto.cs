using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.UserDtos
{
   public  class UserEditDto
    {
        public string UserName { get; set; }

       
        public string Bio { get; set; }

       
        public bool IsPublic { get; set; }

        public string ImgUrl { get; set; }
    }
    public class UserEditDtoValidation : AbstractValidator<UserEditDto>
    { 
         
        public UserEditDtoValidation() {
            RuleFor(x => x.UserName).NotEmpty()
             .WithMessage("The userName cannot be empty!")
             .MaximumLength(30)
             .WithMessage("The length cannot be more than 30!")
             .MinimumLength(7)
             .WithMessage("The length cannot be less than 7!")
             .Must(u => !u.Contains(" "))
             .WithMessage("The username cannot contain spaces!")
               .Matches("[a-zA-Z]").WithMessage("The username must contain at least one letter!");
            RuleFor(x => x.Bio)
            .MaximumLength(60)
            .WithMessage("The length cannot be more than 30!");

        }

    }

}
