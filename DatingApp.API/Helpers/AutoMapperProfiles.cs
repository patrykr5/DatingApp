using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.Where(p => p.IsMain).Select(x => x.Url).FirstOrDefault() ?? string.Empty))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.Where(p => p.IsMain).Select(x => x.Url).FirstOrDefault() ?? string.Empty))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>()
                .ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(x => x.SenderPhotoUrl,
                    opt => opt.MapFrom(src => src.Sender.Photos.Where(p => p.IsMain).Select(x => x.Url).FirstOrDefault() ?? string.Empty))
                .ForMember(x => x.RecipientPhotoUrl,
                    opt => opt.MapFrom(src => src.Recipient.Photos.Where(p => p.IsMain).Select(x => x.Url).FirstOrDefault() ?? string.Empty));
            CreateMap<Message, MessageAfterCreatedDto>()
                .ForMember(x => x.SenderPhotoUrl,
                    opt => opt.MapFrom(src => src.Sender.Photos.Where(p => p.IsMain).Select(x => x.Url).FirstOrDefault() ?? string.Empty))
                .ForMember(x => x.RecipientPhotoUrl,
                    opt => opt.MapFrom(src => src.Recipient.Photos.Where(p => p.IsMain).Select(x => x.Url).FirstOrDefault() ?? string.Empty));;
        }
    }
}