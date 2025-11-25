using AutoMapper;
using Loans.Domain;
using Loans.Domain.Dtos;
using Loans.Domain.Entities;

namespace Loans.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));

            CreateMap<RegisterDto, User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.Role, opt => opt.Ignore())
                .ForMember(d => d.IsBlocked, opt => opt.Ignore());

            CreateMap<Loan, LoanDto>();

            CreateMap<LoanCreateDto, Loan>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => LoanStatus.InProcess));

            CreateMap<LoanUpdateDto, Loan>()
                .ForAllMembers(opt =>
                    opt.Condition((s, d, value) => value != null)
                );

            CreateMap<LoanStatusUpdateDto, Loan>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status));
        }
    }

}