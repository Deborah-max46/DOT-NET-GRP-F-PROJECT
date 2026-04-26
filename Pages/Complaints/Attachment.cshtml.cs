using ConsumersVoiceSystemPrototype.Data;
using ConsumersVoiceSystemPrototype.Models;
using ConsumersVoiceSystemPrototype.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ConsumersVoiceSystemPrototype.Pages.Complaints;

[Authorize]
public class AttachmentModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly ComplaintWorkflowService _workflow;

    public AttachmentModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> users,
        ComplaintWorkflowService workflow)
    {
        _db = db;
        _users = users;
        _workflow = workflow;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var att = await _db.ComplaintAttachments
            .AsNoTracking()
            .Include(a => a.Complaint)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (att == null) return NotFound();

        var uid = _users.GetUserId(User);
        if (uid == null) return Forbid();

        var access = await _workflow.GetAccessAsync(att.Complaint, uid, User);
        if (access == null || !access.CanView) return Forbid();

        // StoredFileName now holds the full R2 public URL
        return Redirect(att.StoredFileName);
    }
}
