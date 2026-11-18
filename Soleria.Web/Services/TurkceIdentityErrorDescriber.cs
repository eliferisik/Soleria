using Microsoft.AspNetCore.Identity;

namespace Soleria.Web.Services;

public class TurkceIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError() => new() { Code = nameof(DefaultError), Description = "Beklenmeyen bir hata oluştu." };
    public override IdentityError DuplicateEmail(string email) => new() { Code = nameof(DuplicateEmail), Description = "Bu e-posta adresiyle zaten bir hesap var." };
    public override IdentityError DuplicateUserName(string userName) => new() { Code = nameof(DuplicateUserName), Description = "Bu e-posta adresiyle zaten bir hesap var." };
    public override IdentityError InvalidEmail(string? email) => new() { Code = nameof(InvalidEmail), Description = "Geçerli bir e-posta adresi girin." };
    public override IdentityError InvalidUserName(string? userName) => new() { Code = nameof(InvalidUserName), Description = "Geçerli bir e-posta adresi girin." };
    public override IdentityError PasswordTooShort(int length) => new() { Code = nameof(PasswordTooShort), Description = $"Parola en az {length} karakter olmalıdır." };
    public override IdentityError PasswordRequiresDigit() => new() { Code = nameof(PasswordRequiresDigit), Description = "Parola en az bir rakam içermelidir." };
    public override IdentityError PasswordRequiresLower() => new() { Code = nameof(PasswordRequiresLower), Description = "Parola en az bir küçük harf içermelidir." };
    public override IdentityError PasswordRequiresUpper() => new() { Code = nameof(PasswordRequiresUpper), Description = "Parola en az bir büyük harf içermelidir." };
    public override IdentityError PasswordRequiresNonAlphanumeric() => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Parola en az bir özel karakter içermelidir." };
    public override IdentityError PasswordMismatch() => new() { Code = nameof(PasswordMismatch), Description = "Parola hatalı." };
}
