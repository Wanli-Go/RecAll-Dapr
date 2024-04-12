using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecAll.Core.List.Api.Commands;
using RecAll.Core.List.Api.Queries;
using RecAll.Core.List.Api.Services;
using System.ComponentModel.DataAnnotations;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ListController
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ListController> _logger;
    private readonly IMediator _mediator;
    private readonly IListQueryService _listQueryService;

    public ListController(IIdentityService identityService,
        ILogger<ListController> logger, IMediator mediator, IListQueryService listQueryService)
    {
        _identityService = identityService;
        _logger = logger;
        _mediator = mediator;
        _listQueryService = listQueryService;
    }

    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> CreateAsync(
        [FromBody] CreateListCommand command)
    {
        _logger.LogInformation(
            "----- Sending command: {CommandName} - UserIdentityGuid: {userIdentityGuid} ({@Command})",
            command.GetType().Name, _identityService.GetUserIdentityGuid(), command);
        var ret = await _mediator.Send(command);
        return ret.ToServiceResultViewModel();
    }

    [Route("update")]
    [HttpPost]
    public async Task<ActionResult<ServiceResultViewModel>> UpdateAsync(
        [FromBody] UpdateListCommand command)
    {
        _logger.LogInformation(
            "----- Sending command: {CommandName} - UserIdentityGuid: {userIdentityGuid} ({@Command})",
            command.GetType().Name, _identityService.GetUserIdentityGuid(),
            command);

        return (await _mediator.Send(command)).ToServiceResultViewModel();
    }

    [Route("list")]
    [HttpGet]
    public async
        Task<ActionResult<
            ServiceResultViewModel<(IEnumerable<ListViewModel>, int)>>>
        ListAsync(int skip = 0, int take = 20) =>
        ServiceResult<(IEnumerable<ListViewModel>, int)>
            .CreateSucceededResult(await _listQueryService.ListAsync(skip, take,
                _identityService.GetUserIdentityGuid()))
            .ToServiceResultViewModel();

    [Route("listByTypeId")]
    [HttpGet]
    public async
        Task<ActionResult<
            ServiceResultViewModel<(IEnumerable<ListViewModel>, int)>>>
        ListByTypeId([Required] int typeId, int skip = 0, int take = 20) =>
        ServiceResult<(IEnumerable<ListViewModel>, int)>
            .CreateSucceededResult(await _listQueryService.ListAsync(typeId,
                skip, take, _identityService.GetUserIdentityGuid()))
            .ToServiceResultViewModel();


    [Route("get/{id}")]
    [HttpGet]
    public async Task<ActionResult<ServiceResultViewModel<ListViewModel>>>
        GetAsync([Required] int id)
    {
        var listViewModel = await _listQueryService.GetAsync(id,
            _identityService.GetUserIdentityGuid());

        if (listViewModel is null)
        {
            _logger.LogWarning(
                $"用户{_identityService.GetUserIdentityGuid()}尝试查看已删除、不存在或不属于自己的List {id}");
            return ServiceResult<ListViewModel>
                .CreateFailedResult(ErrorMessage
                    .NotFoundOrDeletedOrIdentityMismatch)
                .ToServiceResultViewModel();
        }

        return ServiceResult<ListViewModel>.CreateSucceededResult(listViewModel)
            .ToServiceResultViewModel();
    }

}
