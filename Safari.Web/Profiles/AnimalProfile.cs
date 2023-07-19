﻿using Safari.Data;
using Safari.Web.ViewModels;
using AutoMapper;

namespace Safari.Web.Profiles;

//AnimalProfile is a class that helps map Animal to AnimalViewModel, using AutoMapper
//It also feeds in information from other tables like AnimalPic, AnimalState, State, AnimalType, and DietType
//so that the actual names are present in the view, not the IDs.
public class AnimalProfile : Profile
{
    public AnimalProfile()
    {
        CreateMap<Animal, AnimalViewModel>()
            .ForMember(dest => dest.States, 
            opt => opt.MapFrom(src => src.AnimalState.Select(s => s.State.Name).ToList()))
            .ForMember(dest => dest.FilePath, 
            opt => opt.MapFrom(src => GetApprovedFilePath(src)))
            .ForMember(dest => dest.AltText, 
            opt => opt.MapFrom(src => GetApprovedAltText(src)))
            .ForMember(dest => dest.AnimalTypeName, 
            opt => opt.MapFrom(src => src.AnimalType.Name))
            .ForMember(dest => dest.DietTypeName, 
            opt => opt.MapFrom(src => src.DietType.Name));
    }

    private string GetApprovedFilePath(Animal animal)
    {
        var approvedAnimalPic = animal.AnimalPic.FirstOrDefault(ap => ap.IsApproved == true);
        return approvedAnimalPic?.FilePath;
    }

    private string GetApprovedAltText(Animal animal)
    {
        var approvedAnimalPic = animal.AnimalPic.FirstOrDefault(ap => ap.IsApproved == true);
        return approvedAnimalPic?.AltText;
    }
}
