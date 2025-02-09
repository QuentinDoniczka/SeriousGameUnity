namespace Project.Api.Models.Validators
{
    public interface IValidator<in T>
    {
        ValidationResult Validate(T model);
    }
}