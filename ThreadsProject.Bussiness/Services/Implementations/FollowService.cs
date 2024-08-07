﻿using AutoMapper;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.ActionDtos;
using ThreadsProject.Bussiness.DTOs.FollowsDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class FollowService : IFollowService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly IFollowingRepository _followingRepository;
        private readonly IUserService _userService;
        private readonly IUserActionService _userActionService;
        private readonly IUserActionRepository _userActionRepository;
        private readonly IMapper _mapper;

        public FollowService(IRequestRepository requestRepository, IFollowerRepository followerRepository, IFollowingRepository followingRepository, IUserService userService, IMapper mapper, IUserActionService userActionService, IUserActionRepository userActionRepository)
        {
            _requestRepository = requestRepository;
            _followerRepository = followerRepository;
            _followingRepository = followingRepository;
            _userService = userService;
            _mapper = mapper;
            _userActionService = userActionService;
            _userActionRepository = userActionRepository;
        }

        public async Task AcceptFollowRequestAsync(int requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);

            if (request == null)
            {
                throw new GlobalAppException("Request not found.");
            }
            if (request.SenderId == request.ReceiverId)
            {
                throw new GlobalAppException("You cannot follow yourself.");
            }

            var sender = await _userService.GetUserByIdAsync(request.SenderId);
            var receiver = await _userService.GetUserByIdAsync(request.ReceiverId);

            if (sender == null || receiver == null)
            {
                throw new GlobalAppException("User not found.");
            }

            await _followerRepository.AddAsync(new Follower { UserId = request.ReceiverId, FollowerUserId = request.SenderId, CreatedDate = DateTime.UtcNow });
            await _followingRepository.AddAsync(new Following { UserId = request.SenderId, FollowingUserId = request.ReceiverId, CreatedDate = DateTime.UtcNow });

            await _requestRepository.DeleteAsync(request);

            // Takip aksiyonu ekle
            var action = new CreateUserActionDto
            {
                UserId = request.SenderId,
                TargetUserId = request.ReceiverId,
                ActionType = "Followed"
            };
            await _userActionService.CreateUserActionAsync(action);
        }

        public async Task FollowAsync(string senderId, string receiverId)

        {
            if (senderId == receiverId)
            {
                throw new GlobalAppException("You cannot follow yourself.");
            }

            var sender = await _userService.GetUserByIdAsync(senderId);
            var receiver = await _userService.GetUserByIdAsync(receiverId);

            if (sender == null || receiver == null)
            {
                throw new GlobalAppException("User not found.");
            }

            // Zaten gönderilmiş bir istek var mı kontrol et
            var existingRequest = await _requestRepository.GetAsync(r => r.SenderId == senderId && r.ReceiverId == receiverId);
            if (existingRequest != null)
            {
                throw new GlobalAppException("A follow request to this user has already been sent and is pending.");
            }

            // Kullanıcı zaten takip ediliyor mu kontrol et
            var existingFollowing = await _followingRepository.GetAsync(f => f.UserId == senderId && f.FollowingUserId == receiverId);
            if (existingFollowing != null)
            {
                throw new GlobalAppException("You are already following this user.");
            }

            if (receiver.IsPublic)
            {
                await _followerRepository.AddAsync(new Follower { UserId = receiverId, FollowerUserId = senderId, CreatedDate = DateTime.UtcNow });
                await _followingRepository.AddAsync(new Following { UserId = senderId, FollowingUserId = receiverId, CreatedDate = DateTime.UtcNow });

                // Takip aksiyonu ekle
                var action = new CreateUserActionDto
                {
                    UserId =senderId,
                    TargetUserId = receiverId,

                    ActionType = "Followed"
                };
                await _userActionService.CreateUserActionAsync(action);
            }
            else
            {
                await _requestRepository.AddAsync(new FollowRequest { SenderId = senderId, ReceiverId = receiverId, CreatedDate = DateTime.UtcNow });
            }
        }

        public async Task<IEnumerable<FollowDto>> GetFollowersAsync(string userId)
        {
            var followers = await _followerRepository.GetAllAsync(f => f.UserId == userId, "FollowerUser");
            return _mapper.Map<IEnumerable<FollowDto>>(followers);
        }

        public async Task<IEnumerable<FollowDto>> GetFollowingsAsync(string userId)
        {
            var followings = await _followingRepository.GetAllAsync(f => f.UserId == userId, "FollowingUser");
            return _mapper.Map<IEnumerable<FollowDto>>(followings);
        }

        public async Task<IEnumerable<FollowRequestDto>> GetFollowRequestsAsync(string userId)
        {
            var requests = await _requestRepository.GetAllAsync(r => r.ReceiverId == userId, "Sender");
            return _mapper.Map<IEnumerable<FollowRequestDto>>(requests);
        }

        public async Task RejectFollowRequestAsync(int requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);

            if (request == null)
            {
                throw new GlobalAppException("Request not found.");
            }

            await _requestRepository.DeleteAsync(request);
        }

        public async Task RemoveFollowingAsync(string userId, string followingId)
        {
            var following = await _followingRepository.GetAsync(f => f.UserId == userId && f.FollowingUserId == followingId);
            if (following == null)
            {
                throw new GlobalAppException("Following relationship not found.");
            }

            var follower = await _followerRepository.GetAsync(f => f.UserId == followingId && f.FollowerUserId == userId);
            if (follower != null)
            {
                await _followerRepository.DeleteAsync(follower);
            }

            await _followingRepository.DeleteAsync(following);

            // Takip aksiyonunu sil
            var existingAction = await _userActionRepository.GetAsync(a => a.UserId == userId && a.ActionType == "Followed");
            if (existingAction != null)
            {
                await _userActionRepository.DeleteAsync(existingAction);
            }
        }

        public async Task RemoveFollowerAsync(string userId, string followerId)
        {
            var follower = await _followerRepository.GetAsync(f => f.UserId == userId && f.FollowerUserId == followerId);
            if (follower == null)
            {
                throw new GlobalAppException("Follower relationship not found.");
            }

            var following = await _followingRepository.GetAsync(f => f.UserId == followerId && f.FollowingUserId == userId);
            if (following != null)
            {
                await _followingRepository.DeleteAsync(following);
            }

            await _followerRepository.DeleteAsync(follower);

            // Takip aksiyonunu sil
            var existingAction = await _userActionRepository.GetAsync(a => a.UserId == followerId && a.ActionType == "Followed");
            if (existingAction != null)
            {
                await _userActionRepository.DeleteAsync(existingAction);
            }
        }

        public async Task<IEnumerable<SentFollowRequestDto>> GetSentFollowRequestsAsync(string userId)
        {
            var sentRequests = await _requestRepository.GetAllAsync(r => r.SenderId == userId, "Receiver");
            return _mapper.Map<IEnumerable<SentFollowRequestDto>>(sentRequests);
        }

        public async Task CancelFollowRequestAsync(string senderId, string receiverId)
        {
            var request = await _requestRepository.GetAsync(r => r.SenderId == senderId && r.ReceiverId == receiverId);

            if (request == null)
            {
                throw new GlobalAppException("Follow request not found.");
            }

            await _requestRepository.DeleteAsync(request);
        }
    }
}
