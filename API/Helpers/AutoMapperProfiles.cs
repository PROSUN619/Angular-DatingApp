using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser,MemberDto>()
            .ForMember(dest => dest.PhotoUrl, option => 
                option.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.Age, option => 
                option.MapFrom(src => src.DateOfBirth.CalculateAge()));
                
            CreateMap<Photo,PhotoDto>();

            CreateMap<MemberUpdateDto,AppUser>();

            CreateMap<RegisterDto,AppUser>();

            CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderPhotoUrl, option => 
                option.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.RecipientPhotoUrl, option => 
                option.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));  

        }
    }
}