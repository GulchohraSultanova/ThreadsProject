﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public interface IPostService
    {
        Task<IQueryable<PostGetDto>> GetExplorePostsAsync();
        Task<IQueryable<PostGetDto>> GetHomePostsAsync(string userId);
        Task<PostGetDto> GetPostAsync(Expression<Func<Post, bool>>? filter = null, params string[] includes);
        Task AddPostAsync(CreatePostDto createPostDto, string userId);
        Task<IQueryable<PostGetDto>> GetUserPostsAsync(string userId);
        Task DeletePostAsync(int id, string userId);
    }
}
