using System.Runtime.Intrinsics.X86;
using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Domain.Data;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Repositories;

public class AssignmentRepository(
    ApplicationContext context,
    IEssayRepository essayRepository,
    IUserRepository userRepository,
    IGroupRepository groupRepository) : IAssignmentRepository
{
    public async Task<AssignmentBaseModal?> CreateNewAssignment(int userId, string assignmentTitle, string instructions,
        int groupId, int essayId, DateTime dueDate)
    {
        try
        {
            var group = context.Groups.Any(x => x.OwnerId == userId && x.Id == groupId);
            if (!context.Groups.Any(x => x.OwnerId == userId && x.Id == groupId) ||
                !context.Essays.Any(x => x.Id == essayId && x.CreatorId == userId)) return null;
            var assgn = new Assignment
            {
                AssignmentTitle = assignmentTitle,
                Instructions = instructions,
                GroupId = groupId,
                EssayId = essayId,
                DueDate = dueDate,
                CreatorId = userId
            };
            await context.Assignments.AddAsync(assgn);
            await context.SaveChangesAsync();
            return new AssignmentBaseModal()
            {
                Id = assgn.Id,
                AssignmentTitle = assgn.AssignmentTitle,
                Instructions = assgn.Instructions,
                GroupId = assgn.GroupId,
                Creator = await userRepository.GetUserInfo(assgn.CreatorId),
                Essay = await essayRepository.GetEssay(assgn.EssayId),
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
            var assgn = await context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assgn == null) return null;
            return new AssignmentBaseModal()
            {
                Id = assgn.Id,
                AssignmentTitle = assgn.AssignmentTitle,
                Instructions = assgn.Instructions,
                GroupId = assgn.GroupId,
                Creator = await userRepository.GetUserInfo(assgn.CreatorId),
                Essay = await essayRepository.GetEssay(assgn.EssayId),
                DueDate = assgn.DueDate,
                CreationTime = assgn.CreationTime,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<AssignmentBaseModal?> UpdateAssignment(int userId, int assignmentId, string assignmentTitle,
        string instructions, int essayId)
    {
        try
        {
            if (!await context.Essays.AnyAsync(x => x.Id == essayId)) return null;
            var assgn = await context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assgn == null) return null;
            assgn.AssignmentTitle = assignmentTitle;
            assgn.Instructions = instructions;
            assgn.EssayId = essayId;
            await context.SaveChangesAsync();
            return new AssignmentBaseModal()
            {
                Id = assgn.Id,
                AssignmentTitle = assgn.AssignmentTitle,
                Instructions = assgn.Instructions,
                GroupId = assgn.GroupId,
                Creator = await userRepository.GetUserInfo(assgn.CreatorId),
                Essay = await essayRepository.GetEssay(assgn.EssayId),
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
            var assignmentsData = await context.Assignments
                .Where(x => x.GroupId == groupId)
                .ToListAsync();
            
            var assignments = new List<AssignmentBaseModal>();
            foreach (var assgn in assignmentsData)
            {
                var creator = await userRepository.GetUserInfo(assgn.CreatorId);
                var essay = await essayRepository.GetEssay(assgn.EssayId);
                assignments.Add(new AssignmentBaseModal()
                {
                    Id = assgn.Id,
                    AssignmentTitle = assgn.AssignmentTitle,
                    Instructions = assgn.Instructions,
                    GroupId = assgn.GroupId,
                    Creator = creator,
                    Essay = essay,
                    DueDate = assgn.DueDate,
                    CreationTime = assgn.CreationTime,
                });
            }
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
                Color = s.Color,
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

            ;
            userAssgn.Text = text;
            userAssgn.WordCount = wordCount;
            userAssgn.StatusId = isSubmitted ? 1 : 2;
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
                Color = s.Color,
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
                Color = status.Color
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
                Color = status.Color
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<AssignmentBaseModal>?> GetUserAssignments(int userId, int statusId)
    {
        try
        {
            var user = await userRepository.GetUserInfo(userId);
            if (user == null) return null;
            var group = await groupRepository.GetStudentGroup(userId);
            if (group == null) return null;
            var assignmentIds = await context.Assignments.Where(x => x.GroupId == group.Id).ToListAsync();
            var assignments = new List<AssignmentBaseModal>();
            foreach (var assignment in assignmentIds)
            {
                var userAssign =
                    await context.UserAssignments.FirstOrDefaultAsync(x => x.AssignmentId == assignment.Id);
                if (userAssign == null && statusId != 3 && statusId != -1) continue;
                assignments.Add(new AssignmentBaseModal()
                {
                    Id = assignment.Id,
                    AssignmentTitle = assignment.AssignmentTitle,
                    Instructions = assignment.Instructions,
                    GroupId = assignment.GroupId,
                    Creator = await userRepository.GetUserInfo(assignment.CreatorId),
                    Essay = await essayRepository.GetEssay(assignment.EssayId),
                    Status = await GetAssignmentStatusById(userAssign?.StatusId ?? 3) ?? null,
                    DueDate = assignment.DueDate,
                    CreationTime = assignment.CreationTime,
                    TotalScore = userAssign is { IsEvaluated: true }
                        ? userAssign.FluencyScore + userAssign.GrammarScore
                        : -1
                });
            }

            return assignments;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<AssignmentBaseModal>?> GetUserNotSeenEvaluatedAssignments(int userId)
    {
        try
        {
            var assgns = await context.UserAssignments
                .Where(x => x.UserId == userId && x.StatusId == 1 && x.IsEvaluated == true && x.FeedbackSeen == false)
                .ToListAsync();
            var assignments = new List<AssignmentBaseModal>();
            foreach (var x in assgns)
            {
                var assgn = await GetAssignmentById(x.AssignmentId);
                assignments.Add(new AssignmentBaseModal()
                {
                    Id = assgn.Id,
                    AssignmentTitle = assgn.AssignmentTitle,
                    Instructions = assgn.Instructions,
                    GroupId = assgn.GroupId,
                    Creator = assgn.Creator,
                    Essay = assgn.Essay,
                    Status = await GetAssignmentStatusById(1),
                    DueDate = assgn.DueDate,
                    CreationTime = assgn.CreationTime,
                    TotalScore = x.FluencyScore + x.GrammarScore
                });
            }
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
            if (assgn == null) return null;
            if (callerId == assgn.UserId && assgn.IsEvaluated)
            {
                assgn.FeedbackSeen = true;
                await context.SaveChangesAsync();
            }

            return new UserAssignmentModal()
            {
                Id = assgn.Id,
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

    public async Task<List<AssignmentBaseModal>?> GetTeacherAssignments(int userId)
    {
        try
        {
            var teacherAssignments =
                await context.Assignments.Where(x => x.CreatorId == userId).Select(x => x.Id).ToListAsync();
            var studentsSubmitAssignments = await context.UserAssignments
                .Where(x => x.StatusId == 1 && !x.IsEvaluated && teacherAssignments.Contains(x.AssignmentId))
                .ToListAsync();
            var jobs = new List<AssignmentBaseModal>();
            foreach (var x in studentsSubmitAssignments)
            {
                var assgn = await GetAssignmentById(x.AssignmentId);
                jobs.Add(new AssignmentBaseModal()
                {
                    Id = assgn.Id,
                    AssignmentTitle = assgn.AssignmentTitle,
                    Instructions = assgn.Instructions,
                    GroupId = assgn.GroupId,
                    Creator = assgn.Creator,
                    Essay = assgn.Essay,
                    Status = await GetAssignmentStatusById(1),
                    DueDate = assgn.DueDate,
                    CreationTime = assgn.CreationTime,
                    TotalScore = -1
                });
            }
            return jobs;
        }
        catch (Exception)
        {
            return null;
        }
    }
}