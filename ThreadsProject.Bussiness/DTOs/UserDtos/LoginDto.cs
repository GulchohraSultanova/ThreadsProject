﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.UserDtos
{
    public record LoginDto
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }
    public class LoginDtoValidation : AbstractValidator<LoginDto>
    {
        public LoginDtoValidation()
        {
            RuleFor(x => x.UserNameOrEmail).NotEmpty()
              .WithMessage("The UserName/Email cannot be empty!");
            RuleFor(x => x.Password).NotEmpty()
                .WithMessage("The UserName/Email cannot be empty!");
        }
    }
}
