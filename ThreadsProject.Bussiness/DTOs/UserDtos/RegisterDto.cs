using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.UserDtos
{
    public record RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class RegisterDtoValidation : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty()
                .WithMessage("The name cannot be empty!")
                .MaximumLength(20)
                .WithMessage("The length cannot be more than 20!")
                .MinimumLength(3)
                .WithMessage("The length cannot be less than 3!")
                ;
            RuleFor(x => x.LastName).NotEmpty()
               .WithMessage("The surName cannot be empty!")
               .MaximumLength(30)
               .WithMessage("The length cannot be more than 20!")
               .MinimumLength(3)
               .WithMessage("The length cannot be less than 3!")
               ;
            RuleFor(x => x.UserName).NotEmpty()
               .WithMessage("The userName cannot be empty!")
               .MaximumLength(30)
               .WithMessage("The length cannot be more than 20!")
               .MinimumLength(7)
               .WithMessage("The length cannot be less than 3!")
               ;
           RuleFor(x => x.Email).NotEmpty()
               .WithMessage("The email cannot be empty!")
               .EmailAddress()
               .WithMessage("Email format is not correct!");
            RuleFor(x => x.Password).NotEmpty()
             .WithMessage("The name cannot be empty!")
             .Must(r =>
             {
                 Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,50}$");
                 Match match = passwordRegex.Match(r);
                 ; return match.Success;

             }).WithMessage("Password format is not correct!");

        }
    }
}
