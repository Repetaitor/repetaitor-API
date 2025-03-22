using System.Runtime.Intrinsics.X86;
using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Domain.Data;
using Core.Domain.Entities;
using Core.Domain.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Repositories;

public class AssignmentRepository(
    ApplicationContext context,
    IEssayRepository essayRepository,
    IUserRepository userRepository,
    IGroupRepository groupRepository) : IAssignmentRepository
{
    public async Task<AssignmentBaseModal?> CreateNewAssignment(int userId, string instructions,
        int groupId, int essayId, DateTime dueDate)
    {
        try
        {
            if (!context.Groups.Any(x => x.OwnerId == userId && x.Id == groupId) ||
                !context.Essays.Any(x => x.Id == essayId && x.CreatorId == userId)) return null;
            var assgn = new Assignment
            {
                Instructions = instructions,
                GroupId = groupId,
                EssayId = essayId,
                DueDate = dueDate,
                CreatorId = userId
            };
            await context.Assignments.AddAsync(assgn);
            await context.SaveChangesAsync();
            assgn = await context.Assignments.Include(assignment => assignment.Creator)
                .Include(assignment => assignment.Essay).FirstOrDefaultAsync(x => x.Id == assgn.Id);
            if (assgn == null) return null;
            return new AssignmentBaseModal()
            {
                Id = assgn.Id,
                Instructions = assgn.Instructions,
                GroupId = assgn.GroupId,
                Creator = UserMapper.ToUserModal(assgn.Creator),
                Essay = EssayMapper.ToEssayModal(assgn.Essay),
                DueDate = assgn.DueDate,
                CreationTime = assgn.CreationTime,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    // private async Task<bool> AssignTaskToAllGroupStudents(int assignmentId, int groupId)
    // {
    //     try
    //     {
    //         var userIds = await context.UserGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId).ToListAsync();
    //         var userAssgn = userIds.Select(userId => new UserAssignment()
    //             {
    //                 AssignmentId = assignmentId,
    //                 UserId = userId,
    //                 StatusId = 3,
    //                 Text = "",
    //                 WordCount = 0,
    //                 GrammarScore = 0,
    //                 FluencyScore = 0,
    //                 FeedbackSeen = false,
    //                 IsEvaluated = false,
    //                 AssignDate = DateTime.Now,
    //             })
    //             .ToList();
    //         await context.UserAssignments.AddRangeAsync(userAssgn);
    //         return true;
    //     }
    //     catch (Exception)
    //     {
    //         return false;
    //     }
    // }

    public async Task<AssignmentBaseModal?> GetAssignmentById(int assignmentId)
    {
        try
        {
            var assgn = await context.Assignments.Include(assignment => assignment.Creator)
                .Include(assignment => assignment.Essay).FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assgn == null) return null;
            return new AssignmentBaseModal()
            {
                Id = assgn.Id,
                Instructions = assgn.Instructions,
                GroupId = assgn.GroupId,
                Creator = UserMapper.ToUserModal(assgn.Creator),
                Essay = EssayMapper.ToEssayModal(assgn.Essay),
                DueDate = assgn.DueDate,
                CreationTime = assgn.CreationTime,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<AssignmentBaseModal?> UpdateAssignment(int userId, int assignmentId, string instructions,
        int essayId, DateTime dueDate)
    {
        try
        {
            if (!await context.Essays.AnyAsync(x => x.Id == essayId)) return null;
            var assgn = await context.Assignments.Include(assignment => assignment.Creator)
                .Include(assignment => assignment.Essay).FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assgn == null) return null;
            assgn.Instructions = instructions;
            assgn.EssayId = essayId;
            assgn.DueDate = dueDate;
            await context.SaveChangesAsync();
            return new AssignmentBaseModal()
            {
                Id = assgn.Id,
                Instructions = assgn.Instructions,
                GroupId = assgn.GroupId,
                Creator = UserMapper.ToUserModal(assgn.Creator),
                Essay = EssayMapper.ToEssayModal(assgn.Essay),
                DueDate = assgn.DueDate,
                CreationTime = assgn.CreationTime,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> EvaluateAssignment(int teacherId, int userId, int assignmentId, int fluencyScore,
        int grammarScore,
        List<EvaluationTextCommentModal> evaluationTextComments, List<GeneralCommentModal> generalComments)
    {
        try
        {
            var assignment = await context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assignment == null || assignment.CreatorId != teacherId) return false;
            var userAssgn =
                await context.UserAssignments.FirstOrDefaultAsync(x =>
                    x.AssignmentId == assignmentId && x.UserId == userId);
            if (userAssgn is not { StatusId: 1 }) return false;
            userAssgn.FluencyScore = fluencyScore;
            userAssgn.GrammarScore = grammarScore;
            userAssgn.IsEvaluated = true;
            var textComments = evaluationTextComments.Select(x => new EvaluationTextComment()
            {
                UserAssignmentId = userAssgn.Id,
                StatusId = x.StatusId,
                StartIndex = x.StartIndex,
                EndIndex = x.EndIndex,
                Comment = x.Comment,
            }).ToList();
            var generalCommentList = evaluationTextComments.Select(x => new GeneralComment()
            {
                UserAssignmentId = userAssgn.Id,
                StatusId = x.StatusId,
                Comment = x.Comment,
            }).ToList();
            await context.EvaluationTextComments.AddRangeAsync(textComments);
            await context.GeneralComments.AddRangeAsync(generalCommentList);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<AssignmentBaseModal>?> GetGroupAssignments(int userId, int groupId)
    {
        try
        {
            var group = await context.Groups.AnyAsync(x => x.OwnerId == userId && x.Id == groupId);
            if (!group) return null;
            var assignments = await context.Assignments
                .Where(x => x.GroupId == groupId).Include(assignment => assignment.Creator)
                .Include(assignment => assignment.Essay).Select(assgn => new AssignmentBaseModal()
                {
                    Id = assgn.Id,
                    Instructions = assgn.Instructions,
                    GroupId = assgn.GroupId,
                    Creator = UserMapper.ToUserModal(assgn.Creator),
                    Essay = EssayMapper.ToEssayModal(assgn.Essay),
                    DueDate = assgn.DueDate,
                    CreationTime = assgn.CreationTime,
                }).ToListAsync();
            return assignments;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<StatusBaseModal>?> GetAssignmentStatuses()
    {
        try
        {
            return await context.AssignmentStatuses.Select(s => new StatusBaseModal()
            {
                Id = s.Id,
                Name = s.Name,
            }).OrderBy(x => x.Name).ToListAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> SaveOrSubmitAssignment(int userId, int assignmentId, string text, int wordCount,
        bool isSubmitted)
    {
        try
        {
            var userAssgn =
                await context.UserAssignments.FirstOrDefaultAsync(x =>
                    x.AssignmentId == assignmentId && x.UserId == userId);
            if (userAssgn == null)
            {
                userAssgn = new UserAssignment()
                {
                    AssignmentId = assignmentId,
                    UserId = userId,
                    StatusId = isSubmitted ? 1 : 2,
                    Text = text,
                    WordCount = wordCount,
                    GrammarScore = 0,
                    FluencyScore = 0,
                    FeedbackSeen = false,
                    IsEvaluated = false,
                    AssignDate = DateTime.Now,
                };
                await context.UserAssignments.AddAsync(userAssgn);
                await context.SaveChangesAsync();
                return true;
            }

            if (userAssgn.StatusId == 1) return false;
            userAssgn.Text = text;
            userAssgn.WordCount = wordCount;
            userAssgn.StatusId = isSubmitted ? 1 : 2;
            userAssgn.SubmitDate = isSubmitted ? DateTime.Now : default;
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<StatusBaseModal>?> GetEvaluationStatuses()
    {
        try
        {
            return await context.EvaluationCommentsStatuses.Select(s => new StatusBaseModal()
            {
                Id = s.Id,
                Name = s.Name,
            }).OrderBy(x => x.Name).ToListAsync();
        }
        catch
        {
            return null;
        }
    }

    private async Task<StatusBaseModal?> GetAssignmentStatusById(int statusId)
    {
        try
        {
            var status = await context.AssignmentStatuses.FirstOrDefaultAsync(x => x.Id == statusId);
            if (status == null) return null;
            return new StatusBaseModal()
            {
                Id = status.Id,
                Name = status.Name,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<StatusBaseModal?> GetEvaluationStatusById(int statusId)
    {
        try
        {
            var status = await context.AssignmentStatuses.FirstOrDefaultAsync(x => x.Id == statusId);
            if (status == null) return null;
            return new StatusBaseModal()
            {
                Id = status.Id,
                Name = status.Name,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<UserAssignmentBaseModal>?> GetUserAssignments(int userId, int statusId)
    {
        try
        {
            var user = await userRepository.GetUserInfo(userId);
            if (user == null) return null;
            var group = await groupRepository.GetStudentGroup(userId);
            if (group == null) return null;
            var assignmentIds =
                await context.Assignments.Where(x => x.GroupId == group.Id).Select(x => x.Id).ToListAsync();
            var assignments = new List<UserAssignmentBaseModal>();
            foreach (var assgnId in assignmentIds)
            {
                var userAssgn =
                    await context.UserAssignments.Include(userAssignment => userAssignment.Status)
                        .Include(userAssignment => userAssignment.Assignment)
                        .Include(userAssignment => userAssignment.User).FirstOrDefaultAsync(x =>
                            x.AssignmentId == assgnId && x.UserId == userId);
                assignments.Add(new UserAssignmentBaseModal()
                {
                    Student = (userAssgn != null
                        ? UserMapper.ToUserModal(userAssgn.User)
                        : await userRepository.GetUserInfo(userId))!,
                    Assignment = (userAssgn != null
                        ? AssignmentMapper.ToAssignmentBaseModal(userAssgn.Assignment)
                        : await GetAssignmentById(assgnId))!,
                    Status = (userAssgn != null
                        ? new StatusBaseModal() { Id = userAssgn.Status.Id, Name = userAssgn.Status.Name }
                        : await GetAssignmentStatusById(3))!,
                    IsEvaluated = userAssgn?.IsEvaluated ?? false,
                    TotalScore = userAssgn != null
                        ? userAssgn.IsEvaluated ? userAssgn.FluencyScore + userAssgn.GrammarScore : -1
                        : -1,
                    ActualWordCount = userAssgn?.WordCount ?? 0,
                    SubmitDate = userAssgn?.SubmitDate,
                });
            }

            return assignments;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<UserAssignmentBaseModal>?> GetUserNotSeenEvaluatedAssignments(int userId)
    {
        try
        {
            var user = await userRepository.GetUserInfo(userId);
            if (user == null) return null;
            var group = await groupRepository.GetStudentGroup(userId);
            if (group == null) return null;
            var assignmentIds =
                await context.Assignments.Where(x => x.GroupId == group.Id).Select(x => x.Id).ToListAsync();

            var assignments = await context.UserAssignments
                .Where(x => x.UserId == userId && assignmentIds.Contains(x.AssignmentId) && x.IsEvaluated &&
                            !x.FeedbackSeen).Include(x => x.User)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Creator)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Essay).Include(x => x.Status)
                .Select(x => new UserAssignmentBaseModal()
                {
                    Student = UserMapper.ToUserModal(x.User),
                    Assignment = AssignmentMapper.ToAssignmentBaseModal(x.Assignment),
                    Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                    TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                    IsEvaluated = x.IsEvaluated,
                    ActualWordCount = x.WordCount,
                    SubmitDate = x.SubmitDate
                }).ToListAsync();


            return assignments;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<List<GeneralCommentModal>?> GetGeneralComments(int userId, int assignmentId)
    {
        try
        {
            var userAssgn =
                await context.UserAssignments.FirstOrDefaultAsync(x =>
                    x.AssignmentId == assignmentId && x.UserId == userId);
            if (userAssgn == null) return [];
            var genComets = await context.GeneralComments.Where(x => x.UserAssignmentId == userAssgn.Id).Select(x =>
                new GeneralCommentModal()
                {
                    Comment = x.Comment,
                    StatusId = x.StatusId,
                }).ToListAsync();
            return genComets;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<List<EvaluationTextCommentModal>?> GetEvaluationTextComments(int userId, int assignmentId)
    {
        try
        {
            var userAssgn =
                await context.UserAssignments.FirstOrDefaultAsync(x =>
                    x.AssignmentId == assignmentId && x.UserId == userId);
            if (userAssgn == null) return [];
            var evalTextComets = await context.EvaluationTextComments.Where(x => x.UserAssignmentId == userAssgn.Id)
                .Select(x =>
                    new EvaluationTextCommentModal()
                    {
                        StartIndex = x.StartIndex,
                        EndIndex = x.EndIndex,
                        Comment = x.Comment,
                        StatusId = x.StatusId,
                    }).ToListAsync();
            return evalTextComets;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<UserAssignmentModal?> GetUserAssignment(int callerId, int userId, int assignmentId)
    {
        try
        {
            var assgn = await context.UserAssignments
                .FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
            if (assgn == null)
            {
                var defAssgn = await GetAssignmentById(assignmentId);
                var userGroup = await groupRepository.GetStudentGroup(userId);
                if (defAssgn == null || userGroup == null || defAssgn.GroupId != userGroup.Id) return null;
                return new UserAssignmentModal()
                {
                    AssignDate = defAssgn.CreationTime,
                    FeedbackSeen = false,
                    FluencyScore = 0,
                    GrammarScore = 0,
                    AssignmentId = defAssgn.Id,
                    Status = await GetAssignmentStatusById(3),
                    Text = "",
                    WordCount = 0,
                    UserId = userId,
                    IsEvaluated = false,
                    GeneralCommentS = [],
                    EvaluationComments = []
                };
            }

            ;
            if (callerId == assgn.UserId && assgn.IsEvaluated)
            {
                assgn.FeedbackSeen = true;
                await context.SaveChangesAsync();
            }

            return new UserAssignmentModal()
            {
                AssignDate = assgn.AssignDate,
                FeedbackSeen = assgn.FeedbackSeen,
                FluencyScore = assgn.FluencyScore,
                GrammarScore = assgn.GrammarScore,
                AssignmentId = assgn.AssignmentId,
                Status = await GetAssignmentStatusById(assgn.StatusId),
                Text = assgn.Text,
                WordCount = assgn.WordCount,
                UserId = assgn.UserId,
                IsEvaluated = assgn.IsEvaluated,
                GeneralCommentS = await GetGeneralComments(assgn.UserId, assgn.AssignmentId) ?? [],
                EvaluationComments = await GetEvaluationTextComments(assgn.UserId, assgn.AssignmentId) ?? []
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<UserAssignmentBaseModal>?> GetTeacherAssignments(int userId)
    {
        try
        {
            var studentsSubmitAssignments = context.UserAssignments.Include(x => x.User)
                .Include(x => x.Assignment).ThenInclude(x => x.Creator)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Essay)
                .Include(x => x.Status)
                .Where(x => x.StatusId == 1 && !x.IsEvaluated && x.Assignment.CreatorId == userId);
                

            return await studentsSubmitAssignments.Select(x => new UserAssignmentBaseModal()
            {
                Student = UserMapper.ToUserModal(x.User),
                Assignment = AssignmentMapper.ToAssignmentBaseModal(x.Assignment),
                Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                ActualWordCount = x.WordCount,
                SubmitDate = x.SubmitDate
            }).ToListAsync();;
        }
        catch (Exception)
        {
            return null;
        }
    }
}