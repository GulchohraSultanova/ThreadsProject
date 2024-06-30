using FluentValidation;
using System.Text.RegularExpressions;

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
public class ChangePasswordDtoValidation : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidation()
    {
        RuleFor(x => x.NewPassword).NotEmpty()
           .WithMessage("The password cannot be empty!")
           .Must(r =>
           {
               Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,50}$");
               Match match = passwordRegex.Match(r);
               return match.Success;
           }).WithMessage("Password format is not correct!")
           .Must(p => !p.Contains(" "))
           .WithMessage("The password cannot contain spaces!");
    }
}