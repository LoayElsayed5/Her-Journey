using DomainLayer.Contracts;
using DomainLayer.IdentityModule;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServicesAbstraction.AuthServices;
using Shared.DTos.IdentityModuleDTo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthServices
{
    public class AccountService(IUnitOfWork _unitOfWork,
                    UserManager<ApplicationUser> _userManager, IConfiguration _configuration) : IAccountService
    {
        public async Task<UserDto> RegisterDoctorAsync(RegisterDoctorDto registerDoctorDto)
        {
            var User = new ApplicationUser
            {
                DisplayName = registerDoctorDto.DisplayName,
                Email = registerDoctorDto.Email,
                UserName = registerDoctorDto.Email.Split('@')[0],
                PhoneNumber = registerDoctorDto.PhoneNumber,
                EmailConfirmed = false
            };
            var result = await _userManager.CreateAsync(User);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(User, "Doctor");
                var Doctor = new Doctor
                {
                    UserId = User.Id,
                    //Specialization = registerDoctorDto.Specialization,
                    YearsOfExperience = registerDoctorDto.YearsOfExperience
                };

                await _unitOfWork.GetRepository<Doctor>().AddAsync(Doctor);
                await _unitOfWork.SaveChangesAsync();
                await SendConfirmationEmailLogic(User);

                return new UserDto
                {
                    DisplayName = User.DisplayName,
                    Email = User.Email,
                    Role = new List<string> { "Doctor" }
                };
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
        }

        public async Task<UserDto> RegisterPatientAsync(RegisterPatientDto dto)
        {
            var user = new ApplicationUser
            {
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                UserName = dto.Email.Split('@')[0],
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Patient");

                var patient = new Patient
                {
                    UserId = user.Id,
                    BloodType = dto.BloodType,
                    DateOfBirth = dto.DateOfBirth,
                    PregnancyWeek = dto.PregnancyWeek,
                    DoctorID = dto.DoctorId
                };

                await _unitOfWork.GetRepository<Patient>().AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();
                await SendConfirmationEmailLogic(user);

                return new UserDto
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Role = new List<string> { "Patient" }
                };
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

        }



        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var User = await _userManager.FindByEmailAsync(loginDto.Email);
            if (User == null)
            {
                throw new Exception("Invalid email or password.");
            }
            if (!await _userManager.IsEmailConfirmedAsync(User))
                throw new Exception("Your account is not activated. Please check your email for the activation link.");

            var IsPassValid = await _userManager.CheckPasswordAsync(User, loginDto.Password);
            if (IsPassValid)
            {
                return new UserDto
                {
                    DisplayName = User.DisplayName,
                    Email = User.Email!,
                    Role = (List<string>)await _userManager.GetRolesAsync(User),
                    Token = await CreateTokenAsync(User)
                };
            }
            else
            {
                throw new Exception("Invalid email or password.");
            }

        }



        public async Task<UserDto> GetCurrentUserAsync(string Email)
        {
            var User = await _userManager.FindByEmailAsync(Email);
            return new UserDto
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await CreateTokenAsync(User),
                Role = (List<string>)await _userManager.GetRolesAsync(User)
            };
        }




        public async Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(dto.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                var passwordResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);
                return passwordResult;
            }

            return result;
        }




        private async Task SendConfirmationEmailLogic(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = $"https://localhost:7121/api/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            var subject = "Account Activation - HerJourney";
            var body = $"<h1>Welcome {user.DisplayName}</h1>" +
                       $"<p>Please confirm your account by clicking this link: <a href='{confirmationLink}'>Click Here</a></p>";

            await SendEmailAsync(user.Email!, subject, body);
        }



        private async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("loayayad2017@gmail.com", "sljlycgqlkeqrxbi")
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("loayayad2017@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email Error: {ex.Message}");
            }
        }


        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var Cliams = new List<Claim>()
            {
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name,user.UserName!),
                new(ClaimTypes.NameIdentifier,user.Id)
            };
            var Roles = await _userManager.GetRolesAsync(user);
            foreach (var role in Roles)
                Cliams.Add(new Claim(ClaimTypes.Role, role));
            var SecretKey = _configuration.GetSection("JwtOptions")["SecretKey"];
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var Creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                claims: Cliams,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: Creds
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }

    }
}
