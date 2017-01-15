namespace WebApp.Controllers
{
    using System.Threading.Tasks;

    using AutoMapper.QueryableExtensions;

    using AvgIdentity.Managers;
    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Mvc;

    using WebApp.Data;
    using WebApp.Models.AdminViewModels;

    public class AdminController : Controller
    {
        private readonly IUserRoleManager<AvgIdentityUser, WebAppDbContext> userRoleManager;

        public AdminController(IUserRoleManager<AvgIdentityUser, WebAppDbContext> userRoleManager)
        {
            this.userRoleManager = userRoleManager;
        }

        public IActionResult AddRole()
        {
            return this.View();
        }

        public async Task<IActionResult> RemoveRole(string id)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.userRoleManager.RemoveRoleAsync(id);
                return this.RedirectToAction("IndexRoles");
            }

            return this.View(id);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.userRoleManager.AddRoleAsync(model.Name);
                return this.RedirectToAction("IndexRoles");
            }

            return this.View(model);
        }

        public IActionResult IndexRoles()
        {
            var roles = this.userRoleManager.GetAllRoles().ProjectTo<RoleViewModel>();
            return this.View(roles);
        }
    }
}