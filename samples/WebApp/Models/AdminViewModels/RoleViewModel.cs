namespace WebApp.Models.AdminViewModels
{
    using AutoMapper;

    public class RoleViewModel : Profile
    {
        public RoleViewModel()
        {
            this.CreateMap<string, RoleViewModel>().ForMember(m => m.Name, opt => opt.MapFrom(c => c));
        }

        public string Name { get; set; }
    }
}
