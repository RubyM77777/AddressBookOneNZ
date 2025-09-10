using AddressBookOneNZ.Models;
using AddressBookOneNZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddressBookOneNZ.Controllers
{
    /// <summary>
    /// // GroupsController for managing groups in the address book.
    /// // IEnumerable-ReadOnlyList-Abstraction-Async Await-Pagination--Clean Architecture
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        // GET api/groups?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetAllGroups(int pageNumber = 1, int pageSize = 10)
        {
            var allGroups = await _groupService.GetAllGroupsAsync(pageNumber, pageSize);

            return Ok(allGroups);
        }

        // GET api/groups/{name}
        [HttpGet("{Name}")]
        public async Task<ActionResult<Group>> GetGroupByName(string Name)
        {
            var group = await _groupService.GetGroupByNameAsync(Name);
            return Ok(group);
        }

        // POST api/groups
        [HttpPost]
        public async Task<ActionResult<Group>> AddNewGroup([FromBody] Group newGroup)
        {
            var addedGroup = await _groupService.AddGroupAsync(newGroup);
            return CreatedAtAction(nameof(GetGroupByName), new { name = addedGroup.Name }, addedGroup);
        }

        // PUT api/groups/{name}
        [HttpPut("{Name}")]   
        public async Task<ActionResult<Group>> UpdateExistingGroup(string Name, [FromBody] Group updateGroup)
        {
            var updatedGroup = await _groupService.UpdateGroupAsync(Name,updateGroup);
            return Ok(updatedGroup);
        }

        // DELETE api/groups/{name}
        [HttpDelete("{Name}")]
        public async Task<ActionResult<Group>> DeleteGroup(string Name)
        {
            var deletedGroup = await _groupService.DeleteGroupAsync(Name);
            return Ok(deletedGroup);
        }
    }
}
