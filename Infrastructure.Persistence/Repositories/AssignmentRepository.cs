using System.Data;
using AutoMapper;
using Core.Application.Interfaces.Repositories;
using Core.Application.Models.RequestsDTO.Essays;
using Core.Application.Models.ReturnViewModels;
using Core.Domain.Entities;
using Infrastructure.Persistence.AppContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Persistence.Repositories;

public class AssignmentRepository(
    ApplicationContext context,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IMapper mapper) : IAssignmentRepository
{
    public async Task<UserPerformanceViewModel> GetUserPerformance(int userId, DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var userGroupedCompletedAssignments = await context.UserAssignments
            .Where(x => x.UserId == userId && x.IsEvaluated && ((fromDate == null && toDate == null) ||
                                                                (x.SubmitDate >= fromDate &&
                                                                 x.SubmitDate <= toDate))).GroupBy(x =>
                new { x.SubmitDate.Year, x.SubmitDate.Month })
            .Select(g => new PerformanceStat()
            {
                DateTime = new DateTime(g.Key.Year, g.Key.Month, 1),
                TotalScoreAvg = double.Round(g.Average(x => x.FluencyScore + x.GrammarScore), 2),
                GrammarScoreAvg = double.Round(g.Average(x => x.GrammarScore), 2),
                FluencyScoreAvg = double.Round(g.Average(x => x.FluencyScore), 2)
            }).ToListAsync();
        return new UserPerformanceViewModel()
        {
            PerformanceStats = userGroupedCompletedAssignments.OrderBy(x => x.DateTime).ToList()
        };
    }

    public async Task<UserPerformanceViewModel> GetAllUserPerformanceForTeacher(int teacherId,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var userGroupedCompletedAssignments = await context.UserAssignments
            .Where(x => x.Assignment.CreatorId == teacherId && x.IsEvaluated && ((fromDate == null && toDate == null) ||
                (x.SubmitDate >= fromDate &&
                 x.SubmitDate <= toDate))).GroupBy(x =>
                new { x.Assignment.CreationTime.Year, x.Assignment.CreationTime.Month })
            .Select(g => new PerformanceStat()
            {
                DateTime = new DateTime(g.Key.Year, g.Key.Month, 1),
                TotalScoreAvg = double.Round(g.Average(x => x.FluencyScore + x.GrammarScore), 2),
                GrammarScoreAvg = double.Round(g.Average(x => x.GrammarScore), 2),
                FluencyScoreAvg = double.Round(g.Average(x => x.FluencyScore), 2)
            }).ToListAsync();
        return new UserPerformanceViewModel()
        {
            PerformanceStats = userGroupedCompletedAssignments
        };
    }

    public async Task<ResultResponse> MakeUserAssignmentPublic(int userId, int assignmentId)
    {
        var userAssignments = await context.UserAssignments
            .Include(x => x.Assignment)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
        if (userAssignments == null)
        {
            throw new Exception("User assignment not found");
        }

        if (userAssignments.Assignment.CreatorId != 1019)
        {
            throw new Exception("Its not AI assignment");
        }

        userAssignments.IsPublic = !userAssignments.IsPublic;
        await context.SaveChangesAsync();
        return new ResultResponse()
        {
            Result = true
        };
    }

    public async Task<List<UserAssignmentBaseModal>> GetPublicUserAssignments(int userId, int assignmentId, int? offset,
        int? limit)
    {
        var userAssignment =
            await context.UserAssignments.FirstOrDefaultAsync(x =>
                x.AssignmentId == assignmentId && x.UserId == userId);
        if (userAssignment is not { StatusId: 1 })
        {
            throw new Exception("You are not allowed to see public assignments of this AI Assignment");
        }

        var assignment =
            await context.Assignments.Include(x => x.Creator).Include(x => x.Essay)
                .FirstOrDefaultAsync(x => x.Id == assignmentId);
        if (assignment is not { CreatorId: 1019 })
        {
            throw new Exception("This is not AI assignment");
        }

        var userAssignments =
            await context.UserAssignments
                .Include(x => x.Status)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Creator)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Essay)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Group)
                .Include(x => x.User)
                .Where(x => x.AssignmentId == assignmentId &&
                            x.Assignment.Group.IsAIGroup == true && x.IsPublic)
                .OrderByDescending(x => x.SubmitDate).Skip(offset ?? 0)
                .Take(limit ?? 5).Select(x =>
                    new UserAssignmentBaseModal()
                    {
                        Student = mapper.Map<UserModal>(x.User),
                        Assignment = mapper.Map<AssignmentBaseModal>(x.Assignment),
                        Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                        IsEvaluated = x.IsEvaluated,
                        TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                        ActualWordCount = x.WordCount,
                        SubmitDate = x.SubmitDate,
                    }).ToListAsync();
        return userAssignments;
    }

    public async Task<ResultResponse> IsAssignmentPublic(int userId, int assignmentId)
    {
        var userAssignment = await context.UserAssignments.FirstOrDefaultAsync(x =>
            x.AssignmentId == assignmentId && x.UserId == userId);
        if (userAssignment == null)
        {
            throw new Exception("User assignment not found");
        }

        return new ResultResponse()
        {
            Result = userAssignment.IsPublic
        };
    }

    public async Task<bool> SaveImagesForAssignment(int userId, int assignmentId, string imageUrl)
    {
        var image = new AssignmentImagesStore()
        {
            UserId = userId,
            AssignmentId = assignmentId,
            ImageUrl = imageUrl
        };

        await context.AssignmentImagesStores.AddAsync(image);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<List<string>> GetUserAssignmentImagesUrl(int userId, int assignmentId)
    {
        return context.AssignmentImagesStores
            .Where(x => x.UserId == userId && x.AssignmentId == assignmentId)
            .Select(x => x.ImageUrl).ToListAsync();
    }

    public async Task<bool> ClearUserAssignemntImagesUrl(int userId, int assignmentId)
    {
        var images = context.AssignmentImagesStores
            .Where(x => x.UserId == userId && x.AssignmentId == assignmentId);
        if (images.IsNullOrEmpty())
        {
            return true;
        }

        context.AssignmentImagesStores.RemoveRange(images);
        var t = await context.SaveChangesAsync();
        return t > 0;
    }

    public async Task<bool> ClearUserAssignemntImagesFromDb(int userId, int assignmentId)
    {
        var imgs = context.UserAssignmentImages
            .Where(x => x.UserId == userId && x.AssignmentId == assignmentId);
        context.UserAssignmentImages.RemoveRange(imgs);
        await context.SaveChangesAsync().ContinueWith(t => t.Result > 0);
        return true;
    }

    public async Task<bool> SaveImagesForAssignmentDb(int userId, int assignmentId, List<string> imagesBase64)
    {
        var images = imagesBase64.Select(image => new UserAssignmentImages()
        {
            UserId = userId,
            AssignmentId = assignmentId,
            ImageBase64 = image
        }).ToList();
        context.UserAssignmentImages.AddRange(images);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<string>> GetImagesForUserAssignmentDb(int userId, int assignmentId)
    {
        var images = await context.UserAssignmentImages.Where(x => x.UserId == userId && x.AssignmentId == assignmentId)
            .Select(x => x.ImageBase64).ToListAsync();
        return images;
    }

    public async Task<UserAssignmentsStatusesStats> GetUserAssignmentsStatusStat(int userId)
    {
        var userAssignments = context.UserAssignments.Where(x => x.UserId == userId);
        return new UserAssignmentsStatusesStats()
        {
            CompletedAssignmentsCount = await userAssignments.CountAsync(x => x.StatusId == 1),
            InProgressAssignmentsCount = await userAssignments.CountAsync(x => x.StatusId == 2),
            PendingAssignmentsCount = await userAssignments.CountAsync(x => x.StatusId == 3)
        };
    }

    public async Task<ResultResponse> DeleteAssignment(int userId, int assignmentId)
    {
        var assignment = await context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
        if (assignment == null)
        {
            throw new Exception("Assignment not found");
        }

        if (assignment.CreatorId != userId)
        {
            throw new Exception("You are not the owner of this assignment");
        }

        context.Assignments.Remove(assignment);
        await context.SaveChangesAsync();
        return new ResultResponse()
        {
            Result = true
        };
    }

    public async Task<UserScoresStatsModel> GetAverageUserScoreByDate(int userId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var res = context.UserAssignments.Where(x =>
            x.UserId == userId && x.IsEvaluated == true);
        if (res.IsNullOrEmpty())
        {
            return new UserScoresStatsModel()
            {
                AvgTotalScore = 0,
                AvgGrammarScore = 0,
                AvgFluencyScore = 0,
            };
        }

        return new UserScoresStatsModel()
        {
            AvgTotalScore = double.Round(await res.AverageAsync(x => x.FluencyScore + x.GrammarScore), 2),
            AvgGrammarScore = double.Round(await res.AverageAsync(x => x.GrammarScore), 2),
            AvgFluencyScore = double.Round(await res.AverageAsync(x => x.FluencyScore), 2)
        };
    }

    public async Task<AssignmentBaseModal> CreateNewAssignment(int userId, string instructions,
        int groupId, int essayId, DateTime dueDate)
    {
        if (!context.Groups.Any(x => x.OwnerId == userId && x.Id == groupId))
            throw new Exception("You are not the owner of this group");
        if (!context.Essays.Any(x => x.Id == essayId && x.CreatorId == userId))
            throw new Exception("Essay not found or you are not the owner of this essay");
        await using var conn = new SqlConnection(context.Database.GetConnectionString());
        await using var cmd = new SqlCommand("CreateNewAssignment", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@instructions", instructions);
        cmd.Parameters.AddWithValue("@group", groupId);
        cmd.Parameters.AddWithValue("@essay", essayId);
        cmd.Parameters.AddWithValue("@dueDate", dueDate);
        cmd.Parameters.AddWithValue("@creator", userId);
        var outputParam = new SqlParameter("@newAssignmentId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(outputParam);

        conn.Open();
        cmd.ExecuteNonQuery();

        var newAssignmentId = (int)outputParam.Value;
        return await GetAssignmentById(newAssignmentId);
    }

    public async Task<AssignmentBaseModal> GetAssignmentById(int assignmentId)
    {
        var assignment = await context.Assignments.Include(assignment => assignment.Creator)
            .Include(assignment => assignment.Essay).FirstOrDefaultAsync(x => x.Id == assignmentId);
        if (assignment == null)
            throw new Exception("Assignment not found");
        return new AssignmentBaseModal
        {
            Id = assignment.Id,
            Instructions = assignment.Instructions,
            GroupId = assignment.GroupId,
            Creator = mapper.Map<UserModal>(assignment.Creator),
            Essay = mapper.Map<EssayModal>(assignment.Essay),
            DueDate = assignment.DueDate,
            CreationTime = assignment.CreationTime,
        };
    }

    public async Task<AssignmentBaseModal> UpdateAssignment(int userId, int assignmentId,
        string instructions,
        int essayId, DateTime dueDate)
    {
        if (!await context.Essays.AnyAsync(x => x.Id == essayId))
            throw new Exception("Essay not found");
        var assignment = await context.Assignments.Include(assignment => assignment.Creator)
            .Include(assignment => assignment.Essay).FirstOrDefaultAsync(x => x.Id == assignmentId);
        if (assignment == null)
            throw new Exception("Assignment not found");
        if (assignment.CreatorId != userId)
            throw new Exception("You are not the owner of this assignment");
        assignment.Instructions = instructions;
        assignment.EssayId = essayId;
        assignment.DueDate = dueDate;
        await context.SaveChangesAsync();
        return new AssignmentBaseModal
        {
            Id = assignment.Id,
            Instructions = assignment.Instructions,
            GroupId = assignment.GroupId,
            Creator = mapper.Map<UserModal>(assignment.Creator),
            Essay = mapper.Map<EssayModal>(assignment.Essay),
            DueDate = assignment.DueDate,
            CreationTime = assignment.CreationTime,
        };
    }

    public async Task<bool> EvaluateAssignment(int teacherId, int userId, int assignmentId,
        int fluencyScore,
        int grammarScore,
        List<EvaluationTextCommentModal> evaluationTextComments, List<GeneralCommentModal> generalComments)
    {
        var assignment = await context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
        if (assignment == null || assignment.CreatorId != teacherId)
            throw new Exception("You are not the owner of this assignment");
        var userAssgn =
            await context.UserAssignments.FirstOrDefaultAsync(x =>
                x.AssignmentId == assignmentId && x.UserId == userId);
        if (userAssgn is not { StatusId: 1 })
            throw new Exception("You can only evaluate submitted assignments");
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
        var generalCommentList = generalComments.Select(x => new GeneralComment()
        {
            UserAssignmentId = userAssgn.Id,
            StatusId = x.StatusId,
            Comment = x.Comment,
        }).ToList();
        userAssgn.EvaluateDate = DateTime.Now;
        await context.EvaluationTextComments.AddRangeAsync(textComments);
        await context.GeneralComments.AddRangeAsync(generalCommentList);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<GroupAssignmentBaseModal>?, int)> GetGroupAssignments(int userId, int groupId,
        int? pageIndex, int? pageSize)
    {
        var group = await context.Groups.AnyAsync(x => x.OwnerId == userId && x.Id == groupId);
        if (!group)
            throw new Exception("You are not the owner of this group");
        var assignments
            = await context.Assignments
                .Include(assignment => assignment.Creator)
                .Include(assignment => assignment.Essay)
                .Where(x => x.GroupId == groupId).OrderByDescending(x => x.CreationTime)
                .Skip((pageIndex - 1) * pageSize ?? 0)
                .Take(pageSize ?? 5).ToListAsync();
        var assgnsModals = assignments.Select(assgn => new GroupAssignmentBaseModal()
        {
            Id = assgn.Id,
            Instructions = assgn.Instructions,
            GroupId = assgn.GroupId,
            Creator = mapper.Map<UserModal>(assgn.Creator),
            Essay = mapper.Map<EssayModal>(assgn.Essay),
            DueDate = assgn.DueDate,
            CreationTime = assgn.CreationTime,
            CompletedPercentage = GetAssignmentCompletePercentage(assgn.Id)
        }).ToList();
        var cnt = await context.Assignments
            .CountAsync(x => x.GroupId == groupId);
        return (assgnsModals, cnt);
    }

    public decimal GetAssignmentCompletePercentage(int assignmentId)
    {
        if (!context.Assignments.Any(x => x.Id == assignmentId))
            throw new Exception("Assignment not found");
        var totalUserAssignments = context.UserAssignments.Count(x => x.AssignmentId == assignmentId);
        if (totalUserAssignments == 0) return 0;
        var completedUserAssignmentsCount = context.UserAssignments.Count(x =>
            x.AssignmentId == assignmentId && x.StatusId == 1);
        return Math.Round((decimal)completedUserAssignmentsCount / totalUserAssignments * 100, 0);
    }

    public async Task<List<StatusBaseModal>> GetAssignmentStatuses()
    {
        return await context.AssignmentStatuses.Select(s => new StatusBaseModal()
        {
            Id = s.Id,
            Name = s.Name,
        }).OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<bool> SaveOrSubmitAssignment(int userId, int assignmentId, string text,
        int wordCount,
        bool isSubmitted)
    {
        var userAssgn =
            await context.UserAssignments.FirstOrDefaultAsync(x =>
                x.AssignmentId == assignmentId && x.UserId == userId);
        if (userAssgn == null || userAssgn.StatusId == 1)
            throw new Exception("You can only save or submit assignments that are in progress");
        userAssgn.Text = text;
        userAssgn.WordCount = wordCount;
        userAssgn.StatusId = isSubmitted ? 1 : 2;
        userAssgn.SubmitDate = isSubmitted ? DateTime.Now : default;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<StatusBaseModal>> GetEvaluationStatuses()
    {
        return await context.EvaluationCommentsStatuses.Select(s => new StatusBaseModal()
        {
            Id = s.Id,
            Name = s.Name,
        }).OrderBy(x => x.Name).ToListAsync();
    }

    private async Task<StatusBaseModal?> GetAssignmentStatusById(int statusId)
    {
        var status = await context.AssignmentStatuses.FirstOrDefaultAsync(x => x.Id == statusId);
        if (status == null) return null;
        return new StatusBaseModal()
        {
            Id = status.Id,
            Name = status.Name,
        };
    }

    private async Task<StatusBaseModal?> GetEvaluationStatusById(int statusId)
    {
        var status = await context.AssignmentStatuses.FirstOrDefaultAsync(x => x.Id == statusId);
        if (status == null) return null;
        return new StatusBaseModal()
        {
            Id = status.Id,
            Name = status.Name,
        };
    }

    public async Task<(List<UserAssignmentBaseModal>?, int)> GetUserAssignments(int userId,
        string statusName,
        bool isAiAssignment, int? pageIndex,
        int? pageSize)
    {
        var userAssgns =
            await context.UserAssignments
                .Include(x => x.Status)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Creator)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Essay)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Group)
                .Include(userAssignment => userAssignment.User)
                .Where(x => x.UserId == userId &&
                            (statusName == string.Empty || x.Status.Name == statusName) &&
                            x.Assignment.Group.IsAIGroup == isAiAssignment)
                .OrderByDescending(x => x.Assignment.CreationTime).Skip((pageIndex - 1) * pageSize ?? 0)
                .Take(pageSize ?? 5).Select(x =>
                    new UserAssignmentBaseModal()
                    {
                        Student = mapper.Map<UserModal>(x.User),
                        Assignment = mapper.Map<AssignmentBaseModal>(x.Assignment),
                        Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                        IsEvaluated = x.IsEvaluated,
                        TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                        ActualWordCount = x.WordCount,
                        SubmitDate = x.SubmitDate,
                        IsPublic = x.IsPublic
                    }).ToListAsync();
        var cnt = await context.UserAssignments.Include(x => x.Status)
            .CountAsync(x =>
                x.UserId == userId &&
                (statusName == string.Empty ||
                 x.Status.Name == statusName) &&
                x.Assignment.Group.IsAIGroup == isAiAssignment);
        return (userAssgns, cnt);
    }

    public async Task<(List<UserAssignmentBaseModal>?, int)> GetUserNotSeenEvaluatedAssignments(
        int userId, int? pageIndex,
        int? pageSize)
    {
        var user = await userRepository.GetUserInfo(userId);
        if (user == null)
            throw new Exception("User not found");
        var group = await groupRepository.GetStudentGroup(userId);
        var assignmentIds =
            await context.Assignments.Where(x => x.GroupId == group.Id).Select(x => x.Id).ToListAsync();

        var assignments = await context.UserAssignments
            .Where(x => x.UserId == userId && assignmentIds.Contains(x.AssignmentId) && x.IsEvaluated &&
                        !x.FeedbackSeen).Skip((pageIndex - 1) * pageSize ?? 0).Take(pageSize ?? 5).Include(x => x.User)
            .Include(x => x.Assignment)
            .ThenInclude(a => a.Creator)
            .Include(x => x.Assignment)
            .ThenInclude(a => a.Essay).Include(x => x.Status)
            .Select(x => new UserAssignmentBaseModal()
            {
                Student = mapper.Map<UserModal>(x.User),
                Assignment = mapper.Map<AssignmentBaseModal>(x.Assignment),
                Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                IsEvaluated = x.IsEvaluated,
                ActualWordCount = x.WordCount,
                SubmitDate = x.SubmitDate
            }).ToListAsync();
        var cnt = await context.UserAssignments
            .CountAsync(x => x.UserId == userId && assignmentIds.Contains(x.AssignmentId) && x.IsEvaluated &&
                             !x.FeedbackSeen);

        return (assignments, cnt);
    }

    private async Task<List<GeneralCommentModal>?> GetGeneralComments(int userId, int assignmentId)
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

    private async Task<List<EvaluationTextCommentModal>?> GetEvaluationTextComments(int userId, int assignmentId)
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

    public async Task<UserAssignmentModal> GetUserAssignment(int callerId, int userId, int assignmentId)
    {
        var assgn = await context.UserAssignments
            .FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
        if (assgn == null)
            throw new Exception("Assignment not found for this user");
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
            GeneralComments = await GetGeneralComments(assgn.UserId, assgn.AssignmentId) ?? [],
            EvaluationComments = await GetEvaluationTextComments(assgn.UserId, assgn.AssignmentId) ?? [],
            IsPublic = assgn.IsPublic,
        };
    }

    public async Task<(List<UserAssignmentBaseModal>?, int)> GetTeacherAssignments(int userId,
        int? pageIndex, int? pageSize)
    {
        var studentsSubmitAssignments = await context.UserAssignments.Include(x => x.User)
            .Include(x => x.Assignment).ThenInclude(x => x.Creator)
            .Include(x => x.Assignment)
            .ThenInclude(a => a.Essay)
            .Include(x => x.Status)
            .Where(x => x.StatusId == 1 && !x.IsEvaluated && x.Assignment.CreatorId == userId)
            .Skip((pageIndex - 1) * pageSize ?? 0)
            .Take(pageSize ?? 5).Select(x => new UserAssignmentBaseModal()
            {
                Student = mapper.Map<UserModal>(x.User),
                Assignment = mapper.Map<AssignmentBaseModal>(x.Assignment),
                Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                ActualWordCount = x.WordCount,
                SubmitDate = x.SubmitDate
            }).ToListAsync();
        var cnt = await context.UserAssignments
            .CountAsync(x => x.StatusId == 1 && !x.IsEvaluated && x.Assignment.CreatorId == userId);
        return (studentsSubmitAssignments, cnt);
    }

    public async Task<(List<UserAssignmentBaseModal>?, int)> GetAssigmentUsersTasks(int userId,
        int assignmentId,
        string statusName,
        int? pageIndex,
        int? pageSize)
    {
        if (!await context.Assignments.AnyAsync(x => x.Id == assignmentId && x.CreatorId == userId))
            throw new Exception("You are not the owner of this assignment");
        var userAssgns =
            await context.UserAssignments.Include(userAssignment => userAssignment.User)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Creator)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Essay)
                .Include(userAssignment => userAssignment.Status).Where(x =>
                    x.AssignmentId == assignmentId &&
                    (statusName == string.Empty || x.Status.Name == statusName.ToLower()))
                .OrderBy(x => x.StatusId).Skip((pageIndex - 1) * pageSize ?? 0)
                .Take(pageSize ?? 10).Select(x =>
                    new UserAssignmentBaseModal()
                    {
                        Student = mapper.Map<UserModal>(x.User),
                        Assignment = mapper.Map<AssignmentBaseModal>(x.Assignment),
                        Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                        IsEvaluated = x.IsEvaluated,
                        TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                        ActualWordCount = x.WordCount,
                        SubmitDate = x.SubmitDate,
                    }).ToListAsync();
        var cnt = await context.UserAssignments.CountAsync(x =>
            x.AssignmentId == assignmentId &&
            (statusName == string.Empty || x.Status.Name == statusName.ToLower()));
        return (userAssgns, cnt);
    }

    public bool DeleteAllAssignments()
    {
        context.Assignments.RemoveRange(context.Assignments);
        return true;
    }

    public async Task<List<UserAssignmentViewForAI>> GetUserAssignmentViewForAI(int aiTeacherId,
        int count)
    {
        return await context.UserAssignments.Include(u => u.Assignment)
            .ThenInclude(a => a.Essay)
            .Where(x => x.Assignment.CreatorId == aiTeacherId && x.StatusId == 1 && !x.IsEvaluated)
            .Take(count)
            .Select(x => new UserAssignmentViewForAI()
            {
                UserId = x.UserId,
                AssignmentId = x.AssignmentId,
                EssayTitle = x.Assignment.Essay.EssayTitle,
                EssayText = x.Text,
                ExpectedWordCount = x.Assignment.Essay.ExpectedWordCount
            }).ToListAsync();
    }

    public async Task<int> GetTeacherCreatedAssignmentsCount(int teacherId)
    {
        return await context.Assignments.CountAsync(x => x.CreatorId == teacherId);
    }

    public async Task<int> GetTeacherNeedToEvaluateAssignmentsCount(int teacherId)
    {
        return await context.UserAssignments
            .Include(x => x.Assignment)
            .CountAsync(x => x.Assignment.CreatorId == teacherId && !x.IsEvaluated && x.StatusId == 1);
    }

    public async Task<GroupsPerformance> GetTeacherGroupsPerformanceByDate(int teacherId,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var groupsStats = await context.UserAssignments
            .Include(x => x.Assignment)
            .Where(x => x.Assignment.CreatorId == teacherId && x.IsEvaluated &&
                        ((fromDate == null && toDate == null) ||
                         (x.SubmitDate >= fromDate && x.SubmitDate <= toDate)))
            .GroupBy(x => new { x.Assignment.CreationTime.Year, x.Assignment.CreationTime.Month })
            .AsNoTracking().ToListAsync();
        return new GroupsPerformance()
        {
            GroupsPerformanceStatsByDate = groupsStats.Select(x =>
                new GroupByDatePerformanceStat()
                {
                    DateTime = new DateTime(x.Key.Year, x.Key.Month, 1),
                    GroupsPerformanceStats = x.GroupBy(t => t.Assignment.GroupId)
                        .Select(g => new GroupPerformanceStat()
                        {
                            Group = mapper.Map<GroupBaseModal>((context.Groups.FirstOrDefault(v => v.Id == g.Key),
                                context.UserGroups.Count(v => v.GroupId == g.Key))),
                            TotalScoreAvg = double.Round(g.Average(y => y.FluencyScore + y.GrammarScore), 2),
                            GrammarScoreAvg = double.Round(g.Average(y => y.GrammarScore), 2),
                            FluencyScoreAvg = double.Round(g.Average(y => y.FluencyScore), 2)
                        }).ToList()
                }
            ).OrderBy(x => x.DateTime).ToList()
        };
    }
    // public async Task<ResponseView<bool>> AssignToAllStudentsInGroup(int assignmentId, int groupId)
    // {
    //     var userIds = await context.UserGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId)
    //         .ToListAsync();
    //     try
    //     {
    //         foreach (var userAssgn in userIds.Select(userId => new UserAssignment()
    //                  {
    //                      AssignmentId = assignmentId,
    //                      UserId = userId,
    //                      StatusId = 3,
    //                      Text = "",
    //                      WordCount = 0,
    //                      GrammarScore = 0,
    //                      FluencyScore = 0,
    //                      FeedbackSeen = false,
    //                      IsEvaluated = false,
    //                      AssignDate = DateTime.Now,
    //                  }))
    //         {
    //             await context.UserAssignments.AddAsync(userAssgn);
    //         }
    //         await context.SaveChangesAsync();
    //         return new ResponseView<bool>()
    //         {
    //             Code = StatusCodesEnum.Success,
    //             Data = true
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         return new ResponseView<bool>()
    //         {
    //             Code = StatusCodesEnum.InternalServerError,
    //             Message = ex.Message,
    //             Data = true
    //         };
    //     }
    // }
    // public async Task<ResponseView<bool>> AssignToStudentAllGroupAssignments(int userId, int groupId)
    // {
    //     var curUserAssignIds = await context.UserAssignments
    //         .Where(x => x.UserId == userId).Select(x => x.AssignmentId).ToListAsync();
    //     var assgnIds = await context.Assignments.Where(x => x.GroupId == groupId && !curUserAssignIds.Contains(x.Id))
    //         .Select(x => x.Id)
    //         .ToListAsync();
    //     try
    //     {
    //         foreach (var userAssgn in assgnIds.Select(assgnId => new UserAssignment()
    //                  {
    //                      AssignmentId = assgnId,
    //                      UserId = userId,
    //                      StatusId = 3,
    //                      Text = "",
    //                      WordCount = 0,
    //                      GrammarScore = 0,
    //                      FluencyScore = 0,
    //                      FeedbackSeen = false,
    //                      IsEvaluated = false,
    //                      AssignDate = DateTime.Now,
    //                  }))
    //         {
    //             await context.UserAssignments.AddAsync(userAssgn);
    //         }
    //
    //         await context.SaveChangesAsync();
    //         return new ResponseView<bool>
    //         {
    //             Code = StatusCodesEnum.Success,
    //             Data = true
    //         };
    //         ;
    //     }
    //     catch (Exception ex)
    //     {
    //         return new ResponseView<bool>
    //         {
    //             Code = StatusCodesEnum.InternalServerError,
    //             Message = ex.Message,
    //             Data = true
    //         };
    //     }
    // }
    // public async Task<ResponseView<bool>> RemoveGroupAssignmentsForUser(int userId, int groupId)
    // {
    //     try
    //     {
    //         var groupAssignmentsIds =
    //             await context.Assignments.Where(x => x.GroupId == groupId).Select(x => x.Id).ToListAsync();
    //         var userAssignmentsInGroup = await context.UserAssignments
    //             .Where(x => x.UserId == userId && x.StatusId == 3 && groupAssignmentsIds.Contains(x.AssignmentId))
    //             .ToListAsync();
    //         context.UserAssignments.RemoveRange(userAssignmentsInGroup);
    //         await context.SaveChangesAsync();
    //         return new ResponseView<bool>()
    //         {
    //             Code = StatusCodesEnum.Success,
    //             Data = true
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         return new ResponseView<bool>()
    //         {
    //             Code = StatusCodesEnum.InternalServerError,
    //             Message = ex.Message,
    //             Data = false
    //         };
    //     }
    // }
}