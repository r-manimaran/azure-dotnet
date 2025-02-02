﻿using Riok.Mapperly.Abstractions;
using WebApi.Dtos;
using WebApi.Models;

namespace WebApi.Mappings;

[Mapper]
public partial class PostMapper
{
    public partial PostResponse ToPostResponse(Post post);

    private string MapCategory(Category category)
    {
        return category?.Name?? string.Empty;
    }

    [MapperIgnoreTarget(nameof(Post.Id))]
    [MapperIgnoreTarget(nameof(Post.Category))]
    public partial Post ToPost(CreatePostRequest request);


    // Update mapping
    [MapperIgnoreTarget(nameof(Post.Category))]
    [MapperIgnoreTarget(nameof(Post.CreatedDate))]
    public partial void UpdatePost(UpdatePostRequest request, Post post);
}
