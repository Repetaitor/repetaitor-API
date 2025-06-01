using System.Data;
using System.Runtime.Intrinsics.X86;
using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Domain.Data;
using Core.Domain.Entities;
using Core.Domain.Mappers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Repositories;

public class AssignmentRepository(
    ApplicationContext context,
    IEssayRepository essayRepository,
    IUserRepository userRepository,
    IGroupRepository groupRepository) : IAssignmentRepository
{
    public async Task<ResponseView<AssignmentBaseModal>> CreateNewAssignment(int userId, string instructions,
        int groupId, int essayId, DateTime dueDate)
    {
        try
        {
            if (!context.Groups.Any(x => x.OwnerId == userId && x.Id == groupId))
                return new ResponseView<AssignmentBaseModal>()
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not the owner of this group or group does not exist",
                    Data = null
                };
            if (!context.Essays.Any(x => x.Id == essayId && x.CreatorId == userId))
                return new ResponseView<AssignmentBaseModal>()
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not the owner of this essay or essay does not exist",
                    Data = null
                };
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
        catch (Exception ex)
        {
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<AssignmentBaseModal>> GetAssignmentById(int assignmentId)
    {
        try
        {
            var assgn = await context.Assignments.Include(assignment => assignment.Creator)
                .Include(assignment => assignment.Essay).FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assgn == null)
                return new ResponseView<AssignmentBaseModal>()
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Assignment not found",
                    Data = null
                };
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.Accepted,
                Data = new AssignmentBaseModal
                    {
                        Id = assgn.Id,
                        Instructions = assgn.Instructions,
                        GroupId = assgn.GroupId,
                        Creator = UserMapper.ToUserModal(assgn.Creator),
                        Essay = EssayMapper.ToEssayModal(assgn.Essay),
                        DueDate = assgn.DueDate,
                        CreationTime = assgn.CreationTime,
                    }
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<AssignmentBaseModal>> UpdateAssignment(int userId, int assignmentId, string instructions,
        int essayId, DateTime dueDate)
    {
        try
        {
            if (!await context.Essays.AnyAsync(x => x.Id == essayId)) return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.NotFound,
                Message = "Essay not found",
                Data = null
            };
            var assgn = await context.Assignments.Include(assignment => assignment.Creator)
                .Include(assignment => assignment.Essay).FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assgn == null) return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.NotFound,
                Message = "Assignment not found",
                Data = null
            };
            assgn.Instructions = instructions;
            assgn.EssayId = essayId;
            assgn.DueDate = dueDate;
            await context.SaveChangesAsync();
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = new AssignmentBaseModal
                {
                    Id = assgn.Id,
                    Instructions = assgn.Instructions,
                    GroupId = assgn.GroupId,
                    Creator = UserMapper.ToUserModal(assgn.Creator),
                    Essay = EssayMapper.ToEssayModal(assgn.Essay),
                    DueDate = assgn.DueDate,
                    CreationTime = assgn.CreationTime,
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<bool>> EvaluateAssignment(int teacherId, int userId, int assignmentId, int fluencyScore,
        int grammarScore,
        List<EvaluationTextCommentModal> evaluationTextComments, List<GeneralCommentModal> generalComments)
    {
        try
        {
            var assignment = await context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assignment == null || assignment.CreatorId != teacherId) return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.Conflict,
                Message = "You are not the owner of this assignment",
                Data = false
            };
            var userAssgn =
                await context.UserAssignments.FirstOrDefaultAsync(x =>
                    x.AssignmentId == assignmentId && x.UserId == userId);
            if (userAssgn is not { StatusId: 1 }) return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.Conflict,
                Message = "This assignment is not submitted",
                Data = false
            };
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
            await context.EvaluationTextComments.AddRangeAsync(textComments);
            await context.GeneralComments.AddRangeAsync(generalCommentList);
            await context.SaveChangesAsync();
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.Success,
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<(List<AssignmentBaseModal>?, int)>> GetGroupAssignments(int userId, int groupId, int? offset,
        int? limit)
    {
        try
        {
            var group = await context.Groups.AnyAsync(x => x.OwnerId == userId && x.Id == groupId);
            if (!group) return new ResponseView<(List<AssignmentBaseModal>?, int)>
            {
                Code = StatusCodesEnum.Conflict,
                Message = "You are not the owner of this group",
                Data = (null, 0)
            };
            var assignments = await context.Assignments
                .Where(x => x.GroupId == groupId).OrderByDescending(x => x.CreationTime).Skip(offset ?? 0)
                .Take(limit ?? 5)
                .Include(assignment => assignment.Creator)
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
            var cnt = await context.Assignments
                .CountAsync(x => x.GroupId == groupId);
            return new ResponseView<(List<AssignmentBaseModal>?, int)>
            {
                Code = StatusCodesEnum.Success,
                Data = (assignments, cnt)
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<(List<AssignmentBaseModal>?, int)>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = (null, 0)
            };
        }
    }

    public async Task<ResponseView<List<StatusBaseModal>>> GetAssignmentStatuses()
    {
        try
        {
            var res = await context.AssignmentStatuses.Select(s => new StatusBaseModal()
            {
                Id = s.Id,
                Name = s.Name,
            }).OrderBy(x => x.Name).ToListAsync();
            return new ResponseView<List<StatusBaseModal>>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch(Exception ex)
        {
            return new ResponseView<List<StatusBaseModal>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = []
            };
        }
    }

    public async Task<ResponseView<bool>> SaveOrSubmitAssignment(int userId, int assignmentId, string text, int wordCount,
        bool isSubmitted)
    {
        try
        {
            var userAssgn =
                await context.UserAssignments.FirstOrDefaultAsync(x =>
                    x.AssignmentId == assignmentId && x.UserId == userId);
            if (userAssgn == null || userAssgn.StatusId == 1) return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.Conflict,
                Message = "You have already submitted this assignment",
                Data = false
            };
            userAssgn.Text = text;
            userAssgn.WordCount = wordCount;
            userAssgn.StatusId = isSubmitted ? 1 : 2;
            userAssgn.SubmitDate = isSubmitted ? DateTime.Now : default;
            await context.SaveChangesAsync();
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.Success,
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<List<StatusBaseModal>>> GetEvaluationStatuses()
    {
        try
        {
            var res = await context.EvaluationCommentsStatuses.Select(s => new StatusBaseModal()
            {
                Id = s.Id,
                Name = s.Name,
            }).OrderBy(x => x.Name).ToListAsync();
            return new ResponseView<List<StatusBaseModal>>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<List<StatusBaseModal>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = []
            };
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

    public async Task<ResponseView<(List<UserAssignmentBaseModal>?, int)>> GetUserAssignments(int userId, string statusName,
        bool IsAIAssignment, int? offset,
        int? limit)
    {
        try
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
                                x.Assignment.Group.IsAIGroup == IsAIAssignment)
                    .OrderByDescending(x => x.Assignment.CreationTime).Skip(offset ?? 0)
                    .Take(limit ?? 5).Select(x =>
                        new UserAssignmentBaseModal()
                        {
                            Student = UserMapper.ToUserModal(x.User),
                            Assignment = AssignmentMapper.ToAssignmentBaseModal(x.Assignment),
                            Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                            IsEvaluated = x.IsEvaluated,
                            TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                            ActualWordCount = x.WordCount,
                            SubmitDate = x.SubmitDate,
                        }).ToListAsync();
            var cnt = await context.UserAssignments.Include(x => x.Status)
                .CountAsync(x =>
                    x.UserId == userId &&
                    (statusName == string.Empty ||
                     x.Status.Name == statusName));
            return new ResponseView<(List<UserAssignmentBaseModal>?, int)>
            {
                Code = StatusCodesEnum.Success,
                Data = (userAssgns, cnt)
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<(List<UserAssignmentBaseModal>?, int)>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = (null, 0)
            };
        }
    }

    public async Task<ResponseView<(List<UserAssignmentBaseModal>?, int)>> GetUserNotSeenEvaluatedAssignments(int userId, int? offset,
        int? limit)
    {
        try
        {
            var user = await userRepository.GetUserInfo(userId);
            if (user.Code != 0 || user.Data == null) return (new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
            {
                Code = StatusCodesEnum.NotFound,
                Message = "User not found",
                Data = (null, 0)
            });
            var group = await groupRepository.GetStudentGroup(userId);
            if (group.Data == null) return (new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
            {
                Code = StatusCodesEnum.NotFound,
                Message = "Group not found",
                Data = (null, 0)
            });
            var assignmentIds =
                await context.Assignments.Where(x => x.GroupId == group.Data.Id).Select(x => x.Id).ToListAsync();

            var assignments = await context.UserAssignments
                .Where(x => x.UserId == userId && assignmentIds.Contains(x.AssignmentId) && x.IsEvaluated &&
                            !x.FeedbackSeen).Skip(offset ?? 0).Take(limit ?? 5).Include(x => x.User)
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
            var cnt = await context.UserAssignments
                .CountAsync(x => x.UserId == userId && assignmentIds.Contains(x.AssignmentId) && x.IsEvaluated &&
                                 !x.FeedbackSeen);

            return (new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
            {
                Code = StatusCodesEnum.Success,
                Data = (assignments, cnt)
            });
        }
        catch (Exception ex)
        {
            return (new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = (null, 0)
            });
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

    public async Task<ResponseView<UserAssignmentModal>> GetUserAssignment(int callerId, int userId, int assignmentId)
    {
        try
        {
            var assgn = await context.UserAssignments
                .FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
            if (assgn == null) return new ResponseView<UserAssignmentModal>()
            {
                Code = StatusCodesEnum.NotFound,
                Message = "Assignment not found",
                Data = null
            };
            if (callerId == assgn.UserId && assgn.IsEvaluated)
            {
                assgn.FeedbackSeen = true;
                await context.SaveChangesAsync();
            }

            return new ResponseView<UserAssignmentModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = new UserAssignmentModal()
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
                    EvaluationComments = await GetEvaluationTextComments(assgn.UserId, assgn.AssignmentId) ?? []
                }
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<UserAssignmentModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<(List<UserAssignmentBaseModal>?, int)>> GetTeacherAssignments(int userId, int? offset, int? limit)
    {
        try
        {
            var studentsSubmitAssignments = await context.UserAssignments.Include(x => x.User)
                .Include(x => x.Assignment).ThenInclude(x => x.Creator)
                .Include(x => x.Assignment)
                .ThenInclude(a => a.Essay)
                .Include(x => x.Status)
                .Where(x => x.StatusId == 1 && !x.IsEvaluated && x.Assignment.CreatorId == userId).Skip(offset ?? 0)
                .Take(limit ?? 5).Select(x => new UserAssignmentBaseModal()
                {
                    Student = UserMapper.ToUserModal(x.User),
                    Assignment = AssignmentMapper.ToAssignmentBaseModal(x.Assignment),
                    Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                    TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                    ActualWordCount = x.WordCount,
                    SubmitDate = x.SubmitDate
                }).ToListAsync();
            var cnt = await context.UserAssignments
                .CountAsync(x => x.StatusId == 1 && !x.IsEvaluated && x.Assignment.CreatorId == userId);
            return (new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
            {
                Code = StatusCodesEnum.Success,
                Data = (studentsSubmitAssignments, cnt)
            });
        }
        catch (Exception ex)
        {
            return (new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = (null, 0)
            });
        }
    }

    public async Task<ResponseView<(List<UserAssignmentBaseModal>?, int)>> GetAssigmentUsersTasks(int userId, int assignmentId,
        string statusName,
        int? offset,
        int? limit)
    {
        try
        {
            if (!await context.Assignments.AnyAsync(x => x.Id == assignmentId && x.CreatorId == userId))
                return (new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not the owner of this assignment",
                    Data = (null, 0)
                });
            var userAssgns =
                await context.UserAssignments.Include(userAssignment => userAssignment.User)
                    .Include(x => x.Assignment)
                    .ThenInclude(a => a.Creator)
                    .Include(x => x.Assignment)
                    .ThenInclude(a => a.Essay)
                    .Include(userAssignment => userAssignment.Status).Where(x =>
                        x.AssignmentId == assignmentId &&
                        (statusName == string.Empty || x.Status.Name == statusName.ToLower()))
                    .OrderBy(x => x.StatusId).Skip(offset ?? 0)
                    .Take(limit ?? 10).Select(x =>
                        new UserAssignmentBaseModal()
                        {
                            Student = UserMapper.ToUserModal(x.User),
                            Assignment = AssignmentMapper.ToAssignmentBaseModal(x.Assignment),
                            Status = new StatusBaseModal() { Id = x.Status.Id, Name = x.Status.Name },
                            IsEvaluated = x.IsEvaluated,
                            TotalScore = x.IsEvaluated ? x.FluencyScore + x.GrammarScore : -1,
                            ActualWordCount = x.WordCount,
                            SubmitDate = x.SubmitDate,
                        }).ToListAsync();
            var cnt = await context.UserAssignments.CountAsync(x =>
                x.AssignmentId == assignmentId &&
                (statusName == string.Empty || x.Status.Name == statusName.ToLower()));
            return new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
            {
                Code = StatusCodesEnum.Success,
                Data = (userAssgns, cnt)
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<(List<UserAssignmentBaseModal>?, int)>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = (null, 0)
            };
        }
    }

    public bool DeleteAllAssignments()
    {
        try
        {
            context.Assignments.RemoveRange(context.Assignments);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<ResponseView<List<UserAssignmentViewForAI>>> GetUserAssignmentViewForAI(int aiTeacherId, int count)
    {
        try
        {
            var userAssgns = await context.UserAssignments.Include(u => u.Assignment)
                .ThenInclude(a => a.Essay).Where(x => x.Assignment.CreatorId == aiTeacherId && x.StatusId == 1)
                .Take(count)
                .Select(x => new UserAssignmentViewForAI()
                {
                    UserId = x.UserId,
                    AssignmentId = x.AssignmentId,
                    EssayTitle = x.Assignment.Essay.EssayTitle,
                    EssayText = x.Text,
                    ExpectedWordCount = x.Assignment.Essay.ExpectedWordCount
                }).ToListAsync();
            return new ResponseView<List<UserAssignmentViewForAI>>()
            {
                Code = StatusCodesEnum.Success,
                Data = userAssgns
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<List<UserAssignmentViewForAI>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = []
            };
        }
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