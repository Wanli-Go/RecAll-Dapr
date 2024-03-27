using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.TextItem.Api.Commands;
using RecAll.Contrib.TextItem.Api.Services;
using TheSalLab.GeneralReturnValues;

// TODO: complete logger and resultviewmodel

namespace RecAll.Contrib.TextItem.Api.Controller;
[ApiController]
[Route("[controller]")]
public class TextItemController
{
    private readonly IIdentityService _identityService;
    private readonly TextItemContext _textItemContext;
    private readonly ILogger<TextItemController> _logger;

    // Constructor Injection
    public TextItemController(IIdentityService identityService,
        TextItemContext textItemContext, ILogger<TextItemController> logger)
    {
        _logger = logger;
        _identityService = identityService;
        _textItemContext = textItemContext;
    }


    [HttpPost]
    [Route("create")]
    public async Task<ServiceResultViewModel<string>> CreateAsync(
        [FromBody] CreateTextItemCommand command)
    {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command);
        var textItem = new Models.TextItem
        {
            Content = command.Content,
            UserIdentityGuid = _identityService.GetUserIdentityGuid(),
            IsDeleted = false
        };
        var textItemEntity = _textItemContext.Add(textItem);
        await _textItemContext.SaveChangesAsync();

        _logger.LogInformation("----- Command {CommandName} handled",
            command.GetType().Name);

        return ServiceResult<string>.CreateSucceededResult(textItemEntity.Entity.Id.ToString())
            .ToServiceResultViewModel();
    }

    [HttpGet]
    [Route("get/{id}")]
    public async Task<ActionResult<Models.TextItem>> GetAsync(int id)
    {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textItemContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null)
        {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem {id}");

            return new BadRequestResult();
        }
        else
        {
            return textItem;
        }

    }

    [Route("getItems")]
    [HttpPost]
    public async Task<ActionResult<IEnumerable<Models.TextItem>>> GetItemsAsync(
        GetItemsCommand command)
    {
        var itemIds = command.Ids.ToList();
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItems = await _textItemContext.TextItems.Where(p =>
                p.ItemId.HasValue && itemIds.Contains(p.ItemId.Value) &&
                p.UserIdentityGuid == userIdentityGuid && !p.IsDeleted)
            .ToListAsync();

        if (textItems.Count != itemIds.Count)
        {
            var missingIds = string.Join(",",
                itemIds.Except(textItems.Select(p => p.ItemId.Value))
                    .Select(p => p.ToString()));

            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem {missingIds}");

            return new BadRequestResult();
        }
        textItems.Sort((x, y) =>
            itemIds.IndexOf(x.ItemId.Value) - itemIds.IndexOf(y.ItemId.Value));

        return textItems;
    }


    [Route("getByItemId/{itemId}")]
    [HttpGet]
    public async Task<ActionResult<Models.TextItem>> GetByItemId(int itemId)
    {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textItemContext.TextItems.FirstOrDefaultAsync(p =>
            p.ItemId == itemId && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null)
        {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem {itemId}");
        }

        return textItem is null
            ? new BadRequestResult()
            : textItem;
    }


    [Route("update")]
    [HttpPost]
    public async Task<ServiceResultViewModel> UpdateAsync(
        [FromBody] UpdateTextItemCommand command)
    {
        _logger.LogInformation(
            "----- Handling command {CommandName} ({@Command})",
            command.GetType().Name, command);

        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textItemContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == command.Id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null)
        {
            _logger.LogWarning(
                $"用户{userIdentityGuid}尝试查看已删除、不存在或不属于自己的TextItem {command.Id}");

            return ServiceResult
                .CreateFailedResult($"Unknown TextItem id: {command.Id}")
                .ToServiceResultViewModel();
        }

        textItem.Content = command.Content;
        await _textItemContext.SaveChangesAsync();

        _logger.LogInformation("----- Command {CommandName} handled",
            command.GetType().Name);

        return ServiceResult.CreateSucceededResult().ToServiceResultViewModel();
    }
}

