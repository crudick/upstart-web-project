using AutoMapper;
using Upstart.Api.Models;
using Upstart.Application.Services;

namespace Upstart.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUserApiRequest, CreateUserRequest>();
        CreateMap<UpdateUserApiRequest, UpdateUserRequest>();
        CreateMap<CreatePollApiRequest, CreatePollRequest>();
        CreateMap<CreatePollAnswerApiRequest, CreatePollAnswerRequest>();
        CreateMap<CreatePollStatApiRequest, CreatePollStatRequest>();
    }
}