using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.ActionDtos;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.RepositoryAbstracts;

public class UserActionService : IUserActionService
{
    private readonly IUserActionRepository _userActionRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserActionService> _logger;

    public UserActionService(
        IUserActionRepository userActionRepository,
        IMapper mapper,
        UserManager<User> userManager,
        ILogger<UserActionService> logger)
    {
        _userActionRepository = userActionRepository;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<UserActionDto>> GetUserActionsAsync(string userId)
    {
        // Kullanıcının aksiyonları ve kullanıcıya yapılan aksiyonları alıyoruz.
        var actions = await _userActionRepository.GetAllAsync(
             a => a.TargetUserId == userId ,  // Aksiyon yapılan kullanıcı ve aksiyonu başlatan kullanıcı kontrolü
             "User", "Post", "Comment", "Post.Images"
         );
        var actionDtos = actions.Select(action => _mapper.Map<UserActionDto>(action)).ToList();
        foreach (var actionDto in actionDtos)
        {
            actionDto.Message = await GetActionMessageAsync(actionDto);
        }
        return actionDtos;
    }

    public async Task CreateUserActionAsync(CreateUserActionDto createUserActionDto)
    {
        try
        {
            _logger.LogInformation("Creating user action. UserId: {UserId}, PostId: {PostId}, ActionType: {ActionType}", createUserActionDto.UserId, createUserActionDto.PostId, createUserActionDto.ActionType);

            var userAction = new UserAction
            {
                UserId = createUserActionDto.UserId,
                TargetUserId = createUserActionDto.TargetUserId,
                PostId = createUserActionDto.PostId,
                CommentId = createUserActionDto.CommentId,
                ActionType = createUserActionDto.ActionType,
                CreatedDate = DateTime.UtcNow
            };

            await _userActionRepository.AddAsync(userAction);

            _logger.LogInformation("User action created successfully. UserId: {UserId}, PostId: {PostId}, ActionType: {ActionType}", createUserActionDto.UserId, createUserActionDto.PostId, createUserActionDto.ActionType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating user action. UserId: {UserId}, PostId: {PostId}, ActionType: {ActionType}", createUserActionDto.UserId, createUserActionDto.PostId, createUserActionDto.ActionType);
            throw;
        }
    }

    public async Task<string> GetActionMessageAsync(UserActionDto userAction)
    {
        var user = await _userManager.FindByIdAsync(userAction.UserId);  // Aksiyonu başlatan kullanıcı
        string message = string.Empty;

        switch (userAction.ActionType)
        {
            case "PostLiked":
                message = "Liked your post.";
                break;
            case "Reposted":
                message = "Reposted your post.";
                break;
            case "Commented":
                message = "Commented on your post.";
                break;
            case "Replied":
                message = "Replied to your comment.";
                break;
            case "Followed":
                message = "Started following you.";
                break;
            default:
                message = "Unknown action.";
                break;
        }

        return message;
    }
}
