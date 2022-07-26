using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplicationDemo.Data;
using WebApplicationDemo.Models;

namespace WebApplicationDemo.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [Display(Name = "User Name")]
            public string UserName { get; set; }


            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Current Password")]
            public string CurrentPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm New password")]
            [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);

           

            Input = new InputModel
            {
                UserName = userName,
                PhoneNumber = phoneNumber,
                Email = email
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var currentUser = _context.Users.Where(u => u.Id == user.Id).SingleOrDefault();

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            currentUser.UserName = Input.UserName;

            currentUser.Email = Input.Email;

            if (! await _userManager.CheckPasswordAsync(currentUser,Input.CurrentPassword))
            {
                StatusMessage = "Current password is not right";
                return RedirectToPage();
            }
            else
            {
                var newPassword = _userManager.PasswordHasher.HashPassword(currentUser,Input.NewPassword);

                currentUser.PasswordHash = newPassword;
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(currentUser);

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(currentUser, Input.PhoneNumber);

                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";

                    return RedirectToPage();
                }
            }

             _context.Users.Update(currentUser);

            await _context.SaveChangesAsync();

            StatusMessage = "Your profile has been updated";
         
            return RedirectToPage();
 
        }
    }
}
