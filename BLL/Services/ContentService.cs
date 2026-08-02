using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ContentService : IContentService
    {
        private readonly IContentRepository _repository;
        private readonly IMapper _mapper;

        public ContentService(IContentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContentDetailResponseDto?> GetContentById(int id)
        {
            var content = await _repository.GetContentByIdWithBlocks(id);
            if (content == null)
                return null;

            return _mapper.Map<ContentDetailResponseDto>(content);
        }

        public async Task<ContentDetailResponseDto?> GetContentBySlug(string slug)
        {
            var content = await _repository.GetContentBySlug(slug);
            if (content == null)
                return null;

            return _mapper.Map<ContentDetailResponseDto>(content);
        }

        public async Task<ContentDetailResponseDto> CreateContentWithBlocksAsync(CreateContentWithBlocksRequestDto request)
        {
            if (request.IsPublished && request.CategoryId.HasValue)
            {
                var category = await _repository.GetCategoryById(request.CategoryId.Value);
                if (category == null)
                {
                    throw new ArgumentException($"Category with id {request.CategoryId} not found.");
                }
                if (!category.IsActive)
                {
                    throw new InvalidOperationException("Cannot publish content because its category is inactive. Please activate the category before publishing.");
                }
            }

            var now = DateTime.UtcNow;
            var content = new Content
            {
                Title = request.Title,
                Slug = GenerateSlug(request.Title),
                Summary = request.Summary,
                ThumbnailUrl = request.ThumbnailUrl,
                CategoryContentId = request.CategoryId,
                IsPublished = request.IsPublished,
                CreatedAt = now,
                PublishedAt = request.IsPublished ? now : null
            };

            var blocks = new List<BlockContent>(request.Blocks?.Count ?? 0);
            for (var i = 0; i < (request.Blocks?.Count ?? 0); i++)
            {
                var item = request.Blocks![i];
                blocks.Add(new BlockContent
                {
                    ContentData = item.ContentData,
                    MediaUrl = item.MediaUrl,
                    Caption = item.Caption,
                    BlockType = string.IsNullOrWhiteSpace(item.BlockType) ? "Text" : item.BlockType,
                    SortOrder = item.SortOrder ?? (i + 1),
                    IsActive = item.IsActive,
                    CreatedAt = now
                });
            }

            var created = await _repository.CreateContentWithBlocksAsync(content, blocks);
            created.BlockContents = blocks;
            return _mapper.Map<ContentDetailResponseDto>(created);
        }

        public async Task<ContentResponseDto> UpdateContent(int id, UpdateContentRequestDto request)
        {
            var content = await _repository.GetContentById(id)
                ?? throw new KeyNotFoundException($"Content with id {id} not found.");

            if (request.IsPublished && request.CategoryId.HasValue)
            {
                var category = await _repository.GetCategoryById(request.CategoryId.Value);
                if (category == null)
                {
                    throw new ArgumentException($"Category with id {request.CategoryId} not found.");
                }
                if (!category.IsActive)
                {
                    throw new InvalidOperationException("Cannot publish content because its category is inactive. Please activate the category before publishing.");
                }
            }

            content.Title = request.Title;
            content.Slug = GenerateSlug(request.Title);
            content.Summary = request.Summary;
            content.ThumbnailUrl = request.ThumbnailUrl;
            content.CategoryContentId = request.CategoryId;
            content.IsPublished = request.IsPublished;
            content.UpdatedAt = DateTime.UtcNow;

            if (request.IsPublished && content.PublishedAt == null)
            {
                content.PublishedAt = DateTime.UtcNow;
            }

            var updated = await _repository.UpdateContent(content);
            return _mapper.Map<ContentResponseDto>(updated);
        }

        public async Task<ContentResponseDto> PublishContent(int id)
        {
            var content = await _repository.GetContentById(id)
                ?? throw new KeyNotFoundException($"Content with id {id} not found.");

            if (content.CategoryContentId.HasValue)
            {
                var category = await _repository.GetCategoryById(content.CategoryContentId.Value);
                if (category == null)
                {
                    throw new ArgumentException($"Category with id {content.CategoryContentId} not found.");
                }
                if (!category.IsActive)
                {
                    throw new InvalidOperationException("Cannot publish content because its category is inactive. Please activate the category before publishing.");
                }
            }

            content.IsPublished = true;
            content.PublishedAt = DateTime.UtcNow;
            content.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateContent(content);
            return _mapper.Map<ContentResponseDto>(updated);
        }

        public async Task<CategoryContentResponseDto> CreateCategory(CreateCategoryContentRequestDto request)
        {
            var category = new CategoryContent
            {
                Name = request.Name,
                Slug = string.IsNullOrWhiteSpace(request.Slug) ? GenerateSlug(request.Name) : request.Slug,
                Description = request.Description,
                IconUrl = request.IconUrl,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateCategory(category);

            return _mapper.Map<CategoryContentResponseDto>(created);
        }

        public async Task<List<CategoryContentResponseDto>> GetAllCategories(string? search = null, bool? isActive = null)
        {
            var categories = await _repository.GetAllCategories(search, isActive);
            return _mapper.Map<List<CategoryContentResponseDto>>(categories);
        }

        public async Task<PagedResultDto<CategoryContentResponseDto>> GetCategoriesPaged(int page, int pageSize, string? search = null, bool? isActive = null)
        {
            var (totalCount, items) = await _repository.GetCategoriesPaged(page, pageSize, search, isActive);
            var dtos = _mapper.Map<List<CategoryContentResponseDto>>(items);
            return new PagedResultDto<CategoryContentResponseDto>(totalCount, dtos);
        }

        public async Task<CategoryContentResponseDto> UpdateCategory(int id, CreateCategoryContentRequestDto request)
        {
            var category = await _repository.GetCategoryById(id)
                ?? throw new KeyNotFoundException($"Category with id {id} not found.");

            var wasActive = category.IsActive;
            var willBeActive = request.IsActive;

            category.Name = request.Name;
            category.Slug = string.IsNullOrWhiteSpace(request.Slug) ? GenerateSlug(request.Name) : request.Slug;
            category.Description = request.Description;
            category.IconUrl = request.IconUrl;
            category.IsActive = request.IsActive;

            var updated = await _repository.UpdateCategory(category);

            // Rule: deactivating a category must unpublish all its public contents
            if (wasActive && !willBeActive)
            {
                await _repository.UnpublishByCategoryIdAsync(id);
            }

            return _mapper.Map<CategoryContentResponseDto>(updated);
        }

        public async Task<BlockContentResponseDto> CreateBlock(CreateBlockContentRequestDto request)
        {
            var block = new BlockContent
            {
                ContentId = request.ContentId,
                ContentData = request.ContentData,
                MediaUrl = request.MediaUrl,
                Caption = request.Caption,
                BlockType = request.BlockType,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateBlock(block);

            return _mapper.Map<BlockContentResponseDto>(created);
        }

        public async Task<BlockContentResponseDto> UpdateBlock(int id, UpdateBlockContentRequestDto request)
        {
            var block = await _repository.GetBlockById(id)
                ?? throw new KeyNotFoundException($"Block with id {id} not found.");

            block.ContentData = request.ContentData;
            block.MediaUrl = request.MediaUrl;
            block.Caption = request.Caption;
            block.BlockType = request.BlockType;
            block.SortOrder = request.SortOrder;
            block.IsActive = request.IsActive;
            block.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateBlock(block);

            return _mapper.Map<BlockContentResponseDto>(updated);
        }

        public async Task RemoveBlock(int id)
        {
            var block = await _repository.GetBlockById(id)
                ?? throw new KeyNotFoundException($"Block with id {id} not found.");

            await _repository.RemoveBlock(id);
        }

        public async Task<PagedResultDto<ContentResponseDto>> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished, int? categoryId = null)
        {
            var (totalCount, items) = await _repository.GetContentsPaged(page, pageSize, search, isPublished, categoryId);

            var dtos = _mapper.Map<List<ContentResponseDto>>(items);
            return new PagedResultDto<ContentResponseDto>(totalCount, dtos);
        }



        private static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var slug = text.ToLowerInvariant().Trim();
            slug = slug.Replace(" ", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            return slug;
        }
    }
}
