// File: Mappings/AutoMapperProfile.cs
using AutoMapper;
using scan2pay.Models;
using scan2pay.DTOs;

namespace scan2pay.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Auth
        CreateMap<RegisterUserDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => Enum.Parse<UserType>(src.UserType, true)));
        CreateMap<ApplicationUser, AuthResponseDto>()
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType.ToString()));
        CreateMap<ApplicationUser, UserProfileDto>()
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType.ToString()));
        CreateMap<UpdateUserProfileDto, ApplicationUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Ne mappe que les non-nuls

        // Wallet
        CreateMap<Wallet, WalletDto>();

        // Transaction
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PayerEmail, opt => opt.MapFrom(src => src.Payer != null ? src.Payer.Email : null))
            .ForMember(dest => dest.PayeeEmail, opt => opt.MapFrom(src => src.Payee != null ? src.Payee.Email : null));

        // QrCode
        //CreateMap<CreateQrCodeDto, QrCode>();
        CreateMap<QrCode, QrCodeDto>();

        // PaymentMethod
        CreateMap<CreatePaymentMethodDto, PaymentMethod>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<PaymentMethodType>(src.Type, true)));
        CreateMap<PaymentMethod, PaymentMethodDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        // Notification
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        /*// Ecommerce
        CreateMap<CreateArticleDto, Article>();
        CreateMap<UpdateArticleDto, Article>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Article, ArticleDto>();

        CreateMap<CommandeArticleDto, CommandeArticle>(); // Pour la création de commande
        CreateMap<CommandeArticle, CommandeArticleDetailDto>()
            .ForMember(dest => dest.NomArticle, opt => opt.MapFrom(src => src.Article.Nom))
            .ForMember(dest => dest.SousTotal, opt => opt.MapFrom(src => src.Quantite * src.PrixAuMomentDeCommande));

        CreateMap<Commande, CommandeDto>()
            .ForMember(dest => dest.Articles, opt => opt.MapFrom(src => src.ArticleLinks))
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client)); // Mapper le client

        CreateMap<Facture, FactureDto>();*/
    }
}